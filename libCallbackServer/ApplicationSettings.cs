using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    [Serializable]
    public class ApplicationSettings: ApplicationTypes.iApplicationSettings
    {
        string APPLICATIONSETTINGSFILEPATH = String.Empty;

        ApplicationTypes.ApplicationSettingsReturn _LastOperationReturn = ApplicationTypes.ApplicationSettingsReturn.NONE;

        com.workflowconcepts.utilities.AESSymmetricEncryption _encDec = null;

        String _WebServerIP = String.Empty;
        String _WebServerPort = String.Empty;
        String _WebServerDataCollectionPort = String.Empty;
        String _WebServerPrefix = String.Empty;

        String _EmailFrom = String.Empty;
        String _EmailTo = String.Empty;
        String _EmailSMTPServer = String.Empty;
        String _EmailSMTPPort = String.Empty;
        String _EmailSMTPUserName = String.Empty;
        String _EmailSMTPPassword = String.Empty;

        String _UCCXNode1IPAddress = String.Empty;
        String _UCCXNode2IPAddress = String.Empty;
        String _UCCXApplicationPort = String.Empty;
        String _UCCXRealtimeDataPort = String.Empty;

        String _UCCXAuthorizationPrefix = String.Empty;
        String _UCCXCallbackPrefix = String.Empty;
        String _UCCXAdminUser = String.Empty;
        String _UCCXAdminPassword = String.Empty;
        String _EncryptedUCCXAdminUser = String.Empty;
        String _EncryptedUCCXAdminPassword = String.Empty;
        String _UCCXNumberOfIVRPorts = String.Empty;
        String _UCCXMaxIVRPortUsagePercent = String.Empty;

        int _MaximumNumberOfDays = Constants.MAXIMUM_NUMBER_OF_DAYS;
        int _MaximumNumberOfAttempts = Constants.MAXIMUM_NUMBER_OF_ATTEMPTS;
        int _MinimumIntervalBetweenRetries = Constants.MAXIMUM_NUMBER_OF_ATTEMPTS;

        bool _BasicInsertionThrottling_Enabled = false;
        int _BasicInsertionThrottling_MaximumRecordsAtATime = 0;

        int _NumberOfAPIThreads = Constants.NUMBER_OF_HANDLER_THREADS;
        int _APIHandlerThreadsSleep = Constants.API_HANDLER_THREADS_SLEEP;

        bool _EmailOnFailure = false;
        bool _EmailOnSuccess = false;

        bool _DebugEnabled = false;
        String _DebugLevel = String.Empty;
        String _DebugRetainUnit = String.Empty;
        String _DebugRetainValue = String.Empty;

        public String UCCXNode1IPAddress
        {
            get { return _UCCXNode1IPAddress; }
            set { _UCCXNode1IPAddress = value; }
        }

        public String UCCXNode2IPAddress
        {
            get { return _UCCXNode2IPAddress; }
            set { _UCCXNode2IPAddress = value; }
        }

        public String UCCXApplicationPort
        {
            get { return _UCCXApplicationPort; }
            set { _UCCXApplicationPort = value; }
        }

        public String UCCXRealtimeDataPort
        {
            get { return _UCCXRealtimeDataPort; }
            set { _UCCXRealtimeDataPort = value; }
        }

        public String UCCXAuthorizationPrefix
        {
            get { return _UCCXAuthorizationPrefix; }
            set { _UCCXAuthorizationPrefix = value; }
        }

        public String UCCXCallbackPrefix
        {
            get { return _UCCXCallbackPrefix; }
            set { _UCCXCallbackPrefix = value; }
        }

        public String UCCXAdminUser
        {
            get { return _UCCXAdminUser; }
            set { _UCCXAdminUser = value; }
        }

        public String UCCXAdminPassword
        {
            get { return _UCCXAdminPassword; }
            set { _UCCXAdminPassword = value; }
        }

        public String UCCXNumberOfIVRPorts
        {
            get { return _UCCXNumberOfIVRPorts; }
            set { _UCCXNumberOfIVRPorts = value; }
        }

        public String UCCXMaxIVRPortUsagePercent
        {
            get { return _UCCXMaxIVRPortUsagePercent; }
            set { _UCCXMaxIVRPortUsagePercent = value; }
        }

        public String EncryptedUCCXAdminUser
        {
            get { return _EncryptedUCCXAdminUser; }
        }

        public String EncryptedUCCXAdminPassword
        {
            get { return _EncryptedUCCXAdminPassword; }
        }

        public String WebServerIP
        {
            get
            {
                return _WebServerIP;
            }

            set { _WebServerIP = value; }
        }

        public String WebServerPort
        {
            get
            {
                if (_WebServerPort == String.Empty || _WebServerPort == null)
                {
                    return Constants.WEBSERVER_PORT;
                }
                else
                {
                    return _WebServerPort;
                }
            }

            set { _WebServerPort = value; }
        }

        public String WebServerPrefix
        {
            get
            {
                if (_WebServerPrefix == string.Empty)
                {
                    return Constants.WEBSERVER_PREFIX;
                }
                else
                {
                    return _WebServerPrefix;
                }
            }

            set { _WebServerPrefix = value; }
        }

        public String WebServerDataCollectionPort
        {
            get
            {
                if (_WebServerDataCollectionPort == string.Empty || _WebServerDataCollectionPort == null)
                {
                    return Constants.WEBSERVER_DATACOLLECTION_PORT;
                }
                else
                {
                    return _WebServerDataCollectionPort;
                }
            }

            set { _WebServerDataCollectionPort = value; }
        }

        public int MaximumNumberOfDays
        {
            get { return _MaximumNumberOfDays; }
            set { _MaximumNumberOfDays = value; }
        }

        public int MaximumNumberOfAttempts
        {
            get { return _MaximumNumberOfAttempts; }
            set { _MaximumNumberOfAttempts = value; }
        }

        public int MinimumIntervalBetweenRetries
        {
            get { return _MinimumIntervalBetweenRetries; }
            set { _MinimumIntervalBetweenRetries = value; }
        }

        public int NumerOfAPIThreads
        {
            get { return _NumberOfAPIThreads; }
            set { _NumberOfAPIThreads = value; }
        }

        public int APIHandlerThreadsSleep
        {
            get { return _APIHandlerThreadsSleep; }
            set { _APIHandlerThreadsSleep = value; }
        }

        public bool BasicInsertionThrottling_Enabled
        {
            get { return _BasicInsertionThrottling_Enabled; }
            set { _BasicInsertionThrottling_Enabled = value; }
        }

        public int BasicInsertionThrottling_MaximumRecordsAtATime
        {
            get { return _BasicInsertionThrottling_MaximumRecordsAtATime; }
            set { _BasicInsertionThrottling_MaximumRecordsAtATime = value; }
        }

        public bool EmailOnSuccess
        {
            get { return _EmailOnSuccess; }
            set { _EmailOnSuccess = value; }
        }

        public bool EmailOnFailure
        {
            get { return _EmailOnFailure; }
            set { _EmailOnFailure = value; }
        }

        public String EmailFrom
        {
            get { return _EmailFrom; }
            set { _EmailFrom = value; }
        }

        public String EmailTo
        {
            get { return _EmailTo; }
            set { _EmailTo = value; }
        }

        public String SMTPServer
        {
            get { return _EmailSMTPServer; }
            set { _EmailSMTPServer = value; }
        }

        public String SMTPPort
        {
            get { return _EmailSMTPPort; }
            set { _EmailSMTPPort = value; }
        }

        public String SMTPUserName
        {
            get { return _EmailSMTPUserName; }
            set { _EmailSMTPUserName = value; }
        }

        public String SMTPPassword
        {
            get { return _EmailSMTPPassword; }
            set { _EmailSMTPPassword = value; }
        }

        public bool Debug
        {
            get { return _DebugEnabled; }
            set { _DebugEnabled = value; }
        }

        public String DebugLevel
        {
            get { return _DebugLevel; }
            set { _DebugLevel = value; }
        }

        public String DebugRetainUnit
        {
            get { return _DebugRetainUnit; }
            set { _DebugRetainUnit = value; }
        }

        public String DebugRetainValue
        {
            get { return _DebugRetainValue; }
            set { _DebugRetainValue = value; }
        }

        public ApplicationTypes.ApplicationSettingsReturn LastOperationReturn
        {
            get { return _LastOperationReturn; }
        }

        public ApplicationSettings(string ApplicationSettingsFilePath)
        {
            APPLICATIONSETTINGSFILEPATH = ApplicationSettingsFilePath;

            _LastOperationReturn = ApplicationTypes.ApplicationSettingsReturn.NONE;

            _BasicInsertionThrottling_Enabled = false;
            _BasicInsertionThrottling_MaximumRecordsAtATime = Constants.BASIC_INSERTION_THROTTLING_MAXIMUM_RECORDS_AT_A_TIME;

            _DebugEnabled = false;
            _DebugLevel = "verbose";
            _DebugRetainUnit = "files";
            _DebugRetainValue = "100";
        }

        public ApplicationTypes.ApplicationSettingsReturn Load()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (!System.IO.File.Exists(APPLICATIONSETTINGSFILEPATH + "\\" + Constants.APPLICATIONSETTINGSFILENAME))
                {
                    Trace.TraceWarning(APPLICATIONSETTINGSFILEPATH + "\\" + Constants.APPLICATIONSETTINGSFILENAME + " was not found.");
                    _LastOperationReturn = ApplicationTypes.ApplicationSettingsReturn.FILE_NOT_FOUND;
                    return _LastOperationReturn;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _LastOperationReturn = ApplicationTypes.ApplicationSettingsReturn.ERROR;
                return _LastOperationReturn;
            }

            try
            {
                _encDec = new com.workflowconcepts.utilities.AESSymmetricEncryption(Constants.ENCRYPTION_PASSWORD,Constants.ENCRYPTION_SALT);

                System.Xml.XmlDocument xmlDoc = null;
                System.Xml.XmlNodeReader reader = null;

                xmlDoc = new System.Xml.XmlDocument();

                xmlDoc.Load(APPLICATIONSETTINGSFILEPATH + "\\" + Constants.APPLICATIONSETTINGSFILENAME);

                reader = new System.Xml.XmlNodeReader(xmlDoc);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:

                            if (reader.Name.Equals("WebServer"))
                            {
                                _WebServerIP = reader.GetAttribute("IP");
                                _WebServerPort = reader.GetAttribute("Port");
                                _WebServerPrefix = reader.GetAttribute("Prefix");
                                _WebServerDataCollectionPort = reader.GetAttribute("DataCollectionPort");
                            }

                            if (reader.Name.Equals("EmailNotifications"))
                            {
                                _EmailFrom = reader.GetAttribute("EmailFrom");
                                _EmailTo = reader.GetAttribute("EmailTo");
                                _EmailSMTPServer = reader.GetAttribute("SMTPServer");
                                _EmailSMTPPort = reader.GetAttribute("SMTPPort");

                                try
                                {
                                    _EmailSMTPUserName = _encDec.Decrypt(reader.GetAttribute("SMTPUser"));
                                }
                                catch
                                {
                                    _EmailSMTPUserName = string.Empty;
                                }

                                try
                                {
                                    _EmailSMTPPassword = _encDec.Decrypt(reader.GetAttribute("SMTPPassword"));
                                }
                                catch
                                {
                                    _EmailSMTPPassword = string.Empty;
                                }

                                try
                                {
                                    _EmailOnFailure = Boolean.Parse(reader.GetAttribute("OnFailure"));
                                }
                                catch
                                {
                                    _EmailOnFailure = true;
                                }

                                try
                                {
                                    _EmailOnSuccess = Boolean.Parse(reader.GetAttribute("OnSuccess"));
                                }
                                catch
                                {
                                    _EmailOnSuccess = false;
                                }
                            }

                            if (reader.Name.Equals("UCCX"))
                            {
                                _UCCXNode1IPAddress = reader.GetAttribute("Node1IPAddress");
                                _UCCXNode2IPAddress = reader.GetAttribute("Node2IPAddress");
                                _UCCXApplicationPort = reader.GetAttribute("ApplicationPort");
                                _UCCXRealtimeDataPort = reader.GetAttribute("RealtimeDataPort");
                                _UCCXAuthorizationPrefix = reader.GetAttribute("AuthorizationPrefix");
                                _UCCXCallbackPrefix = reader.GetAttribute("CallbackPrefix");
                                _UCCXNumberOfIVRPorts = reader.GetAttribute("NumberOfIVRPorts");
                                _UCCXMaxIVRPortUsagePercent = reader.GetAttribute("MaxIVRPortUsagePercent");

                                try
                                {
                                    int iUCCXMaxLicenseUsagePercent = int.Parse(_UCCXMaxIVRPortUsagePercent);

                                    if (iUCCXMaxLicenseUsagePercent < 0)
                                    {
                                        _UCCXMaxIVRPortUsagePercent = "0";
                                    }

                                    if (iUCCXMaxLicenseUsagePercent >= 100)
                                    {
                                        _UCCXMaxIVRPortUsagePercent = "99";
                                    }
                                }
                                catch
                                {
                                    _UCCXMaxIVRPortUsagePercent = "0";
                                }

                                try
                                {
                                    int iUccxNumberOfLicenses = int.Parse(_UCCXNumberOfIVRPorts);

                                    if (iUccxNumberOfLicenses < 0)
                                    {
                                        _UCCXNumberOfIVRPorts = "0";
                                    }
                                }
                                catch
                                {
                                    _UCCXNumberOfIVRPorts = "0";
                                }

                                try
                                {
                                    _UCCXAdminUser = _encDec.Decrypt(reader.GetAttribute("AdminUser"));
                                    _EncryptedUCCXAdminUser = reader.GetAttribute("AdminUser");
                                }
                                catch
                                {
                                    _UCCXAdminUser = String.Empty;
                                }

                                try
                                {
                                    _UCCXAdminPassword = _encDec.Decrypt(reader.GetAttribute("AdminPassword"));
                                    _EncryptedUCCXAdminPassword = reader.GetAttribute("AdminPassword");
                                }
                                catch
                                {
                                    _UCCXAdminPassword = String.Empty;
                                }
                            }

                            if (reader.Name.Equals("CallbackRecords"))
                            {
                                try
                                {
                                    _MaximumNumberOfDays = int.Parse(reader.GetAttribute("MaximumNumberOfDays"));

                                    if(_MaximumNumberOfDays < 0)
                                    {
                                        _MaximumNumberOfDays = Constants.MAXIMUM_NUMBER_OF_DAYS;
                                    }
                                }
                                catch
                                {
                                    _MaximumNumberOfDays = Constants.MAXIMUM_NUMBER_OF_DAYS;
                                }

                                try
                                {
                                    _MaximumNumberOfAttempts = int.Parse(reader.GetAttribute("MaximumNumberOfAttempts"));

                                    if (_MaximumNumberOfAttempts <= 0)
                                    {
                                        _MaximumNumberOfAttempts = Constants.MAXIMUM_NUMBER_OF_ATTEMPTS;
                                    }
                                }
                                catch
                                {
                                    _MaximumNumberOfAttempts = Constants.MAXIMUM_NUMBER_OF_ATTEMPTS;
                                }

                                try
                                {
                                    _MinimumIntervalBetweenRetries = int.Parse(reader.GetAttribute("MinimumIntervalBetweenRetries"));

                                    if (_MinimumIntervalBetweenRetries <= 0)
                                    {
                                        _MinimumIntervalBetweenRetries = Constants.MINIMUM_INTERVAL_BETWEEN_RETRIES;
                                    }
                                }
                                catch
                                {
                                    _MinimumIntervalBetweenRetries = Constants.MINIMUM_INTERVAL_BETWEEN_RETRIES;
                                }
                            }

                            if (reader.Name.Equals("BasicInsertionThrottling"))
                            {
                                try
                                {
                                    _BasicInsertionThrottling_Enabled = Boolean.Parse(reader.GetAttribute("Enabled"));
                                }
                                catch
                                {
                                    _BasicInsertionThrottling_Enabled = false;
                                }

                                try
                                {
                                    _BasicInsertionThrottling_MaximumRecordsAtATime = int.Parse(reader.GetAttribute("MaximumAtATime"));

                                    if (_BasicInsertionThrottling_MaximumRecordsAtATime <= 0)
                                    {
                                        _BasicInsertionThrottling_MaximumRecordsAtATime = Constants.BASIC_INSERTION_THROTTLING_MAXIMUM_RECORDS_AT_A_TIME;
                                    }
                                }
                                catch
                                {
                                    _BasicInsertionThrottling_MaximumRecordsAtATime = Constants.BASIC_INSERTION_THROTTLING_MAXIMUM_RECORDS_AT_A_TIME;
                                }
                            }

                            if (reader.Name.Equals("API"))
                            {
                                try
                                {
                                    _NumberOfAPIThreads = int.Parse(reader.GetAttribute("NumberOfHandlerThreads"));

                                    if(_NumberOfAPIThreads <= 0)
                                    {
                                        _NumberOfAPIThreads = Constants.NUMBER_OF_HANDLER_THREADS;
                                    }
                                }
                                catch
                                {
                                    _NumberOfAPIThreads = Constants.NUMBER_OF_HANDLER_THREADS;
                                }

                                try
                                {
                                    _APIHandlerThreadsSleep = int.Parse(reader.GetAttribute("HandlerThreadsSleep"));

                                    if (_APIHandlerThreadsSleep <= 0)
                                    {
                                        _APIHandlerThreadsSleep = Constants.API_HANDLER_THREADS_SLEEP;
                                    }
                                }
                                catch
                                {
                                    _APIHandlerThreadsSleep = Constants.API_HANDLER_THREADS_SLEEP;
                                }
                            }

                            if (reader.Name.Equals("Debug"))
                            {
                                try
                                {
                                    _DebugEnabled = Boolean.Parse(reader.GetAttribute("Enabled"));
                                }
                                catch
                                {
                                    _DebugEnabled = false;
                                }
                                
                                _DebugLevel = reader.GetAttribute("Level");
                                _DebugRetainUnit = reader.GetAttribute("RetainUnit");
                                _DebugRetainValue = reader.GetAttribute("RetainValue");
                            }

                            break;
                    }
                }

                reader.Close();
                reader = null;

                xmlDoc = null;

                _encDec = null;

                _LastOperationReturn = ApplicationTypes.ApplicationSettingsReturn.SUCCESS;
                return _LastOperationReturn;
            }
            catch (Exception ex)
            {
                _encDec = null;
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _LastOperationReturn = ApplicationTypes.ApplicationSettingsReturn.ERROR;
                return _LastOperationReturn;
            }
        }

        public ApplicationTypes.ApplicationSettingsReturn Save()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                _encDec = new com.workflowconcepts.utilities.AESSymmetricEncryption(Constants.ENCRYPTION_PASSWORD,Constants.ENCRYPTION_SALT);

                System.Xml.XmlWriterSettings _XMLSettings = new System.Xml.XmlWriterSettings();

                _XMLSettings.Indent = true;

                System.Xml.XmlWriter _XMLWriter = System.Xml.XmlWriter.Create(APPLICATIONSETTINGSFILEPATH + "\\" + Constants.APPLICATIONSETTINGSFILENAME, _XMLSettings);

                _XMLWriter.WriteStartDocument();

                _XMLWriter.WriteStartElement("ApplicationSettings");

                _XMLWriter.WriteStartElement("WebServer");
                _XMLWriter.WriteAttributeString("IP", WebServerIP);
                _XMLWriter.WriteAttributeString("Port", WebServerPort);
                _XMLWriter.WriteAttributeString("Prefix", WebServerPrefix);
                _XMLWriter.WriteAttributeString("DataCollectionPort", WebServerDataCollectionPort);
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("EmailNotifications");
                _XMLWriter.WriteAttributeString("EmailFrom", _EmailFrom);
                _XMLWriter.WriteAttributeString("EmailTo", _EmailTo);
                _XMLWriter.WriteAttributeString("SMTPServer", _EmailSMTPServer);
                _XMLWriter.WriteAttributeString("SMTPPort", _EmailSMTPPort);
                _XMLWriter.WriteAttributeString("SMTPUser", _encDec.Encrypt(_EmailSMTPUserName));
                _XMLWriter.WriteAttributeString("SMTPPassword", _encDec.Encrypt(_EmailSMTPPassword));
                _XMLWriter.WriteAttributeString("OnFailure", _EmailOnFailure.ToString());
                _XMLWriter.WriteAttributeString("OnSuccess", _EmailOnSuccess.ToString());
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("UCCX");
                _XMLWriter.WriteAttributeString("Node1IPAddress", _UCCXNode1IPAddress);
                _XMLWriter.WriteAttributeString("Node2IPAddress", _UCCXNode2IPAddress);
                _XMLWriter.WriteAttributeString("ApplicationPort", _UCCXApplicationPort);
                _XMLWriter.WriteAttributeString("RealtimeDataPort", _UCCXRealtimeDataPort);
                _XMLWriter.WriteAttributeString("AuthorizationPrefix", _UCCXAuthorizationPrefix);
                _XMLWriter.WriteAttributeString("CallbackPrefix", _UCCXCallbackPrefix);
                _XMLWriter.WriteAttributeString("AdminUser", _encDec.Encrypt(_UCCXAdminUser));
                _XMLWriter.WriteAttributeString("AdminPassword", _encDec.Encrypt(_UCCXAdminPassword));
                _XMLWriter.WriteAttributeString("NumberOfIVRPorts", _UCCXNumberOfIVRPorts);
                _XMLWriter.WriteAttributeString("MaxIVRPortUsagePercent", _UCCXMaxIVRPortUsagePercent);
                _XMLWriter.WriteFullEndElement();

                _EncryptedUCCXAdminUser = _encDec.Encrypt(_UCCXAdminUser);
                _EncryptedUCCXAdminPassword = _encDec.Encrypt(_UCCXAdminPassword);

                _XMLWriter.WriteStartElement("CallbackRecords");
                _XMLWriter.WriteAttributeString("MaximumNumberOfDays", _MaximumNumberOfDays.ToString());
                _XMLWriter.WriteAttributeString("MaximumNumberOfAttempts", _MaximumNumberOfAttempts.ToString());
                _XMLWriter.WriteAttributeString("MinimumIntervalBetweenRetries", _MinimumIntervalBetweenRetries.ToString());
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("BasicInsertionThrottling");
                _XMLWriter.WriteAttributeString("Enabled", _BasicInsertionThrottling_Enabled.ToString());
                _XMLWriter.WriteAttributeString("MaximumAtATime", _BasicInsertionThrottling_MaximumRecordsAtATime.ToString());
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("API");
                _XMLWriter.WriteAttributeString("NumberOfHandlerThreads", _NumberOfAPIThreads.ToString());
                _XMLWriter.WriteAttributeString("HandlerThreadsSleep", _APIHandlerThreadsSleep.ToString());
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("Debug");
                _XMLWriter.WriteAttributeString("Enabled", _DebugEnabled.ToString());
                _XMLWriter.WriteAttributeString("Level", _DebugLevel);
                _XMLWriter.WriteAttributeString("RetainUnit", _DebugRetainUnit);
                _XMLWriter.WriteAttributeString("RetainValue", _DebugRetainValue);
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteEndDocument();

                _XMLWriter.Flush();
                _XMLWriter.Close();
                _XMLWriter = null;

                _XMLSettings = null;

                _encDec = null;

                return ApplicationTypes.ApplicationSettingsReturn.SUCCESS;
            }
            catch (Exception ex)
            {
                _encDec = null;

                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                return ApplicationTypes.ApplicationSettingsReturn.ERROR;
            }
        }

        public ApplicationTypes.ApplicationSettingsReturn CreateDefaultSettings()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                _encDec = new com.workflowconcepts.utilities.AESSymmetricEncryption(Constants.ENCRYPTION_PASSWORD,Constants.ENCRYPTION_SALT);

                System.Xml.XmlWriterSettings _XMLSettings = new System.Xml.XmlWriterSettings();

                _XMLSettings.Indent = true;

                System.Xml.XmlWriter _XMLWriter = System.Xml.XmlWriter.Create(APPLICATIONSETTINGSFILEPATH + "\\" + Constants.APPLICATIONSETTINGSFILENAME, _XMLSettings);

                _XMLWriter.WriteStartDocument();

                _XMLWriter.WriteStartElement("ApplicationSettings");

                _XMLWriter.WriteStartElement("WebServer");
                _XMLWriter.WriteAttributeString("IP", Constants.IPAddress);
                _XMLWriter.WriteAttributeString("Port", Constants.WEBSERVER_PORT);
                _XMLWriter.WriteAttributeString("Prefix", Constants.WEBSERVER_PREFIX);
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("EmailNotifications");
                _XMLWriter.WriteAttributeString("EmailFrom", _EmailFrom);
                _XMLWriter.WriteAttributeString("EmailTo", _EmailTo);
                _XMLWriter.WriteAttributeString("SMTPServer", _EmailSMTPServer);
                _XMLWriter.WriteAttributeString("SMTPPort", _EmailSMTPPort);
                _XMLWriter.WriteAttributeString("SMTPUser", _encDec.Encrypt(_EmailSMTPUserName));
                _XMLWriter.WriteAttributeString("SMTPPassword", _encDec.Encrypt(_EmailSMTPPassword));
                _XMLWriter.WriteAttributeString("OnFailure", _EmailOnFailure.ToString());
                _XMLWriter.WriteAttributeString("OnSuccess", _EmailOnSuccess.ToString());
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("CallbackRecords");
                _XMLWriter.WriteAttributeString("MaximumNumberOfDays", Constants.MAXIMUM_NUMBER_OF_DAYS.ToString());
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("BasicInsertionThrottling");
                _XMLWriter.WriteAttributeString("Enabled", _BasicInsertionThrottling_Enabled.ToString());
                _XMLWriter.WriteAttributeString("MaximumAtATime", _BasicInsertionThrottling_MaximumRecordsAtATime.ToString());
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("API");
                _XMLWriter.WriteAttributeString("NumberOfHandlerThreads", Constants.NUMBER_OF_HANDLER_THREADS.ToString());
                _XMLWriter.WriteAttributeString("HandlerThreadsSleep", Constants.API_HANDLER_THREADS_SLEEP.ToString());
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteStartElement("Debug");
                _XMLWriter.WriteAttributeString("Enabled", "true");
                _XMLWriter.WriteAttributeString("Level", "verbose");
                _XMLWriter.WriteAttributeString("RetainUnit", "Files");
                _XMLWriter.WriteAttributeString("RetainValue", "100");
                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteFullEndElement();

                _XMLWriter.WriteEndDocument();

                _XMLWriter.Flush();
                _XMLWriter.Close();
                _XMLWriter = null;

                _XMLSettings = null;

                _encDec = null;

                return ApplicationTypes.ApplicationSettingsReturn.SUCCESS;
            }
            catch (Exception ex)
            {
                _encDec = null;

                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                return ApplicationTypes.ApplicationSettingsReturn.ERROR;
            }
        }

        public bool ParseArgs(string[] args)
        {
            Trace.TraceInformation("Enter.");

            if (args == null)
            {
                Trace.TraceWarning("args is null");
                return false;
            }

            try
            {
                foreach (string arg in args)
                {
                    int firstTokenIndex = arg.IndexOf("/");
                    int secondTokenIndex = arg.IndexOf(":");

                    if (firstTokenIndex >= 0 && secondTokenIndex >= 0)
                    {
                        string parameterName = arg.Substring(firstTokenIndex + 1, secondTokenIndex - firstTokenIndex - 1).ToLower();
                        string value = arg.Substring(secondTokenIndex + 1);

                        if (parameterName == "debug")
                        {
                            try
                            {
                                _DebugEnabled = Boolean.Parse(value);
                            }
                            catch
                            {
                                _DebugEnabled = true;
                            }                            
                        }
                        else if (parameterName == "debuglevel")
                        {
                            _DebugLevel = value;
                        }
                        else if (parameterName == "retainunit")
                        {
                            _DebugRetainUnit = value;
                        }
                        else if (parameterName == "retainvalue")
                        {
                            _DebugRetainValue = value;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }
    }
}

