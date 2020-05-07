package expressodatacollection;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StringReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.tanukisoftware.wrapper.WrapperManager;
import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.xml.sax.InputSource;

public class NodeMonitor implements Runnable
{
    private UCCXCluster _Cluster = null;
    
    public NodeMonitor(UCCXCluster Cluster)
    {
        this._Cluster = Cluster;
        new Thread(this, this.getClass().getName()).start(); 
    }
    
    public void run() 
    {
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + Thread.currentThread().getName() + " has started.");
        //System.out.println("NodeMonitor.run(): " + Thread.currentThread().getName() + " has started.");
        
        this._Cluster.setCurrentMaster("","");
        
        try
        {
            while(true)
            {   
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): begin checking current master...");
               
                if(IsNodeMaster(this._Cluster.getUCCXNode1IPAddress()))
                {
                    this._Cluster.setCurrentMaster(this._Cluster.getUCCXNode1IPAddress(),this._Cluster.getUCCXNode1RTRPort());
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Node 1 is master");
                    //System.out.println("NodeMonitor.run(): Node 1 is master");
                }
                else if(IsNodeMaster(this._Cluster.getUCCXNode2IPAddress()))
                {
                    this._Cluster.setCurrentMaster(this._Cluster.getUCCXNode2IPAddress(),this._Cluster.getUCCXNode2RTRPort());
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Node 2 is master");
                    //System.out.println("NodeMonitor.run(): Node 2 is master");
                }
                else
                {
                    if(this._Cluster.getCurrentMasterIPAddress().length() > 0)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): no master was not detected but was previously set; assume current master as being " + this._Cluster.getCurrentMasterIPAddress());
                    }
                    else
                    {
                        this._Cluster.setCurrentMaster("","");
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_WARN,this.getClass().getName() + ".run(): no master was found");
                        //System.out.println("NodeMonitor.run(): no master was found");
                    }
                }
                
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): end checking current master...");
                
                Thread.sleep(45000);
            } 
        }
        catch(Exception ex)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): Exception:" + ex.getMessage());
            //System.out.println("NodeMonitor.run(): Exception:" + ex.getMessage());
        }
        
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + Thread.currentThread().getName() + " will stop.");
        //System.out.println("NodeMonitor.run(): " + Thread.currentThread().getName() + " will stop.");
    }
    
    private boolean IsNodeMaster(String IPAddress)
    {
        HttpURLConnection connection = null;      
        //OutputStreamWriter wr = null;      
        BufferedReader rd  = null;      
        StringBuilder sb = null;      
        String line = null;          
        URL serverAddress = null;
        
        DocumentBuilderFactory dbf = null;
        DocumentBuilder db = null;
        Document doc = null;
        Node child = null;
        
        boolean result = false;
        
        try 
        {          
            serverAddress = new URL("http://" + IPAddress + "/uccx/isDBMaster");
            //set up out communications stuff
            connection = null;
            //Set up the initial connection
            connection = (HttpURLConnection)serverAddress.openConnection();
            connection.setRequestMethod("GET");
            connection.setDoOutput(true);
            connection.setReadTimeout(10000);
            connection.connect();
            
            //get the output stream writer and write the output to the server
            //not needed in this example
            //wr = new OutputStreamWriter(connection.getOutputStream());
            //wr.write("");
            //wr.flush();
            
            //read the result from the server
            rd  = new BufferedReader(new InputStreamReader(connection.getInputStream()));
            sb = new StringBuilder();
            
            while ((line = rd.readLine()) != null)
            {
                sb.append(line + '\n');
            }
            
            dbf = DocumentBuilderFactory.newInstance();
            
            db = dbf.newDocumentBuilder();
            
            InputSource is = new InputSource();
            is.setCharacterStream(new StringReader(sb.toString()));

            doc = db.parse(is);
            
            doc.getDocumentElement().normalize();
            
            child = doc.getDocumentElement().getFirstChild();
            
            while(true)
            {
                child = child.getNextSibling();
                
                if(child == null)
                {
                    break;
                }
                
                if(child.getNodeName() == "isMaster")
                {
                    result = Boolean.parseBoolean(child.getTextContent());
                }
            }            
        }
        catch (MalformedURLException e) 
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): MalformedURLException:" + e.getMessage());
            //System.out.println("NodeMonitor.run(): MalformedURLException:" + e.getMessage());
            
            result = false;
        }
        catch (ProtocolException e) 
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): ProtocolException:" + e.getMessage());
            //System.out.println("NodeMonitor.run(): ProtocolException:" + e.getMessage());
            
            result = false;
        }
        catch (IOException e) 
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): IOException:" + e.getMessage());
            //System.out.println("NodeMonitor.run(): IOException:" + e.getMessage());
            
            try
            {
                rd  = new BufferedReader(new InputStreamReader(connection.getErrorStream()));
                sb = new StringBuilder();
                
                while ((line = rd.readLine()) != null)
                {
                    sb.append(line + '\n');
                }
            
                String error = sb.toString();
                int begin = error.indexOf("HTTP Status");
                
                if(begin >= 0)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): Status Code:" + error.substring(begin,error.indexOf("</", begin)) + System.getProperty("line.separator") + "Make sure this machine's IP address has been added to Server Name list in 'Real Time Snapshot Writing Configuration for Wallboard' on all UCCX Nodes.");
                    //System.out.println("NodeMonitor.run(): Status Code:" + error.substring(begin,error.indexOf("</", begin)));
                    //System.out.println("NodeMonitor.run(): Make sure this machine's IP address has been added to Server Name list in 'Real Time Snapshot Writing Configuration for Wallboard' on all UCCX Nodes.");
                }
                else
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): Error Description:" + error);
                    //System.out.println("NodeMonitor.run(): Error Description:" + error);
                }
                
                result = false;
            }
            catch(Exception ex)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): Exception getting the error stream:" + ex.getMessage());
                //System.out.println("NodeMonitor.run(): Exception getting the error stream:" + ex.getMessage());
                
                result = false;
            }
        }
        catch(Exception e)
        {
             WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".run(): Exception:" + e.getMessage());
             //System.out.println("NodeMonitor.run(): Exception:" + e.getMessage());
             
             result = false;
        }
        finally
        {
            //close the connection, set all objects to null
            
            child = null;
            
            doc = null;
            db = null;
            dbf = null;
            
            connection.disconnect();
            connection = null;
            
            rd = null;
            sb = null;
            //wr = null;
            
        }
        
        return result;
    }
}
