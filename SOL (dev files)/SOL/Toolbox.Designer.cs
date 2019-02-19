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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Toolbox));
            this.GoToRename = new System.Windows.Forms.Button();
            this.GoToDTranslator = new System.Windows.Forms.Button();
            this.GoToCounter = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.sldBrOpacity = new System.Windows.Forms.TrackBar();
            this.toolTipCntrl = new System.Windows.Forms.ToolTip(this.components);
            this.aboutBtn = new System.Windows.Forms.Button();
            this.updatesBtn = new System.Windows.Forms.Button();
            this.AddBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).BeginInit();
            this.SuspendLayout();
            // 
            // GoToRename
            // 
            this.GoToRename.Location = new System.Drawing.Point(10, 91);
            this.GoToRename.Name = "GoToRename";
            this.GoToRename.Size = new System.Drawing.Size(283, 23);
            this.GoToRename.TabIndex = 31;
            this.GoToRename.Text = "Mass rename";
            this.toolTipCntrl.SetToolTip(this.GoToRename, "Rename multiple files of a folder based on a regular expression or one of the pre" +
        "-defined filters.");
            this.GoToRename.UseVisualStyleBackColor = true;
            this.GoToRename.Click += new System.EventHandler(this.GoToRename_Click);
            // 
            // GoToDTranslator
            // 
            this.GoToDTranslator.Location = new System.Drawing.Point(10, 62);
            this.GoToDTranslator.Name = "GoToDTranslator";
            this.GoToDTranslator.Size = new System.Drawing.Size(283, 23);
            this.GoToDTranslator.TabIndex = 29;
            this.GoToDTranslator.Text = "Discord Translator";
            this.toolTipCntrl.SetToolTip(this.GoToDTranslator, "Translate normal text to discord emote letters.");
            this.GoToDTranslator.UseVisualStyleBackColor = true;
            this.GoToDTranslator.Click += new System.EventHandler(this.GoToDTranslator_Click);
            // 
            // GoToCounter
            // 
            this.GoToCounter.Location = new System.Drawing.Point(10, 33);
            this.GoToCounter.Name = "GoToCounter";
            this.GoToCounter.Size = new System.Drawing.Size(283, 23);
            this.GoToCounter.TabIndex = 28;
            this.GoToCounter.Text = "Counter";
            this.toolTipCntrl.SetToolTip(this.GoToCounter, "Rather self explanitory; lets you count things.\r\ne.g. the amount of times Rythian" +
        " has raged or the amount of times someone has died in a game.");
            this.GoToCounter.UseVisualStyleBackColor = true;
            this.GoToCounter.Click += new System.EventHandler(this.GoToCounter_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "8Bit Shadows toolbox of \'usefull\' programs";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(131, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Opacity";
            // 
            // sldBrOpacity
            // 
            this.sldBrOpacity.LargeChange = 25;
            this.sldBrOpacity.Location = new System.Drawing.Point(10, 220);
            this.sldBrOpacity.Maximum = 100;
            this.sldBrOpacity.Minimum = 10;
            this.sldBrOpacity.Name = "sldBrOpacity";
            this.sldBrOpacity.Size = new System.Drawing.Size(283, 45);
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
            // aboutBtn
            // 
            this.aboutBtn.Location = new System.Drawing.Point(10, 178);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(288, 23);
            this.aboutBtn.TabIndex = 32;
            this.aboutBtn.Text = "About";
            this.aboutBtn.UseVisualStyleBackColor = true;
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // updatesBtn
            // 
            this.updatesBtn.Location = new System.Drawing.Point(10, 149);
            this.updatesBtn.Name = "updatesBtn";
            this.updatesBtn.Size = new System.Drawing.Size(288, 23);
            this.updatesBtn.TabIndex = 33;
            this.updatesBtn.Text = "Updates";
            this.updatesBtn.UseVisualStyleBackColor = true;
            this.updatesBtn.Click += new System.EventHandler(this.updatesBtn_Click);
            // 
            // AddBtn
            // 
            this.AddBtn.Enabled = false;
            this.AddBtn.Location = new System.Drawing.Point(10, 120);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(283, 23);
            this.AddBtn.TabIndex = 34;
            this.AddBtn.Text = "Additions";
            this.toolTipCntrl.SetToolTip(this.AddBtn, "Rename multiple files of a folder based on a regular expression or one of the pre" +
        "-defined filters.");
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // Toolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 257);
            this.Controls.Add(this.AddBtn);
            this.Controls.Add(this.updatesBtn);
            this.Controls.Add(this.GoToRename);
            this.Controls.Add(this.GoToDTranslator);
            this.Controls.Add(this.GoToCounter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sldBrOpacity);
            this.Controls.Add(this.aboutBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Toolbox";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Toolbox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sldBrOpacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GoToRename;
        private System.Windows.Forms.ToolTip toolTipCntrl;
        private System.Windows.Forms.Button GoToDTranslator;
        private System.Windows.Forms.Button GoToCounter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar sldBrOpacity;
        private System.Windows.Forms.Button aboutBtn;
        private System.Windows.Forms.Button updatesBtn;
        private System.Windows.Forms.Button AddBtn;
    }
}

