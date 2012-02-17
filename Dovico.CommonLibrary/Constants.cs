using System.Globalization;

namespace Dovico.CommonLibrary
{
    public class Constants
    {
        // US English culture (used by places like CDovicoID when parsing data - everything is expected in US English when in server-side code)
        public static readonly CultureInfo CULTURE_US_ENGLISH = new CultureInfo("en-US");

        // The default rounding precesion for doubles
        public static readonly int DEFAULT_DOUBLE_ROUNDING_PRECISION = 2;

        // The date format expected and returned by the API
        public static readonly string API_DATE_FORMAT = "yyyy-MM-dd";
    }
}
