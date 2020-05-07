package contactdatacollection;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.tanukisoftware.wrapper.WrapperManager;
import org.w3c.dom.Document;

public class ApplicationSettings 
{
    boolean bUseWrapperManager = false;
    
    private String _UCCXNode1IP = "";
    private String _UCCXNode2IP = "";
    
    private String _UCCXRealtimeDataPort = "";
    
    private String _UCCXAdminUser = "";
    private String _UCCXAdminPassword = "";
    
    private String _WebServerPort = "";
    
    private String _EmailFrom = "";
    private String _EmailTo = "";
    private String _EmailSMTPServer = "";
    private String _EmailSMTPPort = "";
    private String _EmailSMTPUser = "";
    private String _EmailSMTPPassword = "";
    private boolean _EmailOnFailure = false;
    private boolean _EmailOnSuccess = false;
    
    public ApplicationSettings(boolean UseWrapperManager) 
    {
        bUseWrapperManager = UseWrapperManager;
        
        _UCCXNode1IP = "";
        _UCCXNode2IP = "";
        _UCCXRealtimeDataPort = "";
        _UCCXAdminUser = "";
        _UCCXAdminPassword = "";
        _WebServerPort = "";
        _EmailFrom = "";
        _EmailTo = "";
        _EmailSMTPServer = "";
        _EmailSMTPPort = "";
        _EmailSMTPUser = "";
        _EmailSMTPPassword = "";
        _EmailOnFailure = false;
        _EmailOnSuccess = false;
    }
    
    public String getUCCXNode1IPAddress()
    {
        return this._UCCXNode1IP;
    }
    
    public String getUCCXNode2IPAddress()
    {
        return this._UCCXNode2IP;
    }
    
    public String getUCCXRealtimeDataPort()
    {
        return _UCCXRealtimeDataPort;
    }
    
    public String getUCCXAdminUser()
    {
        return _UCCXAdminUser;
    }
    
    public String getUCCXAdminPassword()
    {
        return _UCCXAdminPassword;
    }
    
    public String getWebServerPort()
    {
        return _WebServerPort;
    }
    
    public String getEmailFrom()
    {
        return _EmailFrom;
    }
    
    public String getEmailTo()
    {
        return _EmailTo;
    }
    
    public String getEmailSMTPServer()
    {
        return _EmailSMTPServer;
    }
    
    public String getEmailSMTPPort()
    {
        return _EmailSMTPPort;
    }
    
    public String getEmailSMTPUser()
    {
        return _EmailSMTPUser;
    }
    
    public String getEmailSMTPPassword()
    {
        return _EmailSMTPPassword;
    }
    
    public boolean getEmailOnFailure()
    {
        return _EmailOnFailure;
    }
    
    public void setEmailOnFailure(boolean EmailOnFailure)
    {
        _EmailOnFailure = EmailOnFailure;
    }
    
    public boolean getEmailOnSuccess()
    {
        return _EmailOnSuccess;
    }
    
    public void setEmailOnSuccess(boolean EmailOnSuccess)
    {
        _EmailOnSuccess = EmailOnSuccess;
    }
    
    public boolean Load()
    {
        try
        {
            DocumentBuilderFactory dbf = null;
            DocumentBuilder db = null;
            Document doc = null;
        
            String sFilePath = System.getenv("SystemDrive") + "\\ProgramData\\Workflow Concepts\\Callback Server\\ApplicationSettings.xml";
            
            java.io.File f = new java.io.File(sFilePath);
            
            if(!f.exists())
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): File " + sFilePath + " was not found.");            
                }
                else
                {
                    System.out.print("ApplicationSettings.Loas(): File " + sFilePath + " was not found.");
                }
                
