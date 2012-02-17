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

        // A DOVICO ID
        public void Append(string sString, CDovicoID idValue)
        {
            // Call our 'string' Append function
            Append(sString);
            Append(idValue.ToString());
        }

        // A double - uses the default system decimal precision
        public void Append(string sString, double dValue)
        {
            // Round the value to our predefined rounding precision
            Append(sString, dValue, true, Constants.DEFAULT_DOUBLE_ROUNDING_PRECISION);
        }

        // A double - allows one to control the rounding precision of doubles
        public void Append(string sString, double dValue, bool bRoundValue, int iRoundingPrecision)
        {
            // If we're to round the value then get the rounded value. Otherwise, just grab the value passed in.
            double dValueToAppend = (bRoundValue ? RoundValue(dValue, iRoundingPrecision) : dValue);

            // Call our 'string' Append function
            Append(sString);
            Append(dValueToAppend.ToString(Constants.CULTURE_US_ENGLISH));
        }


        // Helper to round a value to the requested precision
        protected double RoundValue(double dValue, int iRoundingPrecision)
        {
            // We use rounding away from zero because, without it, the value 1912.225 is rounded to 1912.22 (2 decimal point precision) rather
            // than 1912.23 as expected
            return Math.Round(dValue, iRoundingPrecision, MidpointRounding.AwayFromZero);
        }


        // Return the string (explicit call)
        public override string ToString() { return m_sbString.ToString(); }

        // Return the string (implicit call)
        public static implicit operator string(CStringBuilder sbString) { return sbString.ToString(); }
    }
}