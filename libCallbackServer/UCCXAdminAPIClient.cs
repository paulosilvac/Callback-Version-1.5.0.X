using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class UCCXAdminAPIClient
    {
        String _UCCXMasterNode = String.Empty;
        String _UCCXUsername = String.Empty;
        String _UCCXPassword = String.Empty;

        String sResponse = String.Empty;

        public UCCXAdminAPIClient()
        {
            _UCCXMasterNode = String.Empty;
            _UCCXUsername = String.Empty;
            _UCCXPassword = String.Empty;
        }

        public UCCXAdminAPIClient(String UCCXMasterNode, String UCCXUsername, String UCCXPassword)
        {
            _UCCXMasterNode = UCCXMasterNode;
            _UCCXUsername = UCCXUsername;
            _UCCXPassword = UCCXPassword;
        }

        public String GetVoiceCSQList()
        {
            try
            {
                if (_UCCXMasterNode == null)
                {
                    Trace.TraceWarning("_UCCXMasterNode is null.");
                    return null;
                }

                if (_UCCXMasterNode == String.Empty)
                {
                    Trace.TraceWarning("_UCCXMasterNode is empty.");
                    return null;
                }

                if (_UCCXUsername == null)
                {
                    Trace.TraceWarning("_UCCXUsername is null.");
                    return null;
                }

                if (_UCCXUsername == String.Empty)
                {
                    Trace.TraceWarning("_UCCXUsername is empty.");
                    return null;
                }

                if (_UCCXPassword == null)
                {
                    Trace.TraceWarning("_UCCXPassword is null.");
                    return null;
                }

                if (_UCCXPassword == String.Empty)
                {
                    Trace.TraceWarning("_UCCXPassword is empty.");
                    return null;
                }

                if (!SendRequest("http://" + _UCCXMasterNode + "/adminapi/csq"))
                {
                    Trace.TraceWarning("SendRequest() returned false.");
                    return null;
                }

                StringBuilder sbCSQList = new StringBuilder();

                sbCSQList.Append("<csqs>");

                System.Xml.XmlDocument xmlDoc = null;

                xmlDoc = new System.Xml.XmlDocument();

                xmlDoc.LoadXml(sResponse);

                System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(xmlDoc);

                String _CSQID = String.Empty;
                String _CSQName = String.Empty;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:

                            if (reader.Name.Equals("id"))
                            {
                                _CSQID = reader.ReadString();
                            }

                            if (reader.Name.Equals("name"))
                            {
                                _CSQName = reader.ReadString();
                            }

                            if (reader.Name.Equals("queueType"))
                            {
                                if (reader.ReadString() == "VOICE")
                                {
                                    sbCSQList.Append("<csq>");
                                    sbCSQList.Append("<id>" + _CSQID + "</id>");
                                    sbCSQList.Append("<name>" + _CSQName + "</name>");
                                    sbCSQList.Append("</csq>");

                                    _CSQID = String.Empty;
                                    _CSQName = String.Empty;
                                }
                            }

                            break;

                        case System.Xml.XmlNodeType.EndElement:

                            break;
                    }
                }

                reader = null;
                xmlDoc = null;

                sbCSQList.Append("</csqs>");

                return sbCSQList.ToString();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace);

                return null;
            }
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

                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(_UCCXUsername + ":" + _UCCXPassword)));

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

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
    }
}
