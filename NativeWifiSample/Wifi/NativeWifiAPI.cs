using System;
using System.Runtime.InteropServices;

namespace Wifi
{
    public class NativeWifiAPI
    {
        #region enumerate

        public enum WLAN_CONNECTION_MODE
        {
            wlan_connection_mode_profile,
            wlan_connection_mode_temporary_profile,
            wlan_connection_mode_discovery_secure,
            wlan_connection_mode_discovery_unsecure,
            wlan_connection_mode_auto,
            wlan_connection_mode_invalid,
        }

        public enum DOT11_BSS_TYPE
        {
            dot11_BSS_type_infrastructure = 1,
            dot11_BSS_type_independent = 2,
            dot11_BSS_type_any = 3,
        }

        public enum DOT11_PHY_TYPE
        {
            dot11_phy_type_unknown,
            dot11_phy_type_any = dot11_phy_type_unknown,
            dot11_phy_type_fhss,
            dot11_phy_type_dsss,
            dot11_phy_type_irbaseband,
            dot11_phy_type_ofdm,
            dot11_phy_type_hrdsss,
            dot11_phy_type_erp,
            dot11_phy_type_ht,
            dot11_phy_type_IHV_start,
            dot11_phy_type_IHV_end,
        }

        public enum DOT11_AUTH_ALGORITHM
        {
            DOT11_AUTH_ALGO_80211_OPEN = 1,
            DOT11_AUTH_ALGO_80211_SHARED_KEY = 2,
            DOT11_AUTH_ALGO_WPA = 3,
            DOT11_AUTH_ALGO_WPA_PSK = 4,
            DOT11_AUTH_ALGO_WPA_NONE = 5,
            DOT11_AUTH_ALGO_RSNA = 6,
            DOT11_AUTH_ALGO_RSNA_PSK = 7,
            DOT11_AUTH_ALGO_IHV_START = -2147483648,
            DOT11_AUTH_ALGO_IHV_END = -1,
        }

        public enum DOT11_CIPHER_ALGORITHM
        {
            DOT11_CIPHER_ALGO_NONE = 0,
            DOT11_CIPHER_ALGO_WEP40 = 1,
            DOT11_CIPHER_ALGO_TKIP = 2,
            DOT11_CIPHER_ALGO_CCMP = 4,
            DOT11_CIPHER_ALGO_WEP104 = 5,
            DOT11_CIPHER_ALGO_WPA_USE_GROUP = 256,
            DOT11_CIPHER_ALGO_RSN_USE_GROUP = 256,
            DOT11_CIPHER_ALGO_WEP = 257,
            DOT11_CIPHER_ALGO_IHV_START = -2147483648,
            DOT11_CIPHER_ALGO_IHV_END = -1,
        }

        public enum WLAN_INTERFACE_STATE
        {
            wlan_interface_state_not_ready = 0,
            wlan_interface_state_connected = 1,
            wlan_interface_state_ad_hoc_network_formed = 2,
            wlan_interface_state_disconnecting = 3,
            wlan_interface_state_disconnected = 4,
            wlan_interface_state_associating = 5,
            wlan_interface_state_discovering = 6,
            wlan_interface_state_authenticating = 7,
        }

        //[Flags]
        public enum WLAN_NOTIFICATION_SOURCE : uint
        {
            None = 0,
            All = 0X0000FFFF,
            ACM = 0X00000008,
            MSM = 0X00000010,
            Security = 0X00000020,
            IHV = 0X00000040
        }

        public enum WLAN_NOTIFICATION_CODE_MSM
        {
            wlan_notification_msm_associating = 1,
            wlan_notification_msm_associated,
            wlan_notification_msm_authenticating,
            wlan_notification_msm_connected,
            wlan_notification_msm_roaming_start,
            wlan_notification_msm_roaming_end,
            wlan_notification_msm_radio_state_change,
            wlan_notification_msm_signal_quality_change,
            wlan_notification_msm_disassociating,
            wlan_notification_msm_disconnected,
            wlan_notification_msm_peer_join,
            wlan_notification_msm_peer_leave,
            wlan_notification_msm_adapter_removal,
            wlan_notification_msm_adapter_operation_mode_change
        }

