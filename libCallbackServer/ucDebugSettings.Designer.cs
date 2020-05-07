namespace com.workflowconcepts.applications.uccx
{
    partial class ucDebugSettings
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
            this.ckbDebugEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbDebugLevel = new System.Windows.Forms.ComboBox();
            this.cbRetainUnit = new System.Windows.Forms.ComboBox();
            this.txtRetainValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ckbDebugEnabled
            // 
            this.ckbDebugEnabled.AutoSize = true;
            this.ckbDebugEnabled.Location = new System.Drawing.Point(3, 3);
            this.ckbDebugEnabled.Name = "ckbDebugEnabled";
            this.ckbDebugEnabled.Size = new System.Drawing.Size(92, 17);
            this.ckbDebugEnabled.TabIndex = 0;
            this.ckbDebugEnabled.Text = "Enable debug";
            this.ckbDebugEnabled.UseVisualStyleBackColor = true;
            this.ckbDebugEnabled.CheckedChanged += new System.EventHandler(this.ckbDebugEnabled_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Debug Level";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Retain Unit";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(242, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Retain Value";
            // 
            // cbDebugLevel
            // 
            this.cbDebugLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDebugLevel.FormattingEnabled = true;
            this.cbDebugLevel.Items.AddRange(new object[] {
            "Verbose",
            "Warning",
            "Critical"});
            this.cbDebugLevel.Location = new System.Drawing.Point(97, 30);
            this.cbDebugLevel.Name = "cbDebugLevel";
            this.cbDebugLevel.Size = new System.Drawing.Size(121, 21);
            this.cbDebugLevel.TabIndex = 4;
            this.cbDebugLevel.SelectedIndexChanged += new System.EventHandler(this.cbDebugLevel_SelectedIndexChanged);
            this.cbDebugLevel.Leave += new System.EventHandler(this.cbDebugLevel_Leave);
            // 
            // cbRetainUnit
            // 
            this.cbRetainUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRetainUnit.FormattingEnabled = true;
            this.cbRetainUnit.Items.AddRange(new object[] {
            "Files",
            "HDSpace",
            "Days"});
            this.cbRetainUnit.Location = new System.Drawing.Point(97, 57);
            this.cbRetainUnit.Name = "cbRetainUnit";
            this.cbRetainUnit.Size = new System.Drawing.Size(121, 21);
            this.cbRetainUnit.TabIndex = 5;
            this.cbRetainUnit.SelectedIndexChanged += new System.EventHandler(this.cbRetainUnit_SelectedIndexChanged);
            // 
            // txtRetainValue
            // 
            this.txtRetainValue.Location = new System.Drawing.Point(316, 57);
            this.txtRetainValue.MaxLength = 3;
            this.txtRetainValue.Name = "txtRetainValue";
            this.txtRetainValue.Size = new System.Drawing.Size(56, 20);
            this.txtRetainValue.TabIndex = 6;
            this.txtRetainValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRetainValue.TextChanged += new System.EventHandler(this.txtRetainValue_TextChanged);
            this.txtRetainValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRetainValue_KeyPress);
            this.txtRetainValue.Leave += new System.EventHandler(this.txtRetainValue_Leave);
            // 
            // ucDebugSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtRetainValue);
            this.Controls.Add(this.cbRetainUnit);
            this.Controls.Add(this.cbDebugLevel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ckbDebugEnabled);
            this.Name = "ucDebugSettings";
            this.Size = new System.Drawing.Size(390, 90);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox ckbDebugEnabled;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox cbDebugLevel;
        public System.Windows.Forms.ComboBox cbRetainUnit;
        public System.Windows.Forms.TextBox txtRetainValue;
    }
}
