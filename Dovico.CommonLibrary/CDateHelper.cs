using System;

namespace Dovico.CommonLibrary
{
    public class CDateHelper
    {
        // Parses an API XML date string returning a DateTime object. Expected in the following format: yyyy-MM-dd (e.g. 2011-01-31)
        // 
        // If there is an issue, an error message is returned
        public static string GetDateFromAPIDateString(string sApiDate, out DateTime dtDate)
        {
            // Grab the year, month and day values
            string sYear = sApiDate.Substring(0, 4);
            string sMonth = sApiDate.Substring(5, 2);
            string sDay = sApiDate.Substring(8, 2);

            // Create a DateTime object from the Year, Month, and Day portions. If the date is not within the range supported by DOVICO then
            // throw an error.
            dtDate = new DateTime(int.Parse(sYear), int.Parse(sMonth), int.Parse(sDay));
            if ((dtDate < Constants.DATE_MINIMUM) || (dtDate > Constants.DATE_MAXIMUM)) { return strings.ERROR_MSG_DATE_RANGE_NOT_SUPPORTED; }

            // No problems so just return an empty string (no errors)
            return "";
        }
    }

}
