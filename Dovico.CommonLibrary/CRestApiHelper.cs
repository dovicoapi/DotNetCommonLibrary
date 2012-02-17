﻿using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace Dovico.CommonLibrary
{
    public class CRestApiHelper
    {
        public const string MIME_TYPE_TEXT_XML = "text/xml";
        public const string MIME_TYPE_APPLICATION_XML = "application/xml";
        public const string MIME_TYPE_APPLICATION_JSON = "application/json";


        // The root URI for REST API calls
        protected static string m_sRootURI = "https://api.dovico.com/";

        /// <summary>
        /// Builds up a full URI that is used to call the REST API
        /// </summary>
        /// <param name="sURIPart" type="string" ref="false" inout="[in]" description="The portion of the URI for the type of data needed (e.g. 'Clients/'). NOTE: End your URI Parts with '/' "/>
        /// <param name="sOptionalQueryStrings" type="string" ref="false" inout="[in]" description="Optional - any query string arguments that should be passed. NOTE: It is expected that the query string parameters have already been encoded as need be"/>
        /// <param name="sRESTAPIVersion" type="string" ref="false" inout="[in]" description="The version of the API that we are targeting"/>
        /// <returns>string (the full URI)</returns>
        /// <history>
        /// <modified author="C. Gerard Gallant" date="2011-11-24" reason="Created"/>
        /// </history>
        public static string BuildURI(string sURIPart, string sOptionalQueryStrings, string sRESTAPIVersion)
        {
            // Check to see if there was a query string passed in. Build up the Query string for the URI
            bool bHaveQueryString = (sOptionalQueryStrings != "");
            string sQueryString = ("?" + sOptionalQueryStrings + (bHaveQueryString ? "&" : "") + "version=" + sRESTAPIVersion);

            // Return the proper URI
            return (m_sRootURI + sURIPart + sQueryString);
        }


        /// <summary>
        /// Builds the Authorization header's content
        /// </summary>
        /// <param name="sConsumerSecret" type="string" ref="false" inout="[in]" description="The consumer secret/3rd party developer token"/>
        /// <param name="sDataAccessToken" type="string" ref="false" inout="[in]" description="The Data Access Token of the person logging in"/>
        /// <returns>string (the content to put in the Authorization header of the request)</returns>
        /// <history>
        /// <modified author="C. Gerard Gallant" date="2011-11-24" reason="Created"/>
        /// </history>
        protected static string BuildAuthorizationHeaderContent(string sConsumerSecret, string sDataAccessToken)
        {
            return ("WRAP access_token=\"client=" + HttpUtility.UrlEncode(sConsumerSecret) +
                "&user_token=" + HttpUtility.UrlEncode(sDataAccessToken) + "\"");
        }


        /// <summary>
        /// Passes the request off to the API and returns the results to the caller
        /// </summary>
        /// <param name="sURI" type="string" ref="false" inout="[in]" description="The full URI to call (use BuildURI to build it up)"/>
        /// <param name="sHttpMethod" type="string" ref="false" inout="[in]" description="The HTTP method to call (e.g. GET, PUT, POST, DELETE)"/>
        /// <param name="sContentType" type="string" ref="false" inout="[in]" description="The type of data that is being passed as well as the type that is expected to be returned (e.g. text/xml)"/>
        /// <param name="sPostPutData" type="string" ref="false" inout="[in]" description="The data to send"/>
        /// <param name="sConsumerSecret" type="string" ref="false" inout="[in]" description="The consumer secret/3rd party developer token"/>
        /// <param name="sDataAccessToken" type="string" ref="false" inout="[in]" description="The Data Access Token of the person logging in"/>
        /// <returns>string (the response from the API)</returns>
        /// <history>
        /// <modified author="C. Gerard Gallant" date="2011-11-24" reason="Created"/>
        /// </history>
        public static string MakeAPIRequest(string sURI, string sHttpMethod, string sContentType, string sPostPutData,
            string sConsumerSecret, string sDataAccessToken)
        {
            string sResults = "";


            try
            {
                // Build up our request object
                HttpWebRequest hwrAPIRequest = (HttpWebRequest)WebRequest.Create(sURI);
                hwrAPIRequest.Headers["Authorization"] = BuildAuthorizationHeaderContent(sConsumerSecret, sDataAccessToken);
                hwrAPIRequest.Accept = sContentType;
                hwrAPIRequest.Method = sHttpMethod; // GET, PUT, POST, DELETE, etc
                hwrAPIRequest.ContentType = sContentType;
                hwrAPIRequest.ContentLength = sPostPutData.Length;

                // If there is data to be included with the request then...
                if (sPostPutData != "")
                {
                    // Get the request stream and write the body string to it
                    Stream strOutput = hwrAPIRequest.GetRequestStream();
                    StreamWriter swWriter = new StreamWriter(strOutput);
                    swWriter.Write(sPostPutData);

                    // Close our StreamWriter and Stream objects
                    swWriter.Flush(); // Make sure the buffers are pushed to the stream before we close the writer
                    swWriter.Close();

                    // I don't usually see this in StreamWriter examples but seeing that you can construct multiple StreamWriter objects on the same stream
                    // after flushing and closing the previous StreamWriter suggest to me that the stream is not closed by the Close call on StreamWriter. As
                    // a result, make sure our Stream object is closed. The MSDN examples for GetRequestStream DO close the stream when done.
                    strOutput.Close();
                } // End if (sPostPutData != "")


                // Make the request and get the response object
                HttpWebResponse hwrResponse = (HttpWebResponse)hwrAPIRequest.GetResponse();

                // Read in the response
                Stream strResponse = hwrResponse.GetResponseStream();
                StreamReader srReader = new StreamReader(strResponse);
                sResults = srReader.ReadToEnd();
                srReader.Close();
                strResponse.Close();

                // Close the response object
                hwrResponse.Close();
            }
            catch (WebException weException)
            {
                HttpStatusCode iStatusCode = HttpStatusCode.InternalServerError;
                string sDescription = weException.Message;

                // If the exception was due to a protocol error (status code 300+) then...
                if (weException.Status == WebExceptionStatus.ProtocolError)
                {
                    // Grab the response, its Status Code, and the response stream
                    HttpWebResponse hwrResponse = (HttpWebResponse)weException.Response;
                    iStatusCode = hwrResponse.StatusCode;
                    sDescription = hwrResponse.StatusDescription;

                    Stream sResponseStream = hwrResponse.GetResponseStream();
                    WebFaultExceptionDetails wfedContent = null;

                    // If we are dealing with JSON then...
                    string sResponseContentType = hwrResponse.ContentType;
                    if (sResponseContentType.Contains(MIME_TYPE_APPLICATION_JSON))
                    {
                        // Convert the JSON string into an object and then grab it's description
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WebFaultExceptionDetails));
                        wfedContent = (WebFaultExceptionDetails)ser.ReadObject(sResponseStream);
                        sDescription = wfedContent.Description;
                    }
                    else if ((sResponseContentType.Contains(MIME_TYPE_TEXT_XML)) || (sResponseContentType.Contains(MIME_TYPE_APPLICATION_XML)))
                    {
                        // Convert the XML string into an object and then grab it's description
                        DataContractSerializer ser = new DataContractSerializer(typeof(WebFaultExceptionDetails));
                        wfedContent = (WebFaultExceptionDetails)ser.ReadObject(sResponseStream);
                        sDescription = wfedContent.Description;
                    }
                    else // Not JSON and not XML (HTML?)...
                    {
                        // Grab the content data from the response object and then close the stream reader object
                        StreamReader srReader = new StreamReader(sResponseStream);
                        sDescription = srReader.ReadToEnd();
                        srReader.Close();
                    }// End if


                    // Close the response stream and then the response itself
                    sResponseStream.Close();
                    hwrResponse.Close();
                } // End if (weException.Status == WebExceptionStatus.ProtocolError)


                // Build up the return value for the caller for this error message.
                sResults = BuildErrorReturnString(iStatusCode, sDescription, sContentType);
            } // End of the  catch (WebException weException) statement.


            // Return the results to the caller
            return sResults;
        }



        /// <summary>
        /// Returns the string needed by the caller (JSON or XML depending on the content type specified)
        /// </summary>
        /// <param name="iStatusCode" type="HttpStatusCode" ref="false" inout="[in]" description="The http status of the error"/>
        /// <param name="sDescription" type="string" ref="false" inout="[in]" description="The error's description"/>
        /// <param name="sContentType" type="string" ref="false" inout="[in]" description="The type of data that the string is to be returned as (e.g. text/xml)"/>
        /// <returns>string (the error response)</returns>
        /// <history>
        /// <modified author="C. Gerard Gallant" date="2012-01-10" reason="Created"/>
        /// </history>
        public static string BuildErrorReturnString(HttpStatusCode iStatusCode, string sDescription, string sContentType)
        {
            WebFaultExceptionDetails wfedContent = new WebFaultExceptionDetails(iStatusCode, sDescription);
            MemoryStream msStream = new MemoryStream();
            
            // If we're to return JSON data then...
            if (sContentType.Contains(MIME_TYPE_APPLICATION_JSON))
            {
                // Convert the object into a JSON string
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WebFaultExceptionDetails));                
                ser.WriteObject(msStream, wfedContent);                
            }
            else if ((sContentType.Contains(MIME_TYPE_TEXT_XML)) || (sContentType.Contains(MIME_TYPE_APPLICATION_XML)))
            {
                // Convert the XML into a string
                DataContractSerializer ser = new DataContractSerializer(typeof(WebFaultExceptionDetails));
                ser.WriteObject(msStream, wfedContent);
            } // End if


            // Return the stream's content as a string
            return Encoding.Default.GetString(msStream.ToArray());
        }

    }
}