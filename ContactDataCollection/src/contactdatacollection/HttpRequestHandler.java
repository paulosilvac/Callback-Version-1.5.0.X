package contactdatacollection;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;
import java.io.IOException;
import java.io.OutputStream;
import java.util.Calendar;
import org.tanukisoftware.wrapper.WrapperManager;
import sun.management.ManagementFactory;

public class HttpRequestHandler implements HttpHandler 
{
    String XMLDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>";
    
    String sAcceptedToken = "";
    
    boolean bUseWrapperManager = false;
    
    RMIClient _RMIClient = null;
    
    java.util.Date _dServiceStart = new java.util.Date();
    
    public HttpRequestHandler(RMIClient RealtimeDataClient, String Token, java.util.Date dServiceStart, boolean UseWrapperManager)
    {
        _RMIClient = RealtimeDataClient;
        sAcceptedToken = Token;
        _dServiceStart = dServiceStart;
        bUseWrapperManager = UseWrapperManager;
    }
        
    public void handle(HttpExchange t)
    {
        try
        {
            String sResponse = "";

            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".handle(): Request query: " + t.getRequestURI().getRawQuery());
            }
            else
            {
                System.out.println("HttpRequestHandler.handle(): Request query: " + t.getRequestURI().getRawQuery());
            }
            
