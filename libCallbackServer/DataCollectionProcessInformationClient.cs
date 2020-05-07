using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace com.workflowconcepts.applications.uccx
{
    class DataCollectionProcessInformationClient
    {
        ApplicationSettings _ApplicationSettings = null;

        XmlDocument xmlDoc = null;

        public DataCollectionProcessInformationClient(ApplicationSettings Settings)
        {
            _ApplicationSettings = Settings;
        }

        public bool GetInformation()
        {
            try
            {
                String sURL = String.Empty;

                sURL = "http://" + _ApplicationSettings.WebServerIP + ":" + _ApplicationSettings.WebServerDataCollectionPort + "/" + "uccxrealtimedata" + "?operation=systemstatus";

                Trace.TraceInformation("sURL = " + sURL);

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sURL);

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                Trace.TraceInformation("Content type is {0} and length is {1}", response.ContentType, response.ContentLength);

                System.IO.Stream stream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                System.IO.StreamReader streamReader = new System.IO.StreamReader(stream, Encoding.UTF8);

                String sResponse = streamReader.ReadToEnd();

                streamReader.Close();
                streamReader.Dispose();
                streamReader = null;

                stream.Close();
                stream.Dispose();
                stream = null;

                response.Close();
                response = null;
                request = null;

                xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(sResponse);

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        public String JVM()
        {
            try
            {
                if (xmlDoc == null)
                {
                    Trace.TraceWarning("xmlDoc is null.");
                    return "ERROR";
                }

                XmlNode n = xmlDoc.SelectSingleNode("//response/jre");

                if (n != null)
                {
                    return n.InnerText;
                }
                else
                {
                    return "ERROR";
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return "ERROR";
            }
        }

        public String StartupTime()
        {
            try
            {
                if (xmlDoc == null)
                {
                    Trace.TraceWarning("xmlDoc is null.");
                    return "ERROR";
                }

                XmlNode n = xmlDoc.SelectSingleNode("//response/processstartedon");

                if (n != null)
                {
                    double dStartupMiliSeconds = Double.Parse(n.InnerText);

                    DateTime d = new DateTime(1970, 1, 1, 0, 0, 0);

                    double dTimezoneOffset = d.ToUniversalTime().Subtract(d).TotalMilliseconds;

                    d = d.AddMilliseconds(dStartupMiliSeconds);

                    if (TimeZoneInfo.Local.IsDaylightSavingTime(d))
                    {
                        d = d.AddMilliseconds(-1.0 * dTimezoneOffset).AddMilliseconds(3600000);
                    }
                    else
                    {
                        d = d.AddMilliseconds(-1.0 * dTimezoneOffset);
                    }

                    return d.ToString();
                }
                else
                {
                    return "ERROR";
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return "ERROR";
            }
        }

        public String UCCXMasterNode()
        {
            try
            {
                if (xmlDoc == null)
                {
                    Trace.TraceWarning("xmlDoc is null.");
                    return "ERROR";
                }

                XmlNode n = xmlDoc.SelectSingleNode("//response/masteruccxnodedetected");

                if (n == null)
                {   
                    return "ERROR";
                }

                bool bUCCXMasterNodeDetected = Boolean.Parse(n.InnerText);

                if (!bUCCXMasterNodeDetected)
                {
                    return "NOT_DETECTED";
                }

                n = xmlDoc.SelectSingleNode("//response/masteruccxnodeip");

                if (n == null)
                {
                    return "ERROR";
                }

                return n.InnerText;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return "ERROR";
            }
        }

        public String ContactsSummary()
        {
            try
            {
                if (xmlDoc == null)
                {
                    Trace.TraceWarning("xmlDoc is null.");
                    return "ERROR";
                }

                XmlNode n = xmlDoc.SelectSingleNode("//response/contactsrealtimedata/success");

                if (n == null)
                {
                    return "ERROR";
                }

                bool bSuccess = Boolean.Parse(n.InnerText);

                if (!bSuccess)
                {
                    return "FAILED";
                }

                n = xmlDoc.SelectSingleNode("//response/contactsrealtimedata/numberofrecords");

                if (n == null)
                {
                    return "ERROR";
                }

                String sNumberOfContacts = n.InnerText;

                n = xmlDoc.SelectSingleNode("//response/contactsrealtimedata/operationduration");

                if (n == null)
                {
                    return "ERROR";
                }

                String sOperationDuration = n.InnerText;

                return sNumberOfContacts + " records in " + sOperationDuration + " ms";
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return "ERROR";
            }
        }

        public String CSQsSummary()
        {
            try
            {
                if (xmlDoc == null)
                {
                    Trace.TraceWarning("xmlDoc is null.");
                    return "ERROR";
                }

                XmlNode n = xmlDoc.SelectSingleNode("//response/csqrealtimedata/success");

                if (n == null)
                {
                    return "ERROR";
                }

                bool bSuccess = Boolean.Parse(n.InnerText);

                if (!bSuccess)
                {
                    return "FAILED";
                }

                n = xmlDoc.SelectSingleNode("//response/csqrealtimedata/numberofrecords");

                if (n == null)
                {
                    return "ERROR";
                }

                String sNumberOfContacts = n.InnerText;

                n = xmlDoc.SelectSingleNode("//response/csqrealtimedata/operationduration");

                if (n == null)
                {
                    return "ERROR";
                }

                String sOperationDuration = n.InnerText;

                return sNumberOfContacts + " records in " + sOperationDuration + " ms";
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return "ERROR";
            }
        }
    }
}
