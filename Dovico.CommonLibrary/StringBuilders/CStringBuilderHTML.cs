
namespace Dovico.CommonLibrary.StringBuilders
{
    public class CStringBuilderHTML : CStringBuilder
    {

        
        // sValue will be HTML Encoded (e.g. '<B>Bold' will become '&lt;B&gt;Bold')
        public void Append(string sString, string sValue)
        {
            Append(sString);
            Append(CXMLHelper.FixXMLString(sValue));
        }
    }
}
