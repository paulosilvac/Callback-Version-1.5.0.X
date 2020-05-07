package contactdatacollection;

import org.tanukisoftware.wrapper.WrapperManager;

public class ShutdownHook extends Thread
{
    ApplicationSettings _ApplicationSettings = null;
    EmailNotifier _Notifier = null;
    boolean bUseWrapperManager = false;
    
    public ShutdownHook(ApplicationSettings Settings, EmailNotifier Notifier, boolean UseWrapperManager)
    {
        _ApplicationSettings = Settings;
        _Notifier = Notifier;
        bUseWrapperManager = UseWrapperManager;
    }
    
    public void run()
    {
        try
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ShutdownHook.run(): Enter.");
            }
            else
            {
                System.out.println("ShutdownHook.run(): Enter.");
            }
            
            if(_ApplicationSettings.getEmailOnSuccess())
            {
                if(_Notifier.SendEmail(_ApplicationSettings.getEmailFrom(), _ApplicationSettings.getEmailTo(), "Contact Data Collection service stopped", ""))
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ShutdownHook.run(): Service stopped email was sent.");
                    }
                    else
                    {
                        System.out.println("ShutdownHook.run(): Service stopped email was sent.");
                    }
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ShutdownHook.run(): Service stopped email failed to send.");
                    }
                    else
                    {
                        System.out.println("ShutdownHook.run(): Service stopped email failed to send.");
                    }
                }
            }//if(_ApplicationSettings.getEmailOnSuccess())
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"ShutdownHook.run(): Exception:" + ex.getMessage());
            }
            else
            {
                System.out.println("ShutdownHook.run(): Exception:" + ex.getMessage());
            }
        }
    }
}
