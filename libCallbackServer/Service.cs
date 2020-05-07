using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public partial class Service : ServiceBase
    {
        ApplicationSettings _Settings = null;
        CallbackSettings _CallbackSettings = null;

        Notifier _Notifier = null;
        WebServer _WebServer = null;

        HTTPRequestQueue _HTTPQueue = null;
        com.vanticore.utilities.debug.DebugManager dm = null;
        HTTPRequestQueueManager _HTTPManager = null;
        HTTPRequestHandler _HTTPHandler = null;
        PerformanceCounters _PerformanceCounters = null;

        CallbackRecordManager _recordManager = null;
        SettingsManager _settingsManager = null;

        ContactRealtimeDataClient _RealtimeDataClient = null;
        ContactServiceQueueRealtimeDataClient _CSQRealtimeDataClient = null;
        CallbackReentryManager _callbackReentryManager = null;
        DataCollectionServiceMonitor _dataCollectionServiceMonitor = null;

        DateTime dNetworkUnavailable = DateTime.MinValue;

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {            
            Trace.TraceInformation("Enter.");
            
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            dm = new com.vanticore.utilities.debug.DebugManager(args);

            dm.Initialize(Constants.SERVICEDISPLAYNAME, Constants.LogFilesPath, com.vanticore.utilities.debug.DebugManager.FileSizes.OneMegaByte);

            Trace.TraceInformation("Debug was turned on via the command line.");

           _settingsManager = new SettingsManager();

            _CallbackSettings = new CallbackSettings();

            if (_CallbackSettings.ReadFromDisk())
            {
                Trace.TraceInformation("_CallbackSettings.ReadFromDisk() returned true.");
                _settingsManager.CallbackSettings = _CallbackSettings;
                _settingsManager.SettingsLastChangedAt = DateTime.Now;
            }
            else
            {
                Trace.TraceWarning("_CallbackSettings.ReadFromDisk() returned false.");
                _settingsManager.CallbackSettings = _CallbackSettings;
                _settingsManager.SettingsLastChangedAt = DateTime.Now;
            }

            _settingsManager.StartUCCXHNodeMonitor();

            _recordManager = new CallbackRecordManager();

            if (_recordManager.ReadFromDisk())
            {
                Trace.TraceInformation("_recordManager.ReadFromDisk() returned true.");
            }
            else
            {
                Trace.TraceInformation("_recordManager.ReadFromDisk() returned false.");
            }

            _recordManager.RecordStatusUpdated += new EventHandler<CallbackRecordStatusUpdateEventArgs>(_recordManager_RecordStatusUpdated);
            _recordManager.RecordsPurged += new EventHandler<CallbackRecordPurgeEventArgs>(_recordManager_RecordsPurged);
            _recordManager.EndOfDayStatusUpdate += new EventHandler<CallbackRecordStatusEndOfDayUpdateEventArgs>(_recordManager_EndOfDayStatusUpdate);
            _WebServer = new WebServer();

            _WebServer.Started += new EventHandler(_WebServer_Started);
            _WebServer.Stopped += new EventHandler(_WebServer_Stopped);
            _WebServer.Aborted += new EventHandler(_WebServer_Aborted);
            _WebServer.Error += new EventHandler<WebServerEventArgs>(_WebServer_Error);

            _HTTPQueue = new HTTPRequestQueue();

            _WebServer.Callback = new AsyncCallback(_HTTPQueue.WebServerCallback);

            _RealtimeDataClient = new ContactRealtimeDataClient();

            if (_RealtimeDataClient.Start())
            {
                Trace.TraceInformation("_RealtimeDataClient.Start() returned true.");
            }
            else
            {
                Trace.TraceWarning("_RealtimeDataClient.Start() returned false.");
            }

            _CSQRealtimeDataClient = new ContactServiceQueueRealtimeDataClient();

            if (_CSQRealtimeDataClient.Start())
            {
                Trace.TraceInformation("_CSQRealtimeDataClient.Start() returned true.");
            }
            else
            {
                Trace.TraceWarning("_CSQRealtimeDataClient.Start() returned false.");
            }

            try
            {
                _HTTPHandler = new HTTPRequestHandler(_recordManager, _settingsManager, _RealtimeDataClient, _CSQRealtimeDataClient);

                _HTTPHandler.HTTPRequestHandlerEvent += new EventHandler<HTTPRequestHandlerEventArgs>(_HTTPHandler_HTTPRequestHandlerEvent);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception instantiating the HTTP Handler:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }

            _PerformanceCounters = new PerformanceCounters();

            _HTTPManager = new HTTPRequestQueueManager(_HTTPQueue, _HTTPHandler, _PerformanceCounters);

            _Settings = new ApplicationSettings(Constants.ApplicationSettingsFilePath);

            if (!System.IO.File.Exists(Constants.ApplicationSettingsFilePath + "\\" + Constants.APPLICATIONSETTINGSFILENAME))
            {
                Trace.TraceWarning(Constants.ApplicationSettingsFilePath + "\\" + Constants.APPLICATIONSETTINGSFILENAME + Environment.NewLine + "Create default settings file.");

                if (_Settings.CreateDefaultSettings() == ApplicationTypes.ApplicationSettingsReturn.SUCCESS)
                {
                    Trace.TraceInformation("Default settings file was created.");
                }
                else
                {
                    Trace.TraceWarning("Default settings file was not created.");
                }
            }

            try
            {
                switch (_Settings.Load())
                {
                    case ApplicationTypes.ApplicationSettingsReturn.SUCCESS:

                        Trace.TraceInformation("_Settings.Load() returned SUCCESS.");

                        try
                        {
                            string _debugCommandLine = string.Empty;

                            if (_Settings.Debug)
                            {
                                _debugCommandLine = "/debug:true";

                                if (_Settings.DebugLevel != string.Empty)
                                {
                                    _debugCommandLine = _debugCommandLine + " /debuglevel:" + _Settings.DebugLevel;
                                }

                                if (_Settings.DebugRetainUnit != string.Empty)
                                {
                                    _debugCommandLine = _debugCommandLine + " /retainunit:" + _Settings.DebugRetainUnit;
                                }

                                if (_Settings.DebugRetainValue != string.Empty)
                                {
                                    _debugCommandLine = _debugCommandLine + " /retainvalue:" + _Settings.DebugRetainValue;
                                }

                                if (dm != null)
                                {
                                    if (!dm.HaveListenersBeenAdded())
                                    {
                                        dm = new com.vanticore.utilities.debug.DebugManager(_debugCommandLine);
                                        dm.Initialize(Constants.SERVICEDISPLAYNAME, Constants.LogFilesPath, com.vanticore.utilities.debug.DebugManager.FileSizes.OneMegaByte);

                                        Trace.TraceInformation("Debug was turned on via application settings file.");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError("Exception reading debug settings from file:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                        }

                        _Settings.ParseArgs(args);

                        _Notifier = new Notifier(_Settings);

                        ResetHTTPRequestQueueManager();

                        ResetWebServer();

                        _settingsManager.ApplicationSettings = _Settings;

                        _RealtimeDataClient.Settings = _settingsManager.ApplicationSettings;
                        _CSQRealtimeDataClient.Settings = _settingsManager.ApplicationSettings;

                        _callbackReentryManager = new CallbackReentryManager(_recordManager, _RealtimeDataClient, _settingsManager);

                        _callbackReentryManager.Started += new EventHandler(_callbackReentryManager_Started);
                        _callbackReentryManager.Stopped += new EventHandler(_callbackReentryManager_Stopped);

                        if (_callbackReentryManager.Start())
                        {
                            Trace.TraceInformation("_callbackReentryManager.Start() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_callbackReentryManager.Start() returned false.");
                        }

                        _dataCollectionServiceMonitor = new DataCollectionServiceMonitor(_settingsManager);

                        _dataCollectionServiceMonitor.Started += new EventHandler(_dataCollectionServiceMonitor_Started);
                        _dataCollectionServiceMonitor.Stopped += new EventHandler(_dataCollectionServiceMonitor_Stopped);
                        _dataCollectionServiceMonitor.MaximumNumberOfCollisionsReached +=new EventHandler<MaximumNumberOfCollisionsReachedEventsArgs>(_dataCollectionServiceMonitor_MaximumNumberOfCollisionsReached);
                        _dataCollectionServiceMonitor.MaximumNumberOfErrorsReached += new EventHandler<MaximumNumberOfErrorsReachedEventsArgs>(_dataCollectionServiceMonitor_MaximumNumberOfErrorsReached);
                        ResetCollectionServiceMonitor();

                        break;

                    case ApplicationTypes.ApplicationSettingsReturn.ERROR:

                        Trace.TraceError("_Settings.Load() returned ERROR; application will proceed with key features disabled.");

                        _Notifier = new Notifier();

                        if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "Error processing application settings file.", EventLogEntryType.Warning))
                        {
                            Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                        }

                        break;

                    case ApplicationTypes.ApplicationSettingsReturn.FILE_NOT_FOUND:

                        Trace.TraceError("_Settings.Load() could not find " + Constants.APPLICATIONSETTINGSFILENAME + " at " + Constants.ApplicationSettingsFilePath + Environment.NewLine + "Application will proceed with key features disabled.");

                        _Notifier = new Notifier();

                        if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "ApplicationSettings.xml file was not found.", EventLogEntryType.Warning))
                        {
                            Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                        }

                        break;

                    case ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE:

                        Trace.TraceError("_Settings.Load() returned INVALID_VALUE; application will proceed with key features disabled.");

                        _Notifier = new Notifier();

                        if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "Got INVALID_VALUE when trying to process the ApplicationSettings.xml file.", EventLogEntryType.Warning))
                        {
                            Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                        }

                        break;

                    case ApplicationTypes.ApplicationSettingsReturn.NONE:

                        Trace.TraceError("_Settings.Load() returned NONE; this is unexpected. Application will proceed with key features disabled.");

                        _Notifier = new Notifier();

                        break;
                }      
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                _Settings = null;

                TerminateService(true, 0);
            }

            ApplicationTypes.SettingsCallback _SettingsChangedCallback = new ApplicationTypes.SettingsCallback(__SettingsChangedCallback);

            try
            {
                System.Runtime.Remoting.Channels.Tcp.TcpChannel _IPC = new System.Runtime.Remoting.Channels.Tcp.TcpChannel(Constants.IPC_PORT);

                System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(_IPC, false);

                System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(typeof(InterprocessCommunicationServer), Constants.IPC_URI, System.Runtime.Remoting.WellKnownObjectMode.Singleton);

                InterprocessCommunicationServer.SetApplicationSettingsReference(_Settings);
                InterprocessCommunicationServer.SetPerformanceCountersReference(_PerformanceCounters);
                InterprocessCommunicationServer.SetDebugManagerReference(dm);
                InterprocessCommunicationServer.SetSettingsChangedCallBack(_SettingsChangedCallback);

                Trace.TraceInformation("IPC channel has been successfully registered.");

                if (_Notifier != null)
                {
                    if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Service started", "", ApplicationTypes.NotificationType.SUCCESS))
                    {
                        Trace.TraceInformation("_Notifier.Email() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("_Notifier.Email() returned false.");
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "Exception setting RPC channel:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace, EventLogEntryType.Error))
                {
                    Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                }
                else
                {
                    Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                }

                TerminateService(true, 0);
            }

            Trace.TraceInformation("Product Version: " + System.Windows.Forms.Application.ProductVersion);

            Trace.TraceInformation("Main thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += new System.Net.NetworkInformation.NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
        
        }
        
        protected override void OnStop()
        {
            Trace.TraceInformation("Enter.");

            TerminateService(false, 0);
        }

        void __SettingsChangedCallback(ApplicationTypes.iApplicationSettings Settings)
        {
            Trace.TraceInformation("Enter.");

            _Settings = (ApplicationSettings)Settings;

            if (_Settings.Save() == ApplicationTypes.ApplicationSettingsReturn.SUCCESS)
            {
                Trace.TraceInformation("Settings file was updated successfully.");

                InterprocessCommunicationServer.SetApplicationSettingsReference(_Settings);

                _Notifier = null;
                _Notifier = new Notifier(_Settings);

                Trace.TraceInformation("Before ResetDebugManager()");

                ResetDebugManager();

                Trace.TraceInformation("Before ResetHTTPRequestQueueManager()");

                ResetHTTPRequestQueueManager();

                Trace.TraceInformation("Before ResetWebServer()");

                ResetWebServer();

                Trace.TraceInformation("After ResetWebServer()");

                _settingsManager.ApplicationSettings = _Settings;

                Trace.TraceInformation("Before ResetCollectionServiceMonitor()");

                ResetCollectionServiceMonitor();

                Trace.TraceInformation("After ResetCollectionServiceMonitor()");

                ResetRealtimeClients();

                if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Settings Updates", "", ApplicationTypes.NotificationType.SUCCESS))
                {
                    Trace.TraceInformation("notifier.Email() returned true.");
                }
                else
                {
                    Trace.TraceWarning("notifier.Email() returned true.");
                }
            }
            else
            {
                Trace.TraceWarning("Settings file was not updated.");
            }
        }

        void ResetHTTPRequestQueueManager()
        {
            Trace.TraceInformation("Enter.");

            if (_HTTPManager != null)
            {
                _HTTPManager.Stop();
            }

            _HTTPManager.Start(_Settings.NumerOfAPIThreads, _Settings.APIHandlerThreadsSleep);

            Trace.TraceInformation("QueueManager was started.");
        }

        void ResetWebServer()
        {
            Trace.TraceInformation("Enter.");

            if (_WebServer == null)
            {
                Trace.TraceWarning("_WebServer is null");
                return;
            }

            try
            {
                if (!_WebServer.IsRunning)
                {
                    Trace.TraceInformation("Web server is not running.");

                    _WebServer.WebServerIP = _Settings.WebServerIP;
                    _WebServer.WebServerPort = _Settings.WebServerPort;
                    _WebServer.WebServerURIPrefix = _Settings.WebServerPrefix;

                    if (_WebServer.Start())
                    {
                        Trace.TraceInformation("Web server was restarted.");
                    }
                    else
                    {
                        Trace.TraceWarning("Web server could not be started; a restart of the windows service may ne needed.");
                    }
                }
                else
                {
                    if (_WebServer.Stop())
                    {
                        Trace.TraceInformation("Web server was stopped.");

                        _WebServer.WebServerIP = _Settings.WebServerIP;
                        _WebServer.WebServerPort = _Settings.WebServerPort;
                        _WebServer.WebServerURIPrefix = _Settings.WebServerPrefix;

                        if (_WebServer.Start())
                        {
                            Trace.TraceInformation("Web server was restarted.");
                        }
                        else
                        {
                            Trace.TraceWarning("Web server could not be started; a restart of the windows service may ne needed.");
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("Web server could not be stopped; a restart of the windows service may ne needed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "ResetWebServer(): Exception:" + "Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace, EventLogEntryType.Error))
                {
                    Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                }
                else
                {
                    Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                }
            }            
        }

        void ResetDebugManager()
        {
            Trace.TraceInformation("Enter.");

            string _debugCommandLine = string.Empty;

            try
            {
                if (dm != null)
                {
                    if (dm.HaveListenersBeenAdded())
                    {
                        dm.RemoveListeners();
                    }

                    dm = null;
                }

                if (_Settings.Debug)
                {
                    _debugCommandLine = "/debug:true";

                    if (_Settings.DebugLevel != string.Empty)
                    {
                        _debugCommandLine = _debugCommandLine + " /debuglevel:" + _Settings.DebugLevel;
                    }

                    if (_Settings.DebugRetainUnit != string.Empty)
                    {
                        _debugCommandLine = _debugCommandLine + " /retainunit:" + _Settings.DebugRetainUnit;
                    }

                    if (_Settings.DebugRetainValue != string.Empty)
                    {
                        _debugCommandLine = _debugCommandLine + " /retainvalue:" + _Settings.DebugRetainValue;
                    }

                    dm = new com.vanticore.utilities.debug.DebugManager(_debugCommandLine);
                    dm.Initialize(Constants.SERVICEDISPLAYNAME, Constants.LogFilesPath, com.vanticore.utilities.debug.DebugManager.FileSizes.OneMegaByte);

                    Trace.TraceInformation("Debug was turned on via the managment console.");
                }
                else
                {
                    Trace.TraceInformation("Debug was turned off via the managment console.");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception reading debug settings from file:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "ResetDebugManager(): Exception reading debug settings from file:" + "Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace, EventLogEntryType.Error))
                {
                    Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                }
                else
                {
                    Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                }
            }
        }

        void ResetCollectionServiceMonitor()
        {
            Trace.TraceInformation("Enter.");

            if (_dataCollectionServiceMonitor == null)
            {
                Trace.TraceWarning("_dataCollectionServiceMonitor is null");
                return;
            }

            _dataCollectionServiceMonitor.Stop();

            _dataCollectionServiceMonitor.SettingsManager = _settingsManager;

            if (_dataCollectionServiceMonitor.Start())
            {
                Trace.TraceInformation("_dataCollectionServiceMonitor.Start() returned true.");
            }
            else
            {
                Trace.TraceWarning("_dataCollectionServiceMonitor.Start() returned false.");
            }
        }

        void ResetRealtimeClients()
        {
            Trace.TraceInformation("Enter.");

            _RealtimeDataClient.Settings = _Settings;
            _CSQRealtimeDataClient.Settings = _Settings;
        }

        #region "_WebServer events"

        void _WebServer_Aborted(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
        }

        void _WebServer_Error(object sender, WebServerEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (_Notifier != null)
            {
                if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Web Server Error", " Built in Web Server raised an error. Please restart this service." + Environment.NewLine + Environment.NewLine + e.Message, ApplicationTypes.NotificationType.FAILURE))
                {
                    Trace.TraceInformation("notifier.Email() returned true.");
                }
                else
                {
                    Trace.TraceWarning("notifier.Email() returned true.");
                }

                if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "Built in Web Server raised an error. Please restart this service." + Environment.NewLine + Environment.NewLine + e.Message, EventLogEntryType.Error))
                {
                    Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                }
                else
                {
                    Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                }
            }
        }

        void _WebServer_Stopped(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
        }

        void _WebServer_Started(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
        }

        void _WebServer_Callback(IAsyncResult result)
        {
            Trace.TraceInformation("Enter.");
        }

        #endregion

        #region "_callbackReentryManager events"

        void _callbackReentryManager_Stopped(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
        }

        void _callbackReentryManager_Started(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
        }

        #endregion

        #region "_dataCollectionServiceMonitor events"

        void _dataCollectionServiceMonitor_Started(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
        }

        void _dataCollectionServiceMonitor_Stopped(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
        }

        void _dataCollectionServiceMonitor_MaximumNumberOfCollisionsReached(object sender, MaximumNumberOfCollisionsReachedEventsArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (_Notifier != null)
            {
                if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Data freeze detected in " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME, e.Description + Environment.NewLine + Environment.NewLine + "Application will attempt to restart the " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " service.", ApplicationTypes.NotificationType.FAILURE))
                {
                    Trace.TraceInformation("_Notifier.Email() returned true.");
                }
                else
                {
                    Trace.TraceWarning("_Notifier.Email() returned false.");
                }
                
            }

            try
            {
                WindowsServiceController ctrl = new WindowsServiceController(Constants.DATACOLLECTIONSERVICEDISPLAYNAME);

                if (ctrl.Status() != ServiceControllerStatus.Running)
                {
                    Trace.TraceWarning(Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " service is in an unexpected state");

                    if (_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " service was in an unexpected state", "Please contact your Systems Administrator.", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }

                    }
                }

                if (ctrl.Stop())
                {
                    Trace.TraceInformation(Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " service was stopped.");
                }
                else
                {
                    if (_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " failed to stop.", "", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }

                    }
                }

                if (ctrl.Start())
                {
                    if (_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " was restarted.", "", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }

                    }
                }
                else
                {
                    if (_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " failed to restart.", "Please contact your Systems Administrator.", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }

                    }
                }

                ctrl = null;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);

                if (_Notifier != null)
                {
                    if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Issue attempting to restart " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME, "Please contact your Systems Administrator.", ApplicationTypes.NotificationType.FAILURE))
                    {
                        Trace.TraceInformation("_Notifier.Email() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("_Notifier.Email() returned false.");
                    }

                }
            }
        }

        void _dataCollectionServiceMonitor_MaximumNumberOfErrorsReached(object sender, MaximumNumberOfErrorsReachedEventsArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (_Notifier != null)
            {
                if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Maximum number of error detected in " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME, e.Description + Environment.NewLine + Environment.NewLine + "Application will attempt to restart the " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " service.", ApplicationTypes.NotificationType.FAILURE))
                {
                    Trace.TraceInformation("_Notifier.Email() returned true.");
                }
                else
                {
                    Trace.TraceWarning("_Notifier.Email() returned false.");
                }
            }

            try
            {
                WindowsServiceController ctrl = new WindowsServiceController(Constants.DATACOLLECTIONSERVICEDISPLAYNAME);

                if (ctrl.Status() != ServiceControllerStatus.Running)
                {
                    Trace.TraceWarning(Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " service is in an unexpected state");

                    if (_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " service was in an unexpected state", "Please contact your Systems Administrator.", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }

                    }
                }

                if (ctrl.Stop())
                {
                    Trace.TraceInformation(Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " service was stopped.");
                }
                else
                {
                    if (_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " failed to stop.", "", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }

                    }
                }

                if (ctrl.Start())
                {
                    if (_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " was restarted.", "", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }

                    }
                }
                else
                {
                    if (_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME + " failed to restart.", "Please contact your Systems Administrator.", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }

                    }
                }

                ctrl = null;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);

                if (_Notifier != null)
                {
                    if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Issue attempting to restart " + Constants.DATACOLLECTIONSERVICEDISPLAYNAME, "Please contact your Systems Administrator.", ApplicationTypes.NotificationType.FAILURE))
                    {
                        Trace.TraceInformation("_Notifier.Email() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("_Notifier.Email() returned false.");
                    }
                }
            }
        }

        #endregion

        #region "_recordManager events"

        void _recordManager_RecordStatusUpdated(object sender, CallbackRecordStatusUpdateEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (e.Record == null)
                {
                    Trace.TraceWarning("e.Record is null.");
                    return;
                }

                if (_Notifier != null)
                {
                    String sBody = String.Empty;

                    sBody = "Contact information: " + Environment.NewLine + Environment.NewLine;

                    sBody = sBody + "ID:" + e.Record.ID;
                    sBody = sBody + Environment.NewLine + "ImplID:" + e.Record.ContactImplementationID;
                    sBody = sBody + Environment.NewLine + "SessionID:" + e.Record.SessionID;
                    sBody = sBody + Environment.NewLine + "RequestDate:" + e.Record.RequestDate.ToString();
                    sBody = sBody + Environment.NewLine + "DNIS:" + e.Record.DNIS;
                    sBody = sBody + Environment.NewLine + "CSQ:" + e.Record.TargetCSQ;
                    sBody = sBody + Environment.NewLine + "Language:" + e.Record.Language;
                    sBody = sBody + Environment.NewLine + "Prompt:" + e.Record.Prompt;
                    sBody = sBody + Environment.NewLine + "CustomVar1:" + e.Record.CustomVar1;
                    sBody = sBody + Environment.NewLine + "CustomVar2:" + e.Record.CustomVar2;
                    sBody = sBody + Environment.NewLine + "CustomVar3:" + e.Record.CustomVar3;
                    sBody = sBody + Environment.NewLine + "CustomVar4:" + e.Record.CustomVar4;
                    sBody = sBody + Environment.NewLine + "CustomVar5:" + e.Record.CustomVar5;
                    sBody = sBody + Environment.NewLine + "Status:" + e.Record.Status.ToString();
                    sBody = sBody + Environment.NewLine + "ReentryDate:" + e.Record.ReentryDate.ToString();
                    sBody = sBody + Environment.NewLine + "QueuedAt:" + e.Record.QueuedAt.ToString();
                    sBody = sBody + Environment.NewLine + "AgentAcknowledgedAt:" + e.Record.AgentAcknowledgedAt.ToString();
                    sBody = sBody + Environment.NewLine + "TargetDialedAt:" + e.Record.TargetDialedAt.ToString();
                    sBody = sBody + Environment.NewLine + "AgentID:" + e.Record.AgentID;

                    if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Callback record is " + e.Record.Status.ToString(), sBody, ApplicationTypes.NotificationType.FAILURE))
                    {
                        Trace.TraceInformation("notifier.Email() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("notifier.Email() returned false.");
                    }
                }
                else
                {
                    Trace.TraceWarning("_notifier is null.");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
        }

        void _recordManager_RecordsPurged(object sender, CallbackRecordPurgeEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (e.PurgedRecords == null)
                {
                    Trace.TraceWarning("e.PurgedRecords is null.");
                    return;
                }


                if (e.PurgedRecords.Count == 0)
                {
                    Trace.TraceWarning("e.PurgedRecords.Count is empty.");
                    return;
                }

                if (_Notifier != null)
                {
                    String sBody = String.Empty;

                    foreach (CallbackRecord record in e.PurgedRecords)
                    {
                        sBody = sBody + "ID:" + record.ID;
                        sBody = sBody + Environment.NewLine + "ImplID:" + record.ContactImplementationID;
                        sBody = sBody + Environment.NewLine + "SessionID:" + record.SessionID;
                        sBody = sBody + Environment.NewLine + "RequestDate:" + record.RequestDate.ToString();
                        sBody = sBody + Environment.NewLine + "DNIS:" + record.DNIS;
                        sBody = sBody + Environment.NewLine + "CSQ:" + record.TargetCSQ;
                        sBody = sBody + Environment.NewLine + "Language:" + record.Language;
                        sBody = sBody + Environment.NewLine + "Prompt:" + record.Prompt;
                        sBody = sBody + Environment.NewLine + "CustomVar1:" + record.CustomVar1;
                        sBody = sBody + Environment.NewLine + "CustomVar2:" + record.CustomVar2;
                        sBody = sBody + Environment.NewLine + "CustomVar3:" + record.CustomVar3;
                        sBody = sBody + Environment.NewLine + "CustomVar4:" + record.CustomVar4;
                        sBody = sBody + Environment.NewLine + "CustomVar5:" + record.CustomVar5;
                        sBody = sBody + Environment.NewLine + "Status:" + record.Status.ToString();
                        sBody = sBody + Environment.NewLine + "ReentryDate:" + record.ReentryDate.ToString();
                        sBody = sBody + Environment.NewLine + "QueuedAt:" + record.QueuedAt.ToString();
                        sBody = sBody + Environment.NewLine + "AgentAcknowledgedAt:" + record.AgentAcknowledgedAt.ToString();
                        sBody = sBody + Environment.NewLine + "TargetDialedAt:" + record.TargetDialedAt.ToString();
                        sBody = sBody + Environment.NewLine + "AgentID:" + record.AgentID;

                        if (record.PurgeDueToAge)
                        {
                            sBody = sBody + Environment.NewLine + "Over maximum number of days: YES";
                        }
                        else
                        {
                            sBody = sBody + Environment.NewLine + "Over maximum number of days: NO";
                        }

                        sBody = sBody + Environment.NewLine + Environment.NewLine;

                    }//foreach (CallbackRecord record in e.PurgedRecords)

                    if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - End of day purge record list", sBody, ApplicationTypes.NotificationType.FAILURE))
                    {
                        Trace.TraceInformation("notifier.Email() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("notifier.Email() returned false.");
                    }
                }
                else
                {
                    Trace.TraceWarning("_notifier is null.");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
        }

        void _recordManager_EndOfDayStatusUpdate(object sender, CallbackRecordStatusEndOfDayUpdateEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (e.Records == null)
                {
                    Trace.TraceWarning("e.Records is null.");
                    return;
                }

                if (e.Records.Count == 0)
                {
                    Trace.TraceWarning("e.Records.Count is empty.");
                    return;
                }

                if (_Notifier != null)
                {
                    String sBody = String.Empty;

                    foreach (CallbackRecord record in e.Records)
                    {
                        sBody = sBody + "ID:" + record.ID;
                        sBody = sBody + Environment.NewLine + "ImplID:" + record.ContactImplementationID;
                        sBody = sBody + Environment.NewLine + "SessionID:" + record.SessionID;
                        sBody = sBody + Environment.NewLine + "RequestDate:" + record.RequestDate.ToString();
                        sBody = sBody + Environment.NewLine + "DNIS:" + record.DNIS;
                        sBody = sBody + Environment.NewLine + "CSQ:" + record.TargetCSQ;
                        sBody = sBody + Environment.NewLine + "Language:" + record.Language;
                        sBody = sBody + Environment.NewLine + "Prompt:" + record.Prompt;
                        sBody = sBody + Environment.NewLine + "CustomVar1:" + record.CustomVar1;
                        sBody = sBody + Environment.NewLine + "CustomVar2:" + record.CustomVar2;
                        sBody = sBody + Environment.NewLine + "CustomVar3:" + record.CustomVar3;
                        sBody = sBody + Environment.NewLine + "CustomVar4:" + record.CustomVar4;
                        sBody = sBody + Environment.NewLine + "CustomVar5:" + record.CustomVar5;
                        sBody = sBody + Environment.NewLine + "Status:" + record.Status.ToString();
                        sBody = sBody + Environment.NewLine + "Age (Days):" + DateTime.Now.Subtract(record.RequestDate).TotalDays.ToString("0.0");
                        sBody = sBody + Environment.NewLine + Environment.NewLine;

                    }//foreach (CallbackRecord record in e.Records)

                    if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - End of day record status update", sBody, ApplicationTypes.NotificationType.FAILURE))
                    {
                        Trace.TraceInformation("notifier.Email() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("notifier.Email() returned false.");
                    }
                }
                else
                {
                    Trace.TraceWarning("_notifier is null.");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
        }

        #endregion

        #region "_HTTPHandler events"

        void _HTTPHandler_HTTPRequestHandlerEvent(object sender, HTTPRequestHandlerEventArgs args)
        {
            Trace.TraceInformation("Enter.");

            if (args != null)
            {
                switch (args.HTTPRequestHandlerEventCode)
                {
                    case HTTPRequestHandler.HTTPRequestHandlerEventCodes.OVERMAXIVRPORTUSAGE:

                        break;

                    case HTTPRequestHandler.HTTPRequestHandlerEventCodes.NONE:

                        break;
                }
            }
        }

        #endregion

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                Exception ex = (Exception)e.ExceptionObject;

                Trace.TraceError("UnhandledException:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                if (_Notifier != null)
                {
                    if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Unhandled Exception", " Unhandled Exception detected in this service. Appliaction will terminate." + Environment.NewLine + Environment.NewLine + "UnhandledException:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace, ApplicationTypes.NotificationType.FAILURE))
                    {
                        Trace.TraceInformation("notifier.Email() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("notifier.Email() returned true.");
                    }

                    if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "Unhandled Exception detected in this service. Appliaction will terminate." + Environment.NewLine + Environment.NewLine + "UnhandledException:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace, EventLogEntryType.Error))
                    {
                        Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                    }
                }
            }
            catch
            {

            }

            TerminateService(true, -1);
        }

        #region "Network Events"
        
        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
        }

        void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (e.IsAvailable)
            {
                Trace.TraceInformation("Network became available.");

                System.Threading.Thread.Sleep(5000);

                try
                {
                    if(_Notifier != null)
                    {
                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Network availability changed", "Network became unavailable at " + dNetworkUnavailable.ToString() + Environment.NewLine + "It is now available, but we recommend you contact your System Administrator.", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("notifier.Email() returned true.");
                        }
                    }
                }
                catch
                {
                }

                dNetworkUnavailable = DateTime.MinValue;
            }
            else
            {
                Trace.TraceWarning("Network became unavailable.");

                dNetworkUnavailable = DateTime.Now;
            }
        }

        #endregion

        void TerminateService(bool ExecuteExit, int ExitCode)
        {
            Trace.TraceInformation("Enter.");

            if (_HTTPManager != null)
            {
                _HTTPManager.Stop();
            }

            _HTTPManager = null;

            if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Service stopped", "", ApplicationTypes.NotificationType.FAILURE))
            {
                Trace.TraceInformation("notifier.Email() returned true.");
            }
            else
            {
                Trace.TraceWarning("notifier.Email() returned false.");
            }

            if (_settingsManager != null)
            {
                if (_settingsManager.CallbackSettings != null)
                {
                    if (_settingsManager.CallbackSettings.WriteToDisk())
                    {
                        Trace.TraceInformation("CallbackSettings was written to disk on Service Stop.");
                    }
                    else
                    {
                        Trace.TraceWarning("CallbackSettings failed to write to disk on Service Stop.");
                    }
                }

                _settingsManager.ApplicationSettings = null;
                _settingsManager.CallbackSettings = null;
                _settingsManager = null;
            }

            if (_Settings != null)
            {
                if (_Settings.Save() == ApplicationTypes.ApplicationSettingsReturn.SUCCESS)
                {
                    Trace.TraceInformation("ApplicationSettings was written to disk on Service Stop.");
                }
                else
                {
                    Trace.TraceWarning("ApplicationSettings failed to write to disk on Service Stop.");
                }

                _Settings = null;
            }

            if (_recordManager != null)
            {
                _recordManager.WriteToDisk();

                _recordManager = null;
            }

            if (_Notifier != null)
            {
                _Notifier = null;
            }

            if (ExecuteExit)
            {
                Environment.Exit(ExitCode);
            }
        }
    }
}
