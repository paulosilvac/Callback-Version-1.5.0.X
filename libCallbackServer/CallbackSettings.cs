using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackSettings
    {
        System.Xml.XmlDocument xmlDoc = null;

        private List<CallbackContactServiceQueue> _Queues = null;

        public List<CallbackContactServiceQueue> Queues
        {
            get { return _Queues; }
        }

        public CallbackSettings()
        {
            _Queues = new List<CallbackContactServiceQueue>();
        }

        public bool ReadFromDisk()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                xmlDoc = new System.Xml.XmlDocument();

                bool bXMLLoaded = false;

                if (System.IO.File.Exists(Constants.ApplicationSettingsFilePath + "\\CallbackSettings.xml"))
                {
                    Trace.TraceInformation(Constants.ApplicationSettingsFilePath + "\\CallbackSettings.xml" + " was found.");

                    try
                    {
                        xmlDoc.Load(Constants.ApplicationSettingsFilePath + "\\CallbackSettings.xml");

                        bXMLLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        bXMLLoaded = false;
                    }
                    
                }
                else
                {
                    Trace.TraceWarning(Constants.ApplicationSettingsFilePath + "\\CallbackSettings.xml" + " was not found; load default callback settings.");

                    try
                    {
                        xmlDoc.LoadXml(com.workflowconcepts.applications.uccx.Properties.Resources.Callback_Settings);

                        bXMLLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        bXMLLoaded = false;
                    }

                }

                if (bXMLLoaded)
                {
                    Trace.TraceInformation("XML was loaded successfully.");

                    return ParseCallbackSettingsXML();
                }
                else
                {
                    Trace.TraceWarning("XML failed to load.");

                    try
                    {
                        xmlDoc.LoadXml(com.workflowconcepts.applications.uccx.Properties.Resources.Callback_Settings);

                        bXMLLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        bXMLLoaded = false;
                    }

                    if (bXMLLoaded)
                    {
                        Trace.TraceInformation("Default XML was loaded successfully.");

                        return ParseCallbackSettingsXML();
                    }
                    else
                    {
                        Trace.TraceWarning("Default XML failed to load.");

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                xmlDoc = null;
                return false;
            }
        }

        public bool ReadFromString(String CallbackSettings)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                xmlDoc = new System.Xml.XmlDocument();

                xmlDoc.LoadXml(CallbackSettings);

                return ParseCallbackSettingsXML();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                xmlDoc = null;
                return false;
            }
        }

        public bool WriteToDisk()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                System.IO.MemoryStream ms = WriteSettingsToMemoryStream(String.Empty,true);

                if (ms != null)
                {
                    ms.Position = 0;

                    using (System.IO.FileStream fs = new System.IO.FileStream(Constants.ApplicationSettingsFilePath + "\\CallbackSettings.xml", System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        ((System.IO.MemoryStream)ms).WriteTo(fs);
                    }

                    Trace.TraceInformation("Settings successfully written to file.");

                    return true;
                }
                else
                {
                    Trace.TraceWarning("Could not write settings to file.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                xmlDoc = null;
                return false;
            }
        }

        public String GetXML(String AppServerURL)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                System.IO.MemoryStream ms = WriteSettingsToMemoryStream(AppServerURL, false);

                if (ms != null)
                {
                    String sSettings = String.Empty;

                    ms.Position = 0;

                    using (System.IO.StreamReader sr = new System.IO.StreamReader(ms))
                    {
                        sSettings = sr.ReadToEnd();
                    }

                    return sSettings;
                }
                else
                {
                    Trace.TraceWarning("Could not write settings to file.");
                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return String.Empty;
            }
        }

        private System.IO.MemoryStream WriteSettingsToMemoryStream(String AppServerURL, bool IncludeXMLDeclaration)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (_Queues == null)
                {
                    Trace.TraceWarning("_Queues is null.");
                    return null;
                }

                System.IO.Stream ms = new System.IO.MemoryStream();

                System.Xml.XmlWriterSettings _XMLSettings = new System.Xml.XmlWriterSettings();

                _XMLSettings.Indent = true;
                _XMLSettings.OmitXmlDeclaration = !IncludeXMLDeclaration;

                System.Xml.XmlWriter _XMLWriter = System.Xml.XmlWriter.Create(ms, _XMLSettings);

                _XMLWriter.WriteStartDocument();

                _XMLWriter.WriteStartElement("callback");
                _XMLWriter.WriteStartElement("csqs");

                foreach (CallbackContactServiceQueue csq in _Queues)
                {
                    _XMLWriter.WriteStartElement("csq");
                    _XMLWriter.WriteAttributeString("name", csq.Name);

                    _XMLWriter.WriteStartElement("CallbackEnabled");
                    _XMLWriter.WriteValue(csq.CallbackEnabled.ToString());
                    _XMLWriter.WriteFullEndElement();//CallbackEnabled

                    if (csq.Profile != null)
                    {
                        _XMLWriter.WriteStartElement("CallerRecording");
                        _XMLWriter.WriteValue(csq.Profile.CallerRecording.ToString());
                        _XMLWriter.WriteFullEndElement();//CallerRecording

                        _XMLWriter.WriteStartElement("RetentionPeriod");
                        _XMLWriter.WriteValue(csq.Profile.RetentionPeriod.ToString());
                        _XMLWriter.WriteFullEndElement();//RetentionPeriod

                        _XMLWriter.WriteStartElement("AppServerURLPrefix");
                        _XMLWriter.WriteValue(AppServerURL);
                        _XMLWriter.WriteFullEndElement();//AppServerURLPrefix


                        _XMLWriter.WriteStartElement("EmailAlerts");
                        _XMLWriter.WriteValue(csq.Profile.EmailAlerts.ToString());
                        _XMLWriter.WriteFullEndElement();//EmailAlerts

                        _XMLWriter.WriteStartElement("AdminEmail");
                        _XMLWriter.WriteValue(csq.Profile.AdminEmail);
                        _XMLWriter.WriteFullEndElement();//AdminEmail

                        _XMLWriter.WriteStartElement("CallerIDVerify");
                        _XMLWriter.WriteValue(csq.Profile.CallerIDVerify);
                        _XMLWriter.WriteFullEndElement();//CallerIDVerify

                        _XMLWriter.WriteStartElement("AbandonCallback");
                        _XMLWriter.WriteValue(csq.Profile.AbandonCallback);
                        _XMLWriter.WriteFullEndElement();//AbandonCallback

                        _XMLWriter.WriteStartElement("AbandonCBMinQTime");
                        _XMLWriter.WriteValue(csq.Profile.AbandonCBMinQTime);
                        _XMLWriter.WriteFullEndElement();//AbandonCBMinQTime

                        _XMLWriter.WriteStartElement("AbandonCBMinInterCallTime");
                        _XMLWriter.WriteValue(csq.Profile.AbandonCBMinInterCallTime);
                        _XMLWriter.WriteFullEndElement();//AbandonCBMinInterCallTime

                        foreach (CallbackBackupCSQ bckCSQ in csq.Profile.BackupCSQs)
                        {
                            _XMLWriter.WriteStartElement("CBQueue");
                            _XMLWriter.WriteAttributeString("csq", bckCSQ.Name);
                            _XMLWriter.WriteAttributeString("overflowtime", bckCSQ.OverflowTime.ToString());
                            _XMLWriter.WriteFullEndElement();//CBQueue

                        }//foreach (CallbackBackupCSQ bckCSQ in csq.Profile.BackupCSQs)

                        _XMLWriter.WriteStartElement("AcceptCallbacksTimeframe");
                        _XMLWriter.WriteStartElement("Begin");
                        _XMLWriter.WriteValue(csq.Profile.AcceptCallbacksTimeframeBegin);
                        _XMLWriter.WriteFullEndElement();//Begin
                        _XMLWriter.WriteStartElement("End");
                        _XMLWriter.WriteValue(csq.Profile.AcceptCallbacksTimeframeEnd);
                        _XMLWriter.WriteFullEndElement();//End
                        _XMLWriter.WriteFullEndElement();//AcceptCallbacksTimeframe

                        _XMLWriter.WriteStartElement("CallbackOfferedAlgorithm");

                        foreach (CallbackAlgorithmFilter filter in csq.Profile.OfferedAlgorithmFilters)
                        {
                            _XMLWriter.WriteStartElement(filter.Name);
                            _XMLWriter.WriteAttributeString("Enabled", filter.Enabled.ToString());
                            _XMLWriter.WriteAttributeString("Operation", filter.Operation);
                            _XMLWriter.WriteAttributeString("Value", filter.Value.ToString());
                            _XMLWriter.WriteFullEndElement();

                        }//foreach (CallbackAlgorithmFilter filter in csq.Profile.OfferedAlgorithmFilters)

                        _XMLWriter.WriteFullEndElement();//CallbackOfferedAlgorithm

                        _XMLWriter.WriteStartElement("CallbackReentryAlgorithm");

                        foreach (CallbackAlgorithmFilter filter in csq.Profile.ReentryAlgorithmFilters)
                        {
                            _XMLWriter.WriteStartElement(filter.Name);
                            _XMLWriter.WriteAttributeString("Enabled", filter.Enabled.ToString());
                            _XMLWriter.WriteAttributeString("Operation", filter.Operation);
                            _XMLWriter.WriteAttributeString("Value", filter.Value.ToString());
                            _XMLWriter.WriteFullEndElement();

                        }//foreach (CallbackReentryAlgorithmFilter filter in csq.Profile.AlgorithmFilters)

                        _XMLWriter.WriteStartElement("CallbackProcessingTimeframe");
                        _XMLWriter.WriteStartElement("Begin");
                        _XMLWriter.WriteValue(csq.Profile.CallbackProcessingTimeframeBegin);
                        _XMLWriter.WriteFullEndElement();//Begin
                        _XMLWriter.WriteStartElement("End");
                        _XMLWriter.WriteValue(csq.Profile.CallbackProcessingTimeframeEnd);
                        _XMLWriter.WriteFullEndElement();//End
                        _XMLWriter.WriteFullEndElement();//CallbackProcessingTimeframe


                        _XMLWriter.WriteStartElement("EndOfDayPurgeCallbackRequests");
                        _XMLWriter.WriteValue(csq.Profile.EndOfDayPurgeCallbackRequests);
                        _XMLWriter.WriteFullEndElement();//EndOfDayPurgeCallbackRequests

                        _XMLWriter.WriteFullEndElement();//CallbackReentryAlgorithm
                    }

                    _XMLWriter.WriteFullEndElement();//CSQ

                }//foreach (CallbackContactServiceQueue csq in _Queues)

                _XMLWriter.WriteFullEndElement();//csqs
                _XMLWriter.WriteFullEndElement();//callback

                _XMLWriter.WriteEndDocument();

                _XMLWriter.Flush();
                _XMLWriter.Close();
                _XMLWriter = null;

                _XMLSettings = null;

                return (System.IO.MemoryStream)ms;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                xmlDoc = null;
                return null;
            }
        }   

        private bool ParseCallbackSettingsXML()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (xmlDoc == null)
                {
                    Trace.TraceWarning("xmlDoc is null.");
                    return false;
                }

                System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(xmlDoc);

                List<CallbackContactServiceQueue> _TempQueues = new List<CallbackContactServiceQueue>();

                CallbackContactServiceQueue _cbCSQ = null;
                CallbackContactServiceQueueSettingsProfile _profile = null;

                CallbackBackupCSQ _bckCSQ = null;
                CallbackAlgorithmFilter _filter = null;

                bool bAcceptCallbacksTimeframeElementEnded = true;
                bool bCallbackProcessingTimeframeEnded = true;

                bool bCallbackReentryAlgorithmElementBegan = false;
                bool bCallbackOfferedAlgorithmElementBegan = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:

                            if (reader.Name.Equals("csq"))
                            {
                                _cbCSQ = new CallbackContactServiceQueue();

                                _cbCSQ.Name = reader.GetAttribute("name");

                                bCallbackReentryAlgorithmElementBegan = false;
                                bCallbackOfferedAlgorithmElementBegan = false;
                            }

                            if (reader.Name.Equals("CallbackEnabled"))
                            {
                                try
                                {
                                    _cbCSQ.CallbackEnabled = bool.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _cbCSQ.CallbackEnabled = false;
                                }
                            }

                            if (reader.Name.Equals("CallerRecording"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                try
                                {
                                    _profile.CallerRecording = bool.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _profile.CallerRecording = false;
                                }
                            }

                            if (reader.Name.Equals("RetentionPeriod"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                try
                                {
                                    _profile.RetentionPeriod = int.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _profile.RetentionPeriod = 7;
                                }
                            }

                            if (reader.Name.Equals("AppServerURLPrefix"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                _profile.AppServerURLPrefix = reader.ReadString();
                            }

                            if (reader.Name.Equals("EmailAlerts"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                try
                                {
                                    _profile.EmailAlerts = bool.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _profile.EmailAlerts = false;
                                }
                            }

                            if (reader.Name.Equals("AdminEmail"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                _profile.AdminEmail = reader.ReadString();
                            }

                            if (reader.Name.Equals("CallerIDVerify"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                try
                                {
                                    _profile.CallerIDVerify = bool.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _profile.CallerIDVerify = false;
                                }
                            }

                            if (reader.Name.Equals("AbandonCallback"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                try
                                {
                                    _profile.AbandonCallback = bool.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _profile.AbandonCallback = false;
                                }
                            }

                            if (reader.Name.Equals("AbandonCBMinQTime"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                try
                                {
                                    _profile.AbandonCBMinQTime = int.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _profile.AbandonCBMinQTime = 0;
                                }
                            }

                            if (reader.Name.Equals("AbandonCBMinInterCallTime"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                try
                                {
                                    _profile.AbandonCBMinInterCallTime = int.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _profile.AbandonCBMinInterCallTime = 0;
                                }
                            }

                            if (reader.Name.Equals("CBQueue"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                _bckCSQ = new CallbackBackupCSQ();

                                _bckCSQ.Name = reader.GetAttribute("csq");

                                try
                                {
                                    _bckCSQ.OverflowTime = int.Parse(reader.GetAttribute("overflowtime"));
                                }
                                catch
                                {
                                    _bckCSQ.OverflowTime = 0;
                                }

                                _profile.BackupCSQs.Add(_bckCSQ);

                                _bckCSQ = null;
                            }

                            if (reader.Name.Equals("AcceptCallbacksTimeframe"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                bAcceptCallbacksTimeframeElementEnded = false;
                            }

                            if (reader.Name.Equals("Begin"))
                            {
                                if (!bAcceptCallbacksTimeframeElementEnded)
                                {
                                    _profile.AcceptCallbacksTimeframeBegin = reader.ReadString();
                                }

                                if (!bCallbackProcessingTimeframeEnded)
                                {
                                    _profile.CallbackProcessingTimeframeBegin = reader.ReadString();
                                }
                            }

                            if (reader.Name.Equals("End"))
                            {
                                if (!bAcceptCallbacksTimeframeElementEnded)
                                {
                                    _profile.AcceptCallbacksTimeframeEnd = reader.ReadString();
                                }

                                if (!bCallbackProcessingTimeframeEnded)
                                {
                                    _profile.CallbackProcessingTimeframeEnd = reader.ReadString();
                                }
                            }

                            if (reader.Name.Equals("CallbackOfferedAlgorithm"))
                            {
                                bCallbackOfferedAlgorithmElementBegan = true;
                            }

                            if (reader.Name.Equals("CallbackReentryAlgorithm"))
                            {
                                bCallbackReentryAlgorithmElementBegan = true;    
                            }

                            if (reader.Name.Equals("TotalInQueue") 
                                || reader.Name.Equals("CSQAgentsReady") 
                                || reader.Name.Equals("CSQCallsWaiting")
                                || reader.Name.Equals("AgentsLoggedIn")
                                || reader.Name.Equals("CallsWaiting")
                                || reader.Name.Equals("LongestQueueTime")
                                || reader.Name.Equals("CallbackRequests"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                _filter = new CallbackAlgorithmFilter();

                                _filter.Name = reader.Name;

                                try
                                {
                                    _filter.Enabled = bool.Parse(reader.GetAttribute("Enabled"));
                                }
                                catch
                                {
                                    _filter.Enabled = false;
                                }

                                _filter.Operation = reader.GetAttribute("Operation");

                                try
                                {
                                    _filter.Value = int.Parse(reader.GetAttribute("Value"));
                                }
                                catch
                                {
                                    _filter.Value = 0;
                                }

                                if (bCallbackOfferedAlgorithmElementBegan)
                                {
                                    _profile.OfferedAlgorithmFilters.Add(_filter);
                                }

                                if(bCallbackReentryAlgorithmElementBegan)
                                {
                                    _profile.ReentryAlgorithmFilters.Add(_filter);
                                }

                                _filter = null;
                            }

                            if (reader.Name.Equals("CallbackProcessingTimeframe"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                bCallbackProcessingTimeframeEnded = false;
                            }

                            if (reader.Name.Equals("EndOfDayPurgeCallbackRequests"))
                            {
                                if (_profile == null)
                                {
                                    _profile = new CallbackContactServiceQueueSettingsProfile();
                                }

                                try
                                {
                                    _profile.EndOfDayPurgeCallbackRequests = bool.Parse(reader.ReadString());
                                }
                                catch
                                {
                                    _profile.EndOfDayPurgeCallbackRequests = false;
                                }
                            }

                            break;

                        case System.Xml.XmlNodeType.EndElement:

                            if (reader.Name.Equals("csq"))
                            {
                                if (_cbCSQ != null)
                                {
                                    _cbCSQ.Profile = _profile;
                                    _TempQueues.Add(_cbCSQ);
                                }
                                else
                                {
                                    Trace.TraceWarning("CSQ was not added to list because _cbCSQ is null.");
                                }

                                _profile = null;
                                _cbCSQ = null;
                            }

                            if (reader.Name.Equals("CallbackReentryAlgorithm"))
                            {
                                bCallbackReentryAlgorithmElementBegan = false;
                            }

                            if (reader.Name.Equals("AcceptCallbacksTimeframe"))
                            {
                                bAcceptCallbacksTimeframeElementEnded = true;
                            }

                            if (reader.Name.Equals("CallbackOfferedAlgorithm"))
                            {
                                bCallbackOfferedAlgorithmElementBegan = false;
                            }

                            if (reader.Name.Equals("CallbackProcessingTimeframe"))
                            {
                                bCallbackProcessingTimeframeEnded = true;
                            }

                            break;

                    }//switch (reader.NodeType)

                }//while (reader.Read())

                reader.Close();
                reader = null;

                xmlDoc = null;

                _Queues = _TempQueues;

                _TempQueues = null;
                _profile = null;
                _cbCSQ = null;

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                xmlDoc = null;
                return false;
            }
        }
    }
}
