using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class ContactServiceQueueInformationClient
    {
        String _UCCXNode = String.Empty;
        String _UCCXApplicationPort = String.Empty;
        String _UCCXRealtimePrefix = String.Empty;

        String sResponse = String.Empty;

        public ContactServiceQueueInformation _Information = null;

        public String UCCXNode
        {
            set { _UCCXNode = value; }
        }

        public String UCCXApplicationPort
        {
            set { _UCCXApplicationPort = value; }
        }

        public String UCCXRealtimePrefix
        {
            set { _UCCXRealtimePrefix = value; }
        }

        public ContactServiceQueueInformationClient()
        {
            _UCCXNode = String.Empty;
            _UCCXApplicationPort = String.Empty;
            _UCCXRealtimePrefix = String.Empty;
            sResponse = String.Empty;
            _Information = null;
        }

        public ContactServiceQueueInformationClient(String UCCXNode, String UCCXApplicationPort, String UCCXRealtimePrefix)
        {
            _UCCXNode = UCCXNode;
            _UCCXApplicationPort = UCCXApplicationPort;
            _UCCXRealtimePrefix = UCCXRealtimePrefix;
            sResponse = String.Empty;
            _Information = null;
        }

        public bool GetRealtimeData(String CSQ)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (CSQ == null)
                {
                    Trace.TraceWarning("CSQ is null.");
                    return false;
                }

                if (CSQ == String.Empty)
                {
                    Trace.TraceWarning("CSQ is empty.");
                    return false;
                }
                if (_UCCXNode == String.Empty)
                {
                    Trace.TraceWarning("_UCCXNode is empty.");
                    return false;
                }

                if (_UCCXApplicationPort == String.Empty)
                {
                    Trace.TraceWarning("__UCCXApplicationPort is empty.");
                    return false;
                }

                if (_UCCXRealtimePrefix == String.Empty)
                {
                    Trace.TraceWarning("_UCCXRealtimePrefix is empty.");
                    return false;
                }

                if (!GetResponseFromServer(CSQ))
                {
                    Trace.TraceWarning("GetResponseFromServer() returned false.");
                    return false;
                }

                if (!ParseResponse())
                {
                    Trace.TraceWarning("ParseResponse() returned false.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                sResponse = String.Empty;
                return false;
            }
        }

        private bool GetResponseFromServer(String CSQ)
        {
            try
            {
                String sURL = String.Empty;

                sURL = "http://" + _UCCXNode + ":" + _UCCXApplicationPort + "/" + _UCCXRealtimePrefix + "?CSQ=" + CSQ;

                Trace.TraceInformation("sURL = " + sURL);

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sURL);

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
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                sResponse = String.Empty;
                return false;
            }
        }

        private bool ParseResponse()
        {
            try
            {
                System.Xml.XmlDocument xmlDoc = null;

                xmlDoc = new System.Xml.XmlDocument();

                xmlDoc.LoadXml(sResponse);

                System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(xmlDoc);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:

                            if (reader.Name.Equals("CSQ"))
                            {
                                _Information = new ContactServiceQueueInformation();
                                _Information.Name = reader.GetAttribute("name");
                            }

                            if (reader.Name.Equals("LoggedIn"))
                            {
                                try
                                {
                                    _Information.AgentsLoggedIn = int.Parse(reader.ReadString());    
                                }
                                catch (Exception ex)
                                {
                                    _Information.AgentsLoggedIn = -99;
                                }
                                
                            }

                            if (reader.Name.Equals("Talking"))
                            {
                                try
                                {
                                    _Information.AgentsTalking = int.Parse(reader.ReadString());
                                }
                                catch (Exception ex)
                                {
                                    _Information.AgentsTalking = -99;
                                }
                            }

                            if (reader.Name.Equals("Ready"))
                            {
                                try
                                {
                                    _Information.AgentsReady = int.Parse(reader.ReadString());
                                }
                                catch (Exception ex)
                                {
                                    _Information.AgentsReady = -99;
                                }
                            }

                            if (reader.Name.Equals("NotReady"))
                            {
                                try
                                {
                                    _Information.AgentsNotReady = int.Parse(reader.ReadString());
                                }
                                catch (Exception ex)
                                {
                                    _Information.AgentsNotReady = -99;
                                }
                            }

                            if (reader.Name.Equals("Work"))
                            {
                                try
                                {
                                    _Information.AgentsWork = int.Parse(reader.ReadString());
                                }
                                catch (Exception ex)
                                {
                                    _Information.AgentsWork = -99;
                                }
                            }

                            if (reader.Name.Equals("CallsWaiting"))
                            {
                                try
                                {
                                    _Information.ContactsWaiting = int.Parse(reader.ReadString());
                                }
                                catch (Exception ex)
                                {
                                    _Information.ContactsWaiting = -99;
                                }
                            }

                            if (reader.Name.Equals("LongestQueueTime"))
                            {
                                try
                                {
                                    _Information.LongestWaitingContact = int.Parse(reader.ReadString());
                                }
                                catch (Exception ex)
                                {
                                    _Information.LongestWaitingContact = -99;
                                }
                            }

                            //if (reader.Name.Equals("TotalInQueue"))
                            //{
                            //    try
                            //    {
                            //        _Information.TotalInQueue = int.Parse(reader.ReadString());
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        _Information.TotalInQueue = -99;
                            //    }
                            //}

                            break;

                        case System.Xml.XmlNodeType.EndElement:

                            if (reader.Name.Equals("CSQ"))
                            {
                                
                            }

                            break;
                    }

                }

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
