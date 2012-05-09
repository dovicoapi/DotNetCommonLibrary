using System.Collections.Generic;
using System.Xml;

namespace Dovico.CommonLibrary.Data
{
    //NOTE:	There are more Employee properties available than are currently defined in this class. The current state of this class is simply a
    //      starting point.
    //
    //		For a list of all Employee properties see the following: http://apideveloper.dovico.com/Employees
    public class CEmployee
    {
        public CDovicoID ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        // Overloaded constructor
        public CEmployee(CDovicoID idEmployee, string sLastName, string sFirstName)
        {
            ID = idEmployee;
            LastName = sLastName;
            FirstName = sFirstName;
        }




        //=====================================================================
        // STATIC METHODS: For use when interacting with the DOVICO Hosted API
        //---------------------------------------------------------------------

        // A request for a specific employee by ID (If there was an error the return value will be null)
        public static CEmployee GetInfo(CDovicoID idEmployee, APIRequestResult aRequestResult)
        {
            // Set the URI for the Employee/{ID}/ request. Process the request and if the returned list is not null (no errors) then return the
            // first item in the list (there should only ever be the one item) 
            aRequestResult.SetRequestURI(("Employees/" + idEmployee.ToString() + "/"), "");
            List<CEmployee> lstEmployees = ProcessRequest(aRequestResult);
            if (lstEmployees != null) { return lstEmployees[0]; }

            // An error happened so just return null.
            return null;
        }

        // A request for the logged in Employee's info (this information is quite limited but may be the only employee information you can obtain
        // if the logged in user has no employee access permissions. this will return the employee's ID, Last Name, and First Name - with the ID,
        // you can call getInfo and 'try' to get the rest of the info if desired)
        //
        // If there was an error the return value will be null
        public static CEmployee GetInfoMe(APIRequestResult aRequestResult)
        {
            // Set the URI for the Employee/Me/ request. Process the request and if the returned list is not null (no errors) then return the first item in the list (there
            // should only ever be the one item)
            aRequestResult.SetRequestURI("Employees/Me/", "");
            List<CEmployee> lstEmployees = ProcessRequest(aRequestResult);
            if (lstEmployees != null) { return lstEmployees[0]; }

            // An error happened so just return null.
            return null;
        }



        // Handles the work of making a request and pulling Employee(s) from the result of a request (if there was an error the return value will
        // be null)
        protected static List<CEmployee> ProcessRequest(APIRequestResult aRequestResult)
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


            // Will hold the list of employees that will be returned to the calling function
            List<CEmployee> lstEmployees = new List<CEmployee>();

            // Grab the root element and get the Previous/Next Page URIs from it (when requesting a specific employee there will be no paging
            // information returned since a single record is all that is ever returned. If that's the case we want our Previous/Next Page URIs to
            // hold 'N/A' rather than "" which is why we pass in the URI_NOT_AVAILABLE constant)
            XmlElement xeDocElement = xdDoc.DocumentElement;
            aRequestResult.ResultPrevPageURI = CXMLHelper.GetChildNodeValue(xeDocElement, Constants.PREV_PAGE_URI, Constants.URI_NOT_AVAILABLE);
            aRequestResult.ResultNextPageURI = CXMLHelper.GetChildNodeValue(xeDocElement, Constants.NEXT_PAGE_URI, Constants.URI_NOT_AVAILABLE);

            XmlElement xeEmployee = null;

            // Grab the list of Employee nodes and loop through the elements... 
            XmlNodeList xnlEmployees = xeDocElement.GetElementsByTagName("Employee");
            int iCount = xnlEmployees.Count;
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                // Grab the current element
                xeEmployee = (XmlElement)xnlEmployees[iIndex];

                // Add the current item to our list
                lstEmployees.Add(new CEmployee(
                    CDovicoID.Parse(CXMLHelper.GetChildNodeValue(xeEmployee, "ID")),
                    CXMLHelper.GetChildNodeValue(xeEmployee, "LastName"),
                    CXMLHelper.GetChildNodeValue(xeEmployee, "FirstName")
                    // NOTE: If this is an Employee/Me/ request, the rest of the fields may not be available
                    ));
            } // End of the for(int iIndex = 0; iIndex < iCount; iIndex++) loop.


            // Return the list of Employees to the caller
            return lstEmployees;
        }	
    }
}
