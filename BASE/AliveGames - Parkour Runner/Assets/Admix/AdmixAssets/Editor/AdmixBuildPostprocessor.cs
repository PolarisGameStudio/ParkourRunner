#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Admix.WebView
{

    public class AdmixBuildPostProcessor
    {
        [PostProcessBuild(700)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {

            if (target != BuildTarget.iOS)
            {
                return;
            }
            string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            string targetGuid = proj.TargetGuidByName("Unity-iPhone");
            proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
            File.WriteAllText(projPath, proj.WriteToString());
        }
    }
}
#endif