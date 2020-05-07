package contactdatacollection;

import org.tanukisoftware.wrapper.WrapperManager;

public class EmailNotifier 
{
    private boolean bUseTLS = false;
    private boolean bUseAuthentication = false;
    
    private String sSMTPServer = "";
    private String sSMTPPort = "";
    private String sSMTPUser = "";
    private String sSMTPPassword = "";
    
    private String sSubjectPrefix = "";
    
    boolean bUseWrapperManager = false;
    
    public boolean getUseAuthentication()
    {
        return bUseAuthentication;
    }
    
    public void setUseAuthentication(boolean UseAuthentication)
    {
        bUseAuthentication = UseAuthentication;
    }
    
    public String getSMTPServer()
    {
        return sSMTPServer;
    }
    
    public String getSMTPPort()
    {
        return sSMTPPort;
    }
    
    public String getSMTPUser()
    {
        return sSMTPUser;
    }
    
    public String getSMTPPassword()
    {
        return sSMTPPassword;
    }
    
    public void setSMTPServer(String SMTPServer)
    {
        sSMTPServer = SMTPServer;
    }
    
    public void setSMTPPort(String SMTPPort)
    {
        sSMTPPort = SMTPPort;
    }
    
    public void setSMTPUser(String SMTPUser)
    {
        sSMTPUser = SMTPUser;
    }
    
    public void setSMTPPassword(String SMTPPassword)
    {
        sSMTPPassword = SMTPPassword;
    }
    
    public void setSubjectPrefix(String SubjectPrefix)
    {
        sSubjectPrefix = SubjectPrefix;
    }
    
    public EmailNotifier(boolean UseWrapperManager)
    {
        sSMTPServer = "";
        sSMTPPort = "";
        sSMTPUser = "";
        sSMTPPassword = "";
        
        sSubjectPrefix = "";
        
        bUseWrapperManager = UseWrapperManager;
    }
    
    public EmailNotifier(String SMTPServer, String SMTPPort, String SMTPUser, String SMTPPassword, String SubjectPrefix, boolean UseWrapperManager)
    {
        sSMTPServer = SMTPServer;
        sSMTPPort = SMTPPort;
        sSMTPUser = SMTPUser;
        sSMTPPassword = SMTPPassword;
        
        sSubjectPrefix = SubjectPrefix;
        
        bUseWrapperManager = UseWrapperManager;
    }
    
    public boolean SendEmail(String From, String To, String Subject, String Body)
    {
        try
        {
            java.util.Properties prop = System.getProperties();
        
            if(bUseTLS)
            {
                prop.setProperty("mail.smtp.starttls.enable", "true");
            }
            
            if(bUseAuthentication)
            {
                prop.setProperty("mail.smtp.auth", "true");
            }
            
            prop.put("mail.smtp.host", sSMTPServer);
            prop.put("mail.smtp.socketFactory.port", sSMTPPort);
            prop.put("mail.smtp.socketFactory.class", "javax.net.SocketFactory");
            prop.put("mail.smtp.auth", "true");
            prop.put("mail.smtp.port", sSMTPPort);
            prop.put("mail.smtp.ssl.enable", "false");
            prop.put("mail.smtp.starttls.enable", "true");
            prop.put("mail.smtp.ssl.trust", sSMTPServer);
            
            javax.mail.Session session = javax.mail.Session.getInstance(prop, null);

            javax.mail.internet.MimeMessage message = new javax.mail.internet.MimeMessage(session);
        
            message.setFrom(new javax.mail.internet.InternetAddress(From));
            message.addRecipients(javax.mail.Message.RecipientType.TO,javax.mail.internet.InternetAddress.parse(To.replace(";", ",")));
            
            message.setSubject(sSubjectPrefix + Subject);
            
            javax.mail.internet.MimeBodyPart textBodyPart = new javax.mail.internet.MimeBodyPart();
            textBodyPart.setText(Body);
        
            javax.mail.internet.MimeMultipart mimeMultipart = new javax.mail.internet.MimeMultipart();
            mimeMultipart.addBodyPart(textBodyPart);
            
            message.setContent(mimeMultipart);
            
            javax.mail.Transport transport = session.getTransport("smtp");
            
            if(bUseAuthentication)
            {
                if(sSMTPUser.equals("") || sSMTPPassword.equals(""))
                {
                    transport.connect();
                }
                else
                {
                    transport.connect(sSMTPServer, Integer.parseInt(sSMTPPort), sSMTPUser, sSMTPPassword);
                }
            }
            else
            {
                transport.connect();
            }
            
            transport.sendMessage(message, message.getAllRecipients());
            
            transport.close();
            transport = null;
            
            message = null;
            
            session = null;
            
            return true;
        }
        catch (javax.mail.internet.AddressException e) 
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"EmailNotifier.SendEmail(): AddressException: " + e.getMessage());
            }
            else
            {
                System.out.println("EmailNotifier.SendEmail(): AddressException: " + e.getMessage());
            }
            
            return false;
        } 
        catch (javax.mail.MessagingException e) 
        {
            try
            {
                java.io.StringWriter sw = new java.io.StringWriter();
                e.printStackTrace(new java.io.PrintWriter(sw));
                
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"EmailNotifier.SendEmail(): MessagingException: " + e.getMessage());
                }
                else
                {
                    System.out.println("EmailNotifier.SendEmail(): MessagingException: " + e.getMessage());
                }
                
                sw.close();
                sw = null;
            }
            catch(Exception ex)
            {
            
            }
            
            return false;
        }
        catch(Exception e)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,"EmailNotifier.SendEmail(): Exception: " + e.getMessage());
            }
            else
            {
                System.out.println("EmailNotifier.SendEmail(): Exception: " + e.getMessage());
            }
            
            return false;
        }
    }
}
