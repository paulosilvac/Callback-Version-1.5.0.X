package expressodatacollection;

import java.util.Calendar;
import java.util.Hashtable;
import java.util.Enumeration;

import com.cisco.wf.reporting.ResourceIAQStatsClient;
import com.cisco.wf.reporting.EsdIAQStatsClient;


//import com.cisco.wfframework.reporting.api.CommException;
//import java.lang.String;

import java.io.StringWriter;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.Transformer;
import javax.xml.transform.stream.StreamResult;

import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.OutputKeys;

import org.w3c.dom.Document;
//import org.w3c.dom.Comment;
import org.w3c.dom.Element;
import org.w3c.dom.Text;
//import org.w3c.dom.Node;
//import org.w3c.dom.NodeList;
//import org.w3c.dom.NamedNodeMap;

import org.tanukisoftware.wrapper.WrapperManager;

public class RMIClient implements Runnable
{
    public enum ConnectionState{Open,Unknown};
    
    private com.cisco.wfframework.reporting.api.RMILayer _RMI;
    
    private UCCXCluster _Cluster = null;
    private RealtimeReportsData _Data = null;
    
    private String _IPAddress = "";
    private int _Port = 0;
    
    /** Creates a new instance of RMIClient */
    public RMIClient(UCCXCluster Cluster, RealtimeReportsData Data)
    {
        this._Cluster = Cluster;
        this._Data = Data;
        this._RMI = null;
        
        new Thread(this, Thread.currentThread().getName()).start();
    }
    
    public void run()
    {
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): has started.");
        //System.out.println("RMIClient.run(): " + Thread.currentThread().getName() + " has started.");
        
        try
        {
            while(true)
            {   
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): before collecting realtime data...");
               
                if(this._RMI == null)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): RMILayer object is null.");
                    
