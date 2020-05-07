namespace com.workflowconcepts.applications.uccx
{
    partial class ucRealtimeProcessInformation
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
            this.components = new System.ComponentModel.Container();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblTotalProcessorTime = new System.Windows.Forms.Label();
            this.lblThreads = new System.Windows.Forms.Label();
            this.lblMemoryUsed = new System.Windows.Forms.Label();
            this.lblWorkingSet = new System.Windows.Forms.Label();
            this.lblVirtualMemory = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tmrRefreshMemoryLabels = new System.Windows.Forms.Timer(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTotalRequests = new System.Windows.Forms.Label();
            this.lblRequestsHandled = new System.Windows.Forms.Label();
            this.lblRequestsFailed = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblAvgProcessingTime = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblRequestQueueSize = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblStartTime
            // 
            this.lblStartTime.Location = new System.Drawing.Point(59, 6);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(166, 14);
            this.lblStartTime.TabIndex = 0;
            this.lblStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotalProcessorTime
            // 
            this.lblTotalProcessorTime.Location = new System.Drawing.Point(111, 21);
            this.lblTotalProcessorTime.Name = "lblTotalProcessorTime";
            this.lblTotalProcessorTime.Size = new System.Drawing.Size(114, 14);
            this.lblTotalProcessorTime.TabIndex = 1;
            this.lblTotalProcessorTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblThreads
            // 
            this.lblThreads.Location = new System.Drawing.Point(111, 36);
            this.lblThreads.Name = "lblThreads";
            this.lblThreads.Size = new System.Drawing.Size(114, 14);
            this.lblThreads.TabIndex = 2;
            this.lblThreads.Text = "0 MB";
            this.lblThreads.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMemoryUsed
            // 
            this.lblMemoryUsed.Location = new System.Drawing.Point(111, 51);
            this.lblMemoryUsed.Name = "lblMemoryUsed";
            this.lblMemoryUsed.Size = new System.Drawing.Size(114, 14);
            this.lblMemoryUsed.TabIndex = 3;
            this.lblMemoryUsed.Text = "0 MB";
            this.lblMemoryUsed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblWorkingSet
            // 
            this.lblWorkingSet.Location = new System.Drawing.Point(111, 66);
            this.lblWorkingSet.Name = "lblWorkingSet";
            this.lblWorkingSet.Size = new System.Drawing.Size(114, 14);
            this.lblWorkingSet.TabIndex = 4;
            this.lblWorkingSet.Text = "0 MB";
            this.lblWorkingSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblVirtualMemory
            // 
            this.lblVirtualMemory.Location = new System.Drawing.Point(111, 81);
            this.lblVirtualMemory.Name = "lblVirtualMemory";
            this.lblVirtualMemory.Size = new System.Drawing.Size(114, 14);
            this.lblVirtualMemory.TabIndex = 5;
            this.lblVirtualMemory.Text = "0 MB";
            this.lblVirtualMemory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 14);
            this.label1.TabIndex = 6;
            this.label1.Text = "StartTime:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 14);
            this.label2.TabIndex = 7;
            this.label2.Text = "TotalProcessorTime:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 14);
            this.label3.TabIndex = 8;
            this.label3.Text = "Threads:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 14);
            this.label4.TabIndex = 9;
            this.label4.Text = "MemoryUsed:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 14);
            this.label5.TabIndex = 10;
            this.label5.Text = "WorkingSet:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(3, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 14);
            this.label6.TabIndex = 11;
            this.label6.Text = "VirtualMemory:";
            // 
            // tmrRefreshMemoryLabels
            // 
            this.tmrRefreshMemoryLabels.Enabled = true;
            this.tmrRefreshMemoryLabels.Interval = 3000;
            this.tmrRefreshMemoryLabels.Tick += new System.EventHandler(this.tmrRefreshMemoryLabels_Tick);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(237, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 14);
            this.label7.TabIndex = 12;
            this.label7.Text = "Total Requests:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(237, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 14);
            this.label8.TabIndex = 13;
            this.label8.Text = "Requests Handled:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(237, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 14);
            this.label9.TabIndex = 14;
            this.label9.Text = "Requests Failed:";
            // 
            // lblTotalRequests
            // 
            this.lblTotalRequests.Location = new System.Drawing.Point(338, 6);
            this.lblTotalRequests.Name = "lblTotalRequests";
            this.lblTotalRequests.Size = new System.Drawing.Size(77, 14);
            this.lblTotalRequests.TabIndex = 15;
            this.lblTotalRequests.Text = "0";
            this.lblTotalRequests.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRequestsHandled
            // 
            this.lblRequestsHandled.Location = new System.Drawing.Point(338, 21);
            this.lblRequestsHandled.Name = "lblRequestsHandled";
            this.lblRequestsHandled.Size = new System.Drawing.Size(77, 14);
            this.lblRequestsHandled.TabIndex = 16;
            this.lblRequestsHandled.Text = "0";
            this.lblRequestsHandled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRequestsFailed
            // 
            this.lblRequestsFailed.Location = new System.Drawing.Point(338, 36);
            this.lblRequestsFailed.Name = "lblRequestsFailed";
            this.lblRequestsFailed.Size = new System.Drawing.Size(77, 14);
            this.lblRequestsFailed.TabIndex = 17;
            this.lblRequestsFailed.Text = "0";
            this.lblRequestsFailed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(237, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(124, 14);
            this.label10.TabIndex = 18;
            this.label10.Text = "Avg Processing Time:";
            // 
            // lblAvgProcessingTime
            // 
            this.lblAvgProcessingTime.Location = new System.Drawing.Point(348, 51);
            this.lblAvgProcessingTime.Name = "lblAvgProcessingTime";
            this.lblAvgProcessingTime.Size = new System.Drawing.Size(67, 14);
            this.lblAvgProcessingTime.TabIndex = 19;
            this.lblAvgProcessingTime.Text = "0";
            this.lblAvgProcessingTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(237, 66);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 14);
            this.label11.TabIndex = 20;
            this.label11.Text = "Request Queue Size:";
            // 
            // lblRequestQueueSize
            // 
            this.lblRequestQueueSize.Location = new System.Drawing.Point(353, 66);
            this.lblRequestQueueSize.Name = "lblRequestQueueSize";
            this.lblRequestQueueSize.Size = new System.Drawing.Size(62, 14);
            this.lblRequestQueueSize.TabIndex = 21;
            this.lblRequestQueueSize.Text = "0";
            this.lblRequestQueueSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ucRealtimeProcessInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblRequestQueueSize);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblAvgProcessingTime);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblRequestsFailed);
            this.Controls.Add(this.lblRequestsHandled);
            this.Controls.Add(this.lblTotalRequests);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblVirtualMemory);
            this.Controls.Add(this.lblWorkingSet);
            this.Controls.Add(this.lblMemoryUsed);
            this.Controls.Add(this.lblThreads);
            this.Controls.Add(this.lblTotalProcessorTime);
            this.Controls.Add(this.lblStartTime);
            this.Name = "ucRealtimeProcessInformation";
            this.Size = new System.Drawing.Size(416, 124);
            this.Load += new System.EventHandler(this.ucRealtimeProcessInformation_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblTotalProcessorTime;
        private System.Windows.Forms.Label lblThreads;
        private System.Windows.Forms.Label lblMemoryUsed;
        private System.Windows.Forms.Label lblWorkingSet;
        private System.Windows.Forms.Label lblVirtualMemory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer tmrRefreshMemoryLabels;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTotalRequests;
        private System.Windows.Forms.Label lblRequestsHandled;
        private System.Windows.Forms.Label lblRequestsFailed;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblAvgProcessingTime;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblRequestQueueSize;
    }
}
