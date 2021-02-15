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

        #endregion

        #region function

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern int WlanOpenHandle(
            uint dwClientVersion,
            IntPtr pReserved, //not in MSDN but required
            [Out] out uint pdwNegotiatedVersion,
            out IntPtr ClientHandle);

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern uint WlanCloseHandle(
            [In] IntPtr hClientHandle,
            IntPtr pReserved);

        [DllImport("Wlanapi.dll", SetLastError = true)]
        public static extern uint WlanEnumInterfaces(
            [In] IntPtr hClientHandle,
            IntPtr pReserved,
            ref IntPtr ppInterfaceList);

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
