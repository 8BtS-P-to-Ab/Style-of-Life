namespace Mass_Renamer
{
    partial class MassRename
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MassRename));
            this.BtnMassRename = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.PreferencesTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.selectionTypeTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.alphanumericSTBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.alphabraicSTBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.regexSTBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.regexTxtBx = new System.Windows.Forms.ToolStripTextBox();
            this.randomSTBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.countingTypeTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.numericaldTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.alphabeticalTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.customTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.disableCountingTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.disableWarningsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.guideBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.sldBrOpacity = new System.Windows.Forms.TrackBar();
            this.textAfterNumTxtBx = new System.Windows.Forms.TextBox();
            this.extentionTxtBx = new System.Windows.Forms.TextBox();
            this.fileRenameTxtBx = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnMassRename
            // 
            this.BtnMassRename.Location = new System.Drawing.Point(12, 135);
            this.BtnMassRename.Name = "BtnMassRename";
            this.BtnMassRename.Size = new System.Drawing.Size(271, 23);
            this.BtnMassRename.TabIndex = 27;
            this.BtnMassRename.Text = "Mass rename";
            this.BtnMassRename.UseVisualStyleBackColor = true;
            this.BtnMassRename.Click += new System.EventHandler(this.BtnMassRename_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.AddExtension = false;
            this.openFileDialog1.AutoUpgradeEnabled = false;
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.InitialDirectory = "C:\\";
            this.openFileDialog1.Multiselect = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(299, 25);
            this.toolStrip1.TabIndex = 29;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PreferencesTSMI,
            this.disableWarningsToolStripMenuItem,
            this.exitBtn});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // PreferencesTSMI
            // 
            this.PreferencesTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectionTypeTSMI,
            this.countingTypeTSMI});
            this.PreferencesTSMI.Name = "PreferencesTSMI";
            this.PreferencesTSMI.Size = new System.Drawing.Size(242, 22);
            this.PreferencesTSMI.Text = "Preferences";
            this.PreferencesTSMI.ToolTipText = "Change how the mass renamer works";
            // 
            // selectionTypeTSMI
            // 
            this.selectionTypeTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alphanumericSTBtn,
            this.alphabraicSTBtn,
            this.regexSTBtn,
            this.randomSTBtn,
            this.toolStripSeparator1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem4});
            this.selectionTypeTSMI.Name = "selectionTypeTSMI";
            this.selectionTypeTSMI.Size = new System.Drawing.Size(150, 22);
            this.selectionTypeTSMI.Text = "Selection type";
            this.selectionTypeTSMI.ToolTipText = "Change how the mass renamer selects files";
            // 
            // alphanumericSTBtn
            // 
            this.alphanumericSTBtn.Checked = true;
            this.alphanumericSTBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alphanumericSTBtn.Name = "alphanumericSTBtn";
            this.alphanumericSTBtn.Size = new System.Drawing.Size(287, 22);
            this.alphanumericSTBtn.Text = "Alphanumeric (default)";
            this.alphanumericSTBtn.ToolTipText = "The mass renamer will select files in the order from the first alphanumeric chara" +
    "cters (0-inf)\r\ne.g. 0,1,2,3,4,5,6,7,8,9,10,11,12,13 or 13a, 14a, 15a, 16a\r\n";
            this.alphanumericSTBtn.Click += new System.EventHandler(this.alphanumericSTBtn_Click);
            // 
            // alphabraicSTBtn
            // 
            this.alphabraicSTBtn.Name = "alphabraicSTBtn";
            this.alphabraicSTBtn.Size = new System.Drawing.Size(287, 22);
            this.alphabraicSTBtn.Text = "Alphabraic";
            this.alphabraicSTBtn.ToolTipText = "The mass renamer will select files in the order from a-z then 0-9\r\ne.g. a,b,c,d o" +
    "r 0,1,10,11,12,13,14,15,16,17,18,19,2,20,21";
            this.alphabraicSTBtn.Click += new System.EventHandler(this.alphabraicSTBtn_Click);
            // 
            // regexSTBtn
            // 
            this.regexSTBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regexTxtBx});
            this.regexSTBtn.Enabled = false;
            this.regexSTBtn.Name = "regexSTBtn";
            this.regexSTBtn.Size = new System.Drawing.Size(287, 22);
            this.regexSTBtn.Text = "Regular expression";
            this.regexSTBtn.ToolTipText = "The mass renamer will select files in the order according to the input regular ex" +
    "pression (regex).\r\nWIP";
            this.regexSTBtn.Click += new System.EventHandler(this.regexSTBtn_Click);
            // 
            // regexTxtBx
            // 
            this.regexTxtBx.AcceptsTab = true;
            this.regexTxtBx.Name = "regexTxtBx";
            this.regexTxtBx.Size = new System.Drawing.Size(160, 23);
            this.regexTxtBx.Text = "Input Regex expression here";
            // 
            // randomSTBtn
            // 
            this.randomSTBtn.Name = "randomSTBtn";
            this.randomSTBtn.Size = new System.Drawing.Size(287, 22);
            this.randomSTBtn.Text = "Random";
            this.randomSTBtn.ToolTipText = "The mass renamer will select files at random (good for shuffling files)";
            this.randomSTBtn.Click += new System.EventHandler(this.randomSTBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(284, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Enabled = false;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(287, 22);
            this.toolStripMenuItem2.Text = "Additions";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.BackColor = System.Drawing.SystemColors.Info;
            this.toolStripMenuItem4.Enabled = false;
            this.toolStripMenuItem4.ForeColor = System.Drawing.SystemColors.InfoText;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(287, 22);
            this.toolStripMenuItem4.Text = "Modding has not been implamented yet";
            // 
            // countingTypeTSMI
            // 
            this.countingTypeTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.numericaldTSMI,
            this.alphabeticalTSMI,
            this.customTSMI,
            this.disableCountingTSMI,
            this.toolStripSeparator2,
            this.toolStripMenuItem1,
            this.toolStripMenuItem3});
            this.countingTypeTSMI.Name = "countingTypeTSMI";
            this.countingTypeTSMI.Size = new System.Drawing.Size(150, 22);
            this.countingTypeTSMI.Text = "Counting type";
            this.countingTypeTSMI.ToolTipText = "Change what method the mass renamer uses to order files";
            // 
            // numericaldTSMI
            // 
            this.numericaldTSMI.Checked = true;
            this.numericaldTSMI.CheckOnClick = true;
            this.numericaldTSMI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.numericaldTSMI.Name = "numericaldTSMI";
            this.numericaldTSMI.Size = new System.Drawing.Size(287, 22);
            this.numericaldTSMI.Text = "Numerical (default)";
            this.numericaldTSMI.ToolTipText = resources.GetString("numericaldTSMI.ToolTipText");
            this.numericaldTSMI.Click += new System.EventHandler(this.numericaldTSMI_Click);
            // 
            // alphabeticalTSMI
            // 
            this.alphabeticalTSMI.CheckOnClick = true;
            this.alphabeticalTSMI.Name = "alphabeticalTSMI";
            this.alphabeticalTSMI.Size = new System.Drawing.Size(287, 22);
            this.alphabeticalTSMI.Text = "Alphabetical";
            this.alphabeticalTSMI.ToolTipText = resources.GetString("alphabeticalTSMI.ToolTipText");
            this.alphabeticalTSMI.Click += new System.EventHandler(this.alphabeticalTSMI_Click);
            // 
            // customTSMI
            // 
            this.customTSMI.Enabled = false;
            this.customTSMI.Name = "customTSMI";
            this.customTSMI.Size = new System.Drawing.Size(287, 22);
            this.customTSMI.Text = "Custom";
            this.customTSMI.ToolTipText = "The mass renamer will order the file selection in the manner of the input collect" +
    "ion.\r\nVery powerfull, much wow.\r\nWIP";
            this.customTSMI.Click += new System.EventHandler(this.customTSMI_Click);
            // 
            // disableCountingTSMI
            // 
            this.disableCountingTSMI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.disableCountingTSMI.Name = "disableCountingTSMI";
            this.disableCountingTSMI.Size = new System.Drawing.Size(287, 22);
            this.disableCountingTSMI.Text = "Disable";
            this.disableCountingTSMI.ToolTipText = resources.GetString("disableCountingTSMI.ToolTipText");
            this.disableCountingTSMI.Click += new System.EventHandler(this.disableCountingTSMI_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(284, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Enabled = false;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(287, 22);
            this.toolStripMenuItem1.Text = "Additions";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.BackColor = System.Drawing.SystemColors.Info;
            this.toolStripMenuItem3.Enabled = false;
            this.toolStripMenuItem3.ForeColor = System.Drawing.SystemColors.InfoText;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(287, 22);
            this.toolStripMenuItem3.Text = "Modding has not been implamented yet";
            // 
            // disableWarningsToolStripMenuItem
            // 
            this.disableWarningsToolStripMenuItem.CheckOnClick = true;
            this.disableWarningsToolStripMenuItem.Name = "disableWarningsToolStripMenuItem";
            this.disableWarningsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.disableWarningsToolStripMenuItem.Text = "Disable warnings and reminders";
            this.disableWarningsToolStripMenuItem.ToolTipText = "If the warnings are getting annoying, you can turn them off.";
            this.disableWarningsToolStripMenuItem.Click += new System.EventHandler(this.disableWarningsToolStripMenuItem_Click);
            // 
            // exitBtn
            // 
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(242, 22);
            this.exitBtn.Text = "Exit";
            this.exitBtn.ToolTipText = "Exit mass renamer";
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.guideBtn,
            this.aboutBtn});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(45, 22);
            this.toolStripDropDownButton2.Text = "Help";
            // 
            // guideBtn
            // 
            this.guideBtn.Name = "guideBtn";
            this.guideBtn.Size = new System.Drawing.Size(107, 22);
            this.guideBtn.Text = "Guide";
            this.guideBtn.ToolTipText = "Get help with how to use this form";
            this.guideBtn.Click += new System.EventHandler(this.guideBtn_Click);
            // 
            // aboutBtn
            // 
            this.aboutBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(107, 22);
            this.aboutBtn.Text = "About";
            this.aboutBtn.ToolTipText = "About SOL\'s mass renamer form";
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // sldBrOpacity
            // 
            this.sldBrOpacity.LargeChange = 25;
            this.sldBrOpacity.Location = new System.Drawing.Point(4, 177);
            this.sldBrOpacity.Maximum = 100;
            this.sldBrOpacity.Minimum = 10;
            this.sldBrOpacity.Name = "sldBrOpacity";
            this.sldBrOpacity.Size = new System.Drawing.Size(283, 45);
            this.sldBrOpacity.TabIndex = 32;
            this.sldBrOpacity.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.sldBrOpacity.Value = 100;
            this.sldBrOpacity.Scroll += new System.EventHandler(this.sldBrOpacity_Scroll);
            // 
            // textAfterNumTxtBx
            // 
            this.textAfterNumTxtBx.Location = new System.Drawing.Point(12, 109);
            this.textAfterNumTxtBx.MaxLength = 255;
            this.textAfterNumTxtBx.Name = "textAfterNumTxtBx";
            this.textAfterNumTxtBx.Size = new System.Drawing.Size(136, 20);
            this.textAfterNumTxtBx.TabIndex = 31;
            this.textAfterNumTxtBx.Text = "Text after number (optional)";
            this.textAfterNumTxtBx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.textAfterNumTxtBx, resources.GetString("textAfterNumTxtBx.ToolTip"));
            this.textAfterNumTxtBx.TextChanged += new System.EventHandler(this.textAfterNumTxtBx_TextChanged);
            this.textAfterNumTxtBx.Enter += new System.EventHandler(this.textAfterNumTxtBx_Enter);
            this.textAfterNumTxtBx.Leave += new System.EventHandler(this.textAfterNumTxtBx_Leave);
            // 
            // extentionTxtBx
            // 
            this.extentionTxtBx.Location = new System.Drawing.Point(147, 109);
            this.extentionTxtBx.Name = "extentionTxtBx";
            this.extentionTxtBx.Size = new System.Drawing.Size(136, 20);
            this.extentionTxtBx.TabIndex = 30;
            this.extentionTxtBx.Text = "Extention (optional)";
            this.extentionTxtBx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.extentionTxtBx, resources.GetString("extentionTxtBx.ToolTip"));
            this.extentionTxtBx.TextChanged += new System.EventHandler(this.extentionTxtBx_TextChanged);
            this.extentionTxtBx.Enter += new System.EventHandler(this.extentionTxtBx_Enter);
            this.extentionTxtBx.Leave += new System.EventHandler(this.extentionTxtBx_Leave);
            // 
            // fileRenameTxtBx
            // 
            this.fileRenameTxtBx.Location = new System.Drawing.Point(12, 28);
            this.fileRenameTxtBx.MaxLength = 255;
            this.fileRenameTxtBx.Multiline = true;
            this.fileRenameTxtBx.Name = "fileRenameTxtBx";
            this.fileRenameTxtBx.Size = new System.Drawing.Size(271, 101);
            this.fileRenameTxtBx.TabIndex = 28;
            this.fileRenameTxtBx.Text = "What to rename the file to";
            this.fileRenameTxtBx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.fileRenameTxtBx, resources.GetString("fileRenameTxtBx.ToolTip"));
            this.fileRenameTxtBx.TextChanged += new System.EventHandler(this.fileRenameTxtBx_TextChanged);
            this.fileRenameTxtBx.Enter += new System.EventHandler(this.fileRenameTxtBx_Enter);
            this.fileRenameTxtBx.Leave += new System.EventHandler(this.fileRenameTxtBx_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(128, 161);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Opacity";
            // 
            // MassRename
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 214);
            this.Controls.Add(this.BtnMassRename);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.sldBrOpacity);
            this.Controls.Add(this.textAfterNumTxtBx);
            this.Controls.Add(this.extentionTxtBx);
            this.Controls.Add(this.fileRenameTxtBx);
            this.Controls.Add(this.label5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MassRename";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mass Renamer";
            this.Load += new System.EventHandler(this.MassRename_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnMassRename;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem PreferencesTSMI;
        private System.Windows.Forms.ToolStripMenuItem selectionTypeTSMI;
        private System.Windows.Forms.ToolStripMenuItem alphanumericSTBtn;
        private System.Windows.Forms.ToolStripMenuItem alphabraicSTBtn;
        private System.Windows.Forms.ToolStripMenuItem regexSTBtn;
        private System.Windows.Forms.ToolStripTextBox regexTxtBx;
        private System.Windows.Forms.ToolStripMenuItem randomSTBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem countingTypeTSMI;
        private System.Windows.Forms.ToolStripMenuItem numericaldTSMI;
        private System.Windows.Forms.ToolStripMenuItem alphabeticalTSMI;
        private System.Windows.Forms.ToolStripMenuItem customTSMI;
        private System.Windows.Forms.ToolStripMenuItem disableCountingTSMI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem disableWarningsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitBtn;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem guideBtn;
        private System.Windows.Forms.ToolStripMenuItem aboutBtn;
        private System.Windows.Forms.TrackBar sldBrOpacity;
        private System.Windows.Forms.TextBox textAfterNumTxtBx;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox extentionTxtBx;
        private System.Windows.Forms.TextBox fileRenameTxtBx;
        private System.Windows.Forms.Label label5;
    }
}

