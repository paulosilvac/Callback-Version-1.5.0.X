using System;

namespace com.workflowconcepts.applications.uccx
{
    public class Constants
    {
        public const string ENCRYPTION_PASSWORD = "WorkflowConceptsEncryptKey";
        public const string ENCRYPTION_SALT = "gHN3p3Z-9x&5?=gCe0[*..PWu(O&AT#8yMF~1MKLd[un>|5,x{0(L5REAR~N|Rf/";

        public const string APPLICATIONSETTINGSFILENAME = "ApplicationSettings.xml";
        public const string APPLICATIONCACHEFILENAME = "Cache.xml";
        public const string SERVICENAME = "callbackserver";
        public const string SERVICEDISCRIPTION = "This service manages callbacks for Cisco UCCX.";
        public const string SERVICEDISPLAYNAME = "Callback Server";
        public const string DATACOLLECTIONSERVICEDISPLAYNAME = "Contact Data Collection";

        public const int STATUSUPDATEINACTIVITY = 120;
        public const int MAXIMUM_NUMBER_OF_ATTEMPTS = 4;
        public const int MINIMUM_INTERVAL_BETWEEN_RETRIES = 1;
        public const int MAXIMUM_RECORD_AGE = 4;

        public const int IPC_PORT = 8500;
        public const string IPC_URI = "ServicesManagment";

        public const string WEBSERVER_PORT = "9000";
        public const string WEBSERVER_DATACOLLECTION_PORT = "9010";
        public const string WEBSERVER_PREFIX = "callbackmanagement";

        public const string INTERFACESERVICECLIENTPORT = "5001";

        public const string COMPANY_WEB_SITE = "http://www.workflowconcepts.com";

        public const int NUMBER_OF_HANDLER_THREADS = 20;
        public const int API_HANDLER_THREADS_SLEEP = 100;

        public const int CHECK_UCCX_MASTER_INTERVAL = 10000;

        public const int MAXIMUM_NUMBER_OF_DAYS = 7;

        public const int CONTACT_REENTRY_TIMEOUT = 8000;

        public const int CONTACT_REALTIMEDATE_REFRESH = 5000;

        public const int MAXIMUM_NUMBER_OF_REQUESTS = 400;

        public const int MAXIMUM_NUMBER_OF_REQUEUES = 3;

        public static string LogFilesPath
        {
            get { return Environment.GetEnvironmentVariable("SystemDrive") + "\\" + System.Windows.Forms.Application.CompanyName + "\\" + System.Windows.Forms.Application.ProductName; }
        }

        public static string ApplicationSettingsFilePath
        {
            get 
            {
                return System.Windows.Forms.Application.CommonAppDataPath.Substring(0, System.Windows.Forms.Application.CommonAppDataPath.LastIndexOf("\\"));
            }             
        }

        public static String IPAddress
        {
            get
            {
                System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                String localIP = String.Empty;
                foreach (System.Net.IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                    }
                }
                return localIP;
            }
        }

        public enum Operations { NONE, ADDRECORD, REMOVERECORD, UPDATERECORD, GETRECORDS, GETRECORDSBYCSQ, CHECKDNISINUSE, CHECKSERVERSTATUS, CHECKCSQSTATUS, AUTHENTICATEUSER, GETSETTINGS, SETSETTINGS, GETCSQLIST, GETCONTACTPOSITIONOFFSET }

        public enum RecordStatus { NEW, INVALID, INACTIVE, PROCESSING, RETRY, FORCERETRY, REQUESTED, QUEUED, AGENTACKNOWLEDGED, AGENTABANDONED, DIALINGTARGET, COMPLETED, PURGED, EXCEEDEDNUMBEROFATTEMPTS, IVR_FAILURE }

        public enum FilterOperations { BIGGERTHANOREQUALTO, SMALLERTHANOREQUALTO };

    }
}
