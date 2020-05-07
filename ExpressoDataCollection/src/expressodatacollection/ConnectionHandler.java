
package expressodatacollection;

import java.net.*;
import java.io.*;
import java.util.ArrayList;
import org.tanukisoftware.wrapper.WrapperManager;

public class ConnectionHandler implements Runnable
{
    private String TOKEN  = "EXPRESSO@WORKFLOWCONCEPTS";
    
    private Socket connection = null;
    private RMIClient _RMIClient = null;
    private UCCXCluster _Cluster = null;
    private RealtimeReportsData _Data = null;
    private ArrayList<java.net.Socket> _Clients = null;
    
    private BufferedInputStream is = null;
    private BufferedOutputStream os = null;
        
    public ConnectionHandler(Socket _Client,UCCXCluster Cluster, RealtimeReportsData Data,ArrayList<java.net.Socket> Clients)
    {
        this.connection = _Client;
        this._Cluster = Cluster;
        this._Data = Data;
        this._Clients = Clients;
        
        is = null;
        os = null;
        
        new Thread(this, "ConnectionHandler").start();         
    }
    
    public void run()
    {
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Enter.");
        //System.out.println("ConnectionHandler.run(): Enter.");
        
        String RemoteAddress = this.connection.getRemoteSocketAddress().toString().substring(1);
        
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Remote address " + RemoteAddress);
        //System.out.println("ConnectionHandler.run(): Remote address " + RemoteAddress);
        
        int character;
        
        StringBuffer process = null;
        
        String message = "";
        
        try 
        {             
            is = new BufferedInputStream(connection.getInputStream());
            os = new BufferedOutputStream(connection.getOutputStream(),5096);
            
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Read and Write streams instantiated.");
            //System.out.println("ConnectionHandler.run(): Read and Write streams instantiated.");
            
            boolean SendRejectionMessage = false;
            
            if(this._Clients.size() >= 5)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): Maximum number of allowed connections has been established");
                //System.out.println("ConnectionHandler.run(): Maximum number of allowed connections has been established.");
                
                SendRejectionMessage = true;
            }
            else
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Under the maximum number of allowed connections.");
                //System.out.println("ConnectionHandler.run(): Under the maximum number of allowed connections.");
                
                SendRejectionMessage = false;
                this._Clients.add(connection);
            }
            
            while(true)
            {
                if(is!=null)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Read stream is still open.");
                    //System.out.println("ConnectionHandler.run(): Read stream is still open.");
                }
                else
                {
                    is = new BufferedInputStream(connection.getInputStream());
                }
            
                process = new StringBuffer();
          
                while((character = is.read()) != 13) 
                {                    
                    if(character == -1)
                    {
                        throw new Exception("Socket " + RemoteAddress + " appears to have closed.");
                    }
                    
                    process.append((char)character);                    
                }
            
                Thread.sleep(250);
                
                if(SendRejectionMessage)
                {
                    message = "<xpvdatacollection><code>100</code><description>Maximum number of allowed connections has been established</description></xpvdatacollection>";
                    
                    WriteToStream(message,true);
                    
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Rejection message was sent.");
                    //System.out.println("ConnectionHandler.run(): Rejection message was sent.");
                    
                    throw new Exception("Maximum number of allowed connections has been established");                    
                }
                
                if(this.TOKEN.contentEquals(process))
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Good token received.");
                    //System.out.println("Server.run(): Good token received.");
                    
                    if(this._Cluster.getCurrentMasterIPAddress().length() != 0)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): Master node detected.");
                        //System.out.println("ConnectionHandler.run(): Master node not detected.");
                        
                        message = "<xpvdatacollection><code>100</code><description>Realtime gathered</description><MasterNode>" + this._Cluster.getCurrentMasterIPAddress() + "</MasterNode><RealtimeData>" + this._Data.getData() + "</RealtimeData></xpvdatacollection>";
                    }
                    else
                    {
                        message = "<xpvdatacollection><code>120</code><description>Master node not detected</description></xpvdatacollection>";                  
                    }
                    
                    //System.out.println("ConnectionHandler.run(): " + message);
                    
                    WriteToStream(message,true);
                }
                else //if(this.TOKEN.contentEquals(process))
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): Bad token received.");
                    //System.out.println("ConnectionHandler.run(): Bad token received");
                    
                    message = "<xpvdatacollection><code>110</code><description>Bad token received</description></xpvdatacollection>";
                    
                    WriteToStream(message,true);
                    
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Rejection message was sent");
                    //System.out.println("ConnectionHandler.run(): Rejection message was sent.");
                    
                    throw new Exception("Bad token received");                    
                    
                } //if(this.TOKEN.contentEquals(process))
            
                process = null;
                
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Request handled.");
                //System.out.println("ConnectionHandler.run(): Request handled.");  
                
            } //while(true)
                
        }
        catch(SocketTimeoutException se)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): SocketException: Connection timed out." + se.getMessage());
            //System.out.println("ConnectionHandler.run(): SocketException: Connection timed out." + se.getMessage());
            
        }
        catch (Exception e) 
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): Exception:" + e.getMessage());
            //System.out.println("ConnectionHandler.run(): Exception:" + e.getMessage());
        }
        finally
        {
            try
            {
                is.close();
                os.close();
                connection.close();               
                
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): ConnectionHandler terminated.");
                //System.out.println("ConnectionHandler.run(): ConnectionHandler terminated.");
            }
            catch(Exception e)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): Exception closing connection:"  + e.getMessage());
                //System.out.println("ConnectionHandler.run(): Exception closing connection:"  + e.getMessage());
            }
            
            is = null;
            os = null;
            connection = null;
        }
        
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Exit. Connection to " + RemoteAddress + " terminated.");
        //System.out.println("ConnectionHandler.run(): Exit. Connection to " + RemoteAddress + " terminated.");
    }
    
    private String FormatLength(String Value)
    {
        String tmp=":";
        
        for(int i =0;i<=(8-Value.length()-1);i++)
        {
            tmp = tmp + "0";
        }
        
        tmp = tmp + Value + ":";
        
        return tmp;
    }
    
    private void WriteToStream(String Message,boolean PrefixWithLength) throws Exception
    {
        if(os!=null)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".WriteToStream(): Write stream is still open.");
            //System.out.println("ConnectionHandler.WriteToStream(): Write stream is still open.");
        }
        else
        {
            os = new BufferedOutputStream(this.connection.getOutputStream(),5096);
        }                     
               
        if(PrefixWithLength)
        {
            Message = FormatLength(String.valueOf(Message.length())) + Message;
        }
        
        os.write(Message.getBytes("US-ASCII"));
        os.flush();
        
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".WriteToStream(): # of bytes written to stream:" + Message.length());
        //System.out.println("ConnectionHandler.run(): # of bytes written to stream:" + Message.length()); 
    }
}
