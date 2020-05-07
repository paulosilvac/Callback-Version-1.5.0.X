using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class ContactRealtimeDataClient
    {
        System.Threading.Timer _tmrTick = null;

        DateTime _LastTick = DateTime.Now;
        DateTime _LastRealtimeDataCollectedAt = DateTime.Now;

        ApplicationSettings _Settings = null;

        String sResponse = String.Empty;

        List<ContactQueuedInformation> QueuedContacts = null;
        List<ContactQueuedInformation> ContactsQueuedForQueue = null;

        private object objLock = null;

        public int NumberOfContactsInIVR
        {
            get
            {
                try
                {
                    if (QueuedContacts == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return QueuedContacts.Count;
                    }
                }
                catch
                {
                    return -1;
                }
            }
        }

        public DateTime LastRealtimeDataCollectedAt
        {
            get { return _LastRealtimeDataCollectedAt; }
        }

        public ApplicationSettings Settings
        {
            set { _Settings = value; }
        }

        public ContactRealtimeDataClient()
        {
            _Settings = null;
            sResponse = String.Empty;
            QueuedContacts = new List<ContactQueuedInformation>();
            ContactsQueuedForQueue = new List<ContactQueuedInformation>();

            objLock = new object();

            _LastTick = DateTime.ParseExact("01/01/1900 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            _LastRealtimeDataCollectedAt = DateTime.ParseExact("01/01/1900 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            _tmrTick = new System.Threading.Timer(_tmrTick_Tick);
        }

        public ContactRealtimeDataClient(ApplicationSettings Settings)
        {
            _Settings = Settings;
            sResponse = String.Empty;
            QueuedContacts = new List<ContactQueuedInformation>();
            ContactsQueuedForQueue = new List<ContactQueuedInformation>();

            objLock = new object();

            _LastTick = DateTime.ParseExact("01/01/1900 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            _LastRealtimeDataCollectedAt = DateTime.ParseExact("01/01/1900 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            _tmrTick = new System.Threading.Timer(_tmrTick_Tick);
        }

        public bool Start()
        {
            Trace.TraceInformation("Enter.");

            _LastTick = DateTime.ParseExact("01/01/1900 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            _tmrTick.Change(Constants.CONTACT_REALTIMEDATE_REFRESH, Constants.CONTACT_REALTIMEDATE_REFRESH);

            return true;
        }

        public bool Stop()
        {
            Trace.TraceInformation("Enter.");

            _tmrTick.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            return true;
        }

        private bool GetRealtimeData()
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

                //Trace.TraceInformation("# of contacts queued: " + QueuedContacts.Count);

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                sResponse = String.Empty;
                return false;
            }
        }

        public bool GetContactsQueuedFor(String CSQ)
        {
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

                ContactsQueuedForQueue.Clear();

                if (QueuedContacts.Count == 0)
                {
                    Trace.TraceInformation("QueuedContacts is empty.");
                    return true;
                }

                lock (objLock)
                {
                    foreach (ContactQueuedInformation cqi in QueuedContacts)
                    {
                        if (cqi.IsContactQueuedFor(CSQ))
                        {
                            ContactsQueuedForQueue.Add(cqi);
                        }

                    }//foreach (ContactQueuedInformation cqi in QueuedContacts)

                }//lock (objLock)

                Trace.TraceInformation("Number of contacts queueing for " + CSQ + ":" + ContactsQueuedForQueue.Count);

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                return false;
            }
        }

        public bool OrderContactsQueuedFor()
        {
            try
            {
                if (ContactsQueuedForQueue == null)
                {
                    Trace.TraceWarning("ContactsQueuedForQueue is null.");
                    return false;
                }

                if (ContactsQueuedForQueue.Count == 0)
                {
                    Trace.TraceWarning("ContactsQueuedForQueue is empty.");
                    return true;
                }

                ContactsQueuedForQueue.Sort();

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                return false;
            }
        }

        public int GetNumberOfContactAheadOf(String ContactID)
        {
            try
            {
                if (ContactsQueuedForQueue == null)
                {
                    Trace.TraceWarning("ContactsQueuedForQueue is null.");
                    return 0;
                }

                if (ContactsQueuedForQueue.Count == 0)
                {
                    Trace.TraceWarning("ContactsQueuedForQueue is empty.");
                    return 0;
                }

                int iCount = 0;

                foreach (ContactQueuedInformation cqi in ContactsQueuedForQueue)
                {
                    if (cqi.ID.CompareTo(ContactID) == -1)
                    {
                        iCount++;
                    }

                }//foreach (ContactQueuedInformation cqi in QueuedContacts)

                return iCount;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                return -1;
            }
        }

        public int GetNumberOfContactAheadOf(String ContactID, String RecordID, long QueueStartTime)
        {
            try
            {
                if (ContactsQueuedForQueue == null)
                {
                    Trace.TraceWarning("ContactsQueuedForQueue is null.");
                    return -1;
                }

                //DateTime ContactQueueStartTime = (new DateTime(1970, 1, 1)).ToLocalTime().AddMilliseconds(QueueStartTime);
                DateTime ContactQueueStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                //Trace.TraceInformation("Record " + RecordID + " ContactQueueStartTime(UTC):" + ContactQueueStartTime);

                ContactQueueStartTime = ContactQueueStartTime.AddMilliseconds(QueueStartTime);

                //Trace.TraceInformation("Record " + RecordID + " QueueStartTime:" + QueueStartTime);
                //Trace.TraceInformation("Record " + RecordID + " ContactQueueStartTime(UTC+QueueStartTime):" + ContactQueueStartTime);

                ContactQueueStartTime = ContactQueueStartTime.ToLocalTime();

                //Trace.TraceInformation("Record " + RecordID + " ContactQueueStartTime(LocalTime):" + ContactQueueStartTime + " Now:" + DateTime.Now);

                if (ContactQueueStartTime.Subtract(DateTime.Now).TotalSeconds > 0)
                {
                    Trace.TraceWarning("Record " + RecordID + " Appears to be scheduled to a future time: " + ContactQueueStartTime.ToString());

                    return -2;
                }

                if (ContactsQueuedForQueue.Count == 0)
                {
                    Trace.TraceWarning("Record " + RecordID + " ContactsQueuedForQueue is empty.");
                    return 0;
                }

                int iCount = 0;

                foreach (ContactQueuedInformation cqi in ContactsQueuedForQueue)
                {
                    if (cqi.ID != ContactID)
                    {
                        String sTMPContactQueueStartTime = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year + " " + cqi.StartTime;

                        DateTime dtTMPContactQueueStartTime = DateTime.Parse(sTMPContactQueueStartTime);

                        //Trace.TraceInformation("Compare for tmp contact ID:" + cqi.ID + " " + dtTMPContactQueueStartTime.ToString() + " ContactID:" + ContactID + " " + ContactQueueStartTime.ToString());

                        if (ContactQueueStartTime.Subtract(dtTMPContactQueueStartTime).TotalSeconds >= 0)
                        {
                            iCount++;
                        }
                    }

                }//foreach (ContactQueuedInformation cqi in ContactsQueuedForQueue)

                //

                return iCount;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                return -1;
            }
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

                    Trace.TraceInformation("GetRealtimeData() returned true. # of contacts collected " + QueuedContacts.Count + " in " + DateTime.Now.Subtract(_LastTick).TotalMilliseconds + " ms");
                }
                else
                {
                    Trace.TraceWarning("GetRealtimeData() returned false;");
                }

                _tmrTick.Change(Constants.CONTACT_REALTIMEDATE_REFRESH, Constants.CONTACT_REALTIMEDATE_REFRESH);

            }//lock (objLock)
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

                sURL = sURL + "/uccxrealtimedata?operation=getcontactdata&token=" + sToken;
                //sURL = sURL + "/uccxrealtimedata?operation=getcontactdata&token=" + sToken + "&testing=true&numberofrecords=150&avgresponsedelay=5000";

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

                QueuedContacts.Clear();

                ContactQueuedInformation _con = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:

                            if (reader.Name.Equals("contact"))
                            {
                                _con = new ContactQueuedInformation();
                            }

                            if (reader.Name.Equals("contactid"))
                            {
                                _con.ID = reader.ReadString();
                            }

                            if (reader.Name.Equals("type"))
                            {
                                _con.Type = reader.ReadString();
                            }

                            if (reader.Name.Equals("implid"))
                            {
                                _con.ImplID = reader.ReadString();
                            }

                            if (reader.Name.Equals("starttime"))
                            {
                                _con.StartTime = reader.ReadString();
                            }

                            if (reader.Name.Equals("duration"))
                            {
                                _con.Duration = reader.ReadString();
                            }

                            if (reader.Name.Equals("application"))
                            {
                                _con.Application = reader.ReadString();
                            }

                            if (reader.Name.Equals("task"))
                            {
                                _con.Task = reader.ReadString();
                            }

                            if (reader.Name.Equals("session"))
                            {
                                _con.Session = reader.ReadString();
                            }

                            if (reader.Name.Equals("queuedfor"))
                            {
                                _con.QueuedFor = reader.ReadString();
                            }

                            break;

                        case System.Xml.XmlNodeType.EndElement:

                            if (reader.Name.Equals("contact"))
                            {
                                QueuedContacts.Add(_con);
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
