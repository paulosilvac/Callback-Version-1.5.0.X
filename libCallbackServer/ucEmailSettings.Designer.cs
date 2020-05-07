namespace com.workflowconcepts.applications.uccx
{
    partial class ucEmailSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtEmailFrom = new System.Windows.Forms.TextBox();
            this.txtEmailTo = new System.Windows.Forms.TextBox();
            this.txtSMTPServer = new System.Windows.Forms.TextBox();
            this.txtSMTPUser = new System.Windows.Forms.TextBox();
            this.txtSMTPPassword = new System.Windows.Forms.TextBox();
            this.ckbEmailFailureNotifications = new System.Windows.Forms.CheckBox();
            this.ckbEmailSuccessNotifications = new System.Windows.Forms.CheckBox();
            this.txtSMTPPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "To";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "SMTP Server";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "User";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(191, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Password";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(309, 137);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 27);
            this.btnTest.TabIndex = 5;
            this.btnTest.Text = "&Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtEmailFrom
            // 
            this.txtEmailFrom.Location = new System.Drawing.Point(41, 33);
            this.txtEmailFrom.Name = "txtEmailFrom";
            this.txtEmailFrom.Size = new System.Drawing.Size(343, 20);
            this.txtEmailFrom.TabIndex = 6;
            this.txtEmailFrom.TextChanged += new System.EventHandler(this.txtEmailFrom_TextChanged);
            // 
            // txtEmailTo
            // 
            this.txtEmailTo.Location = new System.Drawing.Point(41, 59);
            this.txtEmailTo.Name = "txtEmailTo";
            this.txtEmailTo.Size = new System.Drawing.Size(343, 20);
            this.txtEmailTo.TabIndex = 7;
            this.txtEmailTo.TextChanged += new System.EventHandler(this.txtEmailTo_TextChanged);
            // 
            // txtSMTPServer
            // 
            this.txtSMTPServer.Location = new System.Drawing.Point(82, 85);
            this.txtSMTPServer.Name = "txtSMTPServer";
            this.txtSMTPServer.Size = new System.Drawing.Size(213, 20);
            this.txtSMTPServer.TabIndex = 8;
            this.txtSMTPServer.TextChanged += new System.EventHandler(this.txtSMTPServer_TextChanged);
            // 
            // txtSMTPUser
            // 
            this.txtSMTPUser.Location = new System.Drawing.Point(41, 111);
            this.txtSMTPUser.Name = "txtSMTPUser";
            this.txtSMTPUser.Size = new System.Drawing.Size(135, 20);
            this.txtSMTPUser.TabIndex = 9;
            this.txtSMTPUser.TextChanged += new System.EventHandler(this.txtSMTPUser_TextChanged);
            // 
            // txtSMTPPassword
            // 
            this.txtSMTPPassword.Location = new System.Drawing.Point(249, 111);
            this.txtSMTPPassword.Name = "txtSMTPPassword";
            this.txtSMTPPassword.PasswordChar = '*';
            this.txtSMTPPassword.Size = new System.Drawing.Size(135, 20);
            this.txtSMTPPassword.TabIndex = 10;
            this.txtSMTPPassword.TextChanged += new System.EventHandler(this.txtSMTPPassword_TextChanged);
            // 
            // ckbEmailFailureNotifications
            // 
            this.ckbEmailFailureNotifications.AutoSize = true;
            this.ckbEmailFailureNotifications.Location = new System.Drawing.Point(3, 3);
            this.ckbEmailFailureNotifications.Name = "ckbEmailFailureNotifications";
            this.ckbEmailFailureNotifications.Size = new System.Drawing.Size(146, 17);
            this.ckbEmailFailureNotifications.TabIndex = 11;
            this.ckbEmailFailureNotifications.Text = "Email Failure Notifications";
            this.ckbEmailFailureNotifications.UseVisualStyleBackColor = true;
            this.ckbEmailFailureNotifications.CheckedChanged += new System.EventHandler(this.ckbEmailFailureNotifications_CheckedChanged);
            // 
            // ckbEmailSuccessNotifications
            // 
            this.ckbEmailSuccessNotifications.AutoSize = true;
            this.ckbEmailSuccessNotifications.Location = new System.Drawing.Point(155, 3);
            this.ckbEmailSuccessNotifications.Name = "ckbEmailSuccessNotifications";
            this.ckbEmailSuccessNotifications.Size = new System.Drawing.Size(156, 17);
            this.ckbEmailSuccessNotifications.TabIndex = 12;
            this.ckbEmailSuccessNotifications.Text = "Email Success Notifications";
            this.ckbEmailSuccessNotifications.UseVisualStyleBackColor = true;
            this.ckbEmailSuccessNotifications.CheckedChanged += new System.EventHandler(this.ckbEmailSuccessNotifications_CheckedChanged);
            // 
            // txtSMTPPort
            // 
            this.txtSMTPPort.Location = new System.Drawing.Point(333, 85);
            this.txtSMTPPort.MaxLength = 5;
            this.txtSMTPPort.Name = "txtSMTPPort";
            this.txtSMTPPort.Size = new System.Drawing.Size(51, 20);
            this.txtSMTPPort.TabIndex = 13;
            this.txtSMTPPort.TextChanged += new System.EventHandler(this.txtSMTPPort_TextChanged);
            this.txtSMTPPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSMTPPort_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(301, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Port";
            // 
            // ucEmailSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSMTPPort);
            this.Controls.Add(this.ckbEmailSuccessNotifications);
            this.Controls.Add(this.ckbEmailFailureNotifications);
            this.Controls.Add(this.txtSMTPPassword);
            this.Controls.Add(this.txtSMTPUser);
            this.Controls.Add(this.txtSMTPServer);
            this.Controls.Add(this.txtEmailTo);
            this.Controls.Add(this.txtEmailFrom);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ucEmailSettings";
            this.Size = new System.Drawing.Size(390, 171);
            this.Load += new System.EventHandler(this.ucEmailSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnTest;
        public System.Windows.Forms.TextBox txtEmailFrom;
        public System.Windows.Forms.TextBox txtEmailTo;
        public System.Windows.Forms.TextBox txtSMTPServer;
        public System.Windows.Forms.TextBox txtSMTPUser;
        public System.Windows.Forms.TextBox txtSMTPPassword;
        public System.Windows.Forms.CheckBox ckbEmailFailureNotifications;
        public System.Windows.Forms.CheckBox ckbEmailSuccessNotifications;
        public System.Windows.Forms.TextBox txtSMTPPort;
        private System.Windows.Forms.Label label6;
    }
}
