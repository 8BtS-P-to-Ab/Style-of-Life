namespace SOL
{
    partial class Toolbox
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
            this.label5 = new System.Windows.Forms.Label();
            this.sldBrOpacity = new System.Windows.Forms.TrackBar();
            this.toolTipCntrl = new System.Windows.Forms.ToolTip(this.components);
            this.AddBtn = new System.Windows.Forms.Button();
            this.DownloadBtn = new System.Windows.Forms.Button();
            this.UninstallBtn = new System.Windows.Forms.Button();
            this.ForceUpdateBtn = new System.Windows.Forms.Button();
            this.ReloadDBtn = new System.Windows.Forms.Button();
            this.ReloadIBtn = new System.Windows.Forms.Button();
            this.aboutBtn = new System.Windows.Forms.Button();
            this.updatesBtn = new System.Windows.Forms.Button();
            this.InstalledAddLstBx = new System.Windows.Forms.ListBox();
            this.DownloadAddLstBx = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.InstalledAddResizer = new System.Windows.Forms.Panel();
            this.InstalledMngRsr = new System.Windows.Forms.Panel();
            this.OpacityPnl = new System.Windows.Forms.Panel();
            this.UpdateAddBtn = new System.Windows.Forms.Button();
            this.searchTxtBx = new System.Windows.Forms.TextBox();
            this.clearTextBtn = new System.Windows.Forms.Button();
            this.clearDwnBtn = new System.Windows.Forms.Button();
            this.downloadSearchTxtBx = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).BeginInit();
            this.InstalledAddResizer.SuspendLayout();
            this.InstalledMngRsr.SuspendLayout();
            this.OpacityPnl.SuspendLayout();
            this.SuspendLayout();
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
            // toolTipCntrl
            // 
            this.toolTipCntrl.AutoPopDelay = 15000;
            this.toolTipCntrl.InitialDelay = 500;
            this.toolTipCntrl.ReshowDelay = 100;
            // 
            // AddBtn
            // 
            this.AddBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.AddBtn.Location = new System.Drawing.Point(3, 187);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(283, 23);
            this.AddBtn.TabIndex = 34;
            this.AddBtn.Text = "Manage additions";
            this.toolTipCntrl.SetToolTip(this.AddBtn, "Enter/exit additions management mode");
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // DownloadBtn
            // 
            this.DownloadBtn.Location = new System.Drawing.Point(306, 283);
            this.DownloadBtn.Name = "DownloadBtn";
            this.DownloadBtn.Size = new System.Drawing.Size(77, 23);
            this.DownloadBtn.TabIndex = 37;
            this.DownloadBtn.Text = "Download selected";
            this.toolTipCntrl.SetToolTip(this.DownloadBtn, "Download the selected addtion from the github server");
            this.DownloadBtn.UseVisualStyleBackColor = true;
            this.DownloadBtn.Visible = false;
            this.DownloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            // 
            // UninstallBtn
            // 
            this.UninstallBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.UninstallBtn.Location = new System.Drawing.Point(147, 187);
            this.UninstallBtn.Name = "UninstallBtn";
            this.UninstallBtn.Size = new System.Drawing.Size(139, 23);
            this.UninstallBtn.TabIndex = 38;
            this.UninstallBtn.Text = "Uninstall selected";
            this.toolTipCntrl.SetToolTip(this.UninstallBtn, "Remove the currently selected addition from your machine");
            this.UninstallBtn.UseVisualStyleBackColor = true;
            this.UninstallBtn.Click += new System.EventHandler(this.RemoveBtn_Click);
            // 
            // ForceUpdateBtn
            // 
            this.ForceUpdateBtn.Location = new System.Drawing.Point(152, 283);
            this.ForceUpdateBtn.Name = "ForceUpdateBtn";
            this.ForceUpdateBtn.Size = new System.Drawing.Size(141, 23);
            this.ForceUpdateBtn.TabIndex = 39;
            this.ForceUpdateBtn.Text = "Force Update (WIP)";
            this.toolTipCntrl.SetToolTip(this.ForceUpdateBtn, "Force SOL to check for any updates");
            this.ForceUpdateBtn.UseVisualStyleBackColor = true;
            this.ForceUpdateBtn.Visible = false;
            // 
            // ReloadDBtn
            // 
            this.ReloadDBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ReloadDBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.ReloadDBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReloadDBtn.Location = new System.Drawing.Point(440, 38);
            this.ReloadDBtn.Name = "ReloadDBtn";
            this.ReloadDBtn.Size = new System.Drawing.Size(23, 23);
            this.ReloadDBtn.TabIndex = 43;
            this.toolTipCntrl.SetToolTip(this.ReloadDBtn, "Refresh the additions store");
            this.ReloadDBtn.UseVisualStyleBackColor = true;
            this.ReloadDBtn.Visible = false;
            this.ReloadDBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // ReloadIBtn
            // 
            this.ReloadIBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ReloadIBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.ReloadIBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReloadIBtn.Location = new System.Drawing.Point(258, -1);
            this.ReloadIBtn.Name = "ReloadIBtn";
            this.ReloadIBtn.Size = new System.Drawing.Size(23, 23);
            this.ReloadIBtn.TabIndex = 44;
            this.toolTipCntrl.SetToolTip(this.ReloadIBtn, "Refresh the additions browser");
            this.ReloadIBtn.UseVisualStyleBackColor = true;
            this.ReloadIBtn.Click += new System.EventHandler(this.ReloadIBtn_Click);
            // 
            // aboutBtn
            // 
            this.aboutBtn.Location = new System.Drawing.Point(10, 283);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(141, 23);
            this.aboutBtn.TabIndex = 32;
            this.aboutBtn.Text = "About";
            this.aboutBtn.UseVisualStyleBackColor = true;
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // updatesBtn
            // 
            this.updatesBtn.Location = new System.Drawing.Point(10, 254);
            this.updatesBtn.Name = "updatesBtn";
            this.updatesBtn.Size = new System.Drawing.Size(283, 23);
            this.updatesBtn.TabIndex = 33;
            this.updatesBtn.Text = "Updates";
            this.updatesBtn.UseVisualStyleBackColor = true;
            this.updatesBtn.Click += new System.EventHandler(this.updatesBtn_Click);
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
            this.InstalledAddLstBx.Size = new System.Drawing.Size(281, 179);
            this.InstalledAddLstBx.Sorted = true;
            this.InstalledAddLstBx.TabIndex = 35;
            this.InstalledAddLstBx.SelectedIndexChanged += new System.EventHandler(this.ProgramsLstBx_SelectedIndexChanged);
            this.InstalledAddLstBx.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.InstalledAddLstBx_MouseDoubleClick);
            // 
            // DownloadAddLstBx
            // 
            this.DownloadAddLstBx.FormattingEnabled = true;
            this.DownloadAddLstBx.HorizontalScrollbar = true;
            this.DownloadAddLstBx.Location = new System.Drawing.Point(306, 38);
            this.DownloadAddLstBx.MultiColumn = true;
            this.DownloadAddLstBx.Name = "DownloadAddLstBx";
            this.DownloadAddLstBx.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.DownloadAddLstBx.Size = new System.Drawing.Size(157, 238);
            this.DownloadAddLstBx.Sorted = true;
            this.DownloadAddLstBx.TabIndex = 36;
            this.DownloadAddLstBx.Visible = false;
            this.DownloadAddLstBx.SelectedIndexChanged += new System.EventHandler(this.DownloadAddLstBx_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 6;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
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
            this.InstalledAddResizer.Size = new System.Drawing.Size(281, 179);
            this.InstalledAddResizer.TabIndex = 40;
            // 
            // InstalledMngRsr
            // 
            this.InstalledMngRsr.Controls.Add(this.AddBtn);
            this.InstalledMngRsr.Controls.Add(this.InstalledAddResizer);
            this.InstalledMngRsr.Controls.Add(this.UninstallBtn);
            this.InstalledMngRsr.Location = new System.Drawing.Point(7, 38);
            this.InstalledMngRsr.Name = "InstalledMngRsr";
            this.InstalledMngRsr.Size = new System.Drawing.Size(290, 211);
            this.InstalledMngRsr.TabIndex = 41;
            // 
            // OpacityPnl
            // 
            this.OpacityPnl.Controls.Add(this.label5);
            this.OpacityPnl.Controls.Add(this.sldBrOpacity);
            this.OpacityPnl.Location = new System.Drawing.Point(-2, 312);
            this.OpacityPnl.Name = "OpacityPnl";
            this.OpacityPnl.Size = new System.Drawing.Size(309, 58);
            this.OpacityPnl.TabIndex = 42;
            // 
            // UpdateAddBtn
            // 
            this.UpdateAddBtn.Location = new System.Drawing.Point(386, 283);
            this.UpdateAddBtn.Name = "UpdateAddBtn";
            this.UpdateAddBtn.Size = new System.Drawing.Size(77, 23);
            this.UpdateAddBtn.TabIndex = 44;
            this.UpdateAddBtn.Text = "Update selected";
            this.UpdateAddBtn.UseVisualStyleBackColor = true;
            this.UpdateAddBtn.Visible = false;
            this.UpdateAddBtn.Click += new System.EventHandler(this.UpdateAddBtn_Click);
            // 
            // searchTxtBx
            // 
            this.searchTxtBx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchTxtBx.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.searchTxtBx.Location = new System.Drawing.Point(12, 12);
            this.searchTxtBx.Name = "searchTxtBx";
            this.searchTxtBx.Size = new System.Drawing.Size(281, 20);
            this.searchTxtBx.TabIndex = 45;
            this.searchTxtBx.Text = "Search";
            this.searchTxtBx.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox1_MouseClick);
            this.searchTxtBx.TextChanged += new System.EventHandler(this.searchTxtBx_TextChanged);
            this.searchTxtBx.Leave += new System.EventHandler(this.searchTxtBx_Leave);
            // 
            // clearTextBtn
            // 
            this.clearTextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearTextBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.clearTextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearTextBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clearTextBtn.Location = new System.Drawing.Point(271, 12);
            this.clearTextBtn.Name = "clearTextBtn";
            this.clearTextBtn.Size = new System.Drawing.Size(22, 20);
            this.clearTextBtn.TabIndex = 46;
            this.clearTextBtn.UseVisualStyleBackColor = true;
            this.clearTextBtn.Click += new System.EventHandler(this.clearTextBtn_Click);
            // 
            // clearDwnBtn
            // 
            this.clearDwnBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearDwnBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.clearDwnBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearDwnBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clearDwnBtn.Location = new System.Drawing.Point(440, 12);
            this.clearDwnBtn.Name = "clearDwnBtn";
            this.clearDwnBtn.Size = new System.Drawing.Size(23, 20);
            this.clearDwnBtn.TabIndex = 48;
            this.clearDwnBtn.UseVisualStyleBackColor = true;
            this.clearDwnBtn.Click += new System.EventHandler(this.clearDwnBtn_Click);
            // 
            // downloadSearchTxtBx
            // 
            this.downloadSearchTxtBx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadSearchTxtBx.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.downloadSearchTxtBx.Location = new System.Drawing.Point(306, 12);
            this.downloadSearchTxtBx.Name = "downloadSearchTxtBx";
            this.downloadSearchTxtBx.Size = new System.Drawing.Size(157, 20);
            this.downloadSearchTxtBx.TabIndex = 47;
            this.downloadSearchTxtBx.Text = "Search";
            this.downloadSearchTxtBx.Visible = false;
            this.downloadSearchTxtBx.MouseClick += new System.Windows.Forms.MouseEventHandler(this.downloadTxtBx_MouseClick);
            this.downloadSearchTxtBx.TextChanged += new System.EventHandler(this.downloadTxtBx_TextChanged);
            this.downloadSearchTxtBx.Leave += new System.EventHandler(this.downloadTxtBx_Leave);
            // 
            // Toolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 365);
            this.Controls.Add(this.clearDwnBtn);
            this.Controls.Add(this.downloadSearchTxtBx);
            this.Controls.Add(this.clearTextBtn);
            this.Controls.Add(this.searchTxtBx);
            this.Controls.Add(this.UpdateAddBtn);
            this.Controls.Add(this.ReloadDBtn);
            this.Controls.Add(this.OpacityPnl);
            this.Controls.Add(this.InstalledMngRsr);
            this.Controls.Add(this.ForceUpdateBtn);
            this.Controls.Add(this.DownloadBtn);
            this.Controls.Add(this.DownloadAddLstBx);
            this.Controls.Add(this.updatesBtn);
            this.Controls.Add(this.aboutBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Toolbox";
            this.Text = "Toolbox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Toolbox_FormClosing);
            this.Load += new System.EventHandler(this.Toolbox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).EndInit();
            this.InstalledAddResizer.ResumeLayout(false);
            this.InstalledMngRsr.ResumeLayout(false);
            this.OpacityPnl.ResumeLayout(false);
            this.OpacityPnl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTipCntrl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar sldBrOpacity;
        private System.Windows.Forms.Button aboutBtn;
        private System.Windows.Forms.Button updatesBtn;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.ListBox InstalledAddLstBx;
        private System.Windows.Forms.ListBox DownloadAddLstBx;
        private System.Windows.Forms.Button DownloadBtn;
        private System.Windows.Forms.Button UninstallBtn;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button ForceUpdateBtn;
        private System.Windows.Forms.Panel InstalledAddResizer;
        private System.Windows.Forms.Panel InstalledMngRsr;
        private System.Windows.Forms.Panel OpacityPnl;
        private System.Windows.Forms.Button ReloadDBtn;
        private System.Windows.Forms.Button ReloadIBtn;
        private System.Windows.Forms.Button UpdateAddBtn;
        private System.Windows.Forms.TextBox searchTxtBx;
        private System.Windows.Forms.Button clearTextBtn;
        private System.Windows.Forms.Button clearDwnBtn;
        private System.Windows.Forms.TextBox downloadSearchTxtBx;
    }
}

