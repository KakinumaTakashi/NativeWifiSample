using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Wifi
{
    public class WifiProfile
    {
        public static WLANProfile CreateProfile()
        {
            WLANProfile _profile = new WLANProfile();
            _profile.SSIDConfig = new SSIDConfig();
            _profile.SSIDConfig.SSID = new SSID();
            _profile.MSM = new MSM();
            _profile.MSM.security = new security();
            _profile.MSM.security.authEncryption = new authEncryption();
            _profile.MSM.security.sharedKey = new sharedKey();

            return _profile;
        }

        [XmlRoot(ElementName ="WLANProfile"/*, Namespace = @"http://www.microsoft.com/networking/WLAN/profile/v1"*/)]
        public class WLANProfile
        {
            public string name;
            public SSIDConfig SSIDConfig;
            public string connectionType;
            public string connectionMode;
            public string autoSwitch;
            public MSM MSM;
        }

        public class SSIDConfig
        {
            public SSID SSID;
        }

        public class SSID
        {
            public string name;
        }

        public class MSM
        {
            public security security;
        }

        public class security
        {
            public authEncryption authEncryption;
            public sharedKey sharedKey;
        }

        public class authEncryption
        {
            public string authentication;
            public string encryption;
            public string useOneX;
        }

        public class sharedKey
        {
            public string keyType;
            [XmlElement("protected")]
            public string _protected;
            public string keyMaterial;
        }
    }
}
