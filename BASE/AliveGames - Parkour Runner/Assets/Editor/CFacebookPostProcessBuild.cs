using System.IO;
using UnityEditor.Callbacks;

namespace UnityEditor.CFacebook {
	public static class CFacebookPostProcessBuild {
		const string FACEBOOK_FRAMEWORKS_FOLDER = "$(PROJECT_DIR)/Frameworks/Plugins/iOS/Facebook";


		[PostProcessBuild(99999999)]
		public static void OnPostProcessBuild(BuildTarget buildTarget, string path) {
#if UNITY_IOS
            if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.iOS)
			{
				string projectPath = path+"/Unity-iPhone.xcodeproj/project.pbxproj";

				// Create a new project object from build target
				UnityEditor.iOS.Xcode.PBXProject project = new UnityEditor.iOS.Xcode.PBXProject();
				var file = File.ReadAllText(projectPath);
				project.ReadFromString(file);

				string target = project.TargetGuidByName("Unity-iPhone");

				project.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", FACEBOOK_FRAMEWORKS_FOLDER);
				
				project.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

				// Finally save the xcode project
				File.WriteAllText(projectPath, project.WriteToString());
			}
#endif
		}
	}
}
