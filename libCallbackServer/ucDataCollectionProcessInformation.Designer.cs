namespace com.workflowconcepts.applications.uccx
{
    partial class ucDataCollectionProcessInformation
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
            this.btnRefreshProcessInformation = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblJVM = new System.Windows.Forms.Label();
            this.lblStartupTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblUCCXMasterNode = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblContactsSummary = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCSQSummary = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRefreshProcessInformation
            // 
            this.btnRefreshProcessInformation.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnRefreshProcessInformation.Location = new System.Drawing.Point(3, 3);
            this.btnRefreshProcessInformation.Name = "btnRefreshProcessInformation";
            this.btnRefreshProcessInformation.Size = new System.Drawing.Size(30, 30);
            this.btnRefreshProcessInformation.TabIndex = 0;
            this.btnRefreshProcessInformation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRefreshProcessInformation.UseVisualStyleBackColor = false;
            this.btnRefreshProcessInformation.Click += new System.EventHandler(this.btnRefreshProcessInformation_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "JVM:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Startup time:";
            // 
            // lblJVM
            // 
            this.lblJVM.Location = new System.Drawing.Point(171, 12);
            this.lblJVM.Name = "lblJVM";
            this.lblJVM.Size = new System.Drawing.Size(100, 13);
            this.lblJVM.TabIndex = 3;
            this.lblJVM.Text = "jvm";
            this.lblJVM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblStartupTime
            // 
            this.lblStartupTime.Location = new System.Drawing.Point(140, 46);
            this.lblStartupTime.Name = "lblStartupTime";
            this.lblStartupTime.Size = new System.Drawing.Size(131, 13);
            this.lblStartupTime.TabIndex = 4;
            this.lblStartupTime.Text = "08/22/2017 10:30:37 AM";
            this.lblStartupTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "UCCX Master Node:";
            // 
            // lblUCCXMasterNode
            // 
            this.lblUCCXMasterNode.Location = new System.Drawing.Point(211, 29);
            this.lblUCCXMasterNode.Name = "lblUCCXMasterNode";
            this.lblUCCXMasterNode.Size = new System.Drawing.Size(60, 13);
            this.lblUCCXMasterNode.TabIndex = 6;
            this.lblUCCXMasterNode.Text = "uccxmasternode";
            this.lblUCCXMasterNode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Contacts:";
            // 
            // lblContactsSummary
            // 
            this.lblContactsSummary.Location = new System.Drawing.Point(163, 63);
            this.lblContactsSummary.Name = "lblContactsSummary";
            this.lblContactsSummary.Size = new System.Drawing.Size(108, 13);
            this.lblContactsSummary.TabIndex = 8;
            this.lblContactsSummary.Text = "contactssummary";
            this.lblContactsSummary.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "CSQs:";
            // 
            // lblCSQSummary
            // 
            this.lblCSQSummary.Location = new System.Drawing.Point(166, 80);
            this.lblCSQSummary.Name = "lblCSQSummary";
            this.lblCSQSummary.Size = new System.Drawing.Size(105, 13);
            this.lblCSQSummary.TabIndex = 10;
            this.lblCSQSummary.Text = "csqssummary";
            this.lblCSQSummary.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ucDataCollectionProcessInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblCSQSummary);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblContactsSummary);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblUCCXMasterNode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblStartupTime);
            this.Controls.Add(this.lblJVM);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRefreshProcessInformation);
            this.Name = "ucDataCollectionProcessInformation";
            this.Size = new System.Drawing.Size(414, 122);
            this.Load += new System.EventHandler(this.ucDataCollectionProcessInformation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRefreshProcessInformation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblJVM;
        private System.Windows.Forms.Label lblStartupTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblUCCXMasterNode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblContactsSummary;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCSQSummary;
    }
}