        public enum WLAN_NOTIFICATION_CODE_ACM
        {
            wlan_notification_acm_autoconf_enabled = 1,
            wlan_notification_acm_autoconf_disabled,
            wlan_notification_acm_background_scan_enabled,
            wlan_notification_acm_background_scan_disabled,
            wlan_notification_acm_bss_type_change,
            wlan_notification_acm_power_setting_change,
            wlan_notification_acm_scan_complete,
            wlan_notification_acm_scan_fail,
            wlan_notification_acm_connection_start,
            wlan_notification_acm_connection_complete,
            wlan_notification_acm_connection_attempt_fail,
            wlan_notification_acm_filter_list_change,
            wlan_notification_acm_interface_arrival,
            wlan_notification_acm_interface_removal,
            wlan_notification_acm_profile_change,
            wlan_notification_acm_profile_name_change,
            wlan_notification_acm_profiles_exhausted,
            wlan_notification_acm_network_not_available,
            wlan_notification_acm_network_available,
            wlan_notification_acm_disconnecting,
            wlan_notification_acm_disconnected,
            wlan_notification_acm_adhoc_network_state_change,
            wlan_notification_acm_profile_unblocked,
            wlan_notification_acm_screen_power_change,
            wlan_notification_acm_profile_blocked,
            wlan_notification_acm_scan_list_refresh
        }

        #endregion

