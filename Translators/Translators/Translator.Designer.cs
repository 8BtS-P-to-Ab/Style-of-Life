namespace Translators
{
    partial class TranslatorsFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranslatorsFrm));
            this.ReloadDTBtn = new System.Windows.Forms.Button();
            this.OpacityPnl = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.sldBrTOpacity = new System.Windows.Forms.TrackBar();
            this.InstalledMngRsr = new System.Windows.Forms.Panel();
            this.ManageBtn = new System.Windows.Forms.Button();
            this.InstalledAddResizer = new System.Windows.Forms.Panel();
            this.ReloadITBtn = new System.Windows.Forms.Button();
            this.InstalledTransLstBx = new System.Windows.Forms.ListBox();
            this.UninstallTBtn = new System.Windows.Forms.Button();
            this.DownloadTBtn = new System.Windows.Forms.Button();
            this.DownloadTransLstBx = new System.Windows.Forms.ListBox();
            this.toolTipCntrl = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.AboutTSB = new System.Windows.Forms.ToolStripButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.OpacityPnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrTOpacity)).BeginInit();
            this.InstalledMngRsr.SuspendLayout();
            this.InstalledAddResizer.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReloadDTBtn
            // 
            this.ReloadDTBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.ReloadDTBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReloadDTBtn.Location = new System.Drawing.Point(445, 28);
            this.ReloadDTBtn.Name = "ReloadDTBtn";
            this.ReloadDTBtn.Size = new System.Drawing.Size(23, 23);
            this.ReloadDTBtn.TabIndex = 54;
            this.ReloadDTBtn.UseVisualStyleBackColor = true;
            this.ReloadDTBtn.Visible = false;
            this.ReloadDTBtn.Click += new System.EventHandler(this.ReloadDBtn_Click);
            // 
            // OpacityPnl
            // 
            this.OpacityPnl.Controls.Add(this.label5);
            this.OpacityPnl.Controls.Add(this.sldBrTOpacity);
            this.OpacityPnl.Location = new System.Drawing.Point(3, 315);
            this.OpacityPnl.Name = "OpacityPnl";
            this.OpacityPnl.Size = new System.Drawing.Size(309, 58);
            this.OpacityPnl.TabIndex = 53;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Right;
            this.label5.Location = new System.Drawing.Point(139, 0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(0, 0, 127, 0);
            this.label5.Size = new System.Drawing.Size(170, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Opacity";
            // 
            // sldBrTOpacity
            // 
            this.sldBrTOpacity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sldBrTOpacity.LargeChange = 25;
            this.sldBrTOpacity.Location = new System.Drawing.Point(10, 16);
            this.sldBrTOpacity.Maximum = 100;
            this.sldBrTOpacity.Minimum = 10;
            this.sldBrTOpacity.Name = "sldBrTOpacity";
            this.sldBrTOpacity.Size = new System.Drawing.Size(297, 45);
            this.sldBrTOpacity.TabIndex = 25;
            this.sldBrTOpacity.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.sldBrTOpacity.Value = 100;
            this.sldBrTOpacity.Scroll += new System.EventHandler(this.sldBrOpacity_Scroll);
            // 
            // InstalledMngRsr
            // 
            this.InstalledMngRsr.Controls.Add(this.ManageBtn);
            this.InstalledMngRsr.Controls.Add(this.InstalledAddResizer);
            this.InstalledMngRsr.Controls.Add(this.UninstallTBtn);
            this.InstalledMngRsr.Location = new System.Drawing.Point(12, 28);
            this.InstalledMngRsr.Name = "InstalledMngRsr";
            this.InstalledMngRsr.Size = new System.Drawing.Size(290, 281);
            this.InstalledMngRsr.TabIndex = 52;
            // 
            // ManageBtn
            // 
            this.ManageBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ManageBtn.Location = new System.Drawing.Point(3, 257);
            this.ManageBtn.Name = "ManageBtn";
            this.ManageBtn.Size = new System.Drawing.Size(283, 23);
            this.ManageBtn.TabIndex = 34;
            this.ManageBtn.Text = "Manage translators";
            this.ManageBtn.UseVisualStyleBackColor = true;
            this.ManageBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // InstalledAddResizer
            // 
            this.InstalledAddResizer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InstalledAddResizer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InstalledAddResizer.Controls.Add(this.ReloadITBtn);
            this.InstalledAddResizer.Controls.Add(this.InstalledTransLstBx);
            this.InstalledAddResizer.Location = new System.Drawing.Point(5, 0);
            this.InstalledAddResizer.Name = "InstalledAddResizer";
            this.InstalledAddResizer.Size = new System.Drawing.Size(281, 249);
            this.InstalledAddResizer.TabIndex = 40;
            // 
            // ReloadITBtn
            // 
            this.ReloadITBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.ReloadITBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReloadITBtn.Location = new System.Drawing.Point(258, -1);
            this.ReloadITBtn.Name = "ReloadITBtn";
            this.ReloadITBtn.Size = new System.Drawing.Size(23, 23);
            this.ReloadITBtn.TabIndex = 44;
            this.ReloadITBtn.UseVisualStyleBackColor = true;
            this.ReloadITBtn.Click += new System.EventHandler(this.ReloadIBtn_Click);
            // 
            // InstalledTransLstBx
            // 
            this.InstalledTransLstBx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InstalledTransLstBx.FormattingEnabled = true;
            this.InstalledTransLstBx.HorizontalScrollbar = true;
            this.InstalledTransLstBx.IntegralHeight = false;
            this.InstalledTransLstBx.Location = new System.Drawing.Point(-1, -1);
            this.InstalledTransLstBx.MultiColumn = true;
            this.InstalledTransLstBx.Name = "InstalledTransLstBx";
            this.InstalledTransLstBx.Size = new System.Drawing.Size(281, 249);
            this.InstalledTransLstBx.Sorted = true;
            this.InstalledTransLstBx.TabIndex = 35;
            this.InstalledTransLstBx.SelectedIndexChanged += new System.EventHandler(this.InstalledAddLstBx_SelectedIndexChanged);
            this.InstalledTransLstBx.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.InstalledAddLstBx_MouseDoubleClick);
            // 
            // UninstallTBtn
            // 
            this.UninstallTBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.UninstallTBtn.Location = new System.Drawing.Point(147, 257);
            this.UninstallTBtn.Name = "UninstallTBtn";
            this.UninstallTBtn.Size = new System.Drawing.Size(139, 23);
            this.UninstallTBtn.TabIndex = 38;
            this.UninstallTBtn.Text = "Uninstall selected";
            this.UninstallTBtn.UseVisualStyleBackColor = true;
            this.UninstallTBtn.Click += new System.EventHandler(this.UninstallBtn_Click);
            // 
            // DownloadTBtn
            // 
            this.DownloadTBtn.Location = new System.Drawing.Point(311, 285);
            this.DownloadTBtn.Name = "DownloadTBtn";
            this.DownloadTBtn.Size = new System.Drawing.Size(77, 23);
            this.DownloadTBtn.TabIndex = 50;
            this.DownloadTBtn.Text = "Download selected";
            this.DownloadTBtn.UseVisualStyleBackColor = true;
            this.DownloadTBtn.Visible = false;
            this.DownloadTBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            // 
            // DownloadTransLstBx
            // 
            this.DownloadTransLstBx.FormattingEnabled = true;
            this.DownloadTransLstBx.HorizontalScrollbar = true;
            this.DownloadTransLstBx.Location = new System.Drawing.Point(311, 28);
            this.DownloadTransLstBx.MultiColumn = true;
            this.DownloadTransLstBx.Name = "DownloadTransLstBx";
            this.DownloadTransLstBx.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.DownloadTransLstBx.Size = new System.Drawing.Size(157, 251);
            this.DownloadTransLstBx.Sorted = true;
            this.DownloadTransLstBx.TabIndex = 49;
            this.DownloadTransLstBx.Visible = false;
            this.DownloadTransLstBx.SelectedIndexChanged += new System.EventHandler(this.DownloadAddLstBx_SelectedIndexChanged);
            // 
            // toolTipCntrl
            // 
            this.toolTipCntrl.AutoPopDelay = 15000;
            this.toolTipCntrl.InitialDelay = 500;
            this.toolTipCntrl.ReshowDelay = 100;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutTSB});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(310, 25);
            this.toolStrip1.TabIndex = 56;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // AboutTSB
            // 
            this.AboutTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.AboutTSB.Image = ((System.Drawing.Image)(resources.GetObject("AboutTSB.Image")));
            this.AboutTSB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AboutTSB.Name = "AboutTSB";
            this.AboutTSB.Size = new System.Drawing.Size(44, 22);
            this.AboutTSB.Text = "About";
            this.AboutTSB.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 6;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TranslatorsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 371);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.ReloadDTBtn);
            this.Controls.Add(this.OpacityPnl);
            this.Controls.Add(this.InstalledMngRsr);
            this.Controls.Add(this.DownloadTBtn);
            this.Controls.Add(this.DownloadTransLstBx);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TranslatorsFrm";
            this.Text = "Installed translators";
            this.Load += new System.EventHandler(this.TranslatorsFrm_Load);
            this.OpacityPnl.ResumeLayout(false);
            this.OpacityPnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrTOpacity)).EndInit();
            this.InstalledMngRsr.ResumeLayout(false);
            this.InstalledAddResizer.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ReloadDTBtn;
        private System.Windows.Forms.Panel OpacityPnl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar sldBrTOpacity;
        private System.Windows.Forms.Panel InstalledMngRsr;
        private System.Windows.Forms.Panel InstalledAddResizer;
        private System.Windows.Forms.Button ReloadITBtn;
        private System.Windows.Forms.ListBox InstalledTransLstBx;
        private System.Windows.Forms.Button ManageBtn;
        private System.Windows.Forms.Button UninstallTBtn;
        private System.Windows.Forms.Button DownloadTBtn;
        private System.Windows.Forms.ListBox DownloadTransLstBx;
        private System.Windows.Forms.ToolTip toolTipCntrl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton AboutTSB;
        private System.Windows.Forms.Timer timer1;
    }
}

