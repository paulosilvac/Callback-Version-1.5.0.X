namespace com.workflowconcepts.applications.uccx
{
    partial class frmCallbackServerManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCallbackServerManager));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuMainFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMainFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMainCallback = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMainCallbackAdministration = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMainCallbackDiagnostics = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMainAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMainAboutCompany = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpbWebServer = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.btnGetCSQsRealtimeData = new System.Windows.Forms.Button();
            this.btnGetContactRealtimeData = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pbCompanyLogo = new System.Windows.Forms.PictureBox();
            this.tabCallbackRecords = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ucUCCXInformation = new com.workflowconcepts.applications.uccx.ucUCCXInformation();
            this.ucDebugSettings = new com.workflowconcepts.applications.uccx.ucDebugSettings();
            this.ucEmailSettings = new com.workflowconcepts.applications.uccx.ucEmailSettings();
            this.ucWebServerSettings = new com.smi.ivr.proxyservices.manager.ucWebServerSettings();
            this.ucCallbackRecordsSettings = new com.workflowconcepts.applications.uccx.ucCallbackRecordsSettings();
            this.ucDataCollectionProcessInformation = new com.workflowconcepts.applications.uccx.ucDataCollectionProcessInformation();
            this.ucDataCollectionServiceController = new com.workflowconcepts.applications.uccx.ucWindowsServiceController();
            this.ucRealtimeProcessInformation = new com.workflowconcepts.applications.uccx.ucRealtimeProcessInformation();
            this.ucWindowsServiceController = new com.workflowconcepts.applications.uccx.ucWindowsServiceController();
            this.mnuMain.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpbWebServer.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCompanyLogo)).BeginInit();
            this.tabCallbackRecords.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMainFile,
            this.mnuMainCallback,
            this.mnuMainAbout});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(886, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            // 
            // mnuMainFile
            // 
            this.mnuMainFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMainFileExit});
            this.mnuMainFile.Name = "mnuMainFile";
            this.mnuMainFile.Size = new System.Drawing.Size(37, 20);
            this.mnuMainFile.Text = "&File";
            // 
            // mnuMainFileExit
            // 
            this.mnuMainFileExit.Name = "mnuMainFileExit";
            this.mnuMainFileExit.Size = new System.Drawing.Size(93, 22);
            this.mnuMainFileExit.Text = "&Exit";
            this.mnuMainFileExit.Click += new System.EventHandler(this.mnuMainFileExit_Click);
            // 
            // mnuMainCallback
            // 
            this.mnuMainCallback.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMainCallbackAdministration,
            this.mnuMainCallbackDiagnostics});
            this.mnuMainCallback.Name = "mnuMainCallback";
            this.mnuMainCallback.Size = new System.Drawing.Size(64, 20);
            this.mnuMainCallback.Text = "&Callback";
            // 
            // mnuMainCallbackAdministration
            // 
            this.mnuMainCallbackAdministration.Name = "mnuMainCallbackAdministration";
            this.mnuMainCallbackAdministration.Size = new System.Drawing.Size(153, 22);
            this.mnuMainCallbackAdministration.Text = "&Administration";
            this.mnuMainCallbackAdministration.Click += new System.EventHandler(this.mnuMainCallbackAdministration_Click);
            // 
            // mnuMainCallbackDiagnostics
            // 
            this.mnuMainCallbackDiagnostics.Name = "mnuMainCallbackDiagnostics";
            this.mnuMainCallbackDiagnostics.Size = new System.Drawing.Size(153, 22);
            this.mnuMainCallbackDiagnostics.Text = "&Diagnostics";
            this.mnuMainCallbackDiagnostics.Click += new System.EventHandler(this.mnuMainCallbackDiagnostics_Click);
            // 
            // mnuMainAbout
            // 
            this.mnuMainAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMainAboutCompany});
            this.mnuMainAbout.Name = "mnuMainAbout";
            this.mnuMainAbout.Size = new System.Drawing.Size(52, 20);
            this.mnuMainAbout.Text = "&About";
            // 
            // mnuMainAboutCompany
            // 
            this.mnuMainAboutCompany.Name = "mnuMainAboutCompany";
            this.mnuMainAboutCompany.Size = new System.Drawing.Size(178, 22);
            this.mnuMainAboutCompany.Text = "&Workflow Concepts";
            this.mnuMainAboutCompany.Click += new System.EventHandler(this.mnuMainAboutCompany_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(797, 480);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 27);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.groupBox3);
            this.tabSettings.Controls.Add(this.groupBox2);
            this.tabSettings.Controls.Add(this.groupBox1);
            this.tabSettings.Controls.Add(this.btnSave);
            this.tabSettings.Controls.Add(this.grpbWebServer);
            this.tabSettings.Controls.Add(this.label3);
            this.tabSettings.Controls.Add(this.label1);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabSettings.Size = new System.Drawing.Size(854, 412);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ucUCCXInformation);
            this.groupBox3.Location = new System.Drawing.Point(32, 153);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(380, 187);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "UCCX Information";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ucDebugSettings);
            this.groupBox2.Location = new System.Drawing.Point(418, 240);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(410, 100);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Debug";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucEmailSettings);
            this.groupBox1.Location = new System.Drawing.Point(418, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 189);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Email Notifications";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(753, 373);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 27);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpbWebServer
            // 
            this.grpbWebServer.Controls.Add(this.ucWebServerSettings);
            this.grpbWebServer.Location = new System.Drawing.Point(32, 45);
            this.grpbWebServer.Name = "grpbWebServer";
            this.grpbWebServer.Size = new System.Drawing.Size(380, 102);
            this.grpbWebServer.TabIndex = 3;
            this.grpbWebServer.TabStop = false;
            this.grpbWebServer.Text = "Web Server";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(70, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(760, 2);
            this.label3.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Settings";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSettings);
            this.tabControl.Controls.Add(this.tabCallbackRecords);
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Location = new System.Drawing.Point(12, 27);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(862, 438);
            this.tabControl.TabIndex = 2;
            // 
            // tabServices
            // 
            this.tabServices.Controls.Add(this.btnGetCSQsRealtimeData);
            this.tabServices.Controls.Add(this.btnGetContactRealtimeData);
            this.tabServices.Controls.Add(this.label6);
            this.tabServices.Controls.Add(this.label5);
            this.tabServices.Controls.Add(this.label8);
            this.tabServices.Controls.Add(this.label7);
            this.tabServices.Controls.Add(this.label4);
            this.tabServices.Controls.Add(this.label2);
            this.tabServices.Controls.Add(this.ucDataCollectionProcessInformation);
            this.tabServices.Controls.Add(this.ucDataCollectionServiceController);
            this.tabServices.Controls.Add(this.ucRealtimeProcessInformation);
            this.tabServices.Controls.Add(this.ucWindowsServiceController);
            this.tabServices.Location = new System.Drawing.Point(4, 22);
            this.tabServices.Name = "tabServices";
            this.tabServices.Size = new System.Drawing.Size(854, 412);
            this.tabServices.TabIndex = 1;
            this.tabServices.Text = "Services";
            this.tabServices.UseVisualStyleBackColor = true;
            // 
            // btnGetCSQsRealtimeData
            // 
            this.btnGetCSQsRealtimeData.Location = new System.Drawing.Point(181, 378);
            this.btnGetCSQsRealtimeData.Name = "btnGetCSQsRealtimeData";
            this.btnGetCSQsRealtimeData.Size = new System.Drawing.Size(151, 23);
            this.btnGetCSQsRealtimeData.TabIndex = 25;
            this.btnGetCSQsRealtimeData.Text = "Get CSQs Realtime Data";
            this.btnGetCSQsRealtimeData.UseVisualStyleBackColor = true;
            this.btnGetCSQsRealtimeData.Click += new System.EventHandler(this.btnGetCSQsRealtimeData_Click);
            // 
            // btnGetContactRealtimeData
            // 
            this.btnGetContactRealtimeData.Location = new System.Drawing.Point(24, 378);
            this.btnGetContactRealtimeData.Name = "btnGetContactRealtimeData";
            this.btnGetContactRealtimeData.Size = new System.Drawing.Size(151, 23);
            this.btnGetContactRealtimeData.TabIndex = 24;
            this.btnGetContactRealtimeData.Text = "Get Contact Realtime Data";
            this.btnGetContactRealtimeData.UseVisualStyleBackColor = true;
            this.btnGetContactRealtimeData.Click += new System.EventHandler(this.btnGetContactRealtimeData_Click);
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Location = new System.Drawing.Point(158, 366);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(680, 2);
            this.label6.TabIndex = 23;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 358);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Test Data Collection Service";
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(246, 195);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(600, 2);
            this.label8.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 187);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(231, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Data Collection Service Controller and Statistics";
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(251, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(594, 1);
            this.label4.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(234, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Callback Server Service Controller and Statistics";
            // 
            // pbCompanyLogo
            // 
            this.pbCompanyLogo.Location = new System.Drawing.Point(15, 480);
            this.pbCompanyLogo.Name = "pbCompanyLogo";
            this.pbCompanyLogo.Size = new System.Drawing.Size(125, 37);
            this.pbCompanyLogo.TabIndex = 3;
            this.pbCompanyLogo.TabStop = false;
            // 
            // tabCallbackRecords
            // 
            this.tabCallbackRecords.Controls.Add(this.groupBox4);
            this.tabCallbackRecords.Controls.Add(this.label10);
            this.tabCallbackRecords.Controls.Add(this.label9);
            this.tabCallbackRecords.Location = new System.Drawing.Point(4, 22);
            this.tabCallbackRecords.Name = "tabCallbackRecords";
            this.tabCallbackRecords.Size = new System.Drawing.Size(854, 412);
            this.tabCallbackRecords.TabIndex = 2;
            this.tabCallbackRecords.Text = "Callback Records";
            this.tabCallbackRecords.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(132, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Callback Records Settings";
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Location = new System.Drawing.Point(156, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(688, 2);
            this.label10.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ucCallbackRecordsSettings);
            this.groupBox4.Location = new System.Drawing.Point(32, 45);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(380, 100);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Callback Records";
            // 
            // ucUCCXInformation
            // 
            this.ucUCCXInformation.Location = new System.Drawing.Point(3, 17);
            this.ucUCCXInformation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucUCCXInformation.Name = "ucUCCXInformation";
            this.ucUCCXInformation.Size = new System.Drawing.Size(390, 162);
            this.ucUCCXInformation.TabIndex = 0;
            // 
            // ucDebugSettings
            // 
            this.ucDebugSettings.Location = new System.Drawing.Point(8, 15);
            this.ucDebugSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucDebugSettings.Name = "ucDebugSettings";
            this.ucDebugSettings.Size = new System.Drawing.Size(390, 79);
            this.ucDebugSettings.TabIndex = 0;
            // 
            // ucEmailSettings
            // 
            this.ucEmailSettings.Location = new System.Drawing.Point(6, 19);
            this.ucEmailSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucEmailSettings.Name = "ucEmailSettings";
            this.ucEmailSettings.Size = new System.Drawing.Size(390, 171);
            this.ucEmailSettings.TabIndex = 9;
            // 
            // ucWebServerSettings
            // 
            this.ucWebServerSettings.Location = new System.Drawing.Point(6, 19);
            this.ucWebServerSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucWebServerSettings.Name = "ucWebServerSettings";
            this.ucWebServerSettings.Size = new System.Drawing.Size(390, 81);
            this.ucWebServerSettings.TabIndex = 0;
            // 
            // ucCallbackRecordsSettings
            // 
            this.ucCallbackRecordsSettings.Location = new System.Drawing.Point(14, 17);
            this.ucCallbackRecordsSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucCallbackRecordsSettings.Name = "ucCallbackRecordsSettings";
            this.ucCallbackRecordsSettings.Size = new System.Drawing.Size(390, 80);
            this.ucCallbackRecordsSettings.TabIndex = 0;
            // 
            // ucDataCollectionProcessInformation
            // 
            this.ucDataCollectionProcessInformation.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ucDataCollectionProcessInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucDataCollectionProcessInformation.Location = new System.Drawing.Point(429, 217);
            this.ucDataCollectionProcessInformation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucDataCollectionProcessInformation.Name = "ucDataCollectionProcessInformation";
            this.ucDataCollectionProcessInformation.Size = new System.Drawing.Size(414, 126);
            this.ucDataCollectionProcessInformation.TabIndex = 21;
            this.ucDataCollectionProcessInformation.Load += new System.EventHandler(this.ucDataCollectionProcessInformation_Load);
            // 
            // ucDataCollectionServiceController
            // 
            this.ucDataCollectionServiceController.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucDataCollectionServiceController.Location = new System.Drawing.Point(24, 217);
            this.ucDataCollectionServiceController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucDataCollectionServiceController.Name = "ucDataCollectionServiceController";
            this.ucDataCollectionServiceController.ServiceName = "";
            this.ucDataCollectionServiceController.Size = new System.Drawing.Size(390, 126);
            this.ucDataCollectionServiceController.TabIndex = 20;
            this.ucDataCollectionServiceController.UseProcessInfo = false;
            // 
            // ucRealtimeProcessInformation
            // 
            this.ucRealtimeProcessInformation.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ucRealtimeProcessInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucRealtimeProcessInformation.Location = new System.Drawing.Point(429, 45);
            this.ucRealtimeProcessInformation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucRealtimeProcessInformation.Name = "ucRealtimeProcessInformation";
            this.ucRealtimeProcessInformation.Size = new System.Drawing.Size(416, 126);
            this.ucRealtimeProcessInformation.TabIndex = 15;
            // 
            // ucWindowsServiceController
            // 
            this.ucWindowsServiceController.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucWindowsServiceController.Location = new System.Drawing.Point(24, 45);
            this.ucWindowsServiceController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucWindowsServiceController.Name = "ucWindowsServiceController";
            this.ucWindowsServiceController.ServiceName = "";
            this.ucWindowsServiceController.Size = new System.Drawing.Size(390, 126);
            this.ucWindowsServiceController.TabIndex = 8;
            this.ucWindowsServiceController.UseProcessInfo = false;
            // 
            // frmCallbackServerManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 527);
            this.Controls.Add(this.pbCompanyLogo);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.mnuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.MaximizeBox = false;
            this.Name = "frmCallbackServerManager";
            this.Text = "Callback Server Manager";
            this.Load += new System.EventHandler(this.frmConnectorManager_Load);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.grpbWebServer.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabServices.ResumeLayout(false);
            this.tabServices.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCompanyLogo)).EndInit();
            this.tabCallbackRecords.ResumeLayout(false);
            this.tabCallbackRecords.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuMainFile;
        private System.Windows.Forms.ToolStripMenuItem mnuMainFileExit;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.GroupBox grpbWebServer;
        private smi.ivr.proxyservices.manager.ucWebServerSettings ucWebServerSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private ucEmailSettings ucEmailSettings;
        private System.Windows.Forms.GroupBox groupBox2;
        //private ucDebugSettings ucDebugSettings;
        private ucDebugSettings ucDebugSettings;
        private System.Windows.Forms.GroupBox groupBox3;
        private ucUCCXInformation ucUCCXInformation;
        private System.Windows.Forms.TabPage tabServices;
        private ucRealtimeProcessInformation ucRealtimeProcessInformation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private ucWindowsServiceController ucWindowsServiceController;
        private ucWindowsServiceController ucDataCollectionServiceController;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private ucDataCollectionProcessInformation ucDataCollectionProcessInformation;
        private System.Windows.Forms.PictureBox pbCompanyLogo;
        private System.Windows.Forms.ToolStripMenuItem mnuMainAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuMainAboutCompany;
        private System.Windows.Forms.Button btnGetContactRealtimeData;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGetCSQsRealtimeData;
        private System.Windows.Forms.ToolStripMenuItem mnuMainCallback;
        private System.Windows.Forms.ToolStripMenuItem mnuMainCallbackAdministration;
        private System.Windows.Forms.ToolStripMenuItem mnuMainCallbackDiagnostics;
        private System.Windows.Forms.TabPage tabCallbackRecords;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private ucCallbackRecordsSettings ucCallbackRecordsSettings;
    }
}