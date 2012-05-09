using System;
using System.Text;

namespace Dovico.CommonLibrary
{
    public class CStringBuilder
    {
        protected StringBuilder m_sbString = new StringBuilder(5000);
        
        // Overloads for adding different types of values to the string builder.
        // 
        // Just the string
        public void Append(string sString) { m_sbString.Append(sString); }

        // An int
        public void Append(string sString, int iValue)
        {
            Append(sString);
            Append(iValue.ToString(Constants.CULTURE_US_ENGLISH));
        }
        
        // A long
        public void Append(string sString, long lValue)
        {
            Append(sString);
            Append(lValue.ToString(Constants.CULTURE_US_ENGLISH));
        }

        // A CDovicoID
        public void Append(string sString, CDovicoID idValue)
        {
            Append(sString);
            Append(idValue.ToString());
        }

        // A double - uses the default system decimal precision
        public void Append(string sString, double dValue) { Append(sString, dValue, true, Constants.DEFAULT_DOUBLE_ROUNDING_PRECISION); }

        // A double - allows one to control the rounding precision of doubles
        public void Append(string sString, double dValue, bool bRoundValue, int iRoundingPrecision)
        {
            // If we're to round the value then get the rounded value. Otherwise, just grab the value passed in.
            double dValueToAppend = (bRoundValue ? RoundValue(dValue, iRoundingPrecision) : dValue);

            // Call our 'string' Append function
            Append(sString);
            Append(dValueToAppend.ToString(Constants.CULTURE_US_ENGLISH));
        }

        // A bool
        public void Append(string sString, bool bValue)
        {
            Append(sString);
            Append((bValue ? Constants.API_BOOL_TRUE : Constants.API_BOOL_FALSE));
        }

        // A Guid
        public void Append(string sString, Guid guidValue)
        {
            Append(sString);
            Append(guidValue.ToString());
        }


        // Helper to round a value to the requested precision
        protected double RoundValue(double dValue, int iRoundingPrecision)
        {
            // We use rounding away from zero because, without it, the value 1912.225 is rounded to 1912.22 (2 decimal point precision) rather
            // than 1912.23 as expected
            return Math.Round(dValue, iRoundingPrecision, MidpointRounding.AwayFromZero);
        }


        // Empties the string builder
        public void Clear() { m_sbString.Clear(); }


        // Return the string (explicit call)
        public override string ToString() { return m_sbString.ToString(); }

        // Return the string (implicit call)
        public static implicit operator string(CStringBuilder sbString) { return sbString.ToString(); }
    }
}