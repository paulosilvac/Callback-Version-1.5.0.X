namespace com.smi.ivr.proxyservices.manager
{
    partial class ucWebServerSettings
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
            this.txtWebServerIPAddress = new System.Windows.Forms.TextBox();
            this.txtWebServerPort = new System.Windows.Forms.TextBox();
            this.txtWebServerPrefix = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtContactDataCollectionPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Prefix";
            // 
            // txtWebServerIPAddress
            // 
            this.txtWebServerIPAddress.Location = new System.Drawing.Point(67, 3);
            this.txtWebServerIPAddress.Name = "txtWebServerIPAddress";
            this.txtWebServerIPAddress.Size = new System.Drawing.Size(134, 20);
            this.txtWebServerIPAddress.TabIndex = 3;
            this.txtWebServerIPAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtWebServerIPAddress.TextChanged += new System.EventHandler(this.txtWebServerIPAddress_TextChanged);
            // 
            // txtWebServerPort
            // 
            this.txtWebServerPort.Location = new System.Drawing.Point(259, 3);
            this.txtWebServerPort.MaxLength = 5;
            this.txtWebServerPort.Name = "txtWebServerPort";
            this.txtWebServerPort.Size = new System.Drawing.Size(77, 20);
            this.txtWebServerPort.TabIndex = 4;
            this.txtWebServerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtWebServerPort.TextChanged += new System.EventHandler(this.txtWebServerPort_TextChanged);
            this.txtWebServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWebServerPort_KeyPress);
            // 
            // txtWebServerPrefix
            // 
            this.txtWebServerPrefix.Location = new System.Drawing.Point(67, 29);
            this.txtWebServerPrefix.Name = "txtWebServerPrefix";
            this.txtWebServerPrefix.Size = new System.Drawing.Size(269, 20);
            this.txtWebServerPrefix.TabIndex = 5;
            this.txtWebServerPrefix.TextChanged += new System.EventHandler(this.txtWebServerPrefix_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Contact Data Collection Port";
            // 
            // txtContactDataCollectionPort
            // 
            this.txtContactDataCollectionPort.Location = new System.Drawing.Point(150, 55);
            this.txtContactDataCollectionPort.MaxLength = 5;
            this.txtContactDataCollectionPort.Name = "txtContactDataCollectionPort";
            this.txtContactDataCollectionPort.Size = new System.Drawing.Size(71, 20);
            this.txtContactDataCollectionPort.TabIndex = 7;
            this.txtContactDataCollectionPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtContactDataCollectionPort.TextChanged += new System.EventHandler(this.txtContactDataCollectionPort_TextChanged);
            this.txtContactDataCollectionPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContactDataCollectionPort_KeyPress);
            // 
            // ucWebServerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtContactDataCollectionPort);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtWebServerPrefix);
            this.Controls.Add(this.txtWebServerPort);
            this.Controls.Add(this.txtWebServerIPAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ucWebServerSettings";
            this.Size = new System.Drawing.Size(348, 85);
            this.Load += new System.EventHandler(this.ucWebServerSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtWebServerIPAddress;
        public System.Windows.Forms.TextBox txtWebServerPort;
        public System.Windows.Forms.TextBox txtWebServerPrefix;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txtContactDataCollectionPort;
    }
}
