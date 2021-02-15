using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Wifi;

namespace NativeWifiSample
{
    public partial class Form1 : Form
    {
        private IntPtr handle = IntPtr.Zero;
        Guid InterfaceGuid;
        //Guid ActiveInterfaceGuid;

        WifiController WifiController;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.TextBoxSSID.Text = @"";
            this.TextBoxKEY.Text = @"";

            //uint _version;

            //if (NativeWifiAPI.WlanOpenHandle(2, IntPtr.Zero, out _version, out handle) != 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("[ERROR] WlanOpenHandle failed");
            //    return;
            //}
            //System.Diagnostics.Debug.WriteLine("[INFO] WlanOpenHandle success");
            //System.Diagnostics.Debug.WriteLine(String.Format("[INFO] NegotiatedVersion = {0}", _version));

            //IntPtr ptr = new IntPtr();
            //if (NativeWifiAPI.WlanEnumInterfaces(handle, IntPtr.Zero, ref ptr) != 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("[ERROR] WlanEnumInterfaces failed");
            //    return;
            //}
            //NativeWifiAPI.WLAN_INTERFACE_INFO_LIST infoList = new NativeWifiAPI.WLAN_INTERFACE_INFO_LIST(ptr);
            //NativeWifiAPI.WlanFreeMemory(ptr);
            //System.Diagnostics.Debug.WriteLine("[INFO] WlanEnumInterfaces success");

            //foreach (WirelessAPI.WLAN_INTERFACE_INFO _info in infoList.InterfaceInfo)
            //{
            //    if (_info.isState == WirelessAPI.WLAN_INTERFACE_STATE.wlan_interface_state_connected)
            //    {
            //        this.ActiveInterfaceGuid = _info.InterfaceGuid;
            //    }
            //}
            //this.InterfaceGuid = infoList.InterfaceInfo[0].InterfaceGuid;
            //System.Diagnostics.Debug.WriteLine(String.Format("[INFO] InterfaceGuid = {0}", this.InterfaceGuid));
            this.WifiController = new WifiController();

            //NativeWifiAPI.WLAN_NOTIFICATION_SOURCE pdwPrevNotifSource;
            //NativeWifiAPI.WLAN_NOTIFICATION_CALLBACK _delegate = new NativeWifiAPI.WLAN_NOTIFICATION_CALLBACK(WlanNotificationCallback);

            //if (NativeWifiAPI.WlanRegisterNotification(this.handle, NativeWifiAPI.WLAN_NOTIFICATION_SOURCE.ACM, false,
            //    _delegate, IntPtr.Zero, IntPtr.Zero, out pdwPrevNotifSource) != 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("[ERROR] WlanRegisterNotification failed");
            //    return;
            //}
            //System.Diagnostics.Debug.WriteLine("[INFO] WlanRegisterNotification success");
            this.WifiController.OnConnected += OnConnected;
            this.WifiController.OnDisconnected += OnDisconnected;
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (handle != IntPtr.Zero)
            //{
            //    if (NativeWifiAPI.WlanCloseHandle(handle, IntPtr.Zero) != 0)
            //    {
            //        System.Diagnostics.Debug.WriteLine("[ERROR] WlanCloseHandle failed");
            //        return;
            //    }
            //    System.Diagnostics.Debug.WriteLine("[INFO] WlanCloseHandle success");
            //}
            this.WifiController.Dispose();
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            this.TextBoxSSID.Enabled = false;
            this.TextBoxKEY.Enabled = false;
            this.ButtonConnect.Enabled = false;
            this.ButtonDisconnect.Enabled = false;

            //StringBuilder _profileXml = new StringBuilder();
            //XmlWriterSettings _profileXmlWriterSettings = new XmlWriterSettings();
            ////_profileXmlWriterSettings.Encoding = Encoding.ASCII;

            //XmlWriter _profileXmlWriter = XmlWriter.Create(_profileXml, _profileXmlWriterSettings);
            //_profileXmlWriter.WriteStartElement("WLANProfile", @"http://www.microsoft.com/networking/WLAN/profile/v1");
            //_profileXmlWriter.WriteElementString("name", this.TextBoxSSID.Text);

            //_profileXmlWriter.WriteStartElement("SSIDConfig");
            //_profileXmlWriter.WriteStartElement("SSID");
            //_profileXmlWriter.WriteElementString("name", this.TextBoxSSID.Text);
            //_profileXmlWriter.WriteEndElement(); // SSID
            //_profileXmlWriter.WriteEndElement(); // SSIDConfig

            //_profileXmlWriter.WriteElementString("connectionType", @"ESS");
            //_profileXmlWriter.WriteElementString("connectionMode", @"auto");
            //_profileXmlWriter.WriteElementString("autoSwitch", @"false");

