namespace RoltorForm
{
    partial class Form1
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
            this.buttonRTDConnect = new System.Windows.Forms.Button();
            this.buttonRTDStart = new System.Windows.Forms.Button();
            this.buttonRTDStop = new System.Windows.Forms.Button();
            this.buttonRTDClose = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonRTDConnect
            // 
            this.buttonRTDConnect.Location = new System.Drawing.Point(5, 21);
            this.buttonRTDConnect.Name = "buttonRTDConnect";
            this.buttonRTDConnect.Size = new System.Drawing.Size(57, 31);
            this.buttonRTDConnect.TabIndex = 1;
            this.buttonRTDConnect.Text = "Connect";
            this.buttonRTDConnect.UseVisualStyleBackColor = true;
            this.buttonRTDConnect.Click += new System.EventHandler(this.buttonRTDConnect_Click);
            // 
            // buttonRTDStart
            // 
            this.buttonRTDStart.Enabled = false;
            this.buttonRTDStart.Location = new System.Drawing.Point(68, 20);
            this.buttonRTDStart.Name = "buttonRTDStart";
            this.buttonRTDStart.Size = new System.Drawing.Size(58, 32);
            this.buttonRTDStart.TabIndex = 3;
            this.buttonRTDStart.Text = "Start";
            this.buttonRTDStart.UseVisualStyleBackColor = true;
            this.buttonRTDStart.Click += new System.EventHandler(this.buttonRTDStart_Click);
            // 
            // buttonRTDStop
            // 
            this.buttonRTDStop.Enabled = false;
            this.buttonRTDStop.Location = new System.Drawing.Point(132, 20);
            this.buttonRTDStop.Name = "buttonRTDStop";
            this.buttonRTDStop.Size = new System.Drawing.Size(60, 32);
            this.buttonRTDStop.TabIndex = 4;
            this.buttonRTDStop.Text = "Stop";
            this.buttonRTDStop.UseVisualStyleBackColor = true;
            this.buttonRTDStop.Click += new System.EventHandler(this.buttonRTDStop_Click);
            // 
            // buttonRTDClose
            // 
            this.buttonRTDClose.Enabled = false;
            this.buttonRTDClose.Location = new System.Drawing.Point(198, 20);
            this.buttonRTDClose.Name = "buttonRTDClose";
            this.buttonRTDClose.Size = new System.Drawing.Size(62, 32);
            this.buttonRTDClose.TabIndex = 5;
            this.buttonRTDClose.Text = "Close";
            this.buttonRTDClose.UseVisualStyleBackColor = true;
            this.buttonRTDClose.Click += new System.EventHandler(this.buttonRTDClose_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 147);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(837, 273);
            this.textBox1.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 432);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonRTDClose);
            this.Controls.Add(this.buttonRTDStop);
            this.Controls.Add(this.buttonRTDStart);
            this.Controls.Add(this.buttonRTDConnect);
            this.Name = "Form1";
            this.Text = "roltor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRTDConnect;
        private System.Windows.Forms.Button buttonRTDStart;
        private System.Windows.Forms.Button buttonRTDStop;
        private System.Windows.Forms.Button buttonRTDClose;
        private System.Windows.Forms.TextBox textBox1;

    }
}

