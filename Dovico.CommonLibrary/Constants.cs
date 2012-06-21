using System;
using System.Globalization;

namespace Dovico.CommonLibrary
{
    public class Constants
    {
        public static string PREV_PAGE_URI = "PrevPageURI";
        public static string NEXT_PAGE_URI = "NextPageURI";
        public static string URI_NOT_AVAILABLE = "N/A"; 

        // US English culture (used by places like CDovicoID when parsing data - everything is expected in US English when in server-side code)
        public static readonly CultureInfo CULTURE_US_ENGLISH = new CultureInfo("en-US");

        // The default rounding precesion for doubles
        public static readonly int DEFAULT_DOUBLE_ROUNDING_PRECISION = 2;

        // The default value for a None item's ID (e.g. when a Project is not assigned to a Client, 0 will be the Client ID value returned with the
        // Project's information)
        public static long NONE_ID = 0L;
        public static string NONE_CAPTION = "[None]";

        public static long ADMIN_TOKEN_EMPLOYEE_ID = 99L;
        
        // The date format expected and returned by the API
        public static readonly string API_DATE_FORMAT = "yyyy-MM-dd";
        
        // The minimum and maximum dates supported by DOVICO
        public static readonly DateTime DATE_MINIMUM = new DateTime(1900, 1, 1, 0, 0, 0, 0);
        public static readonly DateTime DATE_MAXIMUM = new DateTime(2199, 12, 31, 23, 59, 59, 0);

        // The ISO format string for use when inserting/updating Date/Time values in SQL Server
        public static string SQL_ISO_8601_DATE_FORMAT = "yyyy-MM-ddTHH':'mm':'ss";

        // The True/False representations from the REST API
        public static string API_BOOL_TRUE = "T";
        public static string API_BOOL_FALSE = "F";
    }
}
