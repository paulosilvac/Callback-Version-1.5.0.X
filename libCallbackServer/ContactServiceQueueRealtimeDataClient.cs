using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class ContactServiceQueueRealtimeDataClient
    {
        System.Threading.Timer _tmrTick = null;

        DateTime _LastTick = DateTime.Now;
        DateTime _LastRealtimeDataCollectedAt = DateTime.Now;

        ApplicationSettings _Settings = null;

        String sResponse = String.Empty;

        public List<ContactServiceQueueInformation> CSQInfo = null;

        private object objLock = null;

        public ApplicationSettings Settings
        {
            get { return _Settings; }
            set { _Settings = value; }
        }

        public DateTime LastRealtimeDataCollectedAt
        {
            get { return _LastRealtimeDataCollectedAt; }
        }

        public ContactServiceQueueRealtimeDataClient()
        {
            _Settings = null;
            sResponse = String.Empty;
            CSQInfo = new List<ContactServiceQueueInformation>();
            objLock = new object();
            
            _LastRealtimeDataCollectedAt = DateTime.MinValue;

            _tmrTick = new System.Threading.Timer(_tmrTick_Tick);
        }

        public ContactServiceQueueRealtimeDataClient(ApplicationSettings Settings)
        {
            _Settings = Settings;
            sResponse = String.Empty;
            CSQInfo = new List<ContactServiceQueueInformation>();
            objLock = new object();

            _LastRealtimeDataCollectedAt = DateTime.MinValue;

            _tmrTick = new System.Threading.Timer(_tmrTick_Tick);
        }

        public bool Start()
        {
            Trace.TraceInformation("Enter.");

            _tmrTick.Change(Constants.CONTACT_REALTIMEDATE_REFRESH, Constants.CONTACT_REALTIMEDATE_REFRESH);

            return true;
        }

        public bool Stop()
        {
            Trace.TraceInformation("Enter.");

            _tmrTick.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            return true;
        }

        void _tmrTick_Tick(object State)
        {
            lock (objLock)
            {
                _tmrTick.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                _LastTick = DateTime.Now;

                if (GetRealtimeData())
                {
                    _LastRealtimeDataCollectedAt = DateTime.Now;

                    Trace.TraceInformation("GetRealtimeData() returned true. # of CSQs collected " + CSQInfo.Count + " in " + DateTime.Now.Subtract(_LastTick).TotalMilliseconds + " ms");
                }
                else
                {
                    Trace.TraceWarning("GetRealtimeData() returned false;");
                }

                _tmrTick.Change(Constants.CONTACT_REALTIMEDATE_REFRESH, Constants.CONTACT_REALTIMEDATE_REFRESH);

            }//lock (objLock)
        }

        public bool GetRealtimeData()
        {
            //Trace.TraceInformation("Enter.");

            try
            {
                if (_Settings == null)
                {
                    Trace.TraceWarning("_Settings is null.");
                    return false;
                }

                if (_Settings.WebServerIP == String.Empty)
                {
                    Trace.TraceWarning("_Settings.WebServerIP is empty.");
                    return false;
                }

                if (_Settings.UCCXAdminUser == String.Empty)
                {
                    Trace.TraceWarning("_Settings.UCCXAdminUser is empty.");
                    return false;
                }

                if (_Settings.UCCXAdminPassword == String.Empty)
                {
                    Trace.TraceWarning("_Settings.UCCXAdminPassword is empty.");
                    return false;
                }

                //Trace.TraceWarning("GetResponseFromServer() has been disabled for development....");
                if (!GetResponseFromServer())
                {
                    Trace.TraceWarning("GetResponseFromServer() returned false.");
                    return false;
                }

                //Use only for development
                //sResponse = com.workflowconcepts.applications.uccx.Properties.Resources.RealtimeContactData;

                if (!ParseResponseIntoArraylist())
                {
                    Trace.TraceWarning("ParseResponseIntoArraylist() retruned false.");
                    return false;
                }

                //Trace.TraceInformation("# of contact service queues: " + CSQInfo.Count);

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                sResponse = String.Empty;
                return false;
            }
        }

        private bool GetResponseFromServer()
        {
            try
            {
                String sURL = String.Empty;

                sURL = "http://" + _Settings.WebServerIP;

                if (_Settings.WebServerDataCollectionPort != String.Empty)
                {
                    sURL = sURL + ":" + _Settings.WebServerDataCollectionPort;
                }

                String sToken = System.Text.RegularExpressions.Regex.Replace(_Settings.EncryptedUCCXAdminUser + _Settings.EncryptedUCCXAdminPassword, "[^A-Za-z0-9]", "");

                sURL = sURL + "/uccxrealtimedata?operation=getcsqdata&token=" + sToken;
                //sURL = sURL + "/uccxrealtimedata?operation=getcsqdata&token=" + sToken + "&testing=true&numberofrecords=150&avgresponsedelay=5000";

                Trace.TraceInformation("sURL = " + sURL);

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sURL);

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                //Trace.TraceInformation("Content type is {0} and length is {1}", response.ContentType, response.ContentLength);

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
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                sResponse = String.Empty;
                return false;
            }
        }

        private bool ParseResponseIntoArraylist()
        {
            try
            {
                int iCode = 0;

                System.Xml.XmlDocument xmlDoc = null;

                xmlDoc = new System.Xml.XmlDocument();

                xmlDoc.LoadXml(sResponse);

                System.Xml.XmlNode nodCode = xmlDoc.SelectSingleNode("//response/code");

                if (nodCode != null)
                {
                    try
                    {
                        iCode = int.Parse(nodCode.InnerText);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Exception casting Code to integer; Code defaulted to -1.");
                        iCode = -1;
                    }
                }
                else
                {
                    Trace.TraceWarning("//response/code element not found. Code defaulted to -1");
                    iCode = -1;
                }

                if (iCode == -1)
                {
                    Trace.TraceWarning("//response/code element = -1");
                    nodCode = null;
                    xmlDoc = null;
                    return false;
                }

                nodCode = null;

                System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(xmlDoc);

                CSQInfo.Clear();

                ContactServiceQueueInformation _con = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:

                            if (reader.Name.Equals("csq"))
                            {
                                _con = new ContactServiceQueueInformation();
                            }

                            if (reader.Name.Equals("id"))
                            {
                                _con.ID = reader.ReadString();
                            }

                            if (reader.Name.Equals("name"))
                            {
                                _con.Name = reader.ReadString();
                            }

                            if (reader.Name.Equals("agentsloggedin"))
                            {
                                _con.AgentsLoggedIn = int.Parse(reader.ReadString());
                            }

                            if (reader.Name.Equals("agentsnotready"))
                            {
                                _con.AgentsNotReady = int.Parse(reader.ReadString());
                            }

                            if (reader.Name.Equals("agentsready"))
                            {
                                _con.AgentsReady = int.Parse(reader.ReadString());
                            }

                            if (reader.Name.Equals("agentstalking"))
                            {
                                _con.AgentsTalking = int.Parse(reader.ReadString());
                            }

                            if (reader.Name.Equals("agentswork"))
                            {
                                _con.AgentsWork = int.Parse(reader.ReadString());
                            }

                            if (reader.Name.Equals("contactswaiting"))
                            {
                                _con.ContactsWaiting = int.Parse(reader.ReadString());
                            }

                            if (reader.Name.Equals("longestwaitingcontact"))
                            {
                                _con.LongestWaitingContact = int.Parse(reader.ReadString());
                            }

                            break;

                        case System.Xml.XmlNodeType.EndElement:

                            if (reader.Name.Equals("csq"))
                            {
                                CSQInfo.Add(_con);
                                _con = null;
                            }

                            break;

                    }//switch (reader.NodeType)

                }//while (reader.Read())

                reader = null;
                xmlDoc = null;

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                return false;
            }
        }
    }
}