                return false;
            }
            
            dbf = DocumentBuilderFactory.newInstance();
            
            db = dbf.newDocumentBuilder();
            
            doc = db.parse(f);
            
            doc.getDocumentElement().normalize();
            
            org.w3c.dom.NodeList nlNode = doc.getElementsByTagName("UCCX");
            org.w3c.dom.Element elElement = null;
            
            if(nlNode == null || nlNode.getLength() == 0)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): UCCX is either null or empty");            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): UCCX is either null or empty");
                }
                
                return false;
            }
            
            if(nlNode.getLength() != 1)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Unexpected number of UCCX elements found.");            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): Unexpected number of UCCX elements found.");
                }
                
                return false;
            }
            
            elElement = (org.w3c.dom.Element)nlNode.item(0);
            
            _UCCXNode1IP = elElement.getAttribute("Node1IPAddress");
            _UCCXNode2IP = elElement.getAttribute("Node2IPAddress");
            _UCCXRealtimeDataPort = elElement.getAttribute("RealtimeDataPort");
            _UCCXAdminUser = elElement.getAttribute("AdminUser");
            _UCCXAdminPassword = elElement.getAttribute("AdminPassword");
            
            elElement = null;
            nlNode = null;
            
            nlNode = doc.getElementsByTagName("WebServer");
            
            if(nlNode == null || nlNode.getLength() == 0)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): WebServer is either null or empty");            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): WebServer is either null or empty");
                }
                
                return false;
            }
            
            if(nlNode.getLength() != 1)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Unexpected number of WebServer elements found.");            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): Unexpected number of WebServer elements found.");
                }
                
                return false;
            }
            
            elElement = (org.w3c.dom.Element)nlNode.item(0);
            
            _WebServerPort = elElement.getAttribute("DataCollectionPort");
            
            elElement = null;
            nlNode = null;
            
            nlNode = doc.getElementsByTagName("EmailNotifications");
            
            if(nlNode == null || nlNode.getLength() == 0)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): EmailNotifications is either null or empty");            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): EmailNotifications is either null or empty");
                }
                
                return false;
            }
            
            if(nlNode.getLength() != 1)
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Unexpected number of EmailNotifications elements found.");            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): Unexpected number of EmailNotifications elements found.");
                }
                
                return false;
            }
            
            elElement = (org.w3c.dom.Element)nlNode.item(0);
            
            AESSymmetricEncription encdec = new AESSymmetricEncription(ApplicationConstants.ENCRYPTION_PASSWORD,ApplicationConstants.ENCRYPTION_SALT);
            
            _EmailFrom = elElement.getAttribute("EmailFrom");
            _EmailTo = elElement.getAttribute("EmailTo");
            _EmailSMTPServer = elElement.getAttribute("SMTPServer");
            _EmailSMTPPort = elElement.getAttribute("SMTPPort");
            
            try
            {
                _EmailSMTPUser = encdec.Decrypt(elElement.getAttribute("SMTPUser"));
            }
            catch(Exception ex)
            {
                _EmailSMTPUser = "";
                
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Exception decrypting SMTPUser: " + ex.getMessage());            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): Exception decrypting SMTPUser: " + ex.getMessage());
                }
            }
            
            try
            {
                _EmailSMTPPassword = encdec.Decrypt(elElement.getAttribute("SMTPPassword"));
            }
            catch(Exception ex)
            {
                _EmailSMTPPassword = "";
                
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Exception decrypting SMTPPassword: " + ex.getMessage());            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): Exception decrypting SMTPPassword: " + ex.getMessage());
                }
            }
            
            try
            {
                _EmailOnFailure = Boolean.parseBoolean(elElement.getAttribute("OnFailure"));
            }
            catch(Exception ex)
            {
                _EmailOnFailure = true;
                
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Exception casting EmailOnFailure: " + ex.getMessage());            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): Exception casting EmailOnFailure: " + ex.getMessage());
                }
            }
            
            try
            {
                _EmailOnSuccess = Boolean.parseBoolean(elElement.getAttribute("OnSuccess"));
            }
            catch(Exception ex)
            {
                _EmailOnSuccess = false;
                
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Exception casting EmailOnSuccess: " + ex.getMessage());            
                }
                else
                {
                    System.out.print("ApplicationSettings.Load(): Exception casting EmailOnSuccess: " + ex.getMessage());
                }
            }
            
            encdec = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            f = null;
            
            return true;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".Load(): Exception: " + ex.getMessage());            
            }
            else
            {
                System.out.print("ApplicationSettings.Load(): Exception: " + ex.getMessage());
            }
            
            return false;
        }
    }
}
