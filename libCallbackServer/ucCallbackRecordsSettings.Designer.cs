namespace com.workflowconcepts.applications.uccx
{
    partial class ucCallbackRecordsSettings
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
            this.txtCallbackRecordsMaximumNumberOfDays = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCallbackRecordsMaximumNumberOfAttempts = new System.Windows.Forms.TextBox();
            this.txtCallbackRecordsMinimumIntervalBetweenRetries = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Maximum number of days";
            // 
            // txtCallbackRecordsMaximumNumberOfDays
            // 
            this.txtCallbackRecordsMaximumNumberOfDays.Location = new System.Drawing.Point(313, 3);
            this.txtCallbackRecordsMaximumNumberOfDays.MaxLength = 2;
            this.txtCallbackRecordsMaximumNumberOfDays.Name = "txtCallbackRecordsMaximumNumberOfDays";
            this.txtCallbackRecordsMaximumNumberOfDays.Size = new System.Drawing.Size(39, 20);
            this.txtCallbackRecordsMaximumNumberOfDays.TabIndex = 1;
            this.txtCallbackRecordsMaximumNumberOfDays.TextChanged += new System.EventHandler(this.txtCallbackRecordsMaximumNumberOfDays_TextChanged);
            this.txtCallbackRecordsMaximumNumberOfDays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCallbackRecordsMaximumNumberOfDays_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Maximum number of attempts";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(201, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Minimum interval between retries (in mins)";
            // 
            // txtCallbackRecordsMaximumNumberOfAttempts
            // 
            this.txtCallbackRecordsMaximumNumberOfAttempts.Location = new System.Drawing.Point(313, 29);
            this.txtCallbackRecordsMaximumNumberOfAttempts.MaxLength = 2;
            this.txtCallbackRecordsMaximumNumberOfAttempts.Name = "txtCallbackRecordsMaximumNumberOfAttempts";
            this.txtCallbackRecordsMaximumNumberOfAttempts.Size = new System.Drawing.Size(39, 20);
            this.txtCallbackRecordsMaximumNumberOfAttempts.TabIndex = 4;
            this.txtCallbackRecordsMaximumNumberOfAttempts.TextChanged += new System.EventHandler(this.txtCallbackRecordsMaximumNumberOfAttempts_TextChanged);
            this.txtCallbackRecordsMaximumNumberOfAttempts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCallbackRecordsMaximumNumberOfAttempts_KeyPress);
            // 
            // txtCallbackRecordsMinimumIntervalBetweenRetries
            // 
            this.txtCallbackRecordsMinimumIntervalBetweenRetries.Location = new System.Drawing.Point(313, 55);
            this.txtCallbackRecordsMinimumIntervalBetweenRetries.MaxLength = 2;
            this.txtCallbackRecordsMinimumIntervalBetweenRetries.Name = "txtCallbackRecordsMinimumIntervalBetweenRetries";
            this.txtCallbackRecordsMinimumIntervalBetweenRetries.Size = new System.Drawing.Size(39, 20);
            this.txtCallbackRecordsMinimumIntervalBetweenRetries.TabIndex = 5;
            this.txtCallbackRecordsMinimumIntervalBetweenRetries.TextChanged += new System.EventHandler(this.txtCallbackRecordsMinimumIntervalBetweenRetries_TextChanged);
            this.txtCallbackRecordsMinimumIntervalBetweenRetries.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCallbackRecordsMinimumIntervalBetweenRetries_KeyPress);
            // 
            // ucCallbackRecordsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtCallbackRecordsMinimumIntervalBetweenRetries);
            this.Controls.Add(this.txtCallbackRecordsMaximumNumberOfAttempts);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCallbackRecordsMaximumNumberOfDays);
            this.Controls.Add(this.label1);
            this.Name = "ucCallbackRecordsSettings";
            this.Size = new System.Drawing.Size(390, 79);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtCallbackRecordsMaximumNumberOfDays;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtCallbackRecordsMaximumNumberOfAttempts;
        public System.Windows.Forms.TextBox txtCallbackRecordsMinimumIntervalBetweenRetries;
    }
}
