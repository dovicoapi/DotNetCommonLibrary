using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace Dovico.CommonLibrary.Data
{
    //NOTE:	There are more Time Entry properties available than are currently defined in this class. The current state of this class is simply a
    //      starting point.
    //
    //		For a list of all Time Entry properties see the following: http://apideveloper.dovico.com/TimeEntries
    public class CTimeEntry
    {
        // Member variables
        protected DateTime m_dtDate = DateTime.Now;

        // Properties rather than methods for serialization
        public string TimeEntryID { get; set; } // Could be a Guid or a CDovicoID. I don't need the actual value right now so I left it as a string (there is a prefix character - T for TempTrans or M for Trans that can be used to tell what type of data it is)
        public CDovicoID ClientID { get; set; }
        public string ClientName { get; set; }
        public CDovicoID ProjectID { get; set; }
        public string ProjectName { get; set; }
        public CDovicoID TaskID { get; set; }
        public string TaskName { get; set; }
//fix_me...would be best if i implemented a special format handler in DovTimer for DateTime objects. Turn the following back into a standard DateTime property as soon as possible!
        public string Date { get { return m_dtDate.ToString(Constants.API_DATE_FORMAT); } } // Read-Only. Exposed as a string for serialization purpoposes (if you need the date as a DateTime value use GetDate below)
        public string StartTime { get; set; } // HHMM format
        public string StopTime { get; set; } // HHMM format
        public double TotalHours { get; set; }
        public string Description { get; set; }

        // Overloaded constructor
        // NOTE: Specify Start/Stop time in HHMM format
        public CTimeEntry(string sTimeEntryID, CDovicoID idClient, string sClientName, CDovicoID idProject, string sProjectName,
            CDovicoID idTask, string sTaskName, DateTime dtDate, string sStartTime, string sStopTime, double dTotalHours, string sDescription)
        {
            TimeEntryID = sTimeEntryID;
            ClientID = idClient;
            ClientName = sClientName;
            ProjectID = idProject;
            ProjectName = sProjectName;
            TaskID = idTask;
            TaskName = sTaskName;
            m_dtDate = dtDate;
            StartTime = sStartTime;
            StopTime = sStopTime;
            TotalHours = dTotalHours;
            Description = sDescription;
        }

        // Method used when you want to get the DateTime version of Date (the Date property was needed as a string to be able to control formatting
        // during serialization)
        public DateTime GetDate() { return m_dtDate; }



        //=====================================================================
        // STATIC METHODS: For use when interacting with the DOVICO Hosted API
        //---------------------------------------------------------------------

        //get list options are:
        // - all time (no filters)
        // - all time (filtered by date range)
        // - all time by employee

        //-------------------------------------------------
        // ALL TIME by EMPLOYEE (filtered by date range)
        //-------------------------------------------------
        public static List<CTimeEntry> GetListForEmployee(CDovicoID idEmployeeID, DateTime dtDateRangeStart, DateTime dtDateRangeEnd,
            ref APIRequestResult aRequestResult)
        {
            // If the Request URI has not been set then set it to return the first page of time entries (if it's already set we may have been called to get a next/previous
            // page)
            if (aRequestResult.GetRequestURI() == "") { aRequestResult.SetRequestURI(("TimeEntries/Employee/" + idEmployeeID + "/"), BuildDateRangeQueryString(dtDateRangeStart, dtDateRangeEnd)); }
            return ProcessRequest(ref aRequestResult);
        }


        // Helper that fires off the request to the REST API and processes the results
        protected static List<CTimeEntry> ProcessRequest(ref APIRequestResult aRequestResult)
        {
            // Make sure the Prev/Next Page URI, if there was an error, etc are reset in the event the user is re-using an object that has already 
            // been used for a different call (don't want a previous call's results giving the caller false information)
            aRequestResult.ResetResultData();


            // Pass the request on to the REST API. If there was an error then exit now
            CRestApiHelper.MakeAPIRequest(aRequestResult);
            if (aRequestResult.HadRequestError) { return null; }

            // Load in the XML returned from the API call
            XmlDocument xdDoc = new XmlDocument();
            xdDoc.LoadXml(aRequestResult.RequestResult);


            // Will hold the list of Time Entries that will be returned to the calling function
            List<CTimeEntry> lstTimeEntries = new List<CTimeEntry>();

            // Grab the root element and get the Previous/Next Page URIs from it (when requesting a specific time entry there will be no paging
            // information returned since a single record is all that is ever returned. If that's the case we want our Previous/Next Page URIs to
            // hold 'N/A' rather than "" which is why we pass in the URI_NOT_AVAILABLE constant)
            XmlElement xeDocElement = xdDoc.DocumentElement;
            aRequestResult.ResultPrevPageURI = CXMLHelper.GetChildNodeValue(xeDocElement, Constants.PREV_PAGE_URI, Constants.URI_NOT_AVAILABLE);
            aRequestResult.ResultNextPageURI = CXMLHelper.GetChildNodeValue(xeDocElement, Constants.NEXT_PAGE_URI, Constants.URI_NOT_AVAILABLE);


            XmlElement xeTimeEntry = null, xeClient = null, xeProject = null, xeTask = null;
            DateTime dtDate = DateTime.Now;

            // Grab the list of Time Entry nodes and loop through the elements... 
            XmlNodeList xnlTimeEntries = xeDocElement.GetElementsByTagName("TimeEntry");
            int iCount = xnlTimeEntries.Count;
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                // Grab the current element and the required sub-elements
                xeTimeEntry = (XmlElement)xnlTimeEntries[iIndex];
                xeClient = (XmlElement)xeTimeEntry.GetElementsByTagName("Client")[0];
                xeProject = (XmlElement)xeTimeEntry.GetElementsByTagName("Project")[0];
                xeTask = (XmlElement)xeTimeEntry.GetElementsByTagName("Task")[0];

                // Parse the date
                CDateHelper.GetDateFromAPIDateString(CXMLHelper.GetChildNodeValue(xeTimeEntry, "Date"), out dtDate);

                // Add the current item to our list
                lstTimeEntries.Add(new CTimeEntry(
                    CXMLHelper.GetChildNodeValue(xeTimeEntry, "ID"),
                    CDovicoID.Parse(CXMLHelper.GetChildNodeValue(xeClient, "ID")),
                    CXMLHelper.GetChildNodeValue(xeClient, "Name"),
                    CDovicoID.Parse(CXMLHelper.GetChildNodeValue(xeProject, "ID")),
                    CXMLHelper.GetChildNodeValue(xeProject, "Name"),
                    CDovicoID.Parse(CXMLHelper.GetChildNodeValue(xeTask, "ID")),
                    CXMLHelper.GetChildNodeValue(xeTask, "Name"),
                    dtDate,
                    CXMLHelper.GetChildNodeValue(xeTimeEntry, "StartTime"),
                    CXMLHelper.GetChildNodeValue(xeTimeEntry, "StopTime"),
                    double.Parse(CXMLHelper.GetChildNodeValue(xeTimeEntry, "TotalHours"), Constants.CULTURE_US_ENGLISH),
                    CXMLHelper.GetChildNodeValue(xeTimeEntry, "Description")
                    ));
            } // End of the for(int iIndex = 0; iIndex < iCount; iIndex++) loop.


            // Return the list of Employees to the caller
            return lstTimeEntries;
        }


        // Helper that builds up the date range query string
        protected static string BuildDateRangeQueryString(DateTime dtDateRangeStart, DateTime dtDateRangeEnd)
        {
            string sDateRangeStart = dtDateRangeStart.ToString(Constants.API_DATE_FORMAT);
            string sDateRangeEnd = dtDateRangeEnd.ToString(Constants.API_DATE_FORMAT);
            return ("daterange=" + HttpUtility.UrlEncode((sDateRangeStart + " " + sDateRangeEnd)));
        }



        // Method used to insert a time entry via the API.
        //
        // NOTE: StartTime, StopTime, and Description are optional and can be left out by passing in 'null'
        //
        // If successful, returns the inserted time entry data from the API. 
        // If there is an error, the return value will be null.
        public static CTimeEntry DoInsert(CDovicoID idProject, CDovicoID idTask, CDovicoID idEmployee, DateTime dtDate, string sStartTimeHHMM,
            string sStopTimeHHMM, double dTotalHours, string sDescription, ref APIRequestResult aRequestResult)
        {
            // Start off the XML needed for the POST call to the API
            CStringBuilderXML sbXML = new CStringBuilderXML(false);
            sbXML.AppendStartTag("TimeEntries");
            sbXML.AppendStartTag("TimeEntry");
            sbXML.AppendTagsWithValue("ProjectID", idProject);
            sbXML.AppendTagsWithValue("TaskID", idTask);
            sbXML.AppendTagsWithValue("EmployeeID", idEmployee);
            sbXML.AppendTagsWithValue("Date", dtDate);

            // If the StartTime has been provided then...(it's optional)
            if (sStartTimeHHMM != null) { sbXML.AppendTagsWithValue("StartTime", sStartTimeHHMM); }

            // If the StopTime has been provided then...(it's optional)
            if (sStopTimeHHMM != null) { sbXML.AppendTagsWithValue("StopTime", sStopTimeHHMM); }

            sbXML.AppendTagsWithValue("TotalHours", dTotalHours);

            // If the Description has been provided then...(it's optional)
            if (sDescription != null) { sbXML.AppendTagsWithValue("Description", sDescription); }

            // Close off the XML needed for the POST call
            sbXML.AppendEndTag("TimeEntry");
            sbXML.AppendEndTag("TimeEntries");
            


            // Configure our request object with the necessary data for the POST
            aRequestResult.SetRequestURI("TimeEntries/", "");
            aRequestResult.RequestHttpMethod = "POST";
            aRequestResult.RequestPostPutData = sbXML;
            
            // Execute the request and get the list of time entries back. If we have a list then return the first item in the list (there should
            // only be the one item)
            List<CTimeEntry> lstTimeEntries = ProcessRequest(ref aRequestResult);
            if (lstTimeEntries != null) { return lstTimeEntries[0]; }

            // An error happened so just return null.
            return null;		
        }
    }
}
