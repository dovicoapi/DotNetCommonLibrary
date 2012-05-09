
namespace Dovico.CommonLibrary.Data
{
    
    // NOTE:    This class will be modified to more closely resemble CEmployee and CTimeEntry. It didn't make sense to keep it in the 
    //          AssignmentTree project when the rest of the API logic is here in the common library


    public class CAssignments
    {
        // Returns the first page of root level assignments for the employee specified
        public static string GetAssignments(CDovicoID idEmployee, ref APIRequestResult aRequestResult)
        {
            // Build up the URI for a request of assignments for the specified employee and then call our overloaded function to do the rest of
            // the work.
            aRequestResult.SetRequestURI(("Assignments/Employee/" + idEmployee.ToString() + "/"), "");
            return GetAssignments("", ref aRequestResult);
        }


        // Returns the assignments for the URI requested (if there are multiple pages of data, pass in the NextPageURI. If you are trying to get
        // the child assignment items, pass in the GetAssignmentsURI value of the item you wish to drill down on)
        public static string GetAssignments(string sAssignmentsURI, ref APIRequestResult aRequestResult)
        {
            // Set the URI if one was specified
            if (sAssignmentsURI != "") { aRequestResult.SetRequestURI(sAssignmentsURI); }

            // Request the list of child assignments
            CRestApiHelper.MakeAPIRequest(aRequestResult);
            return (aRequestResult.HadRequestError ? aRequestResult.GetRequestErrorMessage() : aRequestResult.RequestResult);
        }
    }
}