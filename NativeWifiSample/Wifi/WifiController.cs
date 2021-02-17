using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Wifi
{
    public delegate void WifiNotificationHandler();

    public class WifiController : IDisposable
    {
        private IntPtr Handle = IntPtr.Zero;

        private WifiInterfaceInfo CurrentWifiInterfaceInfo = null;
        private List<WifiInterfaceInfo> CurrentWifiInterfaceInfoList = new List<WifiInterfaceInfo>();

        private string CurrentSSID = null;

        private bool RecoveryMode = false;
        private string BeforeProfileName = null;
        private string BeforeProfile = null;


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
                        DEBUG_LOG(LOG_DEBUG, String.Format("Set current interface = {0}", _wifiInfo.InterfaceDescription));
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

        public void Connect(string ssid, string key, bool RecoveryMode)
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

                if (RecoveryMode)
                {
                    // 有効なアクセスポイント一覧を取得
                    IntPtr _ppAvailableNetworkList = new IntPtr();
                    if (NativeWifiAPI.WlanGetAvailableNetworkList(
                        this.Handle, ref this.CurrentWifiInterfaceInfo.InterfaceGuid, 0, IntPtr.Zero,
                        ref _ppAvailableNetworkList) != 0)
                    {
                        throw new Exception(DEBUG_LOG(LOG_ERROR, "WlanGetAvailableNetworkList failed"));
                    }
                    DEBUG_LOG(LOG_INFO, "WlanGetAvailableNetworkList success");
                    NativeWifiAPI.WLAN_AVAILABLE_NETWORK_LIST _availableNetworkList =
                        new NativeWifiAPI.WLAN_AVAILABLE_NETWORK_LIST(_ppAvailableNetworkList);
                    // 接続中のアクセスポイントを検索
                    string _beforeProfileName = null;
                    string _beforeProfile = null;
                    foreach (NativeWifiAPI.WLAN_AVAILABLE_NETWORK _availableNetwork in _availableNetworkList.wlanAvailableNetwork)
                    {
                        if ((_availableNetwork.dwFlags & NativeWifiAPI.WLAN_AVAILABLE_NETWORK_CONNECTED) != 0)
                        {
                            DEBUG_LOG(LOG_DEBUG, String.Format("SSID before connection = {0}", _availableNetwork.dot11Ssid.ucSSID));
                            _beforeProfileName = _availableNetwork.strProfileName;

                            uint _flags = 0, _access = 0;
                            IntPtr _buf = new IntPtr();
                            uint _ret = NativeWifiAPI.WlanGetProfile(this.Handle, ref this.CurrentWifiInterfaceInfo.InterfaceGuid,
                                _beforeProfileName, IntPtr.Zero, ref _buf, ref _flags, ref _access);
                            if (_ret == 1168 /*ERROR_NOT_FOUND*/)
                            {
                                this.BeforeProfile = null;
                            }
                            else if (_ret != 0 /*NO_ERROR*/)
                            {
                                throw new Exception(DEBUG_LOG(LOG_ERROR, "WlanGetProfile failed"));
                            }
                            else
                            {
                                _beforeProfile = Marshal.PtrToStringAuto(_buf);
                            }
                            NativeWifiAPI.WlanFreeMemory(_buf);
                        }
                    }
                    // 接続中のアクセスポイントがある場合はプロファイルを退避
                    if (!String.IsNullOrEmpty(_beforeProfileName))
                    {
                        this.BeforeProfileName = _beforeProfileName;
                        this.BeforeProfile = _beforeProfile;
                    }
                }

                this.CurrentSSID = ssid;
                this.RecoveryMode = RecoveryMode;

                WifiProfile.WLANProfile _profile = WifiProfile.CreateProfile();
                _profile.name = ssid;
                _profile.SSIDConfig.SSID.name = ssid;
                _profile.connectionType = "ESS";
                _profile.connectionMode = "auto";
                _profile.autoSwitch = "false";
                _profile.MSM.security.authEncryption.authentication = "WPA2PSK";
                _profile.MSM.security.authEncryption.encryption = "AES";
                _profile.MSM.security.authEncryption.useOneX = "false";
                _profile.MSM.security.sharedKey.keyType = "passPhrase";
                _profile.MSM.security.sharedKey._protected = "false";
                _profile.MSM.security.sharedKey.keyMaterial = key;

                StringBuilder _profileXml = new StringBuilder();
                XmlWriter _profileXmlWriter = XmlWriter.Create(_profileXml);
                XmlSerializer _serializer = new XmlSerializer(
                    typeof(WifiProfile.WLANProfile), @"http://www.microsoft.com/networking/WLAN/profile/v1");
                _serializer.Serialize(_profileXmlWriter, _profile);
                DEBUG_LOG(LOG_DEBUG, _profileXml.ToString());

                // 一時プロファイルで接続する
                NativeWifiAPI.WLAN_CONNECTION_PARAMETERS _param = new NativeWifiAPI.WLAN_CONNECTION_PARAMETERS();
                _param.wlanConnectionMode = NativeWifiAPI.WLAN_CONNECTION_MODE.wlan_connection_mode_temporary_profile;
                _param.strProfile = _profileXml.ToString();
                _param.dot11BssType = NativeWifiAPI.DOT11_BSS_TYPE.dot11_BSS_type_infrastructure;
                _param.dwFlags = 0;

                ExecConnect(_param);
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

        private void ExecConnect(NativeWifiAPI.WLAN_CONNECTION_PARAMETERS _param)
        {
            DEBUG_LOG(LOG_DEBUG, "ExecConnect start");

            try
            {
                uint _ret;
                if ((_ret = NativeWifiAPI.WlanConnect(
                    this.Handle, ref this.CurrentWifiInterfaceInfo.InterfaceGuid, ref _param, new IntPtr())) != 0)
                {
                    this.CurrentSSID = null;
                    throw new Exception(DEBUG_LOG(LOG_ERROR, String.Format("WlanConnect failed (return code = {0})", _ret)));
                }
                DEBUG_LOG(LOG_INFO, "WlanConnect success");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DEBUG_LOG(LOG_DEBUG, "ExecConnect end");
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

                if (this.RecoveryMode)
                {
                    XmlSerializer _serializer = new XmlSerializer(
                        typeof(WifiProfile.WLANProfile), @"http://www.microsoft.com/networking/WLAN/profile/v1");
                    WifiProfile.WLANProfile _profile =
                        (WifiProfile.WLANProfile)_serializer.Deserialize(new StringReader(this.BeforeProfile));

                    if (!_profile.connectionMode.Equals("auto"))
                    {
                        NativeWifiAPI.WLAN_CONNECTION_PARAMETERS _param = new NativeWifiAPI.WLAN_CONNECTION_PARAMETERS();

                        // OSでプロファイル作成済みであればこの設定で成功する
                        _param.wlanConnectionMode = NativeWifiAPI.WLAN_CONNECTION_MODE.wlan_connection_mode_profile;
                        _param.strProfile = this.BeforeProfileName;
                        _param.dot11BssType = NativeWifiAPI.DOT11_BSS_TYPE.dot11_BSS_type_infrastructure;
                        _param.dwFlags = 0;

                        ExecConnect(_param);
                    }
                }
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
