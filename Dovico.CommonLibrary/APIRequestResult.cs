
namespace Dovico.CommonLibrary
{
    public class APIRequestResult
    {
        // Properties
        public string ConsumerSecret { get; set; }
        public string DataAccessToken { get; set; }
        public string ApiVersionTargeted { get; set; }
        public string ContentType { get; set; }
        public string RequestHttpMethod { get; set; }
        public string RequestPostPutData { get; set; }

        public bool HadRequestError { get; protected set; }
        
        public string RequestResult { get; set; }
        public string ResultPrevPageURI { get; set; }
        public string ResultNextPageURI { get; set; }

        // Protected member variables
        protected string m_sRequestURI = "";                        
        //private bool m_bShowErrorsToUser = true; // By default we show REST API errors to the user
       // protected bool m_bHadRequestError = false;
        protected string m_sRequestErrorMessage = "";        
        //private Document m_xdRequestResult = null;
       

        
        // Overloaded constructor
        public APIRequestResult(string sConsumerSecret, string sDataAccessToken, string sApiVersionTargeted)//, boolean bShowErrorsToUser)
        {
            ConsumerSecret = sConsumerSecret;
            DataAccessToken = sDataAccessToken;
            ApiVersionTargeted = sApiVersionTargeted;

            // Initialize the rest of the properties to the proper default values
            ContentType = CRestApiHelper.MIME_TYPE_TEXT_XML; // XML
            RequestHttpMethod = "GET"; // Most requests will be GETs so this is the default so that the caller doesn't have to explicitly set it every time  
            RequestPostPutData = "";
            //m_bShowErrorsToUser = bShowErrorsToUser;
            
            // Make sure the result member variables are correctly initialized too
            ResetResultData();
        }


        // Helper to reset all result data
        public void ResetResultData()
        {
            HadRequestError = false;
            m_sRequestErrorMessage = "";
            RequestResult = "";
            ResultPrevPageURI = Constants.URI_NOT_AVAILABLE;
            ResultPrevPageURI = Constants.URI_NOT_AVAILABLE;
        }


        //------------------
        // Getters
        //------------------
        public string GetRequestURI() { return m_sRequestURI; }

        //public bool GetHadRequestError() { return m_bHadRequestError; }
        public string GetRequestErrorMessage() { return m_sRequestErrorMessage; } //this can be turned into a property

        //------------------
        // Setters
        //------------------
        public void SetRequestURI(string sURIPart, string sOptionalQueryStrings) { m_sRequestURI = CRestApiHelper.BuildURI(sURIPart, sOptionalQueryStrings, ApiVersionTargeted); }
        public void SetRequestURI(string sURI) { m_sRequestURI = sURI; }

        // Helper to set the error message and a flag indicating that there was an error
        public void SetRequestErrorMessage(string sRequestErrorMessage) //this can be turned into a property
        {
            m_sRequestErrorMessage = sRequestErrorMessage;
            HadRequestError = true;
        }
    }
}