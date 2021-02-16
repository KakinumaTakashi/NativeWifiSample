﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace Wifi
{
    public delegate void WifiNotificationHandler();

    public class WifiController : IDisposable
    {
        private IntPtr Handle = IntPtr.Zero;

        private WifiInterfaceInfo CurrentWifiInterfaceInfo = null;
        private List<WifiInterfaceInfo> CurrentWifiInterfaceInfoList = new List<WifiInterfaceInfo>();

        private string CurrentSSID = null;


        public WifiController()
        {
            DEBUG_LOG(LOG_DEBUG, "constructor start");

            try
            {
                // ハンドルオープン
                uint _version;
                if (NativeWifiAPI.WlanOpenHandle(2, IntPtr.Zero, out _version, out this.Handle) != 0)
                {
                    throw new Exception(DEBUG_LOG(LOG_ERROR, "WlanOpenHandle failed"));
                }
                DEBUG_LOG(LOG_INFO, "WlanOpenHandle success");
                DEBUG_LOG(LOG_DEBUG, String.Format("NegotiatedVersion = {0}", _version));

                // WiFiアダプタの列挙
                IntPtr ptr = new IntPtr();
                if (NativeWifiAPI.WlanEnumInterfaces(this.Handle, IntPtr.Zero, ref ptr) != 0)
                {
                    throw new Exception(DEBUG_LOG(LOG_ERROR, "WlanEnumInterfaces failed"));
                }
                NativeWifiAPI.WLAN_INTERFACE_INFO_LIST infoList = new NativeWifiAPI.WLAN_INTERFACE_INFO_LIST(ptr);
                NativeWifiAPI.WlanFreeMemory(ptr);
                DEBUG_LOG(LOG_INFO, "WlanEnumInterfaces success");

                // WiFiアダプタリストを設定
                foreach (NativeWifiAPI.WLAN_INTERFACE_INFO _info in infoList.InterfaceInfo)
                {
                    WifiInterfaceInfo _wifiInfo = new WifiInterfaceInfo()
                    {
                        InterfaceGuid = _info.InterfaceGuid,
                        InterfaceDescription = _info.strInterfaceDescription,
                        State = (WIFI_INTERFACE_STATE)_info.isState
                    };
                    this.CurrentWifiInterfaceInfoList.Add(_wifiInfo);

                    // 接続中のWiFiアダプタがある場合は現在のアダプタに設定
                    if (_info.isState == NativeWifiAPI.WLAN_INTERFACE_STATE.wlan_interface_state_connected)
                    {
                        this.CurrentWifiInterfaceInfo = _wifiInfo;
                    }
                }

                // イベント通知ハンドラの登録
                NativeWifiAPI.WLAN_NOTIFICATION_SOURCE pdwPrevNotifSource;
                NativeWifiAPI.WLAN_NOTIFICATION_CALLBACK _delegate = new NativeWifiAPI.WLAN_NOTIFICATION_CALLBACK(WlanNotificationCallback);

                if (NativeWifiAPI.WlanRegisterNotification(this.Handle, NativeWifiAPI.WLAN_NOTIFICATION_SOURCE.ACM, false,
                    _delegate, IntPtr.Zero, IntPtr.Zero, out pdwPrevNotifSource) != 0)
                {
                    throw new Exception(DEBUG_LOG(LOG_ERROR, "WlanRegisterNotification failed"));
                }
                DEBUG_LOG(LOG_INFO, "WlanRegisterNotification success");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DEBUG_LOG(LOG_DEBUG, "constructor end");
            }
        }

        public void Dispose()
        {
            DEBUG_LOG(LOG_DEBUG, "Dispose start");

            //if (this.Handle == IntPtr.Zero) return;

            try
            {
                if (NativeWifiAPI.WlanCloseHandle(this.Handle, IntPtr.Zero) != 0)
                {
                    throw new Exception(DEBUG_LOG(LOG_ERROR, "WlanCloseHandle failed"));
                }
                DEBUG_LOG(LOG_INFO, "WlanCloseHandle success");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DEBUG_LOG(LOG_DEBUG, "Dispose end");
            }
        }

        public void Connect(string ssid, string key)
        {
            DEBUG_LOG(LOG_DEBUG, "Connect start");

            //if (this.Handle == IntPtr.Zero) return;
            //if (this.CurrentWifiInterfaceInfo == null) return;

            try
            {
                if (String.IsNullOrEmpty(ssid))
                {
                    throw new ArgumentNullException("ssid");
                }
                if (String.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException("key");
                }

                this.CurrentSSID = ssid;

                StringBuilder _profileXml = new StringBuilder();
                XmlWriterSettings _profileXmlWriterSettings = new XmlWriterSettings();
                //_profileXmlWriterSettings.Encoding = Encoding.ASCII;

                XmlWriter _profileXmlWriter = XmlWriter.Create(_profileXml, _profileXmlWriterSettings);
                _profileXmlWriter.WriteStartElement("WLANProfile", @"http://www.microsoft.com/networking/WLAN/profile/v1");
                _profileXmlWriter.WriteElementString("name", ssid);

                _profileXmlWriter.WriteStartElement("SSIDConfig");
                _profileXmlWriter.WriteStartElement("SSID");
                _profileXmlWriter.WriteElementString("name", ssid);
                _profileXmlWriter.WriteEndElement(); // SSID
                _profileXmlWriter.WriteEndElement(); // SSIDConfig

                _profileXmlWriter.WriteElementString("connectionType", @"ESS");
                _profileXmlWriter.WriteElementString("connectionMode", @"auto");
                _profileXmlWriter.WriteElementString("autoSwitch", @"false");

                _profileXmlWriter.WriteStartElement("MSM");
                _profileXmlWriter.WriteStartElement("security");
                _profileXmlWriter.WriteStartElement("authEncryption");
                _profileXmlWriter.WriteElementString("authentication", @"WPA2PSK");
                _profileXmlWriter.WriteElementString("encryption", @"AES");
                _profileXmlWriter.WriteElementString("useOneX", @"false");
                _profileXmlWriter.WriteEndElement(); // authEncryption

                _profileXmlWriter.WriteStartElement("sharedKey");
                _profileXmlWriter.WriteElementString("keyType", @"passPhrase");
                _profileXmlWriter.WriteElementString("protected", @"false");
                _profileXmlWriter.WriteElementString("keyMaterial", key);
                _profileXmlWriter.WriteEndElement(); // sharedKey

                _profileXmlWriter.WriteEndElement(); // security
                _profileXmlWriter.WriteEndElement(); // MSM

                _profileXmlWriter.WriteEndElement(); // WLANProfile

                _profileXmlWriter.Close();

                System.Diagnostics.Debug.WriteLine(_profileXml.ToString());

                NativeWifiAPI.WLAN_CONNECTION_PARAMETERS _param = new NativeWifiAPI.WLAN_CONNECTION_PARAMETERS();

                // OSでプロファイル作成済みであればこの設定で成功する
                //_param.wlanConnectionMode = WirelessAPI.WLAN_CONNECTION_MODE.wlan_connection_mode_profile;
                //_param.strProfile = "Buffalo-G-F16E"; // SSID
                //_param.dot11BssType = WirelessAPI.DOT11_BSS_TYPE./*dot11_BSS_type_infrastructure*/;
                //_param.dwFlags = 0;

                // 一時プロファイルで接続する場合
                _param.wlanConnectionMode = NativeWifiAPI.WLAN_CONNECTION_MODE.wlan_connection_mode_temporary_profile;
                _param.strProfile = _profileXml.ToString();
                _param.dot11BssType = NativeWifiAPI.DOT11_BSS_TYPE.dot11_BSS_type_infrastructure;
                _param.dwFlags = 0;

                if (NativeWifiAPI.WlanConnect(
                    this.Handle, ref this.CurrentWifiInterfaceInfo.InterfaceGuid, ref _param, new IntPtr()) != 0)
                {
                    this.CurrentSSID = null;
                    throw new Exception(DEBUG_LOG(LOG_ERROR, "WlanConnect failed"));
                }
                DEBUG_LOG(LOG_INFO, "WlanConnect success");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DEBUG_LOG(LOG_DEBUG, "Connect end");
            }
        }

        public void Disconnect()
        {
            DEBUG_LOG(LOG_DEBUG, "Disconnect start");

            //if (this.Handle == IntPtr.Zero) return;
            //if (this.CurrentWifiInterfaceInfo == null) return;

            try
            {
                if (NativeWifiAPI.WlanDisconnect(
                    this.Handle, ref this.CurrentWifiInterfaceInfo.InterfaceGuid, IntPtr.Zero) != 0)
                {
                    throw new Exception(DEBUG_LOG(LOG_ERROR, "WlanDisconnect failed"));
                }
                DEBUG_LOG(LOG_INFO, "WlanDisconnect success");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DEBUG_LOG(LOG_DEBUG, "Disconnect end");
            }
        }

        private void WlanNotificationCallback(ref NativeWifiAPI.WLAN_NOTIFICATION_DATA notificationData, IntPtr context)
        {
            DEBUG_LOG(LOG_DEBUG, String.Format("WlanNotificationCallback start : {0}",
                Enum.GetName(typeof(NativeWifiAPI.WLAN_NOTIFICATION_CODE_ACM), notificationData.notificationCode)));

            switch ((NativeWifiAPI.WLAN_NOTIFICATION_CODE_ACM)notificationData.NotificationCode)
            {
                case NativeWifiAPI.WLAN_NOTIFICATION_CODE_ACM.wlan_notification_acm_connection_complete:
                    {
                        NativeWifiAPI.WLAN_CONNECTION_NOTIFICATION_DATA _data =
                            (NativeWifiAPI.WLAN_CONNECTION_NOTIFICATION_DATA)Marshal.PtrToStructure(
                                notificationData.dataPtr, typeof(NativeWifiAPI.WLAN_CONNECTION_NOTIFICATION_DATA));

                        DEBUG_LOG(LOG_DEBUG, String.Format("SSID : {0}", _data.dot11Ssid.ucSSID));

                        if (_data.dot11Ssid.ucSSID.Equals(this.CurrentSSID))
                        {
                            WifiConnectionEventArgs _args = new WifiConnectionEventArgs()
                            {
                                ssid = _data.dot11Ssid.ucSSID
                            };

                            this.OnConnectedHandler?.Invoke(notificationData.interfaceGuid, _args);
                        }
                    }
                    break;

                case NativeWifiAPI.WLAN_NOTIFICATION_CODE_ACM.wlan_notification_acm_disconnected:
                    {
                        NativeWifiAPI.WLAN_CONNECTION_NOTIFICATION_DATA _data =
                            (NativeWifiAPI.WLAN_CONNECTION_NOTIFICATION_DATA)Marshal.PtrToStructure(
                                notificationData.dataPtr, typeof(NativeWifiAPI.WLAN_CONNECTION_NOTIFICATION_DATA));

                        DEBUG_LOG(LOG_DEBUG, String.Format("SSID : {0}", _data.dot11Ssid.ucSSID));

                        if (_data.dot11Ssid.ucSSID.Equals(this.CurrentSSID))
                        {
                            WifiConnectionEventArgs _args = new WifiConnectionEventArgs()
                            {
                                ssid = _data.dot11Ssid.ucSSID
                            };

                            this.OnDisconnectedHandler?.Invoke(notificationData.interfaceGuid, null);

                            this.CurrentSSID = null;
                        }
                    }
                    break;

                default:
                    break;
            }

            DEBUG_LOG(LOG_DEBUG, "WlanNotificationCallback end");
        }

        private EventHandler OnConnectedHandler;
        public event EventHandler OnConnected
        {
            add
            {
                this.OnConnectedHandler = value;
            }
            remove
            {
                this.OnConnectedHandler = null;
            }
        }

        public class WifiConnectionEventArgs : EventArgs
        {
            public string ssid;
        }

        private EventHandler OnDisconnectedHandler;
        public event EventHandler OnDisconnected
        {
            add
            {
                this.OnDisconnectedHandler = value;
            }
            remove
            {
                this.OnDisconnectedHandler = null;
            }
        }


        public enum WIFI_INTERFACE_STATE
        {
            NOT_READY = 0,
            CONNECTED = 1,
            AD_HOC_NETWORK_FORMED = 2,
            DISCONNECTING = 3,
            DISCONNECTED = 4,
            ASSOCIATING = 5,
            DISCOVERING = 6,
            AUTHENTICATING = 7,
        }

        public class WifiInterfaceInfo
        {
            public Guid InterfaceGuid;
            public string InterfaceDescription;
            public WIFI_INTERFACE_STATE State;
        }


        private const string LOG_INFO  = "[INFO ] ";
        private const string LOG_ERROR = "[ERROR] ";
        private const string LOG_DEBUG = "[DEBUG] ";

        private string DEBUG_LOG(string type, string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("WifiController : " + type + message);
#endif
            return message;
        }
    }
}
