﻿﻿using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace Adverty.Editor
{
    [InitializeOnLoad]
    internal class VulkanCompatibilityChecker :
#if UNITY_2018_1_OR_NEWER
        IPostprocessBuildWithReport
#else
        IPostprocessBuild
#endif
    {
        private const int MINIMAL_REVISION_FOR_CUSTOM_VULKAN_PLUGIN = 601; //0f1 (2019.1.0f1)
        private const string UNITY_VULKAN_UNSUPPORTED_MESSAGE = "Vulkan on Unity version {0} is not supported by Adverty";
        
        public int callbackOrder 
        {
            get
            {
                return 1;
            }
        }

        static VulkanCompatibilityChecker()
        {
            EditorApplication.delayCall = ProcessPlayerSettings;
        }

        private static bool IsMeetVulkanRequirements(List<GraphicsDeviceType> deviceTypeList)
        {
            if (deviceTypeList[0] == GraphicsDeviceType.Vulkan) //is Vulkan has priority?
            {
                int[] parsedVersion = Utils.ParseVersion(Application.unityVersion);

                int p0 = parsedVersion[0];
                int p1 = parsedVersion[1];
                int p2 = parsedVersion[2];

                return p0 >= 2019 && p1 >= 1 && p2 >= MINIMAL_REVISION_FOR_CUSTOM_VULKAN_PLUGIN;
            }

            return true;
        }

        private static void ProcessPlayerSettings()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            List<GraphicsDeviceType> includedDeviceTypes = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android).ToList();

            if (!IsMeetVulkanRequirements(includedDeviceTypes))
            {
                Debug.LogWarning(string.Format(UNITY_VULKAN_UNSUPPORTED_MESSAGE, Application.unityVersion));
            }
        }

#if UNITY_2018_1_OR_NEWER
        public void OnPostprocessBuild(BuildReport report)
        {
            if(report.summary.platform == BuildTarget.Android)
            {
                ProcessPlayerSettings();
            }
        }
#else
        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            if(target == BuildTarget.Android)
            {
                ProcessPlayerSettings();
            }
        }
#endif
    }
}