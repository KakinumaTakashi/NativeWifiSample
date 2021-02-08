using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NativeWifiSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr handle = IntPtr.Zero;
            uint _version;

            try
            {
                if (WirelessAPI.WlanOpenHandle(2, IntPtr.Zero, out _version, out handle) != 0)
                {
                    return;
                }

                IntPtr ptr = new IntPtr();
                if (WirelessAPI.WlanEnumInterfaces(handle, IntPtr.Zero, ref ptr) != 0)
                {
                    return;
                }
                WirelessAPI.WLAN_INTERFACE_INFO_LIST infoList = new WirelessAPI.WLAN_INTERFACE_INFO_LIST(ptr);
                WirelessAPI.WlanFreeMemory(ptr);

                Guid _guid = infoList.InterfaceInfo[0].InterfaceGuid;


                string profileFormat =
                    "<?xml version=\"1.0\" encoding=\"US-ASCII\"?>" +
                    "<WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\">\n" +
                    "<name>Buffalo-G-F16E</name>\n" +
                    "<SSIDConfig>\n" +
                    "<SSID>\n" +
                    "<name>Buffalo-G-F16E</name>\n" +
                    "</SSID>\n" +
                    "</SSIDConfig>\n" +
                    "<connectionType>ESS</connectionType>\n" +
                    "<connectionMode>auto</connectionMode>\n" +
                    "<autoSwitch>false</autoSwitch>\n" +
                    "<MSM>\n" +
                    "<security>\n" +
                    "<authEncryption>\n" +
                    "<authentication>WPA2PSK</authentication>\n" +
                    "<encryption>AES</encryption>\n" +
                    "<useOneX>false</useOneX>\n" +
                    "</authEncryption>\n" +
                    "<sharedKey>\n" +
                    "<keyType>passPhrase</keyType>\n" +
                    "<protected>false</protected>\n" +
                    "<keyMaterial>xxxxxxxxxx</keyMaterial>\n" +
                    "</sharedKey>\n" +
                    "</security>\n" +
                    "</MSM>\n" +
                    "</WLANProfile>\n";

                WirelessAPI.WLAN_CONNECTION_PARAMETERS _param = new WirelessAPI.WLAN_CONNECTION_PARAMETERS();
                
                // OSでプロファイル作成済みであればこの設定で成功する
                //_param.wlanConnectionMode = WirelessAPI.WLAN_CONNECTION_MODE.wlan_connection_mode_profile;
                //_param.strProfile = "Buffalo-G-F16E"; // SSID
                //_param.dot11BssType = WirelessAPI.DOT11_BSS_TYPE.dot11_BSS_type_infrastructure;
                //_param.dwFlags = 0;

                // 一時プロファイルで接続する場合
                _param.wlanConnectionMode = WirelessAPI.WLAN_CONNECTION_MODE.wlan_connection_mode_temporary_profile;
                _param.strProfile = profileFormat;
                _param.dot11BssType = WirelessAPI.DOT11_BSS_TYPE.dot11_BSS_type_infrastructure;
                _param.dwFlags = 0;

                uint _result = WirelessAPI.WlanConnect(handle, ref _guid, ref _param, new IntPtr());

            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    WirelessAPI.WlanCloseHandle(handle, IntPtr.Zero);
                }
            }
        }
    }
}
