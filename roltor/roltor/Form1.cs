using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RoltorForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        RoltorLib.Roltor myRoltor = new RoltorLib.Roltor();

        private void buttonRTDConnect_Click(object sender, EventArgs e)
        {
            if (myRoltor.Connect())
            {
                this.buttonRTDConnect.Enabled = false;
                this.buttonRTDStart.Enabled = true;
                this.buttonRTDClose.Enabled = true;
                string sTemp = "CONNECT " + DateTime.Now.ToString();
                label1.Text = sTemp;
                System.Diagnostics.Debug.WriteLine(sTemp);
            }
        }

        private void buttonRTDStart_Click(object sender, EventArgs e)
        {
            if (myRoltor.Start())
            {
                this.buttonRTDStart.Enabled = false;
                this.buttonRTDStop.Enabled = true;
                string sTemp = "START " + DateTime.Now.ToString();
                label1.Text = sTemp;
                System.Diagnostics.Debug.WriteLine(sTemp);
            }
        }

        private void buttonRTDStop_Click(object sender, EventArgs e)
        {
            if (myRoltor.Stop())
            {
                this.buttonRTDStart.Enabled = true;
                this.buttonRTDStop.Enabled = false;
                string sTemp = "STOP " + DateTime.Now.ToString();
                label1.Text = sTemp;
                System.Diagnostics.Debug.WriteLine(sTemp);
            }
        }

        private void buttonRTDClose_Click(object sender, EventArgs e)
        {
            if(this.buttonRTDStop.Enabled)
                buttonRTDStop_Click(sender, e);

            if (myRoltor.Close())
            {
                this.buttonRTDConnect.Enabled = true;
                this.buttonRTDStart.Enabled = false;
                this.buttonRTDStop.Enabled = false;
                this.buttonRTDClose.Enabled = false;
                string sTemp = "CLOSE " + DateTime.Now.ToString();
                label1.Text = sTemp;
                System.Diagnostics.Debug.WriteLine(sTemp);
            }
        }

    }
}