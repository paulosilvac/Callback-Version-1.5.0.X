using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class SettingsManager
    {
        private object objLock = null;
        private DateTime dtSettingsLastChangedAt = DateTime.Now;
        private DateTime dtServiceStartedAt = DateTime.Now;

        public ApplicationSettings ApplicationSettings = null;
        public CallbackSettings CallbackSettings = null;

        private String _UCCXMasterNode = String.Empty;
        private bool _UCCXMasterNodeDetected = false;

        private System.Threading.Thread _thrMonitorUCCXMasterNode = null;
        
        String sResponse = String.Empty;

        public String UCCXMasterNode
        {
            get { return _UCCXMasterNode; }
        }

        public bool UCCXMasterNodeDetected
        {
            get { return _UCCXMasterNodeDetected; }
        }

        public DateTime SettingsLastChangedAt
        {
            get { return dtSettingsLastChangedAt; }
            set { dtSettingsLastChangedAt = value;}
        }

        public DateTime ServiceStartedAt
        {
            get { return dtServiceStartedAt; }
            set { dtServiceStartedAt = value; }
        }

        public SettingsManager()
        {
            ApplicationSettings = null;
            CallbackSettings = null;

            dtSettingsLastChangedAt = DateTime.Now;
            _UCCXMasterNode = String.Empty;
            _UCCXMasterNodeDetected = false;
            sResponse = String.Empty;
        }

        public bool SetSettings(String sSettings)
        {
            Trace.TraceInformation("Enter.");

            if (sSettings == null)
            {
                Trace.TraceWarning("sSettings is null.");
                return false;
            }

            if (sSettings == String.Empty)
            {
                Trace.TraceWarning("sSettings is empty.");
                return false;
            }

            if (CallbackSettings == null)
            {
                Trace.TraceWarning("CallbackSettings is null.");
                return false;
            }

            if (CallbackSettings.ReadFromString(sSettings))
            {
                Trace.TraceInformation("CallbackSettings.ReadFromString() returned true.");

                if (CallbackSettings.WriteToDisk())
                {
                    Trace.TraceInformation("CallbackSettings.WriteToDisk() returned true.");

                    dtSettingsLastChangedAt = DateTime.Now;

                    return true;
                }
                else
                {
                    Trace.TraceWarning("CallbackSettings.WriteToDisk() returned false.");

                    return false;
                }
            }
            else
            {
                Trace.TraceWarning("CallbackSettings.ReadFromString() returned false.");

                return false;
            }
        }

        public String GetSettings()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                if (ApplicationSettings != null)
                {
                    String sAppServerURL = "http://" + UCCXMasterNode + ":" + ApplicationSettings.UCCXApplicationPort + "/" + ApplicationSettings.WebServerPrefix + "?";

                    if (CallbackSettings != null)
                    {
                        sb.Append(CallbackSettings.GetXML(sAppServerURL));
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                return "<callback><csqs></csqs></callback>";
            }
        }

        public bool StartUCCXHNodeMonitor()
        {
            Trace.TraceInformation("Enter.");

            if (_thrMonitorUCCXMasterNode != null)
            {
                Trace.TraceWarning("_thrMonitorUCCXMasterNode is not null.");
                return false;
            }

            _thrMonitorUCCXMasterNode = new System.Threading.Thread(_MonitorUCCXMasterNode);

            _thrMonitorUCCXMasterNode.Start();

            return true;
        }

        public bool UpdateAppServerURL()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (ApplicationSettings == null)
                {
                    Trace.TraceWarning("ApplicationSettings is null.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        void _MonitorUCCXMasterNode()
        {
            Trace.TraceInformation("Enter.");

            do
            {
                try
                {
                    if (ApplicationSettings != null)
                    {
                        if (ApplicationSettings.UCCXNode1IPAddress != String.Empty && ApplicationSettings.UCCXNode2IPAddress != String.Empty)
                        {
                            if (IsNodeMaster(ApplicationSettings.UCCXNode1IPAddress))
                            {
                                Trace.TraceInformation("UCCX Node 1 was detected as Master.");
                                _UCCXMasterNode = ApplicationSettings.UCCXNode1IPAddress;
                                _UCCXMasterNodeDetected = true;
                            }
                            else if (IsNodeMaster(ApplicationSettings.UCCXNode2IPAddress))
                            {
                                Trace.TraceInformation("UCCX Node 2 was detected as Master.");
                                _UCCXMasterNode = ApplicationSettings.UCCXNode2IPAddress;
                                _UCCXMasterNodeDetected = true;
                            }
                            else
                            {
                                Trace.TraceWarning("Neither of the UCCX nodes defined was detected to be Master.");
                                _UCCXMasterNode = String.Empty;
                                _UCCXMasterNodeDetected = false;
                            }
                        }
                        else if (ApplicationSettings.UCCXNode1IPAddress != String.Empty)
                        {
                            Trace.TraceInformation("UCCX only has Node 1 defined; assume Node 1 as Master.");
                            _UCCXMasterNode = ApplicationSettings.UCCXNode1IPAddress;
                            _UCCXMasterNodeDetected = true;
                        }
                        else if (ApplicationSettings.UCCXNode2IPAddress != String.Empty)
                        {
                            Trace.TraceInformation("UCCX only has Node 2 defined; assume Node 2 as Master.");
                            _UCCXMasterNode = ApplicationSettings.UCCXNode2IPAddress;
                            _UCCXMasterNodeDetected = true;
                        }
                        else
                        {
                            Trace.TraceInformation("No UCCX node has been defined. No master assumed.");
                            _UCCXMasterNode = String.Empty;
                            _UCCXMasterNodeDetected = false;
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("ApplicationSettings is null.");
                        _UCCXMasterNodeDetected = false;
                    }

                    System.Threading.Thread.Sleep(Constants.CHECK_UCCX_MASTER_INTERVAL);
                }
                catch (Exception innerEx)
                {
                    Trace.TraceWarning("Exception: " + innerEx.Message + Environment.NewLine + "StackTrace: " + innerEx.StackTrace);
                    _UCCXMasterNodeDetected = false;
                    return;
                }
            }

            while (true);
        }

        private bool IsNodeMaster(String IPAddress)
        {
            if (IPAddress == null)
            {
                Trace.TraceWarning("IPAddress is null.");
                return false;
            }

            if (IPAddress == String.Empty)
            {
                Trace.TraceWarning("IPAddress is empty.");
                return false;
            }

            String sURL = "http://" + IPAddress + "/uccx/isDBMaster";

            Trace.TraceInformation("Checking if " + IPAddress + " is master.");

            sResponse = String.Empty;

            if (!SendRequest(sURL))
            {
                Trace.TraceWarning("SendRequest() returned false for URL -> " + sURL);
                return false;
            }

            if (sResponse == String.Empty)
            {
                Trace.TraceWarning("sResponse is empty");
                return false;
            }

            return ParseResponse();
        }

        private bool SendRequest(String URL)
        {
            try
            {
                if (URL == null)
                {
                    Trace.TraceWarning("URL is null.");
                    return false;
                }

                if (URL == String.Empty)
                {
                    Trace.TraceWarning("URL is empty.");
                    return false;
                }

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                Trace.TraceInformation("Content type is {0} and length is {1}", response.ContentType, response.ContentLength);

                System.IO.Stream stream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                System.IO.StreamReader streamReader = new System.IO.StreamReader(stream, Encoding.UTF8);

                sResponse = streamReader.ReadToEnd();

                streamReader.Close();
                streamReader.Dispose();
                streamReader = null;

                stream.Close();
                stream.Dispose();
                stream = null;

                response.Close();
                response = null;
                request = null;

                return true;
            }
            catch (System.Net.WebException webEx)
            {
                Trace.TraceWarning("WebException: " + webEx.Message + Environment.NewLine + "StackTrace: " + webEx.StackTrace + Environment.NewLine + "Status: " + webEx.Status);

                if (webEx.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    Trace.TraceWarning("Make sure the IP of the this machine has been added to the Server Name field via Tools -> Real Time Snapshot Config");
                }

                sResponse = String.Empty;

                return false;
            }
            catch (Exception ex)
            {   
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace);
                sResponse = String.Empty;
                return false;
            }
        }

        private bool ParseResponse()
        {
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
                writer.Write(sResponse);
                writer.Flush();
                stream.Position = 0;

                System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument(stream);

                System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();

                String sValue = nav.SelectSingleNode("/node/isMaster/text()").Value;
                
                nav = null;
                doc = null;
                stream.Close();
                stream = null;

                return Boolean.Parse(sValue);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace);
                return false;
            }
        }
    }
}
