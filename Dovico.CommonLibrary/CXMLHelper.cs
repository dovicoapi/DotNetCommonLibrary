using System.Xml;

namespace Dovico.CommonLibrary
{
    public class CXMLHelper
    {
        // Helper to pull a node's value (same as calling the overloaded method and passing "" for the default value)
        public static string GetChildNodeValue(XmlElement xeElement, string sTagName)
        {
            // Call the overloaded version of this function with the default return value being an empty string
            return GetChildNodeValue(xeElement, sTagName, "");
        }


        // Helper to pull a node's value returning the Default value should the requested node not exist or is empty
        public static string GetChildNodeValue(XmlElement xeElement, string sTagName, string sDefaultValue)
        {
            XmlNode xnChild = GetChildNode(xeElement, sTagName);
            if (xnChild != null)
            {
                // Grab the first child of the element (the text node itself). If we have an object then return it's value 
                XmlNode xnFirstChild = xnChild.FirstChild;
                if (xnFirstChild != null) { return xnFirstChild.Value; }
            } // End if (xnChild != null)

            // Either the tag wasn't found OR there was no value provided. Return the default value to the caller instead
            return sDefaultValue;
        }


        // Helper that returns a requested child node in the element specified (returns null if not found)
        protected static XmlNode GetChildNode(XmlElement xeElement, string sTagName)
        {
            // Loop through the element's child nodes...(we were doing xeElement.getElementsByTagName originally but it was returning us even
            // elements of sub-nodes which is not desired)
            for (XmlNode xnChild = xeElement.FirstChild; xnChild != null; xnChild = xnChild.NextSibling)
            {
                // If the current child node is an element AND is the element we're looking for then...
                if ((xnChild.NodeType == XmlNodeType.Element) && (sTagName.Equals(xnChild.Name)))
                {
                    // Pass the node back to the caller (exits the loop)
                    return xnChild;
                } // End if ((xnChild.NodeType == XmlNodeType.Element) && (sTagName.Equals(xnChild.Name)))
            } // End of the for (XmlNode xnChild = xnNode.FirstChild; xnChild != null; xnChild = xnChild.NextSibling) loop.


            // We didn't find the child node that was requested
            return null;
        }


        // Helper that tells the caller if the requested element is a direct child of the element specified or not
        public static bool DoesChildNodeExist(XmlElement xeElement, string sTagName) { return (GetChildNode(xeElement, sTagName) != null); }


        // Helper to replace all unsafe XML characters with safe ones
        public static string FixXMLString(string sValue)
        {
            // Replace all unsafe XML characters with safe ones
            string sReturn = sValue.Replace("&", "&amp;");
            sReturn = sReturn.Replace("'", "&apos;");
            sReturn = sReturn.Replace("\"", "&quot;");
            sReturn = sReturn.Replace("<", "&lt;");
            sReturn = sReturn.Replace(">", "&gt;");

            // Return the encoded string to the caller
            return sReturn;
        }


        // Helper to replace all unsafe XML characters with safe ones when dealing with an Element (e.g. <Element>SomeValue</Element>)
        public static string FixXMLStringForElement(string sValue)
        {
            // Replace all unsafe XML Element characters with safe ones
            string sReturn = sValue.Replace("&", "&amp;");
            sReturn = sReturn.Replace("<", "&lt;");
            sReturn = sReturn.Replace(">", "&gt;");

            // Return the encoded string to the caller
            return sReturn;
        }
    }
}
