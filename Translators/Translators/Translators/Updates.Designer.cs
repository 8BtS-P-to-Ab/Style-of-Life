namespace Translators
{
    partial class Updates
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
            this.PreviousBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.clearTextBtn = new System.Windows.Forms.Button();
            this.searchTxtBx = new System.Windows.Forms.TextBox();
            this.listBoxAll = new System.Windows.Forms.ListBox();
            this.treeViewAll = new System.Windows.Forms.TreeView();
            this.tabControler = new System.Windows.Forms.TabControl();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // PreviousBtn
            // 
            this.PreviousBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PreviousBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.PreviousBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviousBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PreviousBtn.Location = new System.Drawing.Point(357, 2);
            this.PreviousBtn.Name = "PreviousBtn";
            this.PreviousBtn.Size = new System.Drawing.Size(22, 20);
            this.PreviousBtn.TabIndex = 57;
            this.PreviousBtn.UseVisualStyleBackColor = true;
            this.PreviousBtn.Click += new System.EventHandler(this.PreviousBtn_Click);
            // 
            // NextBtn
            // 
            this.NextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.NextBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.NextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NextBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.NextBtn.Location = new System.Drawing.Point(378, 2);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(22, 20);
            this.NextBtn.TabIndex = 56;
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // clearTextBtn
            // 
            this.clearTextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearTextBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.clearTextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearTextBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clearTextBtn.Location = new System.Drawing.Point(399, 2);
            this.clearTextBtn.Name = "clearTextBtn";
            this.clearTextBtn.Size = new System.Drawing.Size(22, 20);
            this.clearTextBtn.TabIndex = 55;
            this.clearTextBtn.UseVisualStyleBackColor = true;
            this.clearTextBtn.Click += new System.EventHandler(this.clearTextBtn_Click);
            // 
            // searchTxtBx
            // 
            this.searchTxtBx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchTxtBx.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.searchTxtBx.Location = new System.Drawing.Point(2, 2);
            this.searchTxtBx.Name = "searchTxtBx";
            this.searchTxtBx.Size = new System.Drawing.Size(356, 20);
            this.searchTxtBx.TabIndex = 54;
            this.searchTxtBx.Text = "Search";
            this.searchTxtBx.MouseClick += new System.Windows.Forms.MouseEventHandler(this.searchTxtBx_MouseClick);
            this.searchTxtBx.TextChanged += new System.EventHandler(this.searchTxtBx_TextChanged);
            this.searchTxtBx.Leave += new System.EventHandler(this.searchTxtBx_Leave);
            // 
            // listBoxAll
            // 
            this.listBoxAll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxAll.FormattingEnabled = true;
            this.listBoxAll.Location = new System.Drawing.Point(3, 48);
            this.listBoxAll.Name = "listBoxAll";
            this.listBoxAll.Size = new System.Drawing.Size(418, 67);
            this.listBoxAll.TabIndex = 50;
            this.listBoxAll.SelectedIndexChanged += new System.EventHandler(this.listBoxAll_SelectedIndexChanged);
            // 
            // treeViewAll
            // 
            this.treeViewAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.treeViewAll.Location = new System.Drawing.Point(3, 114);
            this.treeViewAll.Name = "treeViewAll";
            this.treeViewAll.ShowLines = false;
            this.treeViewAll.Size = new System.Drawing.Size(418, 14);
            this.treeViewAll.TabIndex = 52;
            // 
            // tabControler
            // 
            this.tabControler.HotTrack = true;
            this.tabControler.Location = new System.Drawing.Point(2, 22);
            this.tabControler.Name = "tabControler";
            this.tabControler.SelectedIndex = 0;
            this.tabControler.Size = new System.Drawing.Size(421, 27);
            this.tabControler.TabIndex = 51;
            this.tabControler.SelectedIndexChanged += new System.EventHandler(this.TabControler_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 2);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(418, 466);
            this.textBox1.TabIndex = 53;
            // 
            // Updates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 471);
            this.Controls.Add(this.PreviousBtn);
            this.Controls.Add(this.NextBtn);
            this.Controls.Add(this.clearTextBtn);
            this.Controls.Add(this.searchTxtBx);
            this.Controls.Add(this.listBoxAll);
            this.Controls.Add(this.treeViewAll);
            this.Controls.Add(this.tabControler);
            this.Controls.Add(this.textBox1);
            this.Name = "Updates";
            this.Text = "Updates";
            this.Load += new System.EventHandler(this.Updates_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button PreviousBtn;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.Button clearTextBtn;
        private System.Windows.Forms.TextBox searchTxtBx;
        private System.Windows.Forms.ListBox listBoxAll;
        private System.Windows.Forms.TreeView treeViewAll;
        private System.Windows.Forms.TabControl tabControler;
        private System.Windows.Forms.TextBox textBox1;
    }
}