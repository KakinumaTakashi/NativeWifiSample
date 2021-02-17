using System;
using System.Windows.Forms;
using Wifi;

namespace NativeWifiSample
{
    public partial class Form1 : Form
    {
        private WifiController WifiController;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.TextBoxSSID.Text = @"";
            this.TextBoxKEY.Text = @"";


            this.WifiController = new WifiController();

            this.WifiController.OnConnected += OnConnected;
            this.WifiController.OnDisconnected += OnDisconnected;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.WifiController.Dispose();
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            this.TextBoxSSID.Enabled = false;
            this.TextBoxKEY.Enabled = false;
            this.ButtonConnect.Enabled = false;
            this.ButtonDisconnect.Enabled = false;

            try
            {
                this.WifiController.Connect(this.TextBoxSSID.Text, this.TextBoxKEY.Text, true);
            }
            catch (Exception)
            {
                this.TextBoxSSID.Enabled = true;
                this.TextBoxKEY.Enabled = true;
                this.ButtonConnect.Enabled = true;
                this.ButtonDisconnect.Enabled = true;
            }
        }

        private void ButtonDisconnect_Click(object sender, EventArgs e)
        {
            this.TextBoxSSID.Enabled = false;
            this.TextBoxKEY.Enabled = false;
            this.ButtonConnect.Enabled = false;
            this.ButtonDisconnect.Enabled = false;

            this.WifiController.Disconnect();
        }

        private void OnConnected(object sender, EventArgs e)
        {
            WifiController.WifiConnectionEventArgs _args = (WifiController.WifiConnectionEventArgs)e;
            System.Diagnostics.Debug.WriteLine(
                String.Format("[INFO] OnConnected called guid : {0} , ssid : {1}", ((Guid)sender).ToString(), _args.ssid));

            this.Invoke((MethodInvoker)(() =>
            {
                this.TextBoxSSID.Enabled = true;
                this.TextBoxKEY.Enabled = true;
                this.ButtonConnect.Enabled = true;
                this.ButtonDisconnect.Enabled = true;
            }));
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("[INFO] OnDisconnected called guid : {0}", ((Guid)sender).ToString()));

            this.Invoke((MethodInvoker)(() =>
            {
                this.TextBoxSSID.Enabled = true;
                this.TextBoxKEY.Enabled = true;
                this.ButtonConnect.Enabled = true;
                this.ButtonDisconnect.Enabled = true;
            }));
        }
    }
}
