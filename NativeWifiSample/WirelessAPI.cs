using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeWifiSample
{
    public class WirelessAPI
    {
        public enum WLAN_CONNECTION_MODE
        {
            wlan_connection_mode_profile,
            wlan_connection_mode_temporary_profile,
            wlan_connection_mode_discovery_secure,
            wlan_connection_mode_discovery_unsecure,
            wlan_connection_mode_auto,
            wlan_connection_mode_invalid,
        }
        /// <summary>
        /// Represents an 802.11 Basic Service Set type
        /// </summary>
        public enum DOT11_BSS_TYPE
        {
            ///<summary>
            /// dot11_BSS_type_infrastructure -> 1
            ///</summary>
            dot11_BSS_type_infrastructure = 1,
            ///<summary>
            /// dot11_BSS_type_independent -> 2
            ///</summary>
            dot11_BSS_type_independent = 2,
            ///<summary>
            /// dot11_BSS_type_any -> 3
            ///</summary>
            dot11_BSS_type_any = 3,
        }

        /// <summary>
        /// Defines the state of the interface. e.g. connected, disconnected.
        /// </summary>
        public enum WLAN_INTERFACE_STATE
        {
            /// <summary>
            /// wlan_interface_state_not_ready -> 0
            /// </summary>
            wlan_interface_state_not_ready = 0,
            /// <summary>
            /// wlan_interface_state_connected -> 1
            /// </summary>
            wlan_interface_state_connected = 1,
            /// <summary>
            /// wlan_interface_state_ad_hoc_network_formed -> 2
            /// </summary>
            wlan_interface_state_ad_hoc_network_formed = 2,
            /// <summary>
            /// wlan_interface_state_disconnecting -> 3
            /// </summary>
            wlan_interface_state_disconnecting = 3,
            /// <summary>
            /// wlan_interface_state_disconnected -> 4
            /// </summary>
            wlan_interface_state_disconnected = 4,
            /// <summary>
            /// wlan_interface_state_associating -> 5
            /// </summary>
            wlan_interface_state_associating = 5,
            /// <summary>
            /// wlan_interface_state_discovering -> 6
            /// </summary>
            wlan_interface_state_discovering = 6,
            /// <summary>
            /// wlan_interface_state_authenticating -> 7
            /// </summary>
            wlan_interface_state_authenticating = 7,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_INTERFACE_INFO
        {
            /// GUID->_GUID
            public Guid InterfaceGuid;

            /// WCHAR[256]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strInterfaceDescription;

            /// WLAN_INTERFACE_STATE->_WLAN_INTERFACE_STATE
            public WLAN_INTERFACE_STATE isState;
        }

        /// <summary>
        /// Contains an array of NIC information
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WLAN_INTERFACE_INFO_LIST
        {
            /// <summary>
            /// Length of <see cref="InterfaceInfo"/> array
            /// </summary>
            public Int32 dwNumberOfItems;
            /// <summary>
            /// This member is not used by the wireless service. Applications can use this member when processing individual interfaces.
            /// </summary>
            public Int32 dwIndex;
            /// <summary>
            /// Array of WLAN interfaces.
            /// </summary>
            public WLAN_INTERFACE_INFO[] InterfaceInfo;

            /// <summary>
            /// Constructor for WLAN_INTERFACE_INFO_LIST.
            /// Constructor is needed because the InterfaceInfo member varies based on how many adapters are in the system.
            /// </summary>
            /// <param name="pList">the unmanaged pointer containing the list.</param>
            public WLAN_INTERFACE_INFO_LIST(IntPtr pList)
            {
                // The first 4 bytes are the number of WLAN_INTERFACE_INFO structures.
                dwNumberOfItems = Marshal.ReadInt32(pList, 0);

                // The next 4 bytes are the index of the current item in the unmanaged API.
                dwIndex = Marshal.ReadInt32(pList, 4);

                // Construct the array of WLAN_INTERFACE_INFO structures.
                InterfaceInfo = new WLAN_INTERFACE_INFO[dwNumberOfItems];

                for (int i = 0; i <= dwNumberOfItems - 1; i++)
                {
                    // The offset of the array of structures is 8 bytes past the beginning.
                    // Then, take the index and multiply it by the number of bytes in the
                    // structure.
                    // The length of the WLAN_INTERFACE_INFO structure is 532 bytes - this
                    // was determined by doing a Marshall.SizeOf(WLAN_INTERFACE_INFO)
                    IntPtr pItemList = new IntPtr(pList.ToInt64() + (i * 532) + 8);

                    // Construct the WLAN_INTERFACE_INFO structure, marshal the unmanaged
                    // structure into it, then copy it to the array of structures.
                    InterfaceInfo[i] = (WLAN_INTERFACE_INFO)Marshal.PtrToStructure(pItemList, typeof(WLAN_INTERFACE_INFO));
                }
            }
        }

        public struct NDIS_OBJECT_HEADER
        {
            byte Type;
            byte Revision;
            ushort Size;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DOT11_SSID
        {
            /// ULONG->unsigned int
            public uint uSSIDLength;
            /// UCHAR[]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string ucSSID;
        }

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



        [DllImport("Wlanapi.dll")]
        public static extern int WlanOpenHandle(
            uint dwClientVersion,
            IntPtr pReserved, //not in MSDN but required
            [Out] out uint pdwNegotiatedVersion,
            out IntPtr ClientHandle);

        [DllImport("Wlanapi", EntryPoint = "WlanCloseHandle")]
        public static extern uint WlanCloseHandle(
            [In] IntPtr hClientHandle,
            IntPtr pReserved);

        [DllImport("Wlanapi", EntryPoint = "WlanEnumInterfaces")]
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

        [DllImport("Wlanapi", EntryPoint = "WlanFreeMemory")]
        public static extern void WlanFreeMemory(
            [In] IntPtr pMemory);
    }
}
