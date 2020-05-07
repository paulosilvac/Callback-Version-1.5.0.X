using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace com.workflowconcepts.applications.uccx
{
    public class DataCollectionServiceInformation
    {
        ApplicationSettings _ApplicationSettings = null;

        String _Hash = String.Empty;

        XmlDocument xmlDoc = null;

        public DataCollectionServiceInformation(ApplicationSettings Settings)
        {
            _ApplicationSettings = Settings;
            _Hash = String.Empty;
        }

        public bool GetInformation()
        {
            try
            {
                String sURL = String.Empty;

                com.workflowconcepts.utilities.AESSymmetricEncryption endDec = new com.workflowconcepts.utilities.AESSymmetricEncryption(Constants.ENCRYPTION_PASSWORD, Constants.ENCRYPTION_SALT);

                String sToken = System.Text.RegularExpressions.Regex.Replace(endDec.Encrypt(_ApplicationSettings.UCCXAdminUser) + endDec.Encrypt(_ApplicationSettings.UCCXAdminPassword), "[^A-Za-z0-9]", "");

                endDec = null;

                sURL = "http://" + _ApplicationSettings.WebServerIP + ":" + _ApplicationSettings.WebServerDataCollectionPort + "/" + "uccxrealtimedata" + "?operation=getagentdata&token=" + sToken;

                //Trace.TraceInformation("sURL = " + sURL);

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sURL);

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                //Trace.TraceInformation("Content type is {0} and length is {1}", response.ContentType, response.ContentLength);

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

                System.Security.Cryptography.HashAlgorithm algorithm = System.Security.Cryptography.MD5.Create();  //or use SHA256.Create();
                byte[] bHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(sResponse));

                StringBuilder sb = new StringBuilder();

                foreach (byte b in bHash)
                {
                    sb.Append(b.ToString("X2"));
                }

                _Hash = sb.ToString();

                sb = null;

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _Hash = String.Empty;
                return false;
            }
        }

        public bool AssertNoError()
        {
            try
            {
                if (xmlDoc == null)
                {
                    Trace.TraceWarning("xmlDoc is null.");
                    return false;
                }

                XmlNode n = xmlDoc.SelectSingleNode("//response/code");

                if (n != null)
                {
                    if (n.InnerText == "0")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        public bool AssertNotEmpty()
        {
            try
            {
                if (xmlDoc == null)
                {
                    Trace.TraceWarning("xmlDoc is null.");
                    return false;
                }

                int iNumberOfResources = xmlDoc.SelectNodes("//response/resources/resource").Count;

                if (iNumberOfResources > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        public String GetHash()
        {
            return _Hash;
        }
    }
}
