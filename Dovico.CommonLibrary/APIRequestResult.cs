using System.Net;

namespace Dovico.CommonLibrary
{
    public class APIRequestResult
    {
        // Properties
        public string ConsumerSecret { get; set; }
        public string DataAccessToken { get; set; }
        public string ApiVersionTargeted { get; set; }
        public string ContentType { get; set; }
        public string RequestHttpMethod { get; set; }
        public string RequestPostPutData { get; set; }

        public bool HadRequestError { get; protected set; }
        public HttpStatusCode RequestErrorStatus { get; protected set; }

        public string RequestResult { get; set; }
        public string ResultPrevPageURI { get; set; }
        public string ResultNextPageURI { get; set; }

        // When we make a request there is a chance that we might hit a throttle limit (currently set at 5 requests per second). If we receive a
        // throttle error, we will wait a bit and retry but we don't want to retry forever or we'd enter into an infinite loop so this variable
        // allows us to count how many retries we've done and if we hit our limit we stop trying the request again.
        public int RequestRetryCount { get; set; }

        // Protected member variables
        protected string m_sRequestURI = "";                        
        protected string m_sRequestErrorMessage = "";        
       

        
        // Overloaded constructor
        public APIRequestResult(string sConsumerSecret, string sDataAccessToken, string sApiVersionTargeted)
        {
            ConsumerSecret = sConsumerSecret;
            DataAccessToken = sDataAccessToken;
            ApiVersionTargeted = sApiVersionTargeted;

            // Initialize the rest of the properties to the proper default values
            ContentType = CRestApiHelper.MIME_TYPE_TEXT_XML; // XML
            RequestHttpMethod = "GET"; // Most requests will be GETs so this is the default so that the caller doesn't have to explicitly set it every time  
            RequestPostPutData = "";
                        
            // Make sure the result member variables are correctly initialized too
            ResetResultData();
        }


        // Helper to reset all result data
        public void ResetResultData()
        {
            HadRequestError = false;
            RequestErrorStatus = HttpStatusCode.OK;
            m_sRequestErrorMessage = "";
            RequestResult = "";
            ResultPrevPageURI = Constants.URI_NOT_AVAILABLE;
            ResultPrevPageURI = Constants.URI_NOT_AVAILABLE;
            RequestRetryCount = 0;
        }


        //------------------
        // Getters
        //------------------
        public string GetRequestURI() { return m_sRequestURI; }
                
        public string GetRequestErrorMessage() { return m_sRequestErrorMessage; } //this can be turned into a property

        //------------------
        // Setters
        //------------------
        public void SetRequestURI(string sURIPart, string sOptionalQueryStrings) { m_sRequestURI = CRestApiHelper.BuildURI(sURIPart, sOptionalQueryStrings, ApiVersionTargeted); }
        public void SetRequestURI(string sURI) { m_sRequestURI = sURI; }

        // Helper to set the error message and a flag indicating that there was an error
        public void SetRequestErrorMessage(string sRequestErrorMessage)
        {
            // Just call our overloaded method and say it was a bad request
            SetRequestErrorMessage(HttpStatusCode.BadRequest, sRequestErrorMessage);
        }
        public void SetRequestErrorMessage(HttpStatusCode iRequestErrorMessageStatus, string sRequestErrorMessage)
        {
            RequestErrorStatus = iRequestErrorMessageStatus;
            m_sRequestErrorMessage = sRequestErrorMessage;
            HadRequestError = true;
        }
    }
}