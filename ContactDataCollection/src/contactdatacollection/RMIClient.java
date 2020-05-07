package contactdatacollection;

import com.cisco.wf.reporting.EsdIAQStatsClient;
import com.cisco.wf.reporting.ResourceIAQStatsClient;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.Document;
import java.util.Calendar;
import org.tanukisoftware.wrapper.WrapperManager;
import java.util.Enumeration;
import java.io.StringWriter;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;
import org.w3c.dom.Element;
import org.w3c.dom.Text;

public class RMIClient 
{
    public enum ConnectionState{Open,Unknown};
    
    private ApplicationSettings _Settings = null;
    
    private com.cisco.wfframework.reporting.api.RMILayer _RMI;
    
    private java.util.Hashtable _ContactRealtimeData = null;
    private java.util.Hashtable _CSQRealtimeData = null;
    private java.util.Hashtable _AgentRealtimeData = null;
    
    public UCCXCluster _Cluster = null;
    
    private EmailNotifier _Notifier = null;
    
    boolean bUseWrapperManager = false;
    
    private String sGetContactsDataHash = "";
    private String sGetCSQDataHash = "";
    
    java.security.MessageDigest md = null;
    
    public RMIClient(UCCXCluster Cluster, EmailNotifier Notifier, ApplicationSettings Settings, boolean UseWrapperManager)
    {
        _Cluster = Cluster;
        _Notifier = Notifier;
        _Settings = Settings;
        
        bUseWrapperManager = UseWrapperManager;
        
        sGetContactsDataHash = "";
        sGetCSQDataHash = "";
        
        try
        {
            md = java.security.MessageDigest.getInstance("SHA-256");
        }
        catch(Exception ex)
        {
            md = null;
            
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".RMIClient(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.println(this.getClass() + ".RMIClient(): Exception:" + ex.getMessage());      
            }
        }
    }
   
    private String CreateHash(String Message)
    {
        try
        {
            if(md == null)
            {
                return "";
            }
            
            if(Message == null)
            {
                return "";
            }
            
            md.update(Message.getBytes());
            
            return new String(md.digest());
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".CreateHash(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.println(this.getClass() + ".CreateHash(): Exception:" + ex.getMessage());      
            }
            
            return "";
        }
    }
    
    public String GetTestContactData(int NumberOfRecords, int AvgResponseDelay)
    {
        try
        {
            Thread.sleep(AvgResponseDelay);
            
            String sReturn = "";
            
            DocumentBuilderFactory dbf = null;
            DocumentBuilder db = null;
            Document doc = null;
            
            Element root = null;
            Element child = null;
            Element grandChild = null;
            
            Text text = null;
            
            dbf = DocumentBuilderFactory.newInstance();
            db = dbf.newDocumentBuilder();
            doc = db.newDocument();
            
            root = doc.createElement("response");
            doc.appendChild(root);
            
            child = doc.createElement("code");
            root.appendChild(child);

            text = doc.createTextNode("0");
            child.appendChild(text);

            child = doc.createElement("description");
            root.appendChild(child);

            text = doc.createTextNode("");
            child.appendChild(text);

            child = doc.createElement("timestamp");
            root.appendChild(child);

            text = doc.createTextNode(String.valueOf((new java.util.Date()).getTime()));
            child.appendChild(text);

            child = doc.createElement("contacts");
            root.appendChild(child);
            
            for(int i = 0; i < NumberOfRecords; i++)
            {
                child = doc.createElement("contact");
                root.appendChild(child);
             
                //contactid
                grandChild = doc.createElement("contactid");
                child.appendChild(grandChild);

                text = doc.createTextNode("12345");
                grandChild.appendChild(text);
                                
                //type
                grandChild = doc.createElement("type");
                child.appendChild(grandChild);

                text = doc.createTextNode("Cisco JTAPI Call");
                grandChild.appendChild(text);
                                
                //implid
                grandChild = doc.createElement("implid");
                child.appendChild(grandChild);

                text = doc.createTextNode("123/1");
                grandChild.appendChild(text);
                         
                //starttime
                grandChild = doc.createElement("starttime");
                child.appendChild(grandChild);

                text = doc.createTextNode("14:26:14");
                grandChild.appendChild(text);
                          
                //duration
                grandChild = doc.createElement("duration");
                child.appendChild(grandChild);

                text = doc.createTextNode("0:0:14");
                grandChild.appendChild(text);
                
                //application
                grandChild = doc.createElement("application");
                child.appendChild(grandChild);

                text = doc.createTextNode("WFC Demo Application");
                grandChild.appendChild(text);
                
                //task
                grandChild = doc.createElement("task");
                child.appendChild(grandChild);

                text = doc.createTextNode("49000107198");
                grandChild.appendChild(text);
                     
                //session
                grandChild = doc.createElement("session");
                child.appendChild(grandChild);

                text = doc.createTextNode("86000000303");
                grandChild.appendChild(text);
                
                //queuedfor
                grandChild = doc.createElement("queuedfor");
                child.appendChild(grandChild);

                text = doc.createTextNode("[Development]]");
                grandChild.appendChild(text);
                                
            }//for(int i = 0; i < NumberOfRecords; i++)
            
            //set up a transformer
            TransformerFactory transfac = TransformerFactory.newInstance();
            Transformer trans = transfac.newTransformer();
            trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
            trans.setOutputProperty(OutputKeys.INDENT, "yes");

            //create string from xml tree
            StringWriter sw = new StringWriter();
            StreamResult result = new StreamResult(sw);
            DOMSource source = new DOMSource(doc);
            trans.transform(source, result);
            
            sReturn = sw.toString();
            
            sw.close();
            sw = null;
            
            result = null;
            
            source = null;
            
            trans = null;
            transfac = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            return sReturn;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetTestContactData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetTestContactData(): Exception:" + ex.getMessage());      
            }
            
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><response><code>-1</code><description>Exception:" + ex.getMessage() + "</description></response>";
        }
    }
    
    public String GetTestCSQData(int NumberOfRecords, int AvgResponseDelay)
    {
        try
        {
            Thread.sleep(AvgResponseDelay);
            
            String sReturn = "";
            
            DocumentBuilderFactory dbf = null;
            DocumentBuilder db = null;
            Document doc = null;
            
            Element root = null;
            Element child = null;
            Element grandChild = null;
            Element greatGrandChild = null;
            
            Text text = null;
            
            dbf = DocumentBuilderFactory.newInstance();
            db = dbf.newDocumentBuilder();
            doc = db.newDocument();
            
            root = doc.createElement("response");
            doc.appendChild(root);
            
            child = doc.createElement("code");
            root.appendChild(child);

            text = doc.createTextNode("0");
            child.appendChild(text);

            child = doc.createElement("description");
            root.appendChild(child);

            text = doc.createTextNode("");
            child.appendChild(text);

            child = doc.createElement("timestamp");
            root.appendChild(child);

            text = doc.createTextNode(String.valueOf((new java.util.Date()).getTime()));
            child.appendChild(text);

            child = doc.createElement("csqs");
            root.appendChild(child);
            
            for(int i = 0; i < NumberOfRecords; i++)
            {
                grandChild = doc.createElement("csq");
                child.appendChild(grandChild);
                
                //id
                greatGrandChild = doc.createElement("id");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("99");
                greatGrandChild.appendChild(text);
                
                //name
                greatGrandChild = doc.createElement("name");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("Demo");
                greatGrandChild.appendChild(text);
                
                //agentsloggedin
                greatGrandChild = doc.createElement("agentsloggedin");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("0");
                greatGrandChild.appendChild(text);
                            
                //agentsnotready
                greatGrandChild = doc.createElement("agentsnotready");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("0");
                greatGrandChild.appendChild(text);
                            
                //agentsready
                greatGrandChild = doc.createElement("agentsready");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("0");
                greatGrandChild.appendChild(text);
                            
                //agentstalking
                greatGrandChild = doc.createElement("agentstalking");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("0");
                greatGrandChild.appendChild(text);
                            
                //agentswork
                greatGrandChild = doc.createElement("agentswork");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("0");
                greatGrandChild.appendChild(text);
                            
                //contactswaiting
                greatGrandChild = doc.createElement("contactswaiting");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("0");
                greatGrandChild.appendChild(text);
                            
                //longestwaitingcontact
                greatGrandChild = doc.createElement("longestwaitingcontact");
                grandChild.appendChild(greatGrandChild);

                text = doc.createTextNode("0");
                greatGrandChild.appendChild(text);
                                
            }//for(int i = 0; i < NumberOfRecords; i++)
            
            //set up a transformer
            TransformerFactory transfac = TransformerFactory.newInstance();
            Transformer trans = transfac.newTransformer();
            trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
            trans.setOutputProperty(OutputKeys.INDENT, "yes");

            //create string from xml tree
            StringWriter sw = new StringWriter();
            StreamResult result = new StreamResult(sw);
            DOMSource source = new DOMSource(doc);
            trans.transform(source, result);
            
            sReturn = sw.toString();
            
            sw.close();
            sw = null;
            
            result = null;
            
            source = null;
            
            trans = null;
            transfac = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            return sReturn;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetTestContactData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetTestContactData(): Exception:" + ex.getMessage());      
            }
            
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><response><code>-1</code><description>Exception:" + ex.getMessage() + "</description></response>";
        }
    }
    
    public String GetContactsData()
    {
        try
        {
            String sReturn = this.GetContactsRealtimeDataXML();
            
            DocumentBuilderFactory dbf = null;
            DocumentBuilder db = null;
            Document doc = null;
            
            dbf = DocumentBuilderFactory.newInstance();
            db = dbf.newDocumentBuilder();
            doc = db.parse(new org.xml.sax.InputSource( new java.io.StringReader(sReturn)));
            
            javax.xml.xpath.XPath xPath = javax.xml.xpath.XPathFactory.newInstance().newXPath();
            String expression = "/response/code";

            org.w3c.dom.NodeList nodeList = (org.w3c.dom.NodeList) xPath.compile(expression).evaluate(doc, javax.xml.xpath.XPathConstants.NODESET);
            
            if(nodeList != null)
            {
                if(nodeList.getLength() == 0)
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Node code was not found.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsData(): Node code was not found.");
                    }
                
                    return sReturn;
                }
                else if(nodeList.getLength() == 1)
                {
                    String sCode = nodeList.item(0).getTextContent();
                    
                    if(sCode.equals("-1"))
                    {
                        return sReturn;
                    }
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Unexpected number of code elements found.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsData(): Unexpected number of code elements found.");
                    }
                
                    return sReturn;
                }
                
            }
            else
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): nodeList is null.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetContactsData(): nodeList is null.");
                }
                
                return sReturn;
            }
            
            nodeList = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            String sCurrentGetContactsDataHash = this.CreateHash(sReturn);
            
            if(sCurrentGetContactsDataHash.equals(""))
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Hash is empty.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetContactsData():  Hash is empty.");
                }
                
                return sReturn;
            }
            
            if(sGetContactsDataHash.equals(""))
            {
                sGetContactsDataHash = sCurrentGetContactsDataHash;
                return sReturn;
            }
            else
            {
                if(!sGetContactsDataHash.equals(sCurrentGetContactsDataHash))
                {
                    sGetContactsDataHash = sCurrentGetContactsDataHash;
                    return sReturn;
                }
                else
                {
                    if(_Settings.getEmailOnFailure())
                    {
                        if(_Notifier.SendEmail(_Settings.getEmailFrom(), _Settings.getEmailTo(), "Contact Data Collection service: Data freeze detected in GetContactsData", ""))
                        {
                            if(bUseWrapperManager)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Email was sent.");
                            }
                            else
                            {
                                System.out.println(this.getClass() + ".GetContactsData(): Email was sent.");
                            }
                        }
                        else
                        {
                            if(bUseWrapperManager)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Email was not sent.");
                            }
                            else
                            {
                                System.out.println(this.getClass() + ".GetContactsData(): Email was not sent.");
                            }
                        }
                    }
                    
                    _RMI = null;
                    
                    sReturn = this.GetContactsRealtimeDataXML();
                    
                    sCurrentGetContactsDataHash = this.CreateHash(sReturn);
                    
                    if(sCurrentGetContactsDataHash.equals(""))
                    {
                        if(bUseWrapperManager)
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Hash is empty.");
                        }
                        else
                        {
                            System.out.println(this.getClass() + ".GetContactsData():  Hash is empty.");
                        }
                        
                        return sReturn;
                    }
                    
                    if(sGetContactsDataHash.equals(""))
                    {
                        sGetContactsDataHash = sCurrentGetContactsDataHash;
                        return sReturn;
                    }
                    else
                    {
                        if(!sGetContactsDataHash.equals(sCurrentGetContactsDataHash))
                        {
                            sGetContactsDataHash = sCurrentGetContactsDataHash;
                            
                            if(_Settings.getEmailOnFailure())
                            {
                                if(_Notifier.SendEmail(_Settings.getEmailFrom(), _Settings.getEmailTo(), "Contact Data Collection service: Data freeze fixed", ""))
                                {
                                    if(bUseWrapperManager)
                                    {
                                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Email was sent.");
                                    }
                                    else
                                    {
                                        System.out.println(this.getClass() + ".GetContactsData(): Email was sent.");
                                    }
                                }
                                else
                                {
                                    if(bUseWrapperManager)
                                    {
                                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Email was not sent.");
                                    }
                                    else
                                    {
                                        System.out.println(this.getClass() + ".GetContactsData(): Email was not sent.");
                                    }
                                }
                            }
                            
                            return sReturn;
                        }
                        else
                        {
                            if(_Settings.getEmailOnFailure())
                            {
                                if(_Notifier.SendEmail(_Settings.getEmailFrom(), _Settings.getEmailTo(), "Contact Data Collection service: Data freeze not fixed", "Please restart the service manually via Callback Server Manager"))
                                {
                                    if(bUseWrapperManager)
                                    {
                                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Email was sent.");
                                    }
                                    else
                                    {
                                        System.out.println(this.getClass() + ".GetContactsData(): Email was sent.");
                                    }
                                }
                                else
                                {
                                    if(bUseWrapperManager)
                                    {
                                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Email was not sent.");
                                    }
                                    else
                                    {
                                        System.out.println(this.getClass() + ".GetContactsData(): Email was not sent.");
                                    }
                                }
                            }
                            
                            return sReturn;
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetContactsData(): Exception:" + ex.getMessage());      
            }
            
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><response><code>-1</code><description>Exception:" + ex.getMessage() + "</description></response>";
        }
    }
    
    public String GetCSQData()
    {
        try
        {
            
            System.out.println(this.getClass() + ".GetContactsData():  StackTrace: " + Thread.currentThread().getStackTrace().length);
            
            for(int i = 0; i < Thread.currentThread().getStackTrace().length; i++)
            {
                System.out.println(this.getClass() + ".GetContactsData():  Index: " + i + " Method name: " + Thread.currentThread().getStackTrace()[i].getMethodName());
            }
            
            String sReturn = this.GetCSQRealtimeDataXML();
            
            DocumentBuilderFactory dbf = null;
            DocumentBuilder db = null;
            Document doc = null;
            
            dbf = DocumentBuilderFactory.newInstance();
            db = dbf.newDocumentBuilder();
            doc = db.parse(new org.xml.sax.InputSource( new java.io.StringReader(sReturn)));
            
            javax.xml.xpath.XPath xPath = javax.xml.xpath.XPathFactory.newInstance().newXPath();
            String expression = "/response/code";

            org.w3c.dom.NodeList nodeList = (org.w3c.dom.NodeList) xPath.compile(expression).evaluate(doc, javax.xml.xpath.XPathConstants.NODESET);
            
            if(nodeList != null)
            {
                if(nodeList.getLength() == 0)
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Node code was not found.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsData(): Node code was not found.");
                    }
                
                    return sReturn;
                }
                else if(nodeList.getLength() == 1)
                {
                    String sCode = nodeList.item(0).getTextContent();
                    
                    if(sCode.equals("-1"))
                    {
                        return sReturn;
                    }
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): Unexpected number of code elements found.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsData(): Unexpected number of code elements found.");
                    }
                
                    return sReturn;
                }
            }
            else
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetContactsData(): nodeList is null.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetContactsData(): nodeList is null.");
                }
                
                return sReturn;
            }
            
            nodeList = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            String sCurrentGetCSQDataHash = this.CreateHash(sReturn);
            
            if(sCurrentGetCSQDataHash.equals(""))
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetCSQData(): Hash is empty.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetCSQData():  Hash is empty.");
                }
                
                return sReturn;
            }
            
            if(sGetCSQDataHash.equals(""))
            {
                sGetCSQDataHash = sCurrentGetCSQDataHash;
                return sReturn;
            }
            else
            {
                if(!sGetCSQDataHash.equals(sCurrentGetCSQDataHash))
                {
                    sGetCSQDataHash = sCurrentGetCSQDataHash;
                    return sReturn;
                }
                else
                {
                    if(_Settings.getEmailOnFailure())
                    {
                        if(_Notifier.SendEmail(_Settings.getEmailFrom(), _Settings.getEmailTo(), "Contact Data Collection service: Data freeze detected in GetCSQData", ""))
                        {
                            if(bUseWrapperManager)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetCSQData(): Email was sent.");
                            }
                            else
                            {
                                System.out.println(this.getClass() + ".GetCSQData(): Email was sent.");
                            }
                        }
                        else
                        {
                            if(bUseWrapperManager)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetCSQData(): Email was not sent.");
                            }
                            else
                            {
                                System.out.println(this.getClass() + ".GetCSQData(): Email was not sent.");
                            }
                        }
                    }
                    
                    _RMI = null;
                    
                    sReturn = this.GetCSQRealtimeDataXML();
                    
                    sCurrentGetCSQDataHash = this.CreateHash(sReturn);
                    
                    if(sCurrentGetCSQDataHash.equals(""))
                    {
                        if(bUseWrapperManager)
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetCSQData(): Hash is empty.");
                        }
                        else
                        {
                            System.out.println(this.getClass() + ".GetCSQData():  Hash is empty.");
                        }
                        
                        return sReturn;
                    }
                    
                    if(sGetCSQDataHash.equals(""))
                    {
                        sGetCSQDataHash = sCurrentGetCSQDataHash;
                        return sReturn;
                    }
                    else
                    {
                        if(!sGetCSQDataHash.equals(sCurrentGetCSQDataHash))
                        {
                            sGetCSQDataHash = sCurrentGetCSQDataHash;
                            
                            if(_Settings.getEmailOnFailure())
                            {
                                if(_Notifier.SendEmail(_Settings.getEmailFrom(), _Settings.getEmailTo(), "Contact Data Collection service: Data freeze fixed", ""))
                                {
                                    if(bUseWrapperManager)
                                    {
                                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetCSQData(): Email was sent.");
                                    }
                                    else
                                    {
                                        System.out.println(this.getClass() + ".GetCSQData(): Email was sent.");
                                    }
                                }
                                else
                                {
                                    if(bUseWrapperManager)
                                    {
                                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetCSQData(): Email was not sent.");
                                    }
                                    else
                                    {
                                        System.out.println(this.getClass() + ".GetCSQData(): Email was not sent.");
                                    }
                                }
                            }
                            
                            return sReturn;
                        }
                        else
                        {
                            if(_Settings.getEmailOnFailure())
                            {
                                if(_Notifier.SendEmail(_Settings.getEmailFrom(), _Settings.getEmailTo(), "Contact Data Collection service: Data freeze not fixed", "Please restart the service manually via Callback Server Manager"))
                                {
                                    if(bUseWrapperManager)
                                    {
                                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetCSQData(): Email was sent.");
                                    }
                                    else
                                    {
                                        System.out.println(this.getClass() + ".GetCSQData(): Email was sent.");
                                    }
                                }
                                else
                                {
                                    if(bUseWrapperManager)
                                    {
                                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"RMIClient.GetCSQData(): Email was not sent.");
                                    }
                                    else
                                    {
                                        System.out.println(this.getClass() + ".GetCSQData(): Email was not sent.");
                                    }
                                }
                            }
                            
                            return sReturn;
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetCSQData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetCSQData(): Exception:" + ex.getMessage());      
            }
            
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><response><code>-1</code><description>Exception:" + ex.getMessage() + "</description></response>";
        }
    }
    
    public String GetContactsRealtimeDataXML()
    {
        try
        {
            String sReturn = "";
            
            DocumentBuilderFactory dbf = null;
            DocumentBuilder db = null;
            Document doc = null;
            
            Element root = null;
            Element child = null;
            Element grandChild = null;
            
            Text text = null;
            
            dbf = DocumentBuilderFactory.newInstance();
            db = dbf.newDocumentBuilder();
            doc = db.newDocument();
            
            root = doc.createElement("response");
            doc.appendChild(root);
            
            if(GetContactsRealtimeData())
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeDataXML(): Number of contacts found: " + _ContactRealtimeData.size());
                }
                else
                {
                    System.out.print("\n RMIClient.GetContactsRealtimeDataXML() returned true. Number of contacts found: " + _ContactRealtimeData.size());
                }
                
                child = doc.createElement("code");
                root.appendChild(child);
            
                text = doc.createTextNode("0");
                child.appendChild(text);
                
                child = doc.createElement("description");
                root.appendChild(child);
            
                text = doc.createTextNode("");
                child.appendChild(text);
                
                child = doc.createElement("timestamp");
                root.appendChild(child);
                        
                text = doc.createTextNode(String.valueOf((new java.util.Date()).getTime()));
                child.appendChild(text);
                
                child = doc.createElement("contacts");
                root.appendChild(child);
            
                if(!_ContactRealtimeData.isEmpty())
                {
                    Enumeration e = _ContactRealtimeData.keys();                                                                                                                        
                
                    while(e.hasMoreElements())
                    {
                        java.lang.String sTaskID = (java.lang.String)e.nextElement(); //s is ImplID
                    
                        java.util.ArrayList arContact = (java.util.ArrayList)_ContactRealtimeData.get(sTaskID);
                        
                        child = doc.createElement("contact");
                        root.appendChild(child);
                            
                        int i = 0;
                        for(i=0;i<arContact.size();i++)
                        {
                            if(i == 0) //ContactID
                            {
                                grandChild = doc.createElement("contactid");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            }
                            else if(i == 1) //Contact Type
                            {
                                grandChild = doc.createElement("type");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            }
                            else if(i == 2) //ImplID
                            {
                                grandChild = doc.createElement("implid");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            }   
                            else if(i == 3) //StartTime
                            {
                                grandChild = doc.createElement("starttime");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            }
                            else if(i == 4) //Duration
                            {
                                grandChild = doc.createElement("duration");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            }  
                            else if(i == 7) //Application
                            {
                                grandChild = doc.createElement("application");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            }  
                            else if(i == 8) //Task
                            {
                                grandChild = doc.createElement("task");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            }  
                            else if(i == 9) //Session
                            {
                                grandChild = doc.createElement("session");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            } 
                            else if(i == 10) //QueuedFor
                            {
                                grandChild = doc.createElement("queuedfor");
                                child.appendChild(grandChild);

                                text = doc.createTextNode((String)arContact.get(i));
                                grandChild.appendChild(text);
                            } 
                            
                        }//for(i=0;i<arContact.size();i++)                 
                    
                    }//while(e.hasMoreElements())
                
                    e = null;
                }
            }
            else
            {
                child = doc.createElement("code");
                root.appendChild(child);
            
                text = doc.createTextNode("-1");
                child.appendChild(text);
                
                child = doc.createElement("description");
                root.appendChild(child);
            
                text = doc.createTextNode("RMIClient.GetContactsRealtimeDataXML() returned false");
                child.appendChild(text);
            }
            
            //set up a transformer
            TransformerFactory transfac = TransformerFactory.newInstance();
            Transformer trans = transfac.newTransformer();
            trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
            trans.setOutputProperty(OutputKeys.INDENT, "yes");

            //create string from xml tree
            StringWriter sw = new StringWriter();
            StreamResult result = new StreamResult(sw);
            DOMSource source = new DOMSource(doc);
            trans.transform(source, result);
            
            sReturn = sw.toString();
            
            sw.close();
            sw = null;
            
            result = null;
            
            source = null;
            
            trans = null;
            transfac = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            return sReturn;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeDataXML(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetContactsRealtimeDataXML(): Exception:" + ex.getMessage());      
            }
            
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><response><code>-1</code><description>Exception:" + ex.getMessage() + "</description></response>";
        }
    }
     
    private boolean GetContactsRealtimeData()
    {
        if(bUseWrapperManager)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): Enter.");
        }
        else
        {
            System.out.println(this.getClass() + ".GetContactsRealtimeData(): Enter.");
        }
        
        try
        {
            if(_RMI == null)
            {
                if(!Connect())
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): Connect() returned false.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsRealtimeData(): Connect() returned false.");
                    }
                    
                    return false;
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".GetContactsRealtimeData(): Connect() returned true.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsRealtimeData(): Connect() returned true.");
                    }
                }
                
            }//if(_RMI == null)
         
            if(State() == ConnectionState.Open)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".GetContactsRealtimeData(): State() is OPEN.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetContactsRealtimeData(): State() is OPEN.");
                }
            }
            else
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): State() is UNKNOWN.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetContactsRealtimeData(): State() is UNKNOWN.");
                }
                
                _RMI = null;
                
                return false;
            }
            
            _ContactRealtimeData = new java.util.Hashtable();
            
            java.util.Hashtable h = _RMI.getSessionActivity();
            
            if(h != null)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): getSessionActivity() call did not return null.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetContactsRealtimeData(): getSessionActivity() call did not return null.");
                }
                
                if(!h.isEmpty())
                {
                    Enumeration e = h.keys();                                                                                                                        

                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): Enumerate over session activity records.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsRealtimeData(): Enumerate over session activity records.");
                    }
                    
                    while(e.hasMoreElements())
                    {
                        java.lang.String sTaskID = (java.lang.String)e.nextElement();

                        java.util.Vector vContacts = _RMI.getContactsByAppTaskId(sTaskID);

                        java.util.ArrayList arContact = new java.util.ArrayList();

                        if(vContacts != null)
                        {
                            if(vContacts.size() == 2)
                            {
                                java.util.Vector vContact = (java.util.Vector)vContacts.get(1);

                                int i = 0;
                                for(i=0;i<vContact.size();i++)
                                {
                                    arContact.add((String)vContact.get(i));

                                }//for(i=0;i<v1.size();i++)

                                arContact.add("");

                                vContact = null;

                            }//if(v.size() == 2)

                        }//if(v != null)

                        _ContactRealtimeData.put(sTaskID, arContact);
                        vContacts = null; 
                        arContact = null;

                    }//while(e.hasMoreElements())

                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): Enumeration complete.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsRealtimeData(): Enumeration complete.");
                    }
                    
                }//if(!h.isEmpty())
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): No records to enumerate over.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsRealtimeData(): No records to enumerate over.");
                    }
                    
                }//if(!h.isEmpty())
                
                h = null;
                
                if(_ContactRealtimeData != null)
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): _ContactRealtimeData is not null.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsRealtimeData(): _ContactRealtimeData is not null.");
                    }
                    
                    if(_ContactRealtimeData.size() > 0)
                    {
                        if(bUseWrapperManager)
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): _ContactRealtimeData is not empty.");
                        }
                        else
                        {
                            System.out.println(this.getClass() + ".GetContactsRealtimeData(): _ContactRealtimeData is not empty.");
                        }
                        
                        h = _RMI.getOverallWaitingContactsInfoStats();

                        if(!h.isEmpty())
                        {
                            if(bUseWrapperManager)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): Enumerate over OverallWaitingContactInfo records.");
                            }
                            else
                            {
                                System.out.println(this.getClass() + ".GetContactsRealtimeData(): Enumerate over OverallWaitingContactInfo records.");
                            }
                            
                            Enumeration e = h.keys();                                                                                                                        

                            while(e.hasMoreElements())
                            {
                                java.lang.String sImplID = (java.lang.String)e.nextElement();

                                java.util.ArrayList alContacts = (java.util.ArrayList)h.get(sImplID);

                                int i = 0;
                                for(i=0;i<alContacts.size();i++)
                                {
                                    com.cisco.wf.reporting.WaitingContactsInfoStatsClient c = (com.cisco.wf.reporting.WaitingContactsInfoStatsClient)alContacts.get(i);

                                    Enumeration eRTData = _ContactRealtimeData.keys();

                                    while(eRTData.hasMoreElements())
                                    {
                                        java.lang.String sTaskID = (java.lang.String)eRTData.nextElement();

                                        java.util.ArrayList alContact = (java.util.ArrayList)_ContactRealtimeData.get(sTaskID);

//                                        if(bUseWrapperManager)
//                                        {
//                                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): alContact size -> " + alContact.size());
//                                        }
//                                        else
//                                        {
//                                            System.out.println(this.getClass() + ".GetContactsRealtimeData(): alContact size -> " + alContact.size());
//                                        }
                                        
                                        if(alContact.size() != 0)
                                        {
                                            if(sImplID.equals((String)alContact.get(2)))
                                            {
                                                alContact.set(10, c.getQueuedCSQS());
                                            }
                                        }

                                        alContact = null;

                                    }//while(eRTData.hasMoreElements())

                                    eRTData = null;

                                    c = null;

                                }//for(i=0;i<alContacts.size();i++)

                                alContacts = null;

                            }//while(e.hasMoreElements())

                            if(bUseWrapperManager)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): Enumeration completed.");
                            }
                            else
                            {
                                System.out.println(this.getClass() + ".GetContactsRealtimeData(): Enumeration completed.");
                            }
                            
                            e = null;

                        }//if(!h.isEmpty())

                    }//if(_ContactRealtimeData.size() > 0)
                    else
                    {
                        if(bUseWrapperManager)
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): _ContactRealtimeData is empty.");
                        }
                        else
                        {
                            System.out.println(this.getClass() + ".GetContactsRealtimeData(): _ContactRealtimeData is empty.");
                        }
                        
                    }//if(_ContactRealtimeData.size() > 0)
                    
                }//if(_ContactRealtimeData != null)
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): _ContactRealtimeData is null.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetContactsRealtimeData(): _ContactRealtimeData is null.");
                    }
                    
                }////if(_ContactRealtimeData != null)
                
                return true;
            }
            else//if(h != null)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): _RMI.getSessionActivity() call returned null/empty.");
                }
                else
                {
                    System.out.print("\n RMIClient.GetContactsRealtimeData(): _RMI.getSessionActivity() call returned null/empty.");      
                }
                
                return true;
                
            }//if(h != null)
            
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetContactsRealtimeData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetContactsRealtimeData(): Exception:" + ex.getMessage());      
            }
            
            _ContactRealtimeData = null;
            
            return false;
        }
    }
    
    public String GetCSQRealtimeDataXML()
    {
        try
        {
            String sReturn = "";
            
            DocumentBuilderFactory dbf = null;
            DocumentBuilder db = null;
            Document doc = null;
            
            Element root = null;
            Element child = null;
            Element grandChild = null;
            Element greatGrandChild = null;
            
            Text text = null;
            
            dbf = DocumentBuilderFactory.newInstance();
            db = dbf.newDocumentBuilder();
            doc = db.newDocument();
            
            root = doc.createElement("response");
            doc.appendChild(root);
            
            if(GetCSQRealtimeData())
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetCSQRealtimeData(): Number of CSQs found: " + _CSQRealtimeData.size());
                }
                else
                {
                    System.out.print("\n RMIClient.GetCSQRealtimeData() returned true. Number of CSQs found: " + _CSQRealtimeData.size());
                }
                
                child = doc.createElement("code");
                root.appendChild(child);
            
                text = doc.createTextNode("0");
                child.appendChild(text);
                
                child = doc.createElement("description");
                root.appendChild(child);
            
                text = doc.createTextNode("");
                child.appendChild(text);
                
                child = doc.createElement("timestamp");
                root.appendChild(child);
                        
                text = doc.createTextNode(String.valueOf((new java.util.Date()).getTime()));
                child.appendChild(text);
                
                child = doc.createElement("csqs");
                root.appendChild(child);
                            
                if(!_CSQRealtimeData.isEmpty())
                {
                    Enumeration e = _CSQRealtimeData.keys();                                                                                                                        
                
                    String[] aCSQ = null;
                    
                    while(e.hasMoreElements())
                    {
                        String sID = (String)e.nextElement();
                    
                        aCSQ = (String[])_CSQRealtimeData.get(sID);
                        
                        grandChild = doc.createElement("csq");
                        child.appendChild(grandChild);
                            
                        int i = 0;
                        for(i=0;i<aCSQ.length;i++)
                        {
                            if(i == 0) //CSQ ID
                            {
                                greatGrandChild = doc.createElement("id");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 1) //CSQ Name
                            {
                                greatGrandChild = doc.createElement("name");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 2) //CSQ Agents Logged In
                            {
                                greatGrandChild = doc.createElement("agentsloggedin");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 3) //CSQ Agents Not Ready
                            {
                                greatGrandChild = doc.createElement("agentsnotready");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 4) //CSQ Agents Ready
                            {
                                greatGrandChild = doc.createElement("agentsready");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 5) //CSQ Agents Talking
                            {
                                greatGrandChild = doc.createElement("agentstalking");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 6) //CSQ Agents Work
                            {
                                greatGrandChild = doc.createElement("agentswork");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 7) //CSQ Contacts Waiting
                            {
                                greatGrandChild = doc.createElement("contactswaiting");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 8) //CSQ Longest Waiting Contact
                            {
                                greatGrandChild = doc.createElement("longestwaitingcontact");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aCSQ[i]);
                                greatGrandChild.appendChild(text);
                            }
                            
                        }//for(i=0;i<aCSQ.length;i++)
                        
                        aCSQ = null;
                        
                    }//while(e.hasMoreElements())
                    
                }//if(!_CSQRealtimeData.isEmpty())
            }
            else
            {
                child = doc.createElement("code");
                root.appendChild(child);
            
                text = doc.createTextNode("-1");
                child.appendChild(text);
                
                child = doc.createElement("description");
                root.appendChild(child);
            
                text = doc.createTextNode("RMIClient.GetCSQRealtimeData() returned false");
                child.appendChild(text);
            }
            
            //set up a transformer
            TransformerFactory transfac = TransformerFactory.newInstance();
            Transformer trans = transfac.newTransformer();
            trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
            trans.setOutputProperty(OutputKeys.INDENT, "yes");

            //create string from xml tree
            StringWriter sw = new StringWriter();
            StreamResult result = new StreamResult(sw);
            DOMSource source = new DOMSource(doc);
            trans.transform(source, result);
            
            sReturn = sw.toString();
            
            sw.close();
            sw = null;
            
            result = null;
            
            source = null;
            
            trans = null;
            transfac = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            return sReturn;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetCSQRealtimeDataXML(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetCSQRealtimeDataXML(): Exception:" + ex.getMessage());      
            }
            
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><response><code>-1</code><description>Exception:" + ex.getMessage() + "</description></response>";
        }
    }
    
    private boolean GetCSQRealtimeData()
    {
        if(bUseWrapperManager)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetCSQRealtimeData(): Enter.");
        }
        else
        {
            System.out.println(this.getClass() + ".GetCSQRealtimeData(): Enter.");
        }
        
        try
        {
            if(_RMI == null)
            {
                if(!Connect())
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetCSQRealtimeData(): Connect() returned false.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetCSQRealtimeData(): Connect() returned false.");
                    }
                    
                    return false;
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".GetCSQRealtimeData(): Connect() returned true.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetCSQRealtimeData(): Connect() returned true.");
                    }
                }
                
            }//if(_RMI == null)
            
            if(State() == ConnectionState.Open)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".GetCSQRealtimeData(): State() is OPEN.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetCSQRealtimeData(): State() is OPEN.");
                }
            }
            else
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetCSQRealtimeData(): State() is UNKNOWN.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetCSQRealtimeData(): State() is UNKNOWN.");
                }
                
                _RMI = null;
                
                return false;
            }
            
            _CSQRealtimeData = new java.util.Hashtable();
            
            java.util.Hashtable h = _RMI.getEsdIAQStats();
            
            if(!h.isEmpty())
            {
                Enumeration e = h.keys();                                                                                                                        

                String[] aCSQ = null;
                
                int iID;
                String sName = "";
                long lWaitingContacts = 0;
                long lLongestCurrentlyWaitingDuration = 0;
                int iResourcesWork = 0;
                int iResourcesLoggedIn = 0;
                int iResourcesTalking = 0;
                int iResourcesReady = 0;
                int iResourcesNotReady = 0;
                
                while(e.hasMoreElements())
                {   
                    java.lang.Integer i = (java.lang.Integer)e.nextElement();
                    
                    EsdIAQStatsClient _r = (EsdIAQStatsClient)h.get(i);
                 
                    iID = _r.getEsdId();
                    sName = _r.getEsdName();
                    iResourcesLoggedIn = _r.getNumResources();
                    iResourcesNotReady = _r.getNumUnavailResources();
                    iResourcesReady = _r.getNumAvailResources();
                    iResourcesTalking = _r.getNumInSessionResources();
                    iResourcesWork = _r.getNumWorkResources();
                    lWaitingContacts = _r.getNumWaitingContacts();
                    lLongestCurrentlyWaitingDuration = _r.getLongestCurrentlyWaitingDuration();
                    
                    aCSQ = new String[]{String.valueOf(iID),sName,String.valueOf(iResourcesLoggedIn),String.valueOf(iResourcesNotReady),String.valueOf(iResourcesReady),String.valueOf(iResourcesTalking),String.valueOf(iResourcesWork),String.valueOf(lWaitingContacts),String.valueOf(lLongestCurrentlyWaitingDuration)};
                    
                    _CSQRealtimeData.put(String.valueOf(iID), aCSQ);
                    
                    aCSQ = null;
                    
                }//while(e.hasMoreElements())
                
                aCSQ = null;
                e = null;
                
            }//if(!h.isEmpty())
            
            h = null;
            
            return true;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetCSQRealtimeData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetCSQRealtimeData(): Exception:" + ex.getMessage());      
            }
            
            return false;
        }
    }
    
    public String GetAgentRealtimeDataXML()
    {
        try
        {
            String sReturn = "";
            
            DocumentBuilderFactory dbf = null;
            DocumentBuilder db = null;
            Document doc = null;
            
            Element root = null;
            Element child = null;
            Element grandChild = null;
            Element greatGrandChild = null;
            
            Text text = null;
            
            dbf = DocumentBuilderFactory.newInstance();
            db = dbf.newDocumentBuilder();
            doc = db.newDocument();
            
            root = doc.createElement("response");
            doc.appendChild(root);
            
            if(GetAgentRealtimeData())
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetAgentRealtimeDataXML(): Number of Agents found: " + _AgentRealtimeData.size());
                }
                else
                {
                    System.out.print("\n RMIClient.GetAgentRealtimeDataXML() returned true. Number of Agents found: " + _AgentRealtimeData.size());
                }
                
                child = doc.createElement("code");
                root.appendChild(child);
            
                text = doc.createTextNode("0");
                child.appendChild(text);
                
                child = doc.createElement("description");
                root.appendChild(child);
            
                text = doc.createTextNode("");
                child.appendChild(text);
                
//                child = doc.createElement("timestamp");
//                root.appendChild(child);
//                        
//                text = doc.createTextNode(String.valueOf((new java.util.Date()).getTime()));
//                child.appendChild(text);
                
                child = doc.createElement("resources");
                root.appendChild(child);
                            
                if(!_AgentRealtimeData.isEmpty())
                {
                    Enumeration e = _AgentRealtimeData.keys();                                                                                                                        
                
                    String[] aAgent = null;
                    
                    while(e.hasMoreElements())
                    {
                        String sResourceId = (String)e.nextElement();
                    
                        aAgent = (String[])_AgentRealtimeData.get(sResourceId);
                        
                        grandChild = doc.createElement("resource");
                        child.appendChild(grandChild);
                            
                        int i = 0;
                        for(i=0;i<aAgent.length;i++)
                        {
                            if(i == 0) //sResourceId
                            {
                                greatGrandChild = doc.createElement("resourceid");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aAgent[i]);
                                greatGrandChild.appendChild(text);
                            }
                            else if(i == 1) //Duration in state
                            {
                                greatGrandChild = doc.createElement("durationinstate");
                                grandChild.appendChild(greatGrandChild);

                                text = doc.createTextNode(aAgent[i]);
                                greatGrandChild.appendChild(text);
                            }
                            
                        }//for(i=0;i<aCSQ.length;i++)
                        
                        aAgent = null;
                        
                    }//while(e.hasMoreElements())
                    
                }//if(!_CSQRealtimeData.isEmpty())
            }
            else
            {
                child = doc.createElement("code");
                root.appendChild(child);
            
                text = doc.createTextNode("-1");
                child.appendChild(text);
                
                child = doc.createElement("description");
                root.appendChild(child);
            
                text = doc.createTextNode("RMIClient.GetAgentRealtimeDataXML() returned false");
                child.appendChild(text);
            }
            
            //set up a transformer
            TransformerFactory transfac = TransformerFactory.newInstance();
            Transformer trans = transfac.newTransformer();
            trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
            trans.setOutputProperty(OutputKeys.INDENT, "yes");

            //create string from xml tree
            StringWriter sw = new StringWriter();
            StreamResult result = new StreamResult(sw);
            DOMSource source = new DOMSource(doc);
            trans.transform(source, result);
            
            sReturn = sw.toString();
            
            sw.close();
            sw = null;
            
            result = null;
            
            source = null;
            
            trans = null;
            transfac = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            return sReturn;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetAgentRealtimeDataXML(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetAgentRealtimeDataXML(): Exception:" + ex.getMessage());      
            }
            
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><response><code>-1</code><description>Exception:" + ex.getMessage() + "</description></response>";
        }
    }
    
    private boolean GetAgentRealtimeData()
    {
        if(bUseWrapperManager)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetAgentRealtimeData(): Enter.");
        }
        else
        {
            System.out.println(this.getClass() + ".GetAgentRealtimeData(): Enter.");
        }
        
        try
        {
            if(_RMI == null)
            {
                if(!Connect())
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetAgentRealtimeData(): Connect() returned false.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetAgentRealtimeData(): Connect() returned false.");
                    }
                    
                    return false;
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".GetAgentRealtimeData(): Connect() returned true.");
                    }
                    else
                    {
                        System.out.println(this.getClass() + ".GetAgentRealtimeData(): Connect() returned true.");
                    }
                }
                
            }//if(_RMI == null)
            
            if(State() == ConnectionState.Open)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".GetAgentRealtimeData(): State() is OPEN.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetAgentRealtimeData(): State() is OPEN.");
                }
            }
            else
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetAgentRealtimeData(): State() is UNKNOWN.");
                }
                else
                {
                    System.out.println(this.getClass() + ".GetAgentRealtimeData(): State() is UNKNOWN.");
                }
                
                _RMI = null;
                
                return false;
            }
            
            _AgentRealtimeData = new java.util.Hashtable();
            
            java.util.Hashtable h = _RMI.getAllResourceIAQStats();
            
            if(!h.isEmpty())
            {
                Enumeration e = h.keys();                                                                                                                        

                String[] aAgent = null;
                
                String sResourceId = "";
                String sDurationInState = "";
                
                while(e.hasMoreElements())
                {   
                    java.lang.String s = (java.lang.String)e.nextElement();
                    
                    ResourceIAQStatsClient _r = (ResourceIAQStatsClient)h.get(s);
                 
                    sResourceId = _r.getResourceId();
                    sDurationInState = String.valueOf(_r.getDurationInState());
                    
                    aAgent = new String[]{sResourceId,sDurationInState};
                    
                    _AgentRealtimeData.put(sResourceId, aAgent);
                    
                    aAgent = null;
                    
                }//while(e.hasMoreElements())
                
                aAgent = null;
                e = null;
                
            }//if(!h.isEmpty())
            
            h = null;
            
            return true;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".GetAgentRealtimeData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.GetAgentRealtimeData(): Exception:" + ex.getMessage());      
            }
            
            return false;
        }
    }
    
    private boolean Connect()
    {        
        try
        {
            if(this._Cluster.getCurrentMasterIPAddress().equals(""))
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Connect(): Master node IP address is empty");
                }
                else
                {
                    System.out.println(this.getClass() + ".Connect(): Master node IP address is empty");
                }
                
                return false;
            }
            
            if(this._Cluster.getCurrentMasterRTRPort().equals(""))
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Connect(): Master node port is empty");
                }
                else
                {
                    System.out.println(this.getClass() + ".Connect(): Master node port is empty");
                }
                
                return false;
            }
            
            Calendar bcal = Calendar.getInstance();
            
            int iPort = Integer.parseInt(_Cluster.getCurrentMasterRTRPort());
            
            System.out.print("Connect to IP " + _Cluster.getCurrentMasterIPAddress() + " Port " + iPort);
            
            _RMI = new com.cisco.wfframework.reporting.api.RMILayer(_Cluster.getCurrentMasterIPAddress(),iPort,1,false);
                
            Calendar acal = Calendar.getInstance();
                
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Connect(): Client connected in " + (acal.getTime().getTime() - bcal.getTime().getTime()) + " ms");
            }
            else
            {
                System.out.println(this.getClass() + ".Connect(): Client connected in " + (acal.getTime().getTime() - bcal.getTime().getTime()) + " ms");
            }
            
            return true;
        }
        catch(com.cisco.wfframework.reporting.api.CommException ce)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Connect(): CommException:" + ce.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.Connect(): CommException:" + ce.getMessage());      
            }
            
            _RMI = null;
            
            return false;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Connect(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.Connect(): Exception:" + ex.getMessage());
            }
            
            _RMI = null;
            
            return false;
        }
    }
    
    private ConnectionState State()
    {
        try
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".State(): before collecting time on RMI.");
            }
            else
            {
                System.out.print("\n RMIClient.State(): before collecting time on RMI.");
            }
            
            String CurrentTime = _RMI.getCurrentTime();
            
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".State(): after collecting time on RMI.");
            }
            else
            {
                System.out.print("\n RMIClient.State(): after collecting time on RMI.");
            }
           
            return ConnectionState.Open;
        }
        catch(Exception e)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".State(): Exception:" + e.getMessage());
            }
            else
            {
                System.out.println(this.getClass() + ".State(): Exception:" + e.getMessage());
            }
            
            _RMI = null;
            
            return ConnectionState.Unknown;
        }
    }
    
    public TestResult TestGetContactsRealtimeData()
    {
        try
        {
            TestResult result = new TestResult();
            
            java.util.Date dBegin = new java.util.Date();
            
            if(GetContactsRealtimeData())
            {
                java.util.Date dEnd = new java.util.Date();
                
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".TestGetContactsRealtimeData(): Number of contacts found: " + _ContactRealtimeData.size());
                }
                else
                {
                    System.out.print("\n RMIClient.TestGetContactsRealtimeData() returned true. Number of contacts found: " + _ContactRealtimeData.size());
                }
                
                result.MethodExecutedWithoutErros = true;
                result.ErrorDescription = "";
                result.NumberOfRecords = _ContactRealtimeData.size();
                result.OperationDuration = dEnd.getTime() - dBegin.getTime();
                
                dBegin = null;
                dEnd = null;
            }
            else
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".TestGetContactsRealtimeData() returned false.");
                }
                else
                {
                    System.out.print("\n RMIClient.TestGetContactsRealtimeData() returned false.");
                }
                
                result.MethodExecutedWithoutErros = false;
                result.ErrorDescription = this.getClass().getName() + ".TestGetContactsRealtimeData() returned false";
            }
            
            return result;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".TestGetContactsRealtimeData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.TestGetContactsRealtimeData(): Exception:" + ex.getMessage());      
            }
            
            return new TestResult(0,0L,false,ex.getMessage());
        }
    }
    
    public TestResult TestGetCSQRealtimeData()
    {
        try
        {
            TestResult result = new TestResult();
            
            java.util.Date dBegin = new java.util.Date();
            
            if(GetCSQRealtimeData())
            {
                java.util.Date dEnd = new java.util.Date();
                
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".TestGetCSQRealtimeData(): Number of csqs found: " + _CSQRealtimeData.size());
                }
                else
                {
                    System.out.print("\n RMIClient.TestGetCSQRealtimeData() returned true. Number of csqs found: " + _CSQRealtimeData.size());
                }
                
                result.MethodExecutedWithoutErros = true;
                result.ErrorDescription = "";
                result.NumberOfRecords = _CSQRealtimeData.size();
                result.OperationDuration = dEnd.getTime() - dBegin.getTime();
                
                dBegin = null;
                dEnd = null;
            }
            else
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".TestGetCSQRealtimeData() returned false.");
                }
                else
                {
                    System.out.print("\n RMIClient.TestGetCSQRealtimeData() returned false.");
                }
                
                result.MethodExecutedWithoutErros = false;
                result.ErrorDescription = this.getClass().getName() + ".TestGetCSQRealtimeData() returned false";
            }
            
            return result;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".TestGetCSQRealtimeData(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.print("\n RMIClient.TestGetCSQRealtimeData(): Exception:" + ex.getMessage());      
            }
            
            return new TestResult(0,0L,false,ex.getMessage());
        }
    }
}
