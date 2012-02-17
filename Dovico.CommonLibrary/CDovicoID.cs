using System.Globalization;

namespace Dovico.CommonLibrary
{
    // DOVICO database IDs are, for the most part, bigint. If that was ever to change in the future, rather than having to change values from
    // 'long' to something else everywhere, you only have to modify this class and all areas automatically get the new variable type. Just
    // make sure you use CDovicoID.Parse rather than long.Parse.
    public class CDovicoID
    {       
        protected long m_lID = 0;
        public long ID { get { return m_lID; } }

        // Constructors
        public CDovicoID() {}
        public CDovicoID(long lID) { m_lID = lID; }


        // Implicit conversions to/from a long/CDovicoID
        public static implicit operator long(CDovicoID rhs) { return rhs.ID; }
        public static implicit operator CDovicoID(long rhs) { return new CDovicoID(rhs); }

        
        // Takes a string and returns a new instance of CDovicoID
        public static CDovicoID Parse(string sValue)
        {
            return new CDovicoID(long.Parse(sValue, NumberStyles.Any, Constants.CULTURE_US_ENGLISH));
        }

        // Returns this object as a string
        public override string ToString() { return m_lID.ToString(Constants.CULTURE_US_ENGLISH); }
    }
}