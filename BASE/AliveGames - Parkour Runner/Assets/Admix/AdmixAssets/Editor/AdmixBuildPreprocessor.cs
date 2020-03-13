using Admix.AdmixCore;
using Admix.AdmixCore.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace Assets.Admix.AdmixAssets.Editor
{
#if UNITY_2018_1_OR_NEWER
    // ReSharper disable once UnusedMember.Global
    public class AdmixBuildPreprocessor : IPreprocessBuildWithReport
    {
#else
    // ReSharper disable once UnusedMember.Global
    public class AdmixBuildPreprocessor : IPreprocessBuild
    {
#endif
        /// <summary>
        /// Update all placements for scenes with BuildIndex.
        /// </summary>
        private void PlacementsUpdate()
        {
            AdmixEditor.UMenuShowEditorWindow();
            ((AdmixEditor)AdmixEditor.AdmixEditorWindow).CheckAllPlacements();
        }

        public int callbackOrder { get { return 0; } }
        /// <summary>
        /// This method will be called before build start.
        /// </summary>
#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
#endif
        {
            MaterialBuildHelper.TurnOffPlacementsTextures();

            PlacementsUpdate();

            bool appTokenIsEmpty = AdmixPreferences.Instance != null && AdmixPreferences.Instance.ApplicationTokenPresent;

            bool applicationIdentifierIsEmpty = AdmixPreferences.Instance != null &&
                                                string.IsNullOrEmpty(AdmixPreferences.Instance.ApplicationIdentifier);

            bool applicationNameIsEmpty = AdmixPreferences.Instance != null &&
                                          string.IsNullOrEmpty(AdmixPreferences.Instance.ApplicationName);

            bool forceSandboxModeEnabled = AdmixPreferences.Instance != null &&
                                           AdmixPreferences.Instance.AppStateMode == AppStateMode.ForceSandbox;

            if (appTokenIsEmpty)
            {
                if (EditorUtility.DisplayDialog("Admix", "Your application token is empty!\nPlease, save placements before the build.",
                    "OK"))
                    Selection.objects = new UnityEngine.Object[] { AdmixPreferences.Instance };
            }
            else if (applicationIdentifierIsEmpty || applicationNameIsEmpty)
            {
                if (EditorUtility.DisplayDialog("Admix",
                    "Your application identifier or application name is empty!\nPlease click \"Save Placements\" button before the build.",
                    "OK"))
                {
                    try
                    {
                        EditorWindow.GetWindow<global::Admix.AdmixCore.Editor.AdmixEditor>();

                    }
                    catch (System.Exception)
                    {
                        AdmixDebug.LogWarning("Failed GetWindow()");
                    }

                    Selection.objects = new UnityEngine.Object[] { AdmixPreferences.Instance };
                }
            }
            else if (forceSandboxModeEnabled)
            {
                if (EditorUtility.DisplayDialog("Admix", "You are in Sandbox mode.",
                    "Switch to Default", "Continue with Sandbox"))
                    AdmixPreferences.Instance.AppStateMode = AppStateMode.Default;
            }

            if ((EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                 EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) && PlayerSettings.stripEngineCode)
                if (EditorUtility.DisplayDialog("Admix",
                    "Strip engine code may break Admix web view!\nPlease, use managed stripping only.",
                    "OK"))
                {
                    PlayerSettings.stripEngineCode = false;
                }
        }
    }
}