                    if(this._Cluster.getCurrentMasterIPAddress().length() > 0)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): master node for the cluster has been detected.");
                        
                        this._IPAddress = this._Cluster.getCurrentMasterIPAddress();
                        this._Port = Integer.parseInt(this._Cluster.getCurrentMasterRTRPort());
                        
                        if(this.Connect())
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Client connected to RMI endpoint.");
                            //System.out.println("RMIClient.run(): Client connected to RMI endpoint.");
                            
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): RMIClinet state: " + this.State().toString());
                            
                            if(this.State() == ConnectionState.Open)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): RMI connection is open.");
                                //System.out.println("RMIClient.run(): RMI connection is open.");
                                
                                java.util.Date dBegin = new java.util.Date();
                                
                                this._Data.setData(CollectRealtimeReportsData());
                                
                                java.util.Date dEnd = new java.util.Date();
                                    
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Data collection took: " + ((dEnd.getTime() - dBegin.getTime())/1000) + " seconds.");
                            }
                            else
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): RMI connection is not open.");
                                //System.out.println("RMIClient.run(): RMI connection is not open.");
                                
                                this._RMI = null;
                            }                            
                        }
                        else
                        {
                            this._Data.setData("");
                            this._RMI = null;
                            
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): connection to realtime data feed failed.");
                            //System.out.println("RMIClient.run(): Client not connected to RMI endpoint.");
                        }                        
                    }
                    else
                    {
                        this._Data.setData("");
                        
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): Master node not detected.");
                        //System.out.println("RMIClient.run(): Master node not detected.");
                    }
                }
                else //this._RMI == null
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): RMILayer object is not null.");
                    
                    if(this._Cluster.getCurrentMasterIPAddress().length() > 0)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): Master node has been detected.");
                        
                        if(this._IPAddress == this._Cluster.getCurrentMasterIPAddress() && this._Port == Integer.parseInt(this._Cluster.getCurrentMasterRTRPort()))
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): RMI client is still connected to the right node.");
                            //System.out.println("RMIClient.run(): RMI client is still connected to the right node.");
                            
                            if(this.State() == ConnectionState.Open)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): RMI connection is still open.");
                                //System.out.println("RMIClient.run(): RMI connection is still open.");
                                
                                java.util.Date dBegin = new java.util.Date();
                                
                                this._Data.setData(CollectRealtimeReportsData());
                                
                                java.util.Date dEnd = new java.util.Date();
                                    
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Data collection took: " + ((dEnd.getTime() - dBegin.getTime())/1000) + " seconds.");
                            }
                            else
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): RMI connection is not open.");
                                //System.out.println("RMIClient.run(): RMI connection is not open.");
                                
                                this._RMI = null;
                            }                            
                        }
                        else
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): Master has changed.");
                            //System.out.println("RMIClient.run(): Master has changed.");
                            
                            this._RMI = null;
                            
                            this._IPAddress = this._Cluster.getCurrentMasterIPAddress();
                            this._Port = Integer.parseInt(this._Cluster.getCurrentMasterRTRPort());
                        
                            if(this.Connect())
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Client connected to RMI endpoint.");
                                //System.out.println("RMIClient.run(): Client connected to RMI endpoint.");
                                
                                if(this.State() == ConnectionState.Open)
                                {
                                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): RMI connection is open.");
                                    //System.out.println("RMIClient.run(): RMI connection is open.");
                                    java.util.Date dBegin = new java.util.Date();
                                    
                                    this._Data.setData(CollectRealtimeReportsData());
                                    
                                    java.util.Date dEnd = new java.util.Date();
                                    
                                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Data collection took: " + ((dEnd.getTime() - dBegin.getTime())/1000) + " seconds.");
                                }
                                else
                                {
                                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): RMI connection is not open.");
                                    //System.out.println("RMIClient.run(): RMI connection is not open.");
                                    
                                    this._RMI = null;
                                } 
                            }
                            else
                            {
                                this._Data.setData("");
                                this._RMI = null;
                                
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): Client not connected to RMI endpoint.");
                                //System.out.println("RMIClient.run(): Client not connected to RMI endpoint.");
                            }
                        }
                    }
                    else
                    {
                        this._Data.setData("");
                        
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): Master node not detected.");
                        //System.out.println("RMIClient.run(): Master node not detected.");
                    }
                    
                } //this._RMI == null
                
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): after collecting realtime data.");
                
                Thread.sleep(5000);
            } 
        }
        catch(Exception ex)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): Exception:" + ex.getMessage());
            //System.out.println("RMIClient.run(): Exception:" + ex.getMessage());
        }
        
        this._Data.setData("");
        this._RMI = null;
        
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + Thread.currentThread().getName() + " will stop.");
        //System.out.println("RMIClient.run(): " + Thread.currentThread().getName() + " will stop.");
    }
    
    private boolean Connect()
    {        
        try
        {
            Calendar bcal = Calendar.getInstance();
                
            _RMI = new com.cisco.wfframework.reporting.api.RMILayer(this._IPAddress,this._Port,1,false);
                
            Calendar acal = Calendar.getInstance();
                
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Connect(): Client connected in " + (acal.getTime().getTime() - bcal.getTime().getTime()) + " ms");
            //System.out.println("RMIClient.Connect(): Client connected in " + (acal.getTime().getTime() - bcal.getTime().getTime()) + " ms");
                
            return true;
        }
        catch(com.cisco.wfframework.reporting.api.CommException ce)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Connect(): CommException:" + ce.getMessage());
            //System.out.print("\n RMIClient.Connect(): CommException:" + ce.getMessage());      
            return false;
        }
        catch(Exception ex)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Connect(): Exception:" + ex.getMessage());
            //System.out.print("\n RMIClient.Connect(): Exception:" + ex.getMessage());
            return false;
        }
    }
    
    private ConnectionState State()
    {
        try
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".State(): before collecting time on RMI.");
            String CurrentTime = _RMI.getCurrentTime();
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".State(): after collecting time on RMI.");
           
            return ConnectionState.Open;
        }
        catch(Exception e)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".State(): Exception:" + e.getMessage());
            //System.out.println("RMIClient.State(): Exception:" + e.getMessage());
            
            return ConnectionState.Unknown;
        }
    }
     
    private String CollectRealtimeReportsData()
    {
        DocumentBuilderFactory dbf = null;
        DocumentBuilder db = null;
        Document doc = null;
        
        Element root = null;
        Element child = null;
        Element grandChild = null;
        
        Text text = null;
         
        Hashtable h = null;
        Enumeration e = null;
        
        try
        {
            dbf = DocumentBuilderFactory.newInstance();
            
            db = dbf.newDocumentBuilder();
            
            doc = db.newDocument();
        
            //DataSetName
            root = doc.createElement("dsReturn");
            doc.appendChild(root);

            h = _RMI.getAllResourceIAQStats();
           
            if(!h.isEmpty())
            {
                e= h.keys();                                                                                                                        
                
                while(e.hasMoreElements())
                {
                    java.lang.String s = (java.lang.String)e.nextElement();
                    
                    ResourceIAQStatsClient _r = (ResourceIAQStatsClient)h.get(s);
                
                    //create child element, add an attribute, and add to root
                    child = doc.createElement("AgentSummary");
                    root.appendChild(child);
            
                    //ResourceID
                    grandChild = doc.createElement("ResourceID");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode( _r.getResourceId());
                    grandChild.appendChild(text);
                    
                    //ResourceName
                    grandChild = doc.createElement("ResourceName");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode( _r.getResourceName());
                    grandChild.appendChild(text);
                    
                     //PresentedContacts
                    grandChild = doc.createElement("CallsPresented");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumPresentedContacts()));
                    grandChild.appendChild(text);
                    
                    //HandledContacts
                    grandChild = doc.createElement("CallsHandled");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumHandledContacts()));
                    grandChild.appendChild(text);
                    
                    //State
                    grandChild = doc.createElement("State");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(Integer.toString(_r.getResourceState()));
                    grandChild.appendChild(text);
                    
                    //DurationInState
                    grandChild = doc.createElement("DurationInState");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getDurationInState()));
                    grandChild.appendChild(text);
                    
                    //AvgHandleDuration
                    grandChild = doc.createElement("AverageHandleTime");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getAvgHandleDuration()));
                    grandChild.appendChild(text);
                    
                    //AvgHoldDuration
                    grandChild = doc.createElement("AverageHoldTime");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getAvgHoldDuration()));
                    grandChild.appendChild(text);
                    
                    //LongestHoldDuration
                    grandChild = doc.createElement("LongestHoldTime");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getLongestHoldDuration()));
                    grandChild.appendChild(text);
                    
                    //AvgTalkDuration
                    grandChild = doc.createElement("AverageTalkTime");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getAvgTalkDuration()));
                    grandChild.appendChild(text);
                    
                    //LongestTalkDuration
                    grandChild = doc.createElement("LongestTalkTime");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getLongestTalkDuration()));
                    grandChild.appendChild(text);
                    
                    //AvgWorkDuration
                    grandChild = doc.createElement("AverageWorkTime");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getAvgWorkDuration()));
                    grandChild.appendChild(text); 
                  
                }
            }

            h.clear();
           
            h = _RMI.getEsdIAQStats();
            
            if(!h.isEmpty())
            {
                e= h.keys();
                
                while(e.hasMoreElements())
                {
                    java.lang.Integer i = (java.lang.Integer)e.nextElement();
                    
                    EsdIAQStatsClient _r = (EsdIAQStatsClient)h.get(i);
                    
                    //create child element, add an attribute, and add to root
                    child = doc.createElement("CSQSummary");
                    root.appendChild(child);
                    
                    //CSQID
                    grandChild = doc.createElement("CSQID");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getEsdId()));
                    grandChild.appendChild(text);
                    
                    //CSQName
                    grandChild = doc.createElement("CSQName");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(_r.getEsdName());
                    grandChild.appendChild(text);
                    
                    //CallsPresented
                    grandChild = doc.createElement("CallsPresented");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumTotalContacts()));
                    grandChild.appendChild(text);
                    
                    //CallsHandled
                    grandChild = doc.createElement("CallsHandled");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumHandledContacts()));
                    grandChild.appendChild(text);
                    
                    //CallsAbandoned
                    grandChild = doc.createElement("CallsAbandoned");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumAbandonedContacts()));
                    grandChild.appendChild(text);
                    
                     //CallsDequeued
                    grandChild = doc.createElement("CallsDequeued");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumDequeuedContacts()));
                    grandChild.appendChild(text);
                    
                     //CallsWaiting
                    grandChild = doc.createElement("CallsWaiting");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumWaitingContacts()));
                    grandChild.appendChild(text);
                    
                    //LongestWaitDuration
                    grandChild = doc.createElement("LongestWaitDuration");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getLongestWaitDuration()));
                    grandChild.appendChild(text);
                    
                    //AvgWaitDuration
                    grandChild = doc.createElement("AverageWaitDuration");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getAvgWaitDuration()));
                    grandChild.appendChild(text);
                    
                     //LongestTalkDuration
                    grandChild = doc.createElement("LongestTalkDuration");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getLongestTalkDuration()));
                    grandChild.appendChild(text);
                    
                    //AvgTalkDuration
                    grandChild = doc.createElement("AverageTalkDuration");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getAvgTalkDuration()));
                    grandChild.appendChild(text);
                    
                    //LoggedInAgents
                    grandChild = doc.createElement("AgentsLoggedIn");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumResources()));
                    grandChild.appendChild(text);
                    
                    //AgentsNotReady
                    grandChild = doc.createElement("AgentsNotReady");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumUnavailResources()));
                    grandChild.appendChild(text);
                    
                    //AgentsReady
                    grandChild = doc.createElement("AgentsReady");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumAvailResources()));
                    grandChild.appendChild(text);
                    
                    //AgentSelected
                    grandChild = doc.createElement("AgentsSelected");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumSelectedResources()));
                    grandChild.appendChild(text);
                    
                    //AgentTalking
                    grandChild = doc.createElement("AgentsTalking");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumInSessionResources()));
                    grandChild.appendChild(text);
                    
                    //AgentWork
                    grandChild = doc.createElement("AgentsWork");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getNumWorkResources()));
                    grandChild.appendChild(text);
                    
                    //OldestContactWaiting
                    grandChild = doc.createElement("OldestCallWaiting");
                    child.appendChild(grandChild);
            
                    text = doc.createTextNode(String.valueOf(_r.getLongestCurrentlyWaitingDuration()));
                    grandChild.appendChild(text);
                    
                }
                
            }        

            //Output the XML

            //set up a transformer
            TransformerFactory transfac = TransformerFactory.newInstance();
            Transformer trans = transfac.newTransformer();
            trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
            trans.setOutputProperty(OutputKeys.INDENT, "yes");

            //create string from xml tree
            StringWriter sw = new StringWriter();
            StreamResult result = new StreamResult(sw);
            DOMSource source = new DOMSource(doc);
            trans.transform(source, result);
            String xmlString = sw.toString();

            //WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".CollectRealtimeReportsData(): realtime data:" + xmlString);
            //System.out.print("\n RMIClient.CollectXPVRealtimeData(): Realtime data:" + xmlString);
            
            return xmlString;
            
        }
        catch(Exception ex)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".CollectRealtimeReportsData(): Exception:" + ex.getMessage());
            //System.out.print("\n RMIClient.CollectXPVRealtimeData(): Exception:" + ex.getMessage());
                       
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_DEBUG,this.getClass().getName() + ".CollectRealtimeReportsData(): realtime data:");
            
            return "";
        }
       
    }
    
    public void Dispose()
    {
        try
        {
            _RMI = null;
            _Data = null;
        }
        catch(Exception ex)
        {
            
        }        
    }
   
}
