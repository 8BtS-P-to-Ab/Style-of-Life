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
            this.UpdateAddBtn = new System.Windows.Forms.Button();
            this.ReloadDBtn = new System.Windows.Forms.Button();
            this.OpacityPnl = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.sldBrOpacity = new System.Windows.Forms.TrackBar();
            this.InstalledMngRsr = new System.Windows.Forms.Panel();
            this.InstalledAddResizer = new System.Windows.Forms.Panel();
            this.ReloadIBtn = new System.Windows.Forms.Button();
            this.InstalledAddLstBx = new System.Windows.Forms.ListBox();
            this.UninstallBtn = new System.Windows.Forms.Button();
            this.AddBtn = new System.Windows.Forms.Button();
            this.DownloadBtn = new System.Windows.Forms.Button();
            this.DownloadAddLstBx = new System.Windows.Forms.ListBox();
            this.toolTipCntrl = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.AboutTSB = new System.Windows.Forms.ToolStripButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.OpacityPnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).BeginInit();
            this.InstalledMngRsr.SuspendLayout();
            this.InstalledAddResizer.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdateAddBtn
            // 
            this.UpdateAddBtn.Location = new System.Drawing.Point(391, 285);
            this.UpdateAddBtn.Name = "UpdateAddBtn";
            this.UpdateAddBtn.Size = new System.Drawing.Size(77, 23);
            this.UpdateAddBtn.TabIndex = 55;
            this.UpdateAddBtn.Text = "Update selected";
            this.UpdateAddBtn.UseVisualStyleBackColor = true;
            this.UpdateAddBtn.Visible = false;
            this.UpdateAddBtn.Click += new System.EventHandler(this.UpdateAddBtn_Click);
            // 
            // ReloadDBtn
            // 
            this.ReloadDBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.ReloadDBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReloadDBtn.Location = new System.Drawing.Point(445, 28);
            this.ReloadDBtn.Name = "ReloadDBtn";
            this.ReloadDBtn.Size = new System.Drawing.Size(23, 23);
            this.ReloadDBtn.TabIndex = 54;
            this.ReloadDBtn.UseVisualStyleBackColor = true;
            this.ReloadDBtn.Visible = false;
            this.ReloadDBtn.Click += new System.EventHandler(this.ReloadDBtn_Click);
            // 
            // OpacityPnl
            // 
            this.OpacityPnl.Controls.Add(this.label5);
            this.OpacityPnl.Controls.Add(this.sldBrOpacity);
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
            // sldBrOpacity
            // 
            this.sldBrOpacity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sldBrOpacity.LargeChange = 25;
            this.sldBrOpacity.Location = new System.Drawing.Point(10, 16);
            this.sldBrOpacity.Maximum = 100;
            this.sldBrOpacity.Minimum = 10;
            this.sldBrOpacity.Name = "sldBrOpacity";
            this.sldBrOpacity.Size = new System.Drawing.Size(297, 45);
            this.sldBrOpacity.TabIndex = 25;
            this.sldBrOpacity.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.sldBrOpacity.Value = 100;
            this.sldBrOpacity.Scroll += new System.EventHandler(this.sldBrOpacity_Scroll);
            // 
            // InstalledMngRsr
            // 
            this.InstalledMngRsr.Controls.Add(this.AddBtn);
            this.InstalledMngRsr.Controls.Add(this.InstalledAddResizer);
            this.InstalledMngRsr.Controls.Add(this.UninstallBtn);
            this.InstalledMngRsr.Location = new System.Drawing.Point(12, 28);
            this.InstalledMngRsr.Name = "InstalledMngRsr";
            this.InstalledMngRsr.Size = new System.Drawing.Size(290, 281);
            this.InstalledMngRsr.TabIndex = 52;
            // 
            // InstalledAddResizer
            // 
            this.InstalledAddResizer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InstalledAddResizer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InstalledAddResizer.Controls.Add(this.ReloadIBtn);
            this.InstalledAddResizer.Controls.Add(this.InstalledAddLstBx);
            this.InstalledAddResizer.Location = new System.Drawing.Point(5, 0);
            this.InstalledAddResizer.Name = "InstalledAddResizer";
            this.InstalledAddResizer.Size = new System.Drawing.Size(281, 249);
            this.InstalledAddResizer.TabIndex = 40;
            // 
            // ReloadIBtn
            // 
            this.ReloadIBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.ReloadIBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReloadIBtn.Location = new System.Drawing.Point(258, -1);
            this.ReloadIBtn.Name = "ReloadIBtn";
            this.ReloadIBtn.Size = new System.Drawing.Size(23, 23);
            this.ReloadIBtn.TabIndex = 44;
            this.ReloadIBtn.UseVisualStyleBackColor = true;
            this.ReloadIBtn.Click += new System.EventHandler(this.ReloadIBtn_Click);
            // 
            // InstalledAddLstBx
            // 
            this.InstalledAddLstBx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InstalledAddLstBx.FormattingEnabled = true;
            this.InstalledAddLstBx.HorizontalScrollbar = true;
            this.InstalledAddLstBx.IntegralHeight = false;
            this.InstalledAddLstBx.Location = new System.Drawing.Point(-1, -1);
            this.InstalledAddLstBx.MultiColumn = true;
            this.InstalledAddLstBx.Name = "InstalledAddLstBx";
            this.InstalledAddLstBx.Size = new System.Drawing.Size(281, 249);
            this.InstalledAddLstBx.Sorted = true;
            this.InstalledAddLstBx.TabIndex = 35;
            this.InstalledAddLstBx.SelectedIndexChanged += new System.EventHandler(this.InstalledAddLstBx_SelectedIndexChanged);
            this.InstalledAddLstBx.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.InstalledAddLstBx_MouseDoubleClick);
            // 
            // UninstallBtn
            // 
            this.UninstallBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.UninstallBtn.Location = new System.Drawing.Point(147, 257);
            this.UninstallBtn.Name = "UninstallBtn";
            this.UninstallBtn.Size = new System.Drawing.Size(139, 23);
            this.UninstallBtn.TabIndex = 38;
            this.UninstallBtn.Text = "Uninstall selected";
            this.UninstallBtn.UseVisualStyleBackColor = true;
            this.UninstallBtn.Click += new System.EventHandler(this.UninstallBtn_Click);
            // 
            // AddBtn
            // 
            this.AddBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.AddBtn.Location = new System.Drawing.Point(3, 257);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(283, 23);
            this.AddBtn.TabIndex = 34;
            this.AddBtn.Text = "Manage translators";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // DownloadBtn
            // 
            this.DownloadBtn.Location = new System.Drawing.Point(311, 285);
            this.DownloadBtn.Name = "DownloadBtn";
            this.DownloadBtn.Size = new System.Drawing.Size(77, 23);
            this.DownloadBtn.TabIndex = 50;
            this.DownloadBtn.Text = "Download selected";
            this.DownloadBtn.UseVisualStyleBackColor = true;
            this.DownloadBtn.Visible = false;
            this.DownloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            // 
            // DownloadAddLstBx
            // 
            this.DownloadAddLstBx.FormattingEnabled = true;
            this.DownloadAddLstBx.HorizontalScrollbar = true;
            this.DownloadAddLstBx.Location = new System.Drawing.Point(311, 28);
            this.DownloadAddLstBx.MultiColumn = true;
            this.DownloadAddLstBx.Name = "DownloadAddLstBx";
            this.DownloadAddLstBx.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.DownloadAddLstBx.Size = new System.Drawing.Size(157, 251);
            this.DownloadAddLstBx.Sorted = true;
            this.DownloadAddLstBx.TabIndex = 49;
            this.DownloadAddLstBx.Visible = false;
            this.DownloadAddLstBx.SelectedIndexChanged += new System.EventHandler(this.DownloadAddLstBx_SelectedIndexChanged);
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
            this.toolStrip1.Size = new System.Drawing.Size(309, 25);
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
            this.ClientSize = new System.Drawing.Size(309, 371);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.UpdateAddBtn);
            this.Controls.Add(this.ReloadDBtn);
            this.Controls.Add(this.OpacityPnl);
            this.Controls.Add(this.InstalledMngRsr);
            this.Controls.Add(this.DownloadBtn);
            this.Controls.Add(this.DownloadAddLstBx);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TranslatorsFrm";
            this.Text = "Installed translators";
            this.Load += new System.EventHandler(this.TranslatorsFrm_Load);
            this.OpacityPnl.ResumeLayout(false);
            this.OpacityPnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).EndInit();
            this.InstalledMngRsr.ResumeLayout(false);
            this.InstalledAddResizer.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button UpdateAddBtn;
        private System.Windows.Forms.Button ReloadDBtn;
        private System.Windows.Forms.Panel OpacityPnl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar sldBrOpacity;
        private System.Windows.Forms.Panel InstalledMngRsr;
        private System.Windows.Forms.Panel InstalledAddResizer;
        private System.Windows.Forms.Button ReloadIBtn;
        private System.Windows.Forms.ListBox InstalledAddLstBx;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.Button UninstallBtn;
        private System.Windows.Forms.Button DownloadBtn;
        private System.Windows.Forms.ListBox DownloadAddLstBx;
        private System.Windows.Forms.ToolTip toolTipCntrl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton AboutTSB;
        private System.Windows.Forms.Timer timer1;
    }
}

