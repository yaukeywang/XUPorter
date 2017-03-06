using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor{

	public enum XCProjectSystemCapabilitiesType
	{
		XCProjectSystemCapabilitiesTypeApplePay = 0,
        XCProjectSystemCapabilitiesTypeAppleAppGroups,
        XCProjectSystemCapabilitiesTypeAppleBackgroundModes,
        XCProjectSystemCapabilitiesTypeDataProtection,
        XCProjectSystemCapabilitiesTypeGameCenter,
        XCProjectSystemCapabilitiesTypeHealthKit,
        XCProjectSystemCapabilitiesTypeHomeKit,
        XCProjectSystemCapabilitiesTypeInAppPurchase,
        XCProjectSystemCapabilitiesTypeInterAppAudio,
        XCProjectSystemCapabilitiesTypeKeychain,
        XCProjectSystemCapabilitiesTypeMaps,
        XCProjectSystemCapabilitiesTypePush,
        XCProjectSystemCapabilitiesTypeSafariKeychain,
        XCProjectSystemCapabilitiesTypeSiri,
        XCProjectSystemCapabilitiesTypeVPNLite,
        XCProjectSystemCapabilitiesTypeWAC,
        XCProjectSystemCapabilitiesTypeWallet,
        XCProjectSystemCapabilitiesTypeiCloud,

        // The max count.
        XCProjectSystemCapabilitiesTypeMax
	};

	public class XCProjectSystemCapabilities  {

        public static readonly string[] capabilityModNames = {
            "ApplePay",
            "AppGroups",
            "BackgroundModes",
            "DataProtection",
            "GameCenter",
            "HealthKit",
            "HomeKit",
            "InAppPurchase",
            "InterAppAudio",
            "Keychain",
            "Maps",
            "PushNotifications",
            "AssociatedDomains",
            "Siri",
            "PersonalVPN",
            "WirelessAccessoryConfiguration",
            "Wallet",
            "iCloud"
        };

        public static readonly string[] capabilityNames = {
            "com.apple.ApplePay",
            "com.apple.ApplicationGroups.iOS",
            "com.apple.BackgroundModes",
            "com.apple.DataProtection",
            "com.apple.GameCenter",
            "com.apple.HealthKit",
            "com.apple.HomeKit",
            "com.apple.InAppPurchase",
            "com.apple.InterAppAudio",
            "com.apple.Keychain",
            "com.apple.Maps.iOS",
            "com.apple.Push",
            "com.apple.SafariKeychain",
            "com.apple.Siri",
            "com.apple.VPNLite",
            "com.apple.WAC",
            "com.apple.Wallet",
            "com.apple.iCloud"
        };

        private static Dictionary<string, int> modNameToCapabilityType = null;

        public XCProjectSystemCapabilities() {
            
            if (null == modNameToCapabilityType)
            {
                modNameToCapabilityType = new Dictionary<string, int>();
                for (int i = 0; i < (int)XCProjectSystemCapabilitiesType.XCProjectSystemCapabilitiesTypeMax; i++) {
                    modNameToCapabilityType.Add(capabilityModNames[i], i);
                }
            }
        }

		public static XCProjectSystemCapabilities createWithProject(PBXProject project){

			XCProjectSystemCapabilities systemCapabilities = new XCProjectSystemCapabilities (); 
			systemCapabilities.weakProject = project;
			return systemCapabilities;
		}

		public static string getEnumType(XCProjectSystemCapabilitiesType type){

            if (XCProjectSystemCapabilitiesType.XCProjectSystemCapabilitiesTypeMax == type)
            {
                return string.Empty;
            }

            return capabilityNames[(int)type];
		}

		public PBXProject weakProject;

        public void visitAddSystemCapabilities(string capabilityName, bool enabled) {

            if (!isValidCapabilityName(capabilityName)) {
                return;
            }

            XCProjectSystemCapabilitiesType type = (XCProjectSystemCapabilitiesType)modNameToCapabilityType[capabilityName];
            visitAddSystemCapabilities(type, enabled);
        }

		public void visitAddSystemCapabilities(XCProjectSystemCapabilitiesType type, bool enabled){

			if (weakProject == null) {
				Debug.Log ("weakProject must not be null");
				return;
			}

			string destributeType = getEnumType (type);
            if (string.IsNullOrEmpty(destributeType)) {
                return;
            }

			Debug.Log ("Add System Capabilities " + destributeType);

			PBXDictionary _Attributes = (PBXDictionary)weakProject.data ["attributes"];
			PBXDictionary _TargetAttributes = (PBXDictionary)_Attributes ["TargetAttributes"];
			PBXList _targets = (PBXList)weakProject.data ["targets"];
			PBXDictionary targetDict = null;
			if (_TargetAttributes.ContainsKey ((string)_targets [0])) {
				targetDict = (PBXDictionary)_TargetAttributes [(string)_targets [0]];
			} else {
				//不会发生
				//return;
				targetDict = new PBXDictionary();
			}
//			Debug.Log ("targetDict:" + targetDict);

			PBXDictionary SystemCapabilities = null;
			if (targetDict != null && targetDict.ContainsKey ("SystemCapabilities")) {
//				Debug.Log ("xxxxxxxxxxxxxxxxxxx");
				SystemCapabilities = (PBXDictionary)targetDict ["SystemCapabilities"];
			} else {
				SystemCapabilities = new PBXDictionary();
			}

			Debug.Log ("before SystemCapabilities:" + SystemCapabilities);
			if (SystemCapabilities!=null && SystemCapabilities.ContainsKey (destributeType)) {
				SystemCapabilities.Remove (destributeType);
			}
			Debug.Log ("after SystemCapabilities:" + SystemCapabilities);
			PBXDictionary enableDict = new PBXDictionary ();
			enableDict.Add ("enabled", enabled?"1":"0");
			SystemCapabilities.Add (destributeType, enableDict);

			if (!targetDict.ContainsKey ("SystemCapabilities")) {
				targetDict.Add("SystemCapabilities",SystemCapabilities);
			}
			if (!_TargetAttributes.ContainsKey ((string)_targets [0])) {
				_TargetAttributes.Add((string)_targets [0],targetDict);
			}
		}

        private bool isValidCapabilityName(string capabilityName) {
            
            if (null == modNameToCapabilityType) {
                return false;
            }

            return modNameToCapabilityType.ContainsKey(capabilityName);
        }
	}
}
