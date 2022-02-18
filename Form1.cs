using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace SerialPort
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cmdClose.Enabled = false;
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames()) 
            {
                txtPort.Items.Add(s);
            }
        }

        public System.IO.Ports.SerialPort sport;

        public void serialport_connect(String port, int baudrate , Parity parity, int databits, StopBits stopbits) 
        {
            DateTime dt = DateTime.Now;
            String dtn = dt.ToShortTimeString();

            sport = new System.IO.Ports.SerialPort(
            port, baudrate, parity, databits, stopbits);
            try
            {
                sport.Open();
                cmdClose.Enabled = true;
                cmdConnect.Enabled = false;
                txtReceive.AppendText("[" + dtn + "] " + "Connected\n");
                sport.DataReceived += new SerialDataReceivedEventHandler(sport_DataReceived);
            }
            catch (Exception e) { MessageBox.Show(e.ToString(), "Error"); }
        }

        private void sport_DataReceived(object sender, SerialDataReceivedEventArgs e) 
        {
            try
            {
                DateTime dt = DateTime.Now;
                String dtn = dt.ToString("HH:mm:ss");
                //txtReceive.AppendText("[" + dtn + "] " + "Received: " + sport.ReadExisting() + "\n");

                //List<string> input = new List<string>();
                //while (sport.BytesToRead != 0)
                //{
                //    input.Add(sport.ReadExisting());
                //}

                //string input = sport.ReadExisting();
                //receive hex data
                //string output = string.Join(",", input.Select(x => "0x" + x.ToString()).ToArray());

                //string dataReturned = "";
                //foreach (var s in input)
                //{
                //    dataReturned += s;
                //}

                //txtReceive.AppendText("[" + dtn + "] " + "Received: " + sport.ReadExisting() + "\n"); //read as string
                //txtReceive.AppendText("[" + dtn + "] " + "Received: " + dataReturned + "\n"); //read as string
                //txtReceive.AppendText(dtn + ";" + sport.ReadLine() + "\n");

                int length = sport.BytesToRead;
                byte[] buf = new byte[length];

                string bufferStr = "";

                List<string> bufferStrList = new List<string>();

                sport.Read(buf, 0, length);

                //bufferStr += System.Text.Encoding.Default.GetString(buf, 0, length);

                foreach (byte b in buf)
                {
                    bufferStr += b.ToString();
                    bufferStrList.Add(b.ToString());
                }
                //txtReceive.AppendText("[" + dtn + "] " + "Received: " + System.Text.Encoding.Default.GetString(buf, 0, buf.Length) + "\n"); //read as string
                //txtReceive.AppendText("[" + dtn + "] " + "Received: " + length + "\n"); //read as string
                //txtReceive.AppendText("[" + dtn + "] " + "Received: " + bufferStr + "\n"); //read as string
                foreach (string s in bufferStrList)
                {
                    txtReceive.AppendText("/" + s + "\n");
                }

                ///////////concatenar zeros e retorno de cada byte para converter para HEX
                //1 - 01
                //3 - 03
                //4 - 04
                //0 - 00
                //210 - D2
                //0 - 00
                //0 - 00
                //90 - 5A
                //10 - 0A

            }
            catch (Exception) { return; } //necessary for when the port is closed in the middle of ReadLine() operation
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            try
            {
                String port = txtPort.Text;
                int baudrate = Convert.ToInt32(cmbbaudrate.Text);
                Parity parity = (Parity)Enum.Parse(typeof(Parity), cmbparity.Text);
                int databits = Convert.ToInt32(cmbdatabits.Text);
                StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), cmbstopbits.Text);

                serialport_connect(port, baudrate, parity, databits, stopbits);
            }
            catch (Exception) { MessageBox.Show("Algum parâmetro de conexão está incorreto!", "Erro!"); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            String dtn = dt.ToShortTimeString();
            String data = txtDatatoSend.Text;
            //sport.Write(data);
            byte[] bytes = data.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray(); //send as hex
            sport.Write(bytes, 0, bytes.Length); //send as hex
            txtReceive.AppendText("[" + dtn + "] " + "Sent: " + data + "\n");
        }

        private void cmdClose_Click_1(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            String dtn = dt.ToShortTimeString();

            if (sport.IsOpen) 
            {
                sport.Close();
                cmdClose.Enabled = false;
                cmdConnect.Enabled = true;
                txtReceive.AppendText("[" + dtn + "] " + "Disconnected\n");
            }
        }
    }
}
