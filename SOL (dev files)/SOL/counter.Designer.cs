namespace SOL
{
    partial class Counter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Counter));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.aboutBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.label5 = new System.Windows.Forms.Label();
            this.sldBrOpacity = new System.Windows.Forms.TrackBar();
            this.btnRemoveCounter = new System.Windows.Forms.Button();
            this.txtBxCounterName = new System.Windows.Forms.TextBox();
            this.btnNewCounter = new System.Windows.Forms.Button();
            this.lstBxCounterSelector = new System.Windows.Forms.ListBox();
            this.btnMinus = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBxCounter = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(354, 25);
            this.toolStrip1.TabIndex = 28;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutBtn});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(45, 22);
            this.toolStripDropDownButton2.Text = "Help";
            // 
            // aboutBtn
            // 
            this.aboutBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(180, 22);
            this.aboutBtn.Text = "About";
            this.aboutBtn.ToolTipText = "About SOL\'s mass renamer form";
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(160, 308);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Opacity";
            // 
            // sldBrOpacity
            // 
            this.sldBrOpacity.LargeChange = 25;
            this.sldBrOpacity.Location = new System.Drawing.Point(34, 324);
            this.sldBrOpacity.Maximum = 100;
            this.sldBrOpacity.Minimum = 10;
            this.sldBrOpacity.Name = "sldBrOpacity";
            this.sldBrOpacity.Size = new System.Drawing.Size(283, 45);
            this.sldBrOpacity.TabIndex = 26;
            this.sldBrOpacity.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.sldBrOpacity.Value = 100;
            this.sldBrOpacity.Scroll += new System.EventHandler(this.sldBrOpacity_Scroll);
            // 
            // btnRemoveCounter
            // 
            this.btnRemoveCounter.Location = new System.Drawing.Point(209, 223);
            this.btnRemoveCounter.Name = "btnRemoveCounter";
            this.btnRemoveCounter.Size = new System.Drawing.Size(100, 23);
            this.btnRemoveCounter.TabIndex = 25;
            this.btnRemoveCounter.Text = "Remove Counter";
            this.btnRemoveCounter.UseVisualStyleBackColor = true;
            this.btnRemoveCounter.Click += new System.EventHandler(this.btnRemoveCounter_Click_1);
            // 
            // txtBxCounterName
            // 
            this.txtBxCounterName.Location = new System.Drawing.Point(34, 28);
            this.txtBxCounterName.Name = "txtBxCounterName";
            this.txtBxCounterName.Size = new System.Drawing.Size(169, 20);
            this.txtBxCounterName.TabIndex = 24;
            // 
            // btnNewCounter
            // 
            this.btnNewCounter.Location = new System.Drawing.Point(209, 26);
            this.btnNewCounter.Name = "btnNewCounter";
            this.btnNewCounter.Size = new System.Drawing.Size(100, 23);
            this.btnNewCounter.TabIndex = 23;
            this.btnNewCounter.Text = "New Counter";
            this.btnNewCounter.UseVisualStyleBackColor = true;
            this.btnNewCounter.Click += new System.EventHandler(this.btnNewCounter_Click);
            // 
            // lstBxCounterSelector
            // 
            this.lstBxCounterSelector.FormattingEnabled = true;
            this.lstBxCounterSelector.Location = new System.Drawing.Point(34, 57);
            this.lstBxCounterSelector.Name = "lstBxCounterSelector";
            this.lstBxCounterSelector.Size = new System.Drawing.Size(275, 160);
            this.lstBxCounterSelector.TabIndex = 22;
            this.lstBxCounterSelector.SelectedIndexChanged += new System.EventHandler(this.lstBxCounterSelector_SelectedIndexChanged);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(119, 266);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(75, 23);
            this.btnMinus.TabIndex = 21;
            this.btnMinus.Text = "Minus";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 224);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Counter:";
            // 
            // txtBxCounter
            // 
            this.txtBxCounter.Location = new System.Drawing.Point(60, 240);
            this.txtBxCounter.Name = "txtBxCounter";
            this.txtBxCounter.ReadOnly = true;
            this.txtBxCounter.Size = new System.Drawing.Size(100, 20);
            this.txtBxCounter.TabIndex = 19;
            this.txtBxCounter.TabStop = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(38, 266);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 18;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // Counter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 381);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sldBrOpacity);
            this.Controls.Add(this.btnRemoveCounter);
            this.Controls.Add(this.txtBxCounterName);
            this.Controls.Add(this.btnNewCounter);
            this.Controls.Add(this.lstBxCounterSelector);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBxCounter);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Counter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Counter";
            this.Load += new System.EventHandler(this.counter_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem aboutBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar sldBrOpacity;
        private System.Windows.Forms.Button btnRemoveCounter;
        private System.Windows.Forms.TextBox txtBxCounterName;
        private System.Windows.Forms.Button btnNewCounter;
        private System.Windows.Forms.ListBox lstBxCounterSelector;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBxCounter;
        private System.Windows.Forms.Button btnAdd;
    }
}