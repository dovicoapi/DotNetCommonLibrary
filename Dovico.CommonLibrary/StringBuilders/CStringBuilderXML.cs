using System;

namespace Dovico.CommonLibrary.StringBuilders
{
    public class CStringBuilderXML : CStringBuilder
    {
        // Constructor (specify if you want the XML header included or not: )
        public CStringBuilderXML(bool bAppendHeader)
        {
            // If we are to include the header then add it in
            if (bAppendHeader == true) { Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>"); }
        }


        // Helper to create a tag (e.g. <TagName>)
        protected void AppendTag(string sTagName, bool bIsStartTag, bool bCloseTheTag)
        {
            // Add the tag (make it a closing tag, e.g. </...>, if it's not a start tag). Close the tag if that was requested (some XML tags
            // may contain attributes so you wouldn't want to close the tag right away)
            Append(("<" + (bIsStartTag ? "" : "/") + sTagName + (bCloseTheTag ? ">" : "")));
        }

        // Helper to create an opening tag (e.g. <TagName>)
        public void AppendStartTag(string sTagName) { AppendTag(sTagName, true, true); }

        // Helper to create an opening tag that does not close so that attributes can be added later (e.g. <TagName )
        public void AppendStartTagNoClose(string sTagName) { AppendTag(sTagName, true, false); }
        
        // Helper to create a closing tag (e.g. </TagName>)
        public void AppendEndTag(string sTagName) { AppendTag(sTagName, false, true); }


        // Overloads for adding different types of values to the string builder. For all Append functions, sString will be added as is 
        // (e.g. 'Name=')
        //
        // sValue will be XML Encoded (e.g. '<B>Bold' will become '&lt;B&gt;Bold')
        public void Append(string sString, string sValue)
        {
            Append(sString);
            Append(CXMLHelper.FixXMLString(sValue));
        }
        public void AppendDateOnly(string sString, DateTime dtValue)
        {
            Append(sString);
            Append(dtValue.ToString(Constants.API_DATE_FORMAT));
        }


        //------
        // The following are overloads created that allow for the creation of Elements. You can specify a tag name and a value so that we don't
        // have to keep doing AppendStartTag, Append, AppendEndTag for every single element built up in the code!
        //------
        public void AppendTagsWithValue(string sTagName, string sValue)
        {
            // NOTE:    Unlike Attributes (e.g. Attrib="SomeValue"), elements (e.g. <Element>SomeValue</Element>) should not have single or 
            //          double quotes encoded.
            AppendStartTag(sTagName);
            Append(CXMLHelper.FixXMLStringForElement(sValue));
            AppendEndTag(sTagName);
        }

        // Overload to handle values of TRACKIT_ID
        public void AppendTagsWithValue(string sTagName, CDovicoID idValue)
        {
            AppendStartTag(sTagName);
            Append("", idValue);
            AppendEndTag(sTagName);
        }

        // Overload to handle values of DateTime
        public void AppendTagsWithValue(string sTagName, DateTime dtValue)
        {
            AppendStartTag(sTagName);
            AppendDateOnly("", dtValue);
            AppendEndTag(sTagName);
        }

        // Overload to handle values of double
        public void AppendTagsWithValue(string sTagName, double dValue)
        {
            AppendStartTag(sTagName);
            Append("", dValue);
            AppendEndTag(sTagName);
        }
    }
}