namespace com.workflowconcepts.applications.uccx
{
    partial class ucBasicInsertionThrottling
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
            this.ckbEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMaximumRecordsAtATime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ckbEnabled
            // 
            this.ckbEnabled.AutoSize = true;
            this.ckbEnabled.Location = new System.Drawing.Point(3, 3);
            this.ckbEnabled.Name = "ckbEnabled";
            this.ckbEnabled.Size = new System.Drawing.Size(99, 17);
            this.ckbEnabled.TabIndex = 0;
            this.ckbEnabled.Text = "Enable/Disable";
            this.ckbEnabled.UseVisualStyleBackColor = true;
            this.ckbEnabled.CheckedChanged += new System.EventHandler(this.ckbEnabled_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Limit insertion to a maximum of ";
            // 
            // txtMaximumRecordsAtATime
            // 
            this.txtMaximumRecordsAtATime.Location = new System.Drawing.Point(171, 20);
            this.txtMaximumRecordsAtATime.MaxLength = 3;
            this.txtMaximumRecordsAtATime.Name = "txtMaximumRecordsAtATime";
            this.txtMaximumRecordsAtATime.Size = new System.Drawing.Size(32, 20);
            this.txtMaximumRecordsAtATime.TabIndex = 2;
            this.txtMaximumRecordsAtATime.TextChanged += new System.EventHandler(this.txtMaximumRecordsAtATime_TextChanged);
            this.txtMaximumRecordsAtATime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaximumRecordsAtATime_KeyPress);
            this.txtMaximumRecordsAtATime.Leave += new System.EventHandler(this.txtMaximumRecordsAtATime_Leave);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(209, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "records at a time.";
            // 
            // ucBasicInsertionThrottling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMaximumRecordsAtATime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ckbEnabled);
            this.Name = "ucBasicInsertionThrottling";
            this.Size = new System.Drawing.Size(390, 46);
            this.Load += new System.EventHandler(this.ucBasicInsertionThrottling_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox ckbEnabled;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtMaximumRecordsAtATime;
        private System.Windows.Forms.Label label2;
    }
}
