using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;
 
namespace TCPClientApp
{
    public partial class Form1 : Form
    {
        TcpClient client = null;

        protected NetworkStream networkStream;

        protected StreamWriter streamOut;

        protected StreamReader streamIn;

        Thread readThread;
 
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (client == null || client.Connected == false)
            {
                try
                {
                    client = new TcpClient();  
                    client.Connect("127.0.0.1", 10001);
                    if (client.Connected)
                    {
                        System.Diagnostics.Debug.WriteLine("Connected");
                        label1.Text = "Connected";
                        networkStream = client.GetStream();

                        streamOut = new StreamWriter(networkStream, System.Text.Encoding.ASCII);
                        streamIn = new StreamReader(networkStream);
                        Start();
                    }
                }
                catch (Exception err)
                {
                    client = null;
                    System.Diagnostics.Debug.WriteLine("Error " + err.Message);
                    label1.Text = "Error " + err.Message;
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                try
                {
                    string sTemp = "CLIENT " + DateTime.Now.ToString();

                    streamOut.WriteLine(sTemp);
                    label1.Text = sTemp;
                    streamOut.Flush();
                }
                catch (Exception err)
                {

                    label1.Text = err.ToString(); 

                }
            }
        }

        public void Start()
        {
            ThreadStart threadStart1 = new ThreadStart(this.Read);

            readThread = new Thread(threadStart1);
            readThread.Name = "Read";

            readThread.Start();
        }

        bool bContinue = false;

        public delegate void LabelAdd(string temp);

        public void LabelAdd1(string temp)
        {
            label1.Text = temp;
        }
        public void LabelAdd2(string temp)
        {
            label2.Text = temp;
        }

        private void Read()
        { 
            bContinue = true;
            while (bContinue)
            {
                //
                //  Get the message and decode it.
                //
                String rawMessageString = "";

                try
                {
                   char[] array = new char[1024];
                    int nLength = streamIn.Read(array, 0, 1024);
                    if (nLength > 0)
                    {
                        rawMessageString = new string(array, 0, nLength);
                        System.Diagnostics.Debug.WriteLine(rawMessageString);
                        this.Invoke(new LabelAdd(this.LabelAdd2), new object[] { rawMessageString });

                    }
                    else
                    {
                        if (!client.Connected)
                        {
                            bContinue = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    bContinue = false;
                }
            }
        }


        public void CloseTCP()
        {
            if (client != null)
            {
                if (client.Connected)
                {
                    streamOut.Close();
                    streamIn.Close();
                    networkStream.Close();
                    client.Close();
                }
                client = null;
            }
            if (readThread != null)
            {
                if (bContinue == true)
                {
                    readThread.Abort();
                    readThread.Join();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CloseTCP();
        }

    }
}