            //_profileXmlWriter.WriteStartElement("MSM");
            //_profileXmlWriter.WriteStartElement("security");
            //_profileXmlWriter.WriteStartElement("authEncryption");
            //_profileXmlWriter.WriteElementString("authentication", @"WPA2PSK");
            //_profileXmlWriter.WriteElementString("encryption", @"AES");
            //_profileXmlWriter.WriteElementString("useOneX", @"false");
            //_profileXmlWriter.WriteEndElement(); // authEncryption

            //_profileXmlWriter.WriteStartElement("sharedKey");
            //_profileXmlWriter.WriteElementString("keyType", @"passPhrase");
            //_profileXmlWriter.WriteElementString("protected", @"false");
            //_profileXmlWriter.WriteElementString("keyMaterial", this.TextBoxKEY.Text);
            //_profileXmlWriter.WriteEndElement(); // sharedKey

            //_profileXmlWriter.WriteEndElement(); // security
            //_profileXmlWriter.WriteEndElement(); // MSM

            //_profileXmlWriter.WriteEndElement(); // WLANProfile

            //_profileXmlWriter.Close();

            //System.Diagnostics.Debug.WriteLine(_profileXml.ToString());

            //NativeWifiAPI.WLAN_CONNECTION_PARAMETERS _param = new NativeWifiAPI.WLAN_CONNECTION_PARAMETERS();

            //// OSでプロファイル作成済みであればこの設定で成功する
            ////_param.wlanConnectionMode = WirelessAPI.WLAN_CONNECTION_MODE.wlan_connection_mode_profile;
            ////_param.strProfile = "Buffalo-G-F16E"; // SSID
            ////_param.dot11BssType = WirelessAPI.DOT11_BSS_TYPE./*dot11_BSS_type_infrastructure*/;
            ////_param.dwFlags = 0;

            //// 一時プロファイルで接続する場合
            //_param.wlanConnectionMode = NativeWifiAPI.WLAN_CONNECTION_MODE.wlan_connection_mode_temporary_profile;
            //_param.strProfile = _profileXml.ToString();
            //_param.dot11BssType = NativeWifiAPI.DOT11_BSS_TYPE.dot11_BSS_type_infrastructure;
            //_param.dwFlags = 0;

            //if (NativeWifiAPI.WlanConnect(this.handle, ref this.InterfaceGuid, ref _param, new IntPtr()) != 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("[ERROR] WlanConnect failed");

            //    this.TextBoxSSID.Enabled = true;
            //    this.TextBoxKEY.Enabled = true;
            //    this.ButtonConnect.Enabled = true;
            //    this.ButtonDisconnect.Enabled = true;

            //    return;
            //}
            //System.Diagnostics.Debug.WriteLine("[INFO] WlanConnect success");

            try
            {
                this.WifiController.Connect(this.TextBoxSSID.Text, this.TextBoxKEY.Text);
            }
            catch (Exception)
            {
                this.TextBoxSSID.Enabled = true;
                this.TextBoxKEY.Enabled = true;
                this.ButtonConnect.Enabled = true;
                this.ButtonDisconnect.Enabled = true;
            }
        }

        //private void WlanNotificationCallback(ref NativeWifiAPI.WLAN_NOTIFICATION_DATA notificationData, IntPtr context)
        //{
        //    System.Diagnostics.Debug.WriteLine(String.Format("[INFO] WlanNotificationCallback : {0}",
        //        Enum.GetName(typeof(NativeWifiAPI.WLAN_NOTIFICATION_CODE_ACM), notificationData.notificationCode)));
            
        //    if ((NativeWifiAPI.WLAN_NOTIFICATION_CODE_ACM)notificationData.NotificationCode ==
        //        NativeWifiAPI.WLAN_NOTIFICATION_CODE_ACM.wlan_notification_acm_connection_complete)
        //    {
        //        this.Invoke((MethodInvoker)(()=>{
        //            this.TextBoxSSID.Enabled = true;
        //            this.TextBoxKEY.Enabled = true;
        //            this.ButtonConnect.Enabled = true;
        //            this.ButtonDisconnect.Enabled = true;
        //        }));

        //        System.Diagnostics.Debug.WriteLine(String.Format("[INFO] {0} connected", this.TextBoxSSID.Text));
        //    }
        //}

        private void ButtonDisconnect_Click(object sender, EventArgs e)
        {
            this.TextBoxSSID.Enabled = false;
            this.TextBoxKEY.Enabled = false;
            this.ButtonConnect.Enabled = false;
            this.ButtonDisconnect.Enabled = false;

            //if (NativeWifiAPI.WlanDisconnect(this.handle, ref this.InterfaceGuid, IntPtr.Zero) != 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("[ERROR] WlanDisconnect failed");
            //    return;
            //}
            //System.Diagnostics.Debug.WriteLine("[INFO] WlanDisconnect success");
            this.WifiController.Disconnect();
        }

    }
}