        #region structure

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_INTERFACE_INFO
        {
            public Guid InterfaceGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strInterfaceDescription;
            public WLAN_INTERFACE_STATE isState;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_INTERFACE_INFO_LIST
        {
            public Int32 dwNumberOfItems;
            public Int32 dwIndex;
            public WLAN_INTERFACE_INFO[] InterfaceInfo;

            public WLAN_INTERFACE_INFO_LIST(IntPtr pList)
            {
                dwNumberOfItems = Marshal.ReadInt32(pList, 0);
                dwIndex = Marshal.ReadInt32(pList, 4);
                InterfaceInfo = new WLAN_INTERFACE_INFO[dwNumberOfItems];

                for (int i = 0; i <= dwNumberOfItems - 1; i++)
                {
                    IntPtr pItemList = new IntPtr(pList.ToInt64() + (i * 532) + 8);
                    InterfaceInfo[i] = (WLAN_INTERFACE_INFO)Marshal.PtrToStructure(pItemList, typeof(WLAN_INTERFACE_INFO));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NDIS_OBJECT_HEADER
        {
            byte Type;
            byte Revision;
            ushort Size;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DOT11_SSID
        {
            public uint uSSIDLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string ucSSID;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DOT11_BSSID_LIST
        {
            NDIS_OBJECT_HEADER Header;
            ulong uNumOfEntries;
            ulong uTotalNumOfEntries;
            IntPtr BSSIDs;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_CONNECTION_PARAMETERS
        {
            public WLAN_CONNECTION_MODE wlanConnectionMode;
            public string strProfile;
            public DOT11_SSID[] pDot11Ssid;
            public DOT11_BSSID_LIST[] pDesiredBssidList;
            public DOT11_BSS_TYPE dot11BssType;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_NOTIFICATION_DATA
        {
            public WLAN_NOTIFICATION_SOURCE notificationSource;
            public int notificationCode;
            public Guid interfaceGuid;
            public int dataSize;
            public IntPtr dataPtr;

            public object NotificationCode
            {
                get
                {
                    if (notificationSource == WLAN_NOTIFICATION_SOURCE.MSM)
                        return (WLAN_NOTIFICATION_CODE_MSM)notificationCode;
                    else if (notificationSource == WLAN_NOTIFICATION_SOURCE.ACM)
                        return (WLAN_NOTIFICATION_CODE_ACM)notificationCode;
                    else
                        return notificationCode;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_CONNECTION_NOTIFICATION_DATA
        {
            public WLAN_CONNECTION_MODE wlanConnectionMode;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strProfileName;
            public DOT11_SSID dot11Ssid;
            public DOT11_BSS_TYPE dot11BssType;
            public int bSecurityEnabled;
            public uint wlanReasonCode;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string strProfileXml;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_AVAILABLE_NETWORK
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strProfileName;
            public DOT11_SSID dot11Ssid;
            public DOT11_BSS_TYPE dot11BssType;
            public uint uNumberOfBssids;
            public bool bNetworkConnectable;
            public uint wlanNotConnectableReason;
            public uint uNumberOfPhyTypes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public DOT11_PHY_TYPE[] dot11PhyTypes;
            public bool bMorePhyTypes;
            public uint wlanSignalQuality;
            public bool bSecurityEnabled;
            public DOT11_AUTH_ALGORITHM dot11DefaultAuthAlgorithm;
            public DOT11_CIPHER_ALGORITHM dot11DefaultCipherAlgorithm;
            public uint dwFlags;
            public uint dwReserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_AVAILABLE_NETWORK_LIST
        {
            public uint dwNumberOfItems;
            public uint dwIndex;
            public WLAN_AVAILABLE_NETWORK[] wlanAvailableNetwork;

            public WLAN_AVAILABLE_NETWORK_LIST(IntPtr ppAvailableNetworkList)
            {
                dwNumberOfItems = (uint)Marshal.ReadInt32(ppAvailableNetworkList);
                dwIndex = (uint)Marshal.ReadInt32(ppAvailableNetworkList, 4);
                wlanAvailableNetwork = new WLAN_AVAILABLE_NETWORK[dwNumberOfItems];

                for (int i = 0; i < dwNumberOfItems; i++)
                {
                    IntPtr pWlanAvailableNetwork = new IntPtr(ppAvailableNetworkList.ToInt32() + i * Marshal.SizeOf(typeof(WLAN_AVAILABLE_NETWORK)) + 8);
                    wlanAvailableNetwork[i] = (WLAN_AVAILABLE_NETWORK)Marshal.PtrToStructure(pWlanAvailableNetwork, typeof(WLAN_AVAILABLE_NETWORK));
                }
            }
        }

        #endregion


        #region consts

        // available network flags
        public const uint WLAN_AVAILABLE_NETWORK_CONNECTED              = 0x00000001;   // This network is currently connected
        public const uint WLAN_AVAILABLE_NETWORK_HAS_PROFILE            = 0x00000002;   // There is a profile for this network
        public const uint WLAN_AVAILABLE_NETWORK_CONSOLE_USER_PROFILE   = 0x00000004;   // The profile is the active console user's per user profile
        public const uint WLAN_AVAILABLE_NETWORK_INTERWORKING_SUPPORTED = 0x00000008;   // Interworking is supported
        public const uint WLAN_AVAILABLE_NETWORK_HOTSPOT2_ENABLED       = 0x00000010;   // Hotspot2 is enabled
        public const uint WLAN_AVAILABLE_NETWORK_ANQP_SUPPORTED         = 0x00000020;   // ANQP is supported
        public const uint WLAN_AVAILABLE_NETWORK_HOTSPOT2_DOMAIN        = 0x00000040;   // Domain network 
        public const uint WLAN_AVAILABLE_NETWORK_HOTSPOT2_ROAMING       = 0x00000080;   // Roaming network
        public const uint WLAN_AVAILABLE_NETWORK_AUTO_CONNECT_FAILED    = 0x00000100;   // This network failed to connect
        
        #endregion


        #region function

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern int WlanOpenHandle(
            uint dwClientVersion,
            IntPtr pReserved,
            out uint pdwNegotiatedVersion,
            out IntPtr hClientHandle);

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern uint WlanCloseHandle(
            IntPtr hClientHandle,
            IntPtr pReserved);

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern uint WlanEnumInterfaces(
            IntPtr hClientHandle,
            IntPtr pReserved,
            ref IntPtr ppInterfaceList);

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern uint WlanGetAvailableNetworkList(
            IntPtr hClientHandle,
            ref Guid pInterfaceGuid,
            uint dwFlags,
            IntPtr pReserved,
            ref IntPtr ppAvailableNetworkList);

        [DllImport("Wlanapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint WlanGetProfile(
            IntPtr hClientHandle,
            ref Guid pInterfaceGuid,
            string profileName,
            IntPtr pReserved,
            //out string profileXml,
            ref IntPtr profileXml,
            //[In, Out, Optional] ref WlanProfileFlags flags
            ref uint pdwFlags,
            //[Out, Optional] out WlanProfileAccessFlags pdwGrantedAccess
            ref uint pdwGrantedAccess
        );

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern uint WlanConnect(
            IntPtr hClientHandle,
            ref Guid pInterfaceGuid,
            ref WLAN_CONNECTION_PARAMETERS pConnectionParameters,
            IntPtr pReserved);

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern uint WlanDisconnect(
            IntPtr hClientHandle,
            ref Guid pInterfaceGuid,
            IntPtr pReserved);

        public delegate void WLAN_NOTIFICATION_CALLBACK(ref WLAN_NOTIFICATION_DATA notificationData, IntPtr context);

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern uint WlanRegisterNotification(
             IntPtr hClientHandle,
             WLAN_NOTIFICATION_SOURCE dwNotifSource,
             bool bIgnoreDuplicate,
             WLAN_NOTIFICATION_CALLBACK funcCallback,
             IntPtr pCallbackContext,
             IntPtr pReserved,
             [Out] out WLAN_NOTIFICATION_SOURCE pdwPrevNotifSource);

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern void WlanFreeMemory(
            [In] IntPtr pMemory);

        #endregion
    }
}
