package contactdatacollection;

import com.sun.net.httpserver.HttpServer;
import java.net.InetSocketAddress;
import org.tanukisoftware.wrapper.WrapperManager;

public class ContactDataCollection 
{
    public static void main(String[] args) throws InterruptedException 
    {
        boolean bUseWrapperManager = false;
        
        if(bUseWrapperManager)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection." + Thread.currentThread().getStackTrace()[1].getMethodName() + ": Enter.");
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): Version: 1.0.0.23");
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): Runtme:" + System.getProperty("java.version"));
        }
        else
        {
            System.out.println("ContactDataCollection.main(): Enter.");
            System.out.println("ContactDataCollection.main(): Version: 1.0.0.17");
            System.out.println("ContactDataCollection.main(): Runtme:" + System.getProperty("java.version"));
        }
        
        ApplicationSettings _ApplicationSettings = new ApplicationSettings(bUseWrapperManager);
        
        if(_ApplicationSettings.Load())
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.Load() returned true.");
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getUCCXNode1IPAddress = " + _ApplicationSettings.getUCCXNode1IPAddress());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getUCCXNode2IPAddress = " + _ApplicationSettings.getUCCXNode2IPAddress());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getUCCXRealtimeDataPort = " + _ApplicationSettings.getUCCXRealtimeDataPort());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getUCCXAdminUser() = " + _ApplicationSettings.getUCCXAdminUser());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getUCCXAdminPassword() = " + _ApplicationSettings.getUCCXAdminPassword());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getEmailFrom() = " + _ApplicationSettings.getEmailFrom());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getEmailTo() = " + _ApplicationSettings.getEmailTo());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getEmailSMTPServer() = " + _ApplicationSettings.getEmailSMTPServer());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getEmailSMTPPort() = " + _ApplicationSettings.getEmailSMTPPort());
                //WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getEmailSMTPUser() = " + _ApplicationSettings.getEmailSMTPUser());
                //WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getEmailSMTPPassword() = " + _ApplicationSettings.getEmailSMTPPassword());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getEmailOnSuccess() = " + _ApplicationSettings.getEmailOnSuccess());
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): _ApplicationSettings.getEmailOnFailure() = " + _ApplicationSettings.getEmailOnFailure());
            }
            else
            {
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.Load() returned true.");
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getUCCXNode1IPAddress = " + _ApplicationSettings.getUCCXNode1IPAddress());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getUCCXNode2IPAddress = " + _ApplicationSettings.getUCCXNode2IPAddress());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getUCCXRealtimeDataPort = " + _ApplicationSettings.getUCCXRealtimeDataPort());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getUCCXAdminUser() = " + _ApplicationSettings.getUCCXAdminUser());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getUCCXAdminPassword() = " + _ApplicationSettings.getUCCXAdminPassword());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getEmailFrom() = " + _ApplicationSettings.getEmailFrom());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getEmailTo() = " + _ApplicationSettings.getEmailTo());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getEmailSMTPServer() = " + _ApplicationSettings.getEmailSMTPServer());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getEmailSMTPPort() = " + _ApplicationSettings.getEmailSMTPPort());
                //System.out.println("ContactDataCollection.main(): _ApplicationSettings.getEmailSMTPUser() = " + _ApplicationSettings.getEmailSMTPUser());
                //System.out.println("ContactDataCollection.main(): _ApplicationSettings.getEmailSMTPPassword() = " + _ApplicationSettings.getEmailSMTPPassword());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getEmailOnSuccess() = " + _ApplicationSettings.getEmailOnSuccess());
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.getEmailOnFailure() = " + _ApplicationSettings.getEmailOnFailure());
            }
        }
        else
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): _ApplicationSettings.Load() returned false.");
            }
            else
            {
                System.out.println("ContactDataCollection.main(): _ApplicationSettings.Load() returned false.");
            }
                
            return;
        }
        
        String sMachineName = "";
        
        try
        {
            sMachineName = java.net.InetAddress.getLocalHost().getHostName() + " - ";
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): Exception getting machine name:" + ex.getMessage());
            }
            else
            {
                System.out.println("ContactDataCollection.main(): Exception  getting machine name:" + ex.getMessage());
            }
            
            sMachineName = "";
        }
        
        EmailNotifier _EmailNotifier = new EmailNotifier(bUseWrapperManager);
        
        _EmailNotifier.setSMTPServer(_ApplicationSettings.getEmailSMTPServer());
        _EmailNotifier.setSMTPPort(_ApplicationSettings.getEmailSMTPPort());
        _EmailNotifier.setSMTPUser(_ApplicationSettings.getEmailSMTPUser());
        _EmailNotifier.setSMTPPassword(_ApplicationSettings.getEmailSMTPPassword());
        _EmailNotifier.setSubjectPrefix(sMachineName);
        _EmailNotifier.setUseAuthentication(true);
        
        UCCXCluster _Cluster = new UCCXCluster(_ApplicationSettings,bUseWrapperManager);
        
        NodeMonitor nm = new NodeMonitor(_Cluster,bUseWrapperManager);
        
        _Cluster.setCurrentMaster(_ApplicationSettings.getUCCXNode1IPAddress(), _ApplicationSettings.getUCCXRealtimeDataPort());
        
        RMIClient rmic = new RMIClient(_Cluster,_EmailNotifier,_ApplicationSettings,bUseWrapperManager);
        
        HttpServer server = null;
        
        Runtime.getRuntime().addShutdownHook(new ShutdownHook(_ApplicationSettings,_EmailNotifier,bUseWrapperManager));
        
        try
        {
            //String sResponse = rmic.GetContactsRealtimeDataXML();
            
            //System.out.println("Response: " + sResponse);
            
            String sToken = _ApplicationSettings.getUCCXAdminUser() + _ApplicationSettings.getUCCXAdminPassword();
            
            sToken = sToken.replaceAll("[^A-Za-z0-9]", "");
            
            String sPrefix = "/uccxrealtimedata";
            int iPort = Integer.parseInt(_ApplicationSettings.getWebServerPort());
            
            server = HttpServer.create(new InetSocketAddress(iPort), 0);
            server.createContext(sPrefix, new HttpRequestHandler(rmic, sToken, new java.util.Date(), bUseWrapperManager));
            
            server.setExecutor(null);
            server.start();
            
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,"ContactDataCollection.main(): Web server is listening on ");
            }
            else
            {
                System.out.println("ContactDataCollection.main(): Web server is listening on " + server.getAddress().toString() + sPrefix);
            }
            
            if(_ApplicationSettings.getEmailOnSuccess())
            {
                if(_EmailNotifier.SendEmail(_ApplicationSettings.getEmailFrom(), _ApplicationSettings.getEmailTo(), "Contact Data Collection service started", ""))
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): Service start email was sent.");
                    }
                    else
                    {
                        System.out.println("ContactDataCollection.main(): Service start email was sent.");
                    }
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): Service start email failed to send.");
                    }
                    else
                    {
                        System.out.println("ContactDataCollection.main(): Service start email failed to send.");
                    }
                }
            }
        }
        catch(java.io.IOException ioex)
        {
            if(_ApplicationSettings.getEmailOnFailure())
            {
                if(_EmailNotifier.SendEmail(_ApplicationSettings.getEmailFrom(), _ApplicationSettings.getEmailTo(), "Contact Data Collection service failed to start", ioex.getMessage()))
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): Startup email was sent.");
                    }
                    else
                    {
                        System.out.println("ContactDataCollection.main(): Startup email was sent.");
                    }
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): Startup email failed to send.");
                    }
                    else
                    {
                        System.out.println("ContactDataCollection.main(): Startup email failed to send.");
                    }
                }
            }
            
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): IOException starting web server:" + ioex.getMessage());
            }
            else
            {
                System.out.println("ContactDataCollection.main(): IOException starting web server:" + ioex.getMessage());
            }
        }
        catch(Exception ex)
        {
            if(_ApplicationSettings.getEmailOnFailure())
            {
                if(_EmailNotifier.SendEmail(_ApplicationSettings.getEmailFrom(), _ApplicationSettings.getEmailTo(), "Contact Data Collection service failed to start", ex.getMessage()))
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): Startup email was sent.");
                    }
                    else
                    {
                        System.out.println("ContactDataCollection.main(): Startup email was sent.");
                    }
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): Startup email failed to send.");
                    }
                    else
                    {
                        System.out.println("ContactDataCollection.main(): Startup email failed to send.");
                    }
                }
            }
            
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ContactDataCollection.main(): Exception starting web server:" + ex.getMessage());
            }
            else
            {
                System.out.println("ContactDataCollection.main(): Exception starting web server:" + ex.getMessage());
            }
        }
    }
}
