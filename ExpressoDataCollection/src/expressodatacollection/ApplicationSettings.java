package expressodatacollection;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.tanukisoftware.wrapper.WrapperManager;
import org.w3c.dom.Document;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;

public class ApplicationSettings 
{    
    private String _UCCXNode1IPAddress;
    private String _UCCXNode1RTRPort;
    
    private String _UCCXNode2IPAddress;
    private String _UCCXNode2RTRPort;
    
    private String _DataCollectorIP;
    private String _DataCollectorPort;
    
    /** Creates a new instance of ApplicationSettings */
    public ApplicationSettings() 
    {
        
    }
    
    public boolean Load()
    {
        java.io.File Folder = null;
        java.io.File f = null;
        DocumentBuilderFactory dbf = null;
        DocumentBuilder db = null;
        Document doc = null;
        Node child = null;
        NamedNodeMap attributes = null;
        Node tmp = null;
        
        try
        {
            Folder = new java.io.File("");
           
            String Parents = "Data Collection\\bin";
            
            int i = Folder.getAbsolutePath().indexOf(Parents);
            
            if( i >= 0 )
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Load(): attempting to load " + Folder.getAbsolutePath().substring(0,i) + "ApplicationSettings.xml");
            
                f = new java.io.File(Folder.getAbsolutePath().substring(0,i) + "ApplicationSettings.xml");
            }
            else
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Load(): attempting to load " + Folder.getAbsolutePath() + "\\ApplicationSettings.xml");
                
                f = new java.io.File(Folder.getAbsolutePath() + "\\ApplicationSettings.xml");
            }          
            
            Folder = null;
            
            dbf = DocumentBuilderFactory.newInstance();
            
            db = dbf.newDocumentBuilder();
            
            doc = db.parse(f);
            
            doc.getDocumentElement().normalize();
            
            child = doc.getDocumentElement().getFirstChild();
            
            while(true)
            {
                child = child.getNextSibling();
                
                if(child == null)
                {
                    break;
                }
                
                if(child.getNodeName() == "XPVServer")
                {
                    attributes = child.getAttributes();
            
                    tmp = attributes.getNamedItem("IP");
            
                    this.setDataCollectorIpAddress(tmp.getTextContent());
            
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Load(): XPVServer.IP = " + tmp.getTextContent());
                }
                else if(child.getNodeName() == "XPVDataCollection")
                {
                    attributes = child.getAttributes();
            
                    tmp = attributes.getNamedItem("DataPort");
            
                    this.setDataCollectorPort(tmp.getTextContent());
                    
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Load(): XPVDataCollection.DataPort = " + tmp.getTextContent());
                }
                else if(child.getNodeName() == "UCCXNode1")
                {
                    attributes = child.getAttributes();
            
                    tmp = attributes.getNamedItem("IP");
            
                    this.setUCCXNode1IPAddress(tmp.getTextContent());

                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Load(): UCCX.UCCXNode1IPAddress = " + tmp.getTextContent());
                    
                    tmp = attributes.getNamedItem("RTRPort");
            
                    this.setUCCXNode1RTRPort(tmp.getTextContent());
                    
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Load(): UCCX.UCCXNode1RTRPort = " + tmp.getTextContent());
                }
                else if(child.getNodeName() == "UCCXNode2")
                {
                    attributes = child.getAttributes();
            
                    tmp = attributes.getNamedItem("IP");
            
                    this.setUCCXNode2IPAddress(tmp.getTextContent());

                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Load(): UCCX.UCCXNode2IPAddress " + tmp.getTextContent());
                    
                    tmp = attributes.getNamedItem("RTRPort");
            
                    this.setUCCXNode2RTRPort(tmp.getTextContent());
                    
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".Load(): UCCX.UCCXNode2RTRPort = " + tmp.getTextContent());
                }
                else
                {
                    //continue;
                }
                 
            }
            
            tmp = null;
            child = null;
            attributes = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            f = null;
            
            return true;
        }
        catch(Exception ex)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Exception: " + ex.getMessage());            
            return false;
        } 
        
    }
    
    private void setUCCXNode1IPAddress(String Value)
    {
        this._UCCXNode1IPAddress = Value;
    }
    
    private void setUCCXNode1RTRPort(String Value)
    {
        this._UCCXNode1RTRPort = Value;
    }
    
    private void setUCCXNode2IPAddress(String Value)
    {
        this._UCCXNode2IPAddress = Value;
    }
    
    private void setUCCXNode2RTRPort(String Value)
    {
        this._UCCXNode2RTRPort = Value;
    }
    
     private void setDataCollectorIpAddress(String Value)
    {
        this._DataCollectorIP = Value;
    }
    
    private void setDataCollectorPort(String Value)
    {
        this._DataCollectorPort = Value;
    }
    
    public String getUCCXNode1IPAddress()
    {
        return this._UCCXNode1IPAddress;
    }
    
    public String getUCCXNode1RTRPort()
    {
        return this._UCCXNode1RTRPort;
    }
    
    public String getUCCXNode2IPAddress()
    {
        return this._UCCXNode2IPAddress;
    }
    
    public String getUCCXNode2RTRPort()
    {
        return this._UCCXNode2RTRPort;
    }
    
    public String getDataCollectorIPAddress()
    {
        return this._DataCollectorIP;
    }
    
    public String getDataCollectorPort()
    {
        return this._DataCollectorPort;
    }
}
