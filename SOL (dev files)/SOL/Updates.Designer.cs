namespace SOL
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
            this.listBoxAll = new System.Windows.Forms.ListBox();
            this.treeViewAll = new System.Windows.Forms.TreeView();
            this.tabControler = new System.Windows.Forms.TabControl();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.searchTxtBx = new System.Windows.Forms.TextBox();
            this.clearTextBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.PreviousBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxAll
            // 
            this.listBoxAll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxAll.FormattingEnabled = true;
            this.listBoxAll.Location = new System.Drawing.Point(2, 47);
            this.listBoxAll.Name = "listBoxAll";
            this.listBoxAll.Size = new System.Drawing.Size(418, 67);
            this.listBoxAll.TabIndex = 1;
            this.listBoxAll.SelectedIndexChanged += new System.EventHandler(this.ListBox1_SelectedIndexChanged);
            // 
            // treeViewAll
            // 
            this.treeViewAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.treeViewAll.FullRowSelect = true;
            this.treeViewAll.Location = new System.Drawing.Point(2, 113);
            this.treeViewAll.Name = "treeViewAll";
            this.treeViewAll.ShowLines = false;
            this.treeViewAll.Size = new System.Drawing.Size(418, 39);
            this.treeViewAll.TabIndex = 2;
            // 
            // tabControler
            // 
            this.tabControler.HotTrack = true;
            this.tabControler.Location = new System.Drawing.Point(1, 21);
            this.tabControler.Name = "tabControler";
            this.tabControler.SelectedIndex = 0;
            this.tabControler.Size = new System.Drawing.Size(421, 27);
            this.tabControler.TabIndex = 1;
            this.tabControler.SelectedIndexChanged += new System.EventHandler(this.TabControler_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(2, 1);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(418, 466);
            this.textBox1.TabIndex = 3;
            // 
            // searchTxtBx
            // 
            this.searchTxtBx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchTxtBx.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.searchTxtBx.Location = new System.Drawing.Point(1, 1);
            this.searchTxtBx.Name = "searchTxtBx";
            this.searchTxtBx.Size = new System.Drawing.Size(356, 20);
            this.searchTxtBx.TabIndex = 4;
            this.searchTxtBx.Text = "Search";
            this.searchTxtBx.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchTxtBx_MouseClick);
            this.searchTxtBx.TextChanged += new System.EventHandler(this.SearchTxtBx_TextChanged);
            this.searchTxtBx.Leave += new System.EventHandler(this.SearchTxtBx_Leave);
            // 
            // clearTextBtn
            // 
            this.clearTextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearTextBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.clearTextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearTextBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clearTextBtn.Location = new System.Drawing.Point(398, 1);
            this.clearTextBtn.Name = "clearTextBtn";
            this.clearTextBtn.Size = new System.Drawing.Size(22, 20);
            this.clearTextBtn.TabIndex = 47;
            this.clearTextBtn.UseVisualStyleBackColor = true;
            this.clearTextBtn.Click += new System.EventHandler(this.clearTextBtn_Click);
            // 
            // NextBtn
            // 
            this.NextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.NextBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.NextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NextBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.NextBtn.Location = new System.Drawing.Point(377, 1);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(22, 20);
            this.NextBtn.TabIndex = 48;
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // PreviousBtn
            // 
            this.PreviousBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PreviousBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.PreviousBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviousBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PreviousBtn.Location = new System.Drawing.Point(356, 1);
            this.PreviousBtn.Name = "PreviousBtn";
            this.PreviousBtn.Size = new System.Drawing.Size(22, 20);
            this.PreviousBtn.TabIndex = 49;
            this.PreviousBtn.UseVisualStyleBackColor = true;
            this.PreviousBtn.Click += new System.EventHandler(this.PreviousBtn_Click);
            // 
            // Updates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 470);
            this.Controls.Add(this.PreviousBtn);
            this.Controls.Add(this.NextBtn);
            this.Controls.Add(this.clearTextBtn);
            this.Controls.Add(this.searchTxtBx);
            this.Controls.Add(this.listBoxAll);
            this.Controls.Add(this.treeViewAll);
            this.Controls.Add(this.tabControler);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Updates";
            this.Text = "Updates";
            this.Load += new System.EventHandler(this.Updates_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView treeViewAll;
        private System.Windows.Forms.ListBox listBoxAll;
        private System.Windows.Forms.TabControl tabControler;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox searchTxtBx;
        private System.Windows.Forms.Button clearTextBtn;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.Button PreviousBtn;
    }
}