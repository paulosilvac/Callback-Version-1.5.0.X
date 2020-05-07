package expressodatacollection;

import java.util.ArrayList;
import org.tanukisoftware.wrapper.WrapperManager;

public class ClientMonitor implements Runnable
{
    private ArrayList<java.net.Socket> _Clients = null;
    
    public ClientMonitor(ArrayList<java.net.Socket> Clients)
    {
        this._Clients = Clients;
        
        new Thread(this, "ClientMonitor").start();
    }
    
    public void run()
    {
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + Thread.currentThread().getName() + " has started.");
        //System.out.println("ClientMonitor.run(): " + Thread.currentThread().getName() + " has started.");
        
        try
        {
            ArrayList<java.net.Socket> _ToBeRemoved = new ArrayList<java.net.Socket>();
            
            while(true)
            {   
                if(this._Clients.size() == 0)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): No clients currently connected.");
                    //System.out.println("ClientMonitor.run(): No clients currently connected.");
                }
                else
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): # of clients currently connected:" + this._Clients.size());
                    //System.out.println("ClientMonitor.run(): # of clients currently connected:" + this._Clients.size());
                    
                    _ToBeRemoved.clear();
                    
                    for(java.net.Socket s:this._Clients)
                    {
                        try
                        {   
                            if(s.isClosed())
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + s.getRemoteSocketAddress().toString().substring(1) + " appears to be closed; it will be removed from the client list.");
                                //System.out.println("ClientMonitor.run(): " + s.getRemoteSocketAddress().toString().substring(1) + " appears to be closed; it will be removed from the client list.");
                                
                                _ToBeRemoved.add(s);
                            }
                            else
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + s.getRemoteSocketAddress().toString().substring(1));
                                System.out.println("ClientMonitor.run(): " + s.getRemoteSocketAddress().toString().substring(1));
                            }
                        }
                        catch(Exception ex)
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Exception: " + ex.getMessage());
                            //System.out.println("ClientMonitor.run(): Exception: " + ex.getMessage());
                        }                        
                    }
                    
                    if(_ToBeRemoved.size() > 0)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + _ToBeRemoved.size() + " clients appear to be closed. Removing them from client list...");
                        //System.out.println("ClientMonitor.run(): " + _ToBeRemoved.size() + " clients appear to be closed. Removing them from client list...");
                        
                        for(java.net.Socket s:_ToBeRemoved)
                        {
                            try
                            {
                                this._Clients.remove(s);
                                
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + s.getRemoteSocketAddress().toString().substring(1) + " was removed.");
                                //System.out.println("ClientMonitor.run(): " + s.getRemoteSocketAddress().toString().substring(1) + " was removed.");
                            }
                            catch(Exception ex)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Exception removing " + s.getRemoteSocketAddress().toString().substring(1) + ex.getMessage());
                                //System.out.println("ClientMonitor.run(): Exception removing " + s.getRemoteSocketAddress().toString().substring(1) + ex.getMessage());
                            }                           
                        }
                        
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): finished removing closed clients them from client list...");
                        //System.out.println("ClientMonitor.run(): finished removing closed clients them from client list...");
                    }
                }
                
                Thread.sleep(5000);
            } 
        }
        catch(Exception ex)
        {
            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): Exception:" + ex.getMessage());
            //System.out.println("NodeMonitor.run(): Exception:" + ex.getMessage());
        }
        
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".run(): " + Thread.currentThread().getName() + " will stop.");
        //System.out.println("NodeMonitor.run(): " + Thread.currentThread().getName() + " will stop.");
    }
}
