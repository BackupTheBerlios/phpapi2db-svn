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
using System.Net;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        TcpListener listener = null;
        
        public Form1()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            listener = new TcpListener(localAddr,10001);

            InitializeComponent();
        }

        Socket sockToClient = null;

        bool bStarted = false;

        private void button2_Click(object sender, EventArgs e)
        {
            if (bStarted == false)
            {
                try
                {
                    listener.Start();
                    bStarted = true;

                    Thread start = new Thread(new ThreadStart(this.WaitConnection));
                    start.Start(); 
                }
                catch(Exception )
                {}
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bStarted)
            {
                if (listenerThread != null)
                {
                    streamOut.Close();
                    streamIn.Close();
                    networkStream.Close();
                }
                if (sockToClient != null)
                {
                    if (sockToClient.Connected)
                    {
                        sockToClient.Close();
                    }
                }
            }
            bStarted = false;
            if (listenerThread != null)
            {
                listenerThread.Abort();
            }
            listener.Stop();
        }

        Thread listenerThread = null;

        protected NetworkStream networkStream;

        protected StreamWriter streamOut;

        protected StreamReader streamIn;


        private void WaitConnection()
        {
            sockToClient = listener.AcceptSocket();
            
            this.Invoke(new LabelAdd(this.LabelAdd1),new object[] {"Connection Recieved"});

            if (sockToClient.Connected)
            {
                networkStream = new NetworkStream(sockToClient);

                streamOut = new StreamWriter(networkStream, System.Text.Encoding.ASCII);
                streamIn = new StreamReader(networkStream);

                listenerThread = new Thread(new ThreadStart(this.listen));
                listenerThread.Start(); 
            }
        }

        public delegate void LabelAdd(string temp);

        public void LabelAdd1(string temp)
        {
            label1.Text = temp;
        }
        public void LabelAdd2(string temp)
        {
            label2.Text = temp;
        }

        private void listen()
        {
            bStarted = true;
            while (bStarted)
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
                        this.Invoke(new LabelAdd(this.LabelAdd1), new object[] { rawMessageString });
                        Thread.Sleep(2000);
                        string sTemp = "SERVER" + DateTime.Now.ToString() + "[" + rawMessageString + "]";
                        streamOut.WriteLine(sTemp);
                        this.Invoke(new LabelAdd(this.LabelAdd2), new object[] { sTemp });
                        streamOut.Flush();
                    }
                    else
                    {
                        bStarted = false;
                    }
                }
                catch (Exception e)
                {
                    bStarted = false;

                    //this.Invoke(new LabelAdd(this.LabelAdd1), new object[] { e.ToString() });
                        
                    //this.Invoke(new LabelAdd(this.LabelAdd2), new object[] { " " });
                       
                    listener.Stop();
                }
            }
        }



    }
}