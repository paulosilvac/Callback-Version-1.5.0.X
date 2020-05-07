using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public partial class frmCallbackServerManager : Form
    {
        private enum Operation { Save, Close };

        InterprocessCommunicationServer _remObj = null;

        ApplicationSettings _ApplicationSettings = null;

        System.ServiceProcess.ServiceControllerStatus DataCollectionServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus.Stopped;
        System.ServiceProcess.ServiceControllerStatus WindowsServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus.Stopped;

        bool _ChangesDetected = false;
        bool _ChangesSaved = false;

        public frmCallbackServerManager()
        {
            InitializeComponent();

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this._FormClosing);
        }

        #region "Form Events"

        private void frmConnectorManager_Load(object sender, EventArgs e)
        {
            pbCompanyLogo.Image = com.workflowconcepts.applications.uccx.Properties.Resources.Workflow_Concepts_logo;
            pbCompanyLogo.SizeMode = PictureBoxSizeMode.Zoom;

            btnGetContactRealtimeData.Enabled = false;
            btnGetCSQsRealtimeData.Enabled = false;

            ucDataCollectionProcessInformation.Enabled = false;

            //Task#1: Disable UI controls
            ucWebServerSettings.Enabled = false;
            ucEmailSettings.Enabled = false;
            ucDebugSettings.Enabled = false;
            ucUCCXInformation.Enabled = false;
            ucCallbackRecordsSettings.Enabled = false;

            this.btnSave.Enabled = false;

            //Task#3: Add Change event handler for the different UI controls
            ucWebServerSettings.Changed += new EventHandler(ucWebServerSettings_Changed);
            ucEmailSettings.Changed += new EventHandler(ucEmailSettings_Changed);
            ucDebugSettings.Changed += new EventHandler(ucDebugSettings_Changed);
            ucUCCXInformation.Changed += new EventHandler(ucUCCXInformation_Changed);
            ucCallbackRecordsSettings.Changed += new EventHandler(ucCallbackRecordsSettings_Changed);

            //Task#4: Set service controller name and statuschanged event handler
            ucWindowsServiceController.StatusChanged += new EventHandler<WindowsServiceControllerEventArgs>(ucWindowsServiceController_StatusChanged);
            ucWindowsServiceController.ServiceName = Constants.SERVICEDISPLAYNAME;
            ucWindowsServiceController.UseProcessInfo = true;

            ucDataCollectionServiceController.StatusChanged += new EventHandler<WindowsServiceControllerEventArgs>(ucDataCollectionServiceController_StatusChanged);
            ucDataCollectionServiceController.ServiceName = Constants.DATACOLLECTIONSERVICEDISPLAYNAME;

            //Task#5: Set MemoryCounters and PerformanceCounters delegates
            ucRealtimeProcessInformation.SetCounterDelegates(GetMemoryCounters, GetPerformanceCounters);

            //Task#6: Start monitoring service
            if (ucWindowsServiceController.StartMonitoring())
            {
                Trace.TraceInformation("ucWindowsServiceController.StartMonitoring() returned true.");
            }
            else
            {
                Trace.TraceWarning("ucWindowsServiceController.StartMonitoring() returned false.");
            }

            if (ucDataCollectionServiceController.StartMonitoring())
            {
                Trace.TraceInformation("ucDataCollectionServiceController.StartMonitoring() returned true.");
            }
            else
            {
                Trace.TraceWarning("ucDataCollectionServiceController.StartMonitoring() returned false.");
            }

            this.Text = "Callback Server Manager - Version " + Application.ProductVersion;

            _ChangesDetected = false;
            _ChangesSaved = false;
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            AssertChangesDetected(e, Operation.Close, true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            Cursor.Current = Cursors.WaitCursor;

            AssertChangesDetected(e, Operation.Save, false);

            Cursor.Current = Cursors.Default;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            AssertChangesDetected(e, Operation.Close, true);
        }

        #endregion

        #region "Private Methods"

        private void AssertChangesDetected(EventArgs e, Operation Op, bool TerminateApplication)
        {
            Trace.TraceInformation("Enter.");

            if (!_ChangesDetected)
            {
                Trace.TraceInformation("No changes were detected.");

                if (Op == Operation.Close)
                {
                    Trace.TraceInformation("Operation = " + Op.ToString() + "; application will terminate.");

                    //_CloseForm();
                    if (TerminateApplication)
                    {
                        this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this._FormClosing);
                        this.Close();
                    }
                }
                else if (Op == Operation.Save)
                {
                    Trace.TraceInformation("Operation = " + Op.ToString() + "; application will remain open.");
                }
                else
                {
                    Trace.TraceWarning("Operation = " + Op.ToString() + "; unknown operation.");
                }

            }
            else
            {
                Trace.TraceInformation("Changes were detected.");

                if (Op == Operation.Close)
                {
                    Trace.TraceInformation("Operation = " + Op.ToString());

                    switch (MessageBox.Show("Do you want to save your changes before exiting the this form?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        case System.Windows.Forms.DialogResult.Yes:

                            if (e is FormClosingEventArgs)
                            {
                                ((FormClosingEventArgs)e).Cancel = true;
                            }

                            switch (SaveChanges())
                            {
                                case ApplicationTypes.ApplicationSettingsReturn.SUCCESS:

                                    Trace.TraceInformation("SaveChanges() returned true.");

                                    _ChangesDetected = false;
                                    _ChangesSaved = true;

                                    //_CloseForm();
                                    if (TerminateApplication)
                                    {
                                        this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this._FormClosing);
                                        this.Close();
                                    }

                                    break;

                                case ApplicationTypes.ApplicationSettingsReturn.ERROR:

                                    MessageBox.Show("Could not save changes." + Environment.NewLine + "Please contact your system's administrator.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                    Trace.TraceWarning("SaveChanges() returned false.");

                                    break;
                            }

                            break;

                        case System.Windows.Forms.DialogResult.No:

                            Trace.TraceWarning("User chose not to save changes.");

                            //_CloseForm();
                            if (TerminateApplication)
                            {
                                this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this._FormClosing);
                                this.Close();
                            }
                            break;
                    }
                }
                else if (Op == Operation.Save)
                {
                    Trace.TraceInformation("Operation = " + Op.ToString());

                    switch (SaveChanges())
                    {
                        case ApplicationTypes.ApplicationSettingsReturn.SUCCESS:

                            Trace.TraceInformation("SaveChanges() returned true");

                            _ChangesSaved = true;
                            _ChangesDetected = false;

                            break;

                        case ApplicationTypes.ApplicationSettingsReturn.ERROR:

                            MessageBox.Show("Could not save changes." + Environment.NewLine + "Please contact your system's administrator.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Trace.TraceWarning("SaveChanges() returned false");

                            break;
                    }
                }
                else
                {
                    Trace.TraceWarning("Operation = " + Op.ToString() + "; unknown operation.");
                }

            }

            Trace.TraceInformation("Exit.");
        }

        private ApplicationTypes.ApplicationSettingsReturn SaveChanges()
        {
            Trace.TraceInformation("Enter.");

            //Web Server settings
            if (ucWebServerSettings.txtWebServerIPAddress.Text.Length == 0)
            {
                MessageBox.Show("Please specify a valid value for the Web Server IP field.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucWebServerSettings.txtWebServerIPAddress.Focus();
                ucWebServerSettings.txtWebServerIPAddress.SelectAll();
                return ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE;
            }

            if (ucWebServerSettings.txtWebServerPort.Text.Length == 0)
            {
                MessageBox.Show("Please specify a valid value for the Web Server Port field.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucWebServerSettings.txtWebServerPort.Focus();
                ucWebServerSettings.txtWebServerPort.SelectAll();
                return ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE;
            }

            if (ucWebServerSettings.txtWebServerPrefix.Text.Length == 0)
            {
                MessageBox.Show("Please specify a valid value for the Web Server Prefix field.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucWebServerSettings.txtWebServerPrefix.Focus();
                ucWebServerSettings.txtWebServerPrefix.SelectAll();
                return ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE;
            }

            if (ucWebServerSettings.txtContactDataCollectionPort.Text.Length == 0)
            {
                MessageBox.Show("Please specify a valid value for the Web Server Data Collection Port field.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucWebServerSettings.txtContactDataCollectionPort.Focus();
                ucWebServerSettings.txtContactDataCollectionPort.SelectAll();
                return ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE;
            }

            //Email Ssettings
            if (this.ucEmailSettings.ckbEmailFailureNotifications.Checked || this.ucEmailSettings.ckbEmailSuccessNotifications.Checked)
            {
                if (this.ucEmailSettings.txtEmailFrom.Text.Length == 0)
                {
                    MessageBox.Show("Please specify a valid value for the Email From field.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControl.SelectedIndex = 0;
                    ucEmailSettings.txtEmailFrom.Focus();
                    ucEmailSettings.txtEmailFrom.SelectAll();
                    return ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE;
                }

                if (this.ucEmailSettings.txtEmailTo.Text.Length == 0)
                {
                    MessageBox.Show("Please specify a valid value for the Email To field.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControl.SelectedIndex = 0;
                    ucEmailSettings.txtEmailTo.Focus();
                    ucEmailSettings.txtEmailTo.SelectAll();
                    return ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE;
                }

                if (this.ucEmailSettings.txtSMTPServer.Text.Length == 0)
                {
                    MessageBox.Show("Please specify a valid value for the SMTP Server field.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControl.SelectedIndex = 0;
                    ucEmailSettings.txtSMTPServer.Focus();
                    ucEmailSettings.txtSMTPServer.SelectAll();
                    return ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE;
                }

                if (this.ucEmailSettings.txtSMTPPort.Text.Length == 0)
                {
                    MessageBox.Show("Please specify a valid value for the SMTP Port field.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControl.SelectedIndex = 0;
                    ucEmailSettings.txtSMTPPort.Focus();
                    ucEmailSettings.txtSMTPPort.SelectAll();
                    return ApplicationTypes.ApplicationSettingsReturn.INVALID_VALUE;
                }
            }

            //UCCX Information
            if (this.ucUCCXInformation.txtUCCXNode1IPAddress.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Node 1 IP Address." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtUCCXNode1IPAddress.Focus();
                ucUCCXInformation.txtUCCXNode1IPAddress.SelectAll();
            }

            if (this.ucUCCXInformation.txtUCCXNode2IPAddress.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Node 2 IP Address." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtUCCXNode2IPAddress.Focus();
                ucUCCXInformation.txtUCCXNode2IPAddress.SelectAll();
            }

            if (this.ucUCCXInformation.txtUCCXApplicationPort.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Application Port." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtUCCXApplicationPort.Focus();
                ucUCCXInformation.txtUCCXApplicationPort.SelectAll();
            }

            if (this.ucUCCXInformation.txtUCCXRealtimeDataPort.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Realtime Data Port." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtUCCXRealtimeDataPort.Focus();
                ucUCCXInformation.txtUCCXRealtimeDataPort.SelectAll();
            }

            if (this.ucUCCXInformation.txtAuthorizationPrefix.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Authorization Prefix." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtAuthorizationPrefix.Focus();
                ucUCCXInformation.txtAuthorizationPrefix.SelectAll();
            }

            if (this.ucUCCXInformation.txtCallbackPrefix.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Callback Prefix." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtCallbackPrefix.Focus();
                ucUCCXInformation.txtCallbackPrefix.SelectAll();
            }

            if (this.ucUCCXInformation.txtUCCXAdminUser.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Admin User." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtUCCXAdminUser.Focus();
                ucUCCXInformation.txtUCCXAdminUser.SelectAll();
            }

            if (this.ucUCCXInformation.txtUCCXAdminPassword.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Admin Password." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtUCCXAdminPassword.Focus();
                ucUCCXInformation.txtUCCXAdminPassword.SelectAll();
            }

            if (this.ucUCCXInformation.txtNumberOfIVRPorts.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Number of Licenses." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtNumberOfIVRPorts.Focus();
                ucUCCXInformation.txtNumberOfIVRPorts.SelectAll();
            }

            if (this.ucUCCXInformation.txtMaxIVRPortUsagePercent.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for UCCX Max License Usage %." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucUCCXInformation.txtMaxIVRPortUsagePercent.Focus();
                ucUCCXInformation.txtMaxIVRPortUsagePercent.SelectAll();
            }

            //Callback Records
            if (this.ucCallbackRecordsSettings.txtCallbackRecordsMaximumNumberOfDays.Text.Length == 0)
            {
                MessageBox.Show("Invalid value for Maximum Number of Days." + Environment.NewLine + "Application won't work properly.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0;
                ucCallbackRecordsSettings.txtCallbackRecordsMaximumNumberOfDays.Text = Constants.MAXIMUM_NUMBER_OF_DAYS.ToString();
                ucCallbackRecordsSettings.txtCallbackRecordsMaximumNumberOfDays.Focus();
                ucCallbackRecordsSettings.txtCallbackRecordsMaximumNumberOfDays.SelectAll();
            }

            try
            {
                _ApplicationSettings.WebServerIP = ucWebServerSettings.txtWebServerIPAddress.Text;
                _ApplicationSettings.WebServerPort = ucWebServerSettings.txtWebServerPort.Text;
                _ApplicationSettings.WebServerPrefix = ucWebServerSettings.txtWebServerPrefix.Text;
                _ApplicationSettings.WebServerDataCollectionPort = ucWebServerSettings.txtContactDataCollectionPort.Text;

                _ApplicationSettings.EmailOnFailure = ucEmailSettings.ckbEmailFailureNotifications.Checked;
                _ApplicationSettings.EmailOnSuccess = ucEmailSettings.ckbEmailSuccessNotifications.Checked;
                _ApplicationSettings.EmailFrom = ucEmailSettings.txtEmailFrom.Text;
                _ApplicationSettings.EmailTo = ucEmailSettings.txtEmailTo.Text;
                _ApplicationSettings.SMTPServer = ucEmailSettings.txtSMTPServer.Text;
                _ApplicationSettings.SMTPPort = ucEmailSettings.txtSMTPPort.Text;
                _ApplicationSettings.SMTPUserName = ucEmailSettings.txtSMTPUser.Text;
                _ApplicationSettings.SMTPPassword = ucEmailSettings.txtSMTPPassword.Text;

                _ApplicationSettings.UCCXNode1IPAddress = ucUCCXInformation.txtUCCXNode1IPAddress.Text;
                _ApplicationSettings.UCCXNode2IPAddress = ucUCCXInformation.txtUCCXNode2IPAddress.Text;
                _ApplicationSettings.UCCXApplicationPort = ucUCCXInformation.txtUCCXApplicationPort.Text;
                _ApplicationSettings.UCCXRealtimeDataPort = ucUCCXInformation.txtUCCXRealtimeDataPort.Text;
                _ApplicationSettings.UCCXAuthorizationPrefix = ucUCCXInformation.txtAuthorizationPrefix.Text;
                _ApplicationSettings.UCCXCallbackPrefix = ucUCCXInformation.txtCallbackPrefix.Text;
                _ApplicationSettings.UCCXAdminUser = ucUCCXInformation.txtUCCXAdminUser.Text;
                _ApplicationSettings.UCCXAdminPassword = ucUCCXInformation.txtUCCXAdminPassword.Text;
                _ApplicationSettings.UCCXNumberOfIVRPorts = ucUCCXInformation.txtNumberOfIVRPorts.Text;
                _ApplicationSettings.UCCXMaxIVRPortUsagePercent = ucUCCXInformation.txtMaxIVRPortUsagePercent.Text;

                _ApplicationSettings.MaximumNumberOfDays = int.Parse(ucCallbackRecordsSettings.txtCallbackRecordsMaximumNumberOfDays.Text);

                _ApplicationSettings.Debug = this.ucDebugSettings.ckbDebugEnabled.Checked;
                _ApplicationSettings.DebugLevel = this.ucDebugSettings.cbDebugLevel.Text;
                _ApplicationSettings.DebugRetainUnit = this.ucDebugSettings.cbRetainUnit.Text;
                _ApplicationSettings.DebugRetainValue = this.ucDebugSettings.txtRetainValue.Text;
                
                if (_remObj.SaveApplicationSettings(_ApplicationSettings))
                {
                    Trace.TraceInformation("_remObj_HTTPHandler.SaveApplicationSettings() returned true.");
                }
                else
                {
                    Trace.TraceWarning("_remObj.SaveApplicationSettings() returned false.");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
            

            return ApplicationTypes.ApplicationSettingsReturn.SUCCESS;
        }

        #endregion

        #region "Menu Events Handlers"

        private void mnuMainFileExit_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            AssertChangesDetected(e, Operation.Close, true);
        }

        private void mnuMainAboutCompany_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            System.Diagnostics.Process.Start(Constants.COMPANY_WEB_SITE);
        }

        private void mnuMainCallbackAdministration_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (ucWebServerSettings.txtWebServerIPAddress.Text == String.Empty)
            {
                MessageBox.Show("No valid IP Address provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucWebServerSettings.txtWebServerPort.Text == String.Empty)
            {
                MessageBox.Show("No valid Port provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucWebServerSettings.txtWebServerPrefix.Text == String.Empty)
            {
                MessageBox.Show("No valid Prefix provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            try
            {
                String sURL = "http://" + ucWebServerSettings.txtWebServerIPAddress.Text + ":" + ucWebServerSettings.txtWebServerPort.Text + "/" + ucWebServerSettings.txtWebServerPrefix.Text;


                sURL = sURL + "/CallbackUI.HTML";

                Trace.TraceInformation("sURL = " + sURL);

                System.Diagnostics.Process.Start(sURL);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
        }

        private void mnuMainCallbackDiagnostics_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (ucWebServerSettings.txtWebServerIPAddress.Text == String.Empty)
            {
                MessageBox.Show("No valid IP Address provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucWebServerSettings.txtWebServerPort.Text == String.Empty)
            {
                MessageBox.Show("No valid Port provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucWebServerSettings.txtWebServerPrefix.Text == String.Empty)
            {
                MessageBox.Show("No valid Prefix provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            try
            {
                String sURL = "http://" + ucWebServerSettings.txtWebServerIPAddress.Text + ":" + ucWebServerSettings.txtWebServerPort.Text + "/" + ucWebServerSettings.txtWebServerPrefix.Text;


                sURL = sURL + "/CallbackStatus.html";

                Trace.TraceInformation("sURL = " + sURL);

                System.Diagnostics.Process.Start(sURL);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
        }

        #endregion

        #region "Changed Events"

        void ucWebServerSettings_Changed(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
            _ChangesDetected = true;
        }

        void ucEmailSettings_Changed(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
            _ChangesDetected = true;
        }

        void ucDebugSettings_Changed(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
            _ChangesDetected = true;
        }

        void ucUserManagement_Changed(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
            _ChangesDetected = true;
        }

        void ucUCCXInformation_Changed(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
            _ChangesDetected = true;
        }

        private void ucCallbackRecordsSettings_Changed(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");
            _ChangesDetected = true;
        }

        void ucWindowsServiceController_StatusChanged(object sender, WindowsServiceControllerEventArgs e)
        {
            Trace.TraceInformation("Service Status: " + e.CurrentStatus.ToString());

            switch (e.CurrentStatus)
            {
                case System.ServiceProcess.ServiceControllerStatus.Running:

                    WindowsServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus.Running;

                    try
                    {
                        if (_remObj != null)
                        {
                            _remObj = null;
                        }

                        _remObj = (InterprocessCommunicationServer)Activator.GetObject(typeof(InterprocessCommunicationServer), "tcp://127.0.0.1:" + Constants.IPC_PORT + "/" + Constants.IPC_URI);

                        if (_ApplicationSettings != null)
                        {
                            _ApplicationSettings = null;
                        }

                        _ApplicationSettings = (ApplicationSettings)_remObj.GetApplicationSettings();

                        ucDataCollectionProcessInformation.SetApplicationSettings(_ApplicationSettings);

                        ucWebServerSettings.txtWebServerIPAddress.Text = _ApplicationSettings.WebServerIP;
                        ucWebServerSettings.txtWebServerPort.Text = _ApplicationSettings.WebServerPort;
                        ucWebServerSettings.txtWebServerPrefix.Text = _ApplicationSettings.WebServerPrefix;
                        ucWebServerSettings.txtContactDataCollectionPort.Text = _ApplicationSettings.WebServerDataCollectionPort;

                        ucEmailSettings.txtEmailFrom.Text = _ApplicationSettings.EmailFrom;
                        ucEmailSettings.txtEmailTo.Text = _ApplicationSettings.EmailTo;
                        ucEmailSettings.txtSMTPServer.Text = _ApplicationSettings.SMTPServer;
                        ucEmailSettings.txtSMTPPort.Text = _ApplicationSettings.SMTPPort;
                        ucEmailSettings.txtSMTPUser.Text = _ApplicationSettings.SMTPUserName;
                        ucEmailSettings.txtSMTPPassword.Text = _ApplicationSettings.SMTPPassword;

                        ucEmailSettings.ckbEmailFailureNotifications.Checked = !_ApplicationSettings.EmailOnFailure;
                        ucEmailSettings.ckbEmailFailureNotifications.Checked = _ApplicationSettings.EmailOnFailure;
                        ucEmailSettings.ckbEmailSuccessNotifications.Checked = !_ApplicationSettings.EmailOnSuccess;
                        ucEmailSettings.ckbEmailSuccessNotifications.Checked = _ApplicationSettings.EmailOnSuccess;

                        ucUCCXInformation.txtUCCXNode1IPAddress.Text = _ApplicationSettings.UCCXNode1IPAddress;
                        ucUCCXInformation.txtUCCXNode2IPAddress.Text = _ApplicationSettings.UCCXNode2IPAddress;
                        ucUCCXInformation.txtUCCXApplicationPort.Text = _ApplicationSettings.UCCXApplicationPort;
                        ucUCCXInformation.txtUCCXRealtimeDataPort.Text = _ApplicationSettings.UCCXRealtimeDataPort;
                        ucUCCXInformation.txtAuthorizationPrefix.Text = _ApplicationSettings.UCCXAuthorizationPrefix;
                        ucUCCXInformation.txtCallbackPrefix.Text = _ApplicationSettings.UCCXCallbackPrefix;
                        ucUCCXInformation.txtUCCXAdminUser.Text = _ApplicationSettings.UCCXAdminUser;
                        ucUCCXInformation.txtUCCXAdminPassword.Text = _ApplicationSettings.UCCXAdminPassword;
                        ucUCCXInformation.txtNumberOfIVRPorts.Text = _ApplicationSettings.UCCXNumberOfIVRPorts;
                        ucUCCXInformation.txtMaxIVRPortUsagePercent.Text = _ApplicationSettings.UCCXMaxIVRPortUsagePercent;

                        ucCallbackRecordsSettings.txtCallbackRecordsMaximumNumberOfDays.Text = _ApplicationSettings.MaximumNumberOfDays.ToString();

                        ucDebugSettings.ckbDebugEnabled.Checked = _ApplicationSettings.Debug;
                        ucDebugSettings.ckbDebugEnabled.Checked = _ApplicationSettings.Debug;

                        ucDebugSettings.cbDebugLevel.Text = _ApplicationSettings.DebugLevel;

                        if (ucDebugSettings.cbDebugLevel.Text.Length == 0)
                        {
                            ucDebugSettings.cbDebugLevel.Text = "Verbose";
                        }

                        ucDebugSettings.cbRetainUnit.Text = _ApplicationSettings.DebugRetainUnit;

                        if (ucDebugSettings.cbRetainUnit.Text.Length == 0)
                        {
                            ucDebugSettings.cbRetainUnit.Text = "Files";
                        }

                        ucDebugSettings.txtRetainValue.Text = _ApplicationSettings.DebugRetainValue;

                        if (ucDebugSettings.txtRetainValue.Text.Length == 0)
                        {
                            ucDebugSettings.txtRetainValue.Text = "100";
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning(ex.Message + Environment.NewLine + ex.StackTrace);

                        MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                    }

                    if (DataCollectionServiceControllerStatus == System.ServiceProcess.ServiceControllerStatus.Running)
                    {
                        ucDataCollectionProcessInformation.Enabled = true;
                    }
                    else
                    {
                        ucDataCollectionProcessInformation.Enabled = false;
                    }

                    ucWebServerSettings.Enabled = true;
                    ucEmailSettings.Enabled = true;
                    ucDebugSettings.Enabled = true;
                    ucUCCXInformation.Enabled = true;
                    ucCallbackRecordsSettings.Enabled = true;

                    mnuMainCallbackAdministration.Enabled = true;
                    mnuMainCallbackDiagnostics.Enabled = true;

                    btnSave.Enabled = true;

                    _ChangesDetected = false;
                    _ChangesSaved = false;

                    break;

                case System.ServiceProcess.ServiceControllerStatus.Stopped:

                    WindowsServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus.Stopped;

                    ucWebServerSettings.Enabled = false;
                    ucEmailSettings.Enabled = false;
                    ucDebugSettings.Enabled = false;
                    ucUCCXInformation.Enabled = false;
                    ucCallbackRecordsSettings.Enabled = false;

                    mnuMainCallbackAdministration.Enabled = false;
                    mnuMainCallbackDiagnostics.Enabled = false;

                    ucDataCollectionProcessInformation.SetApplicationSettings(null);
                    ucDataCollectionProcessInformation.Enabled = false;
                    ucDataCollectionProcessInformation.Reset();

                    ucRealtimeProcessInformation.ResetMemoryCounterLabels();

                    btnSave.Enabled = false;

                    try
                    {
                        if (_remObj != null)
                        {
                            _remObj = null;
                        }

                        if (_ApplicationSettings != null)
                        {
                            _ApplicationSettings = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                    }

                    _ChangesDetected = false;
                    _ChangesSaved = false;

                    break;

                default:

                    WindowsServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus.Stopped;

                    mnuMainCallbackAdministration.Enabled = false;
                    mnuMainCallbackDiagnostics.Enabled = false;

                    ucDataCollectionProcessInformation.Enabled = false;

                    break;
            }
        }

        void ucDataCollectionServiceController_StatusChanged(object sender, WindowsServiceControllerEventArgs e)
        {
            Trace.TraceInformation("Service Status: " + e.CurrentStatus.ToString());

            switch (e.CurrentStatus)
            {
                case System.ServiceProcess.ServiceControllerStatus.Running:

                    DataCollectionServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus.Running;
                    
                    if (this.WindowsServiceControllerStatus == System.ServiceProcess.ServiceControllerStatus.Running)
                    {
                        ucDataCollectionProcessInformation.Enabled = true;
                    }
                    else
                    {
                        ucDataCollectionProcessInformation.Enabled = false;
                    }

                    btnGetCSQsRealtimeData.Enabled = true;
                    btnGetContactRealtimeData.Enabled = true;

                    break;

                case System.ServiceProcess.ServiceControllerStatus.Stopped:

                    DataCollectionServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus.Stopped;

                    ucDataCollectionProcessInformation.Reset();

                    ucDataCollectionProcessInformation.Enabled = false;

                    btnGetCSQsRealtimeData.Enabled = false;
                    btnGetContactRealtimeData.Enabled = false;

                    break;

                default:

                    DataCollectionServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus.Stopped;

                    ucDataCollectionProcessInformation.Enabled = false;

                    ucDataCollectionProcessInformation.Enabled = false;

                    btnGetCSQsRealtimeData.Enabled = false;
                    btnGetContactRealtimeData.Enabled = false;

                    break;
            }
        }

        #endregion

        MemoryCounters GetMemoryCounters()
        {
            if (_remObj == null)
            {
                return null;
            }

            return _remObj.GetMemoryCounters();
        }

        PerformanceCountersSummary GetPerformanceCounters()
        {
            if (_remObj == null)
            {
                return null;
            }

            return _remObj.GetPerformanceCounters();
        }

        private void btnGetContactRealtimeData_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (ucWebServerSettings.txtWebServerIPAddress.Text == String.Empty)
            {
                MessageBox.Show("No valid IP Address provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucWebServerSettings.txtContactDataCollectionPort.Text == String.Empty)
            {
                MessageBox.Show("No valid Contact Data Collection Port provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucUCCXInformation.txtUCCXAdminUser.Text == String.Empty)
            {
                MessageBox.Show("No valid UCCX Admin User provided in UCCX Information.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucUCCXInformation.txtUCCXAdminPassword.Text == String.Empty)
            {
                MessageBox.Show("No valid UCCX Admin Password provided in UCCX Information.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            try
            {
                String sURL = "http://" + ucWebServerSettings.txtWebServerIPAddress.Text + ":" + ucWebServerSettings.txtContactDataCollectionPort.Text;

                com.workflowconcepts.utilities.AESSymmetricEncryption endDec = new com.workflowconcepts.utilities.AESSymmetricEncryption(Constants.ENCRYPTION_PASSWORD, Constants.ENCRYPTION_SALT);

                String sToken = System.Text.RegularExpressions.Regex.Replace(endDec.Encrypt(ucUCCXInformation.txtUCCXAdminUser.Text) + endDec.Encrypt(ucUCCXInformation.txtUCCXAdminPassword.Text), "[^A-Za-z0-9]", "");

                endDec = null;

                sURL = sURL + "/uccxrealtimedata?operation=getcontactdata&token=" + sToken;

                Trace.TraceInformation("sURL = " + sURL);

                System.Diagnostics.Process.Start(sURL);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
        }

        private void btnGetCSQsRealtimeData_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            Trace.TraceInformation("Enter.");

            if (ucWebServerSettings.txtWebServerIPAddress.Text == String.Empty)
            {
                MessageBox.Show("No valid IP Address provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucWebServerSettings.txtContactDataCollectionPort.Text == String.Empty)
            {
                MessageBox.Show("No valid Contact Data Collection Port provided for the Web Server.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucUCCXInformation.txtUCCXAdminUser.Text == String.Empty)
            {
                MessageBox.Show("No valid UCCX Admin User provided in UCCX Information.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            if (ucUCCXInformation.txtUCCXAdminPassword.Text == String.Empty)
            {
                MessageBox.Show("No valid UCCX Admin Password provided in UCCX Information.", Application.ProductName, MessageBoxButtons.OK);

                return;
            }

            try
            {
                String sURL = "http://" + ucWebServerSettings.txtWebServerIPAddress.Text + ":" + ucWebServerSettings.txtContactDataCollectionPort.Text;

                com.workflowconcepts.utilities.AESSymmetricEncryption endDec = new com.workflowconcepts.utilities.AESSymmetricEncryption(Constants.ENCRYPTION_PASSWORD, Constants.ENCRYPTION_SALT);

                String sToken = System.Text.RegularExpressions.Regex.Replace(endDec.Encrypt(ucUCCXInformation.txtUCCXAdminUser.Text) + endDec.Encrypt(ucUCCXInformation.txtUCCXAdminPassword.Text), "[^A-Za-z0-9]", "");

                endDec = null;

                sURL = sURL + "/uccxrealtimedata?operation=getcsqdata&token=" + sToken;

                Trace.TraceInformation("sURL = " + sURL);

                System.Diagnostics.Process.Start(sURL);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
        }

        private void ucDataCollectionProcessInformation_Load(object sender, EventArgs e)
        {
            if (_ApplicationSettings == null)
            {
                Trace.TraceWarning("_ApplicationSettings is null.");
                //MessageBox.Show("No valid UCCX Admin Password provided in UCCX Information.", Application.ProductName, MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
        }
    }
}