            if(_RMIClient != null)
            {
                java.util.Map<String, String> params = queryToMap(t.getRequestURI().getQuery());

                if(params != null)
                {
                    String sToken = "";
                    String sOperation = "";
                    boolean bTesting = false;
                    int iNumberOfRecords = 0;
                    int iAvgResponseDelay = 0;
                    
                    for (java.util.Map.Entry<String, String> param : params.entrySet())
                    {
                        if(param.getKey().equals("token"))
                        {
                            sToken = param.getValue();
                        }
                        
                        if(param.getKey().equals("operation"))
                        {
                            sOperation = param.getValue();
                        }
                        
                        if(param.getKey().equals("testing"))
                        {
                            try
                            {
                                bTesting = Boolean.parseBoolean(param.getValue());
                            }
                            catch(Exception ex)
                            {
                                if(bUseWrapperManager)
                                {
                                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".handle(): Exception: " + ex.getMessage());
                                }
                                else
                                {
                                    System.out.println("HttpRequestHandler.handle(): Exception: " + ex.getMessage());
                                }
                                
                                bTesting = false;
                            }
                        }
                        
                        if(param.getKey().equals("numberofrecords"))
                        {
                            try
                            {
                                iNumberOfRecords = Integer.parseInt(param.getValue());
                            }
                            catch(Exception ex)
                            {
                                if(bUseWrapperManager)
                                {
                                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".handle(): Exception: " + ex.getMessage());
                                }
                                else
                                {
                                    System.out.println("HttpRequestHandler.handle(): Exception: " + ex.getMessage());
                                }
                                
                                iNumberOfRecords = 1;
                            }
                        }
                        
                        if(param.getKey().equals("avgresponsedelay"))
                        {
                            try
                            {
                                iAvgResponseDelay = Integer.parseInt(param.getValue());
                            }
                            catch(Exception ex)
                            {
                                if(bUseWrapperManager)
                                {
                                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".handle(): Exception: " + ex.getMessage());
                                }
                                else
                                {
                                    System.out.println("HttpRequestHandler.handle(): Exception: " + ex.getMessage());
                                }
                                
                                iAvgResponseDelay = 100;
                            }
                        }
                        
                    }//for (java.util.Map.Entry<String, String> param : params.entrySet())
                    
                    if(sOperation.equals("getcontactdata"))
                    {
                        if(sToken.equals(sAcceptedToken))
                        {
                            if(bTesting)
                            {   
                                sResponse = _RMIClient.GetTestContactData(iNumberOfRecords,iAvgResponseDelay);
                                
                                //sResponse = XMLDeclaration;
                                //sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                                //sResponse = sResponse + System.getProperty("line.separator") + "<code>-100</code>";
                                //sResponse = sResponse + System.getProperty("line.separator") + "<description>Fake contact data with " + iNumberOfRecords + " records will be generated</description>";
                                //sResponse = sResponse + System.getProperty("line.separator") + "</response>";
                            }
                            else
                            {
                                Calendar calBefore = Calendar.getInstance();

                                sResponse = _RMIClient.GetContactsData();

                                Calendar calAfter = Calendar.getInstance();

                                if(bUseWrapperManager)
                                {
                                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".handle(): realtime data retrieved in " + (calAfter.getTime().getTime() - calBefore.getTime().getTime()) + " ms");
                                }
                                else
                                {
                                    System.out.println("HttpRequestHandler.handle(): realtime data retrieved in " + (calAfter.getTime().getTime() - calBefore.getTime().getTime()) + " ms");
                                }
                            }
                        }
                        else
                        {
                            sResponse = XMLDeclaration;
                            sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<code>-1</code>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<description>Invalid token received.</description>";
                            sResponse = sResponse + System.getProperty("line.separator") + "</response>";
                        }
                    }
                    else if(sOperation.equals("getcsqdata"))
                    {
                        if(sToken.equals(sAcceptedToken))
                        {
                            if(bTesting)
                            {
                                sResponse = _RMIClient.GetTestCSQData(iNumberOfRecords,iAvgResponseDelay);
                                
                                //sResponse = XMLDeclaration;
                                //sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                                //sResponse = sResponse + System.getProperty("line.separator") + "<code>-100</code>";
                                //sResponse = sResponse + System.getProperty("line.separator") + "<description>Fake CSQ data with " + iNumberOfRecords + " records will be generated</description>";
                                //sResponse = sResponse + System.getProperty("line.separator") + "</response>";
                            }
                            else
                            {
                                Calendar calBefore = Calendar.getInstance();

                                sResponse = _RMIClient.GetCSQData();

                                Calendar calAfter = Calendar.getInstance();

                                if(bUseWrapperManager)
                                {
                                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".handle(): realtime data retrieved in " + (calAfter.getTime().getTime() - calBefore.getTime().getTime()) + " ms");
                                }
                                else
                                {
                                    System.out.println("HttpRequestHandler.handle(): realtime data retrieved in " + (calAfter.getTime().getTime() - calBefore.getTime().getTime()) + " ms");
                                }
                            }
                        }
                        else
                        {
                            sResponse = XMLDeclaration;
                            sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<code>-1</code>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<description>Invalid token received.</description>";
                            sResponse = sResponse + System.getProperty("line.separator") + "</response>";
                        }
                    }
                    else if(sOperation.equals("getagentdata"))
                    {
                        if(sToken.equals(sAcceptedToken))
                        {
                            Calendar calBefore = Calendar.getInstance();

                            sResponse = _RMIClient.GetAgentRealtimeDataXML();

                            Calendar calAfter = Calendar.getInstance();

                            if(bUseWrapperManager)
                            {
                                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".handle(): realtime data retrieved in " + (calAfter.getTime().getTime() - calBefore.getTime().getTime()) + " ms");
                            }
                            else
                            {
                                System.out.println("HttpRequestHandler.handle(): realtime data retrieved in " + (calAfter.getTime().getTime() - calBefore.getTime().getTime()) + " ms");
                            }
                        }
                        else
                        {
                            sResponse = XMLDeclaration;
                            sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<code>-1</code>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<description>Invalid token received.</description>";
                            sResponse = sResponse + System.getProperty("line.separator") + "</response>";
                        }
                    }
                    else if(sOperation.equals("systemstatus"))
                    {
                        sResponse = XMLDeclaration;
                        sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<code>0</code>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<uccx1ip>" + _RMIClient._Cluster.getUCCXNode1IPAddress() + "</uccx1ip>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<uccx2ip>" + _RMIClient._Cluster.getUCCXNode2IPAddress() + "</uccx2ip>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<uccx1rtrport>" + _RMIClient._Cluster.getUCCXNode1RTRPort() + "</uccx1rtrport>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<uccx2rtrport>" + _RMIClient._Cluster.getUCCXNode2RTRPort() + "</uccx2rtrport>";
                                
                        if(_RMIClient._Cluster.getCurrentMasterIPAddress().equals(""))
                        {
                            sResponse = sResponse + System.getProperty("line.separator") + "<masteruccxnodedetected>false</masteruccxnodedetected>";
                        }
                        else
                        {
                            sResponse = sResponse + System.getProperty("line.separator") + "<masteruccxnodedetected>true</masteruccxnodedetected>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<masteruccxnodeip>" + _RMIClient._Cluster.getCurrentMasterIPAddress() + "</masteruccxnodeip>";
                            
                            TestResult result = _RMIClient.TestGetContactsRealtimeData();
                            
                            sResponse = sResponse + System.getProperty("line.separator") + "<contactsrealtimedata>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<success>" + result.MethodExecutedWithoutErros + "</success>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<errordescription>" + result.ErrorDescription + "</errordescription>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<numberofrecords>" + result.NumberOfRecords + "</numberofrecords>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<operationduration>" + result.OperationDuration + "</operationduration>";
                            sResponse = sResponse + System.getProperty("line.separator") + "</contactsrealtimedata>";
                            
                            result = null;
                            
                            result = _RMIClient.TestGetCSQRealtimeData();
                            
                            sResponse = sResponse + System.getProperty("line.separator") + "<csqrealtimedata>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<success>" + result.MethodExecutedWithoutErros + "</success>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<errordescription>" + result.ErrorDescription + "</errordescription>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<numberofrecords>" + result.NumberOfRecords + "</numberofrecords>";
                            sResponse = sResponse + System.getProperty("line.separator") + "<operationduration>" + result.OperationDuration + "</operationduration>";
                            sResponse = sResponse + System.getProperty("line.separator") + "</csqrealtimedata>";
                            
                            result = null;
                        }
                        
                        sResponse = sResponse + System.getProperty("line.separator") + "<description>TO BE IMPLEMENTED</description>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<jre>" + System.getProperty("java.version") + "</jre>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<processstartedon>" + _dServiceStart.getTime() + "</processstartedon>";
                        sResponse = sResponse + System.getProperty("line.separator") + "</response>";
                    }
                    else
                    {
                        if(bUseWrapperManager)
                        {
                            WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".handle(): Unknown operation requested.");
                        }
                        else
                        {
                            System.out.println("HttpRequestHandler.handle(): Unknown operation requested.");
                        }
                        
                        sResponse = XMLDeclaration;
                        sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<code>-1</code>";
                        sResponse = sResponse + System.getProperty("line.separator") + "<description>Unknown operation requested.</description>";
                        sResponse = sResponse + System.getProperty("line.separator") + "</response>";
                    }
                }
                else
                {
                    if(bUseWrapperManager)
                    {
                        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".handle(): No parameters found in request.");
                    }
                    else
                    {
                        System.out.println("HttpRequestHandler.handle(): No parameters found in request.");
                    }
                    
                    sResponse = XMLDeclaration;
                    sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                    sResponse = sResponse + System.getProperty("line.separator") + "<code>-1</code>";
                    sResponse = sResponse + System.getProperty("line.separator") + "<description>No parameters found in request.</description>";
                    sResponse = sResponse + System.getProperty("line.separator") + "</response>";
                }
            }
            else
            {
                if(bUseWrapperManager)
                {
                    WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".handle(): Realtime data client is null.");
                }
                else
                {
                    System.out.println("HttpRequestHandler.handle(): Realtime data client is null.");
                }
                
                sResponse = XMLDeclaration;
                sResponse = sResponse + System.getProperty("line.separator") + "<response>";
                sResponse = sResponse + System.getProperty("line.separator") + "<code>-1</code>";
                sResponse = sResponse + System.getProperty("line.separator") + "<description>Realtime data client is null.</description>";
                sResponse = sResponse + System.getProperty("line.separator") + "</response>";
            }

            t.sendResponseHeaders(200, sResponse.length());
            OutputStream os = t.getResponseBody();
            os.write(sResponse.getBytes());
            os.close();
            os = null;
        }
        catch(Exception ex)
        {
            if(bUseWrapperManager)
            {
                WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_ERROR,this.getClass().getName() + ".handle(): Exception: " + ex.getMessage());
            }
            else
            {
                System.out.println("HttpRequestHandler.handle(): Exception: " + ex.getMessage());
            }
        }
    }
        
    private  java.util.Map<String, String> queryToMap(String query)
    {
        try
        {
            java.util.Map<String, String> result = new java.util.HashMap<String, String>();

            for (String param : query.split("&")) 
            {
                String pair[] = param.split("=");
                if (pair.length>1) 
                {
                    result.put(pair[0], pair[1]);
                }
                else
                {
                    result.put(pair[0], "");
                }
            }

            return result;
        }
        catch(Exception ex)
        {
            return null;
        }
    }
}
