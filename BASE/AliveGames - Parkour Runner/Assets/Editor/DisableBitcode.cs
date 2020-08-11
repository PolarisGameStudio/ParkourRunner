/*
#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Editor {
	public class DisableBitcode {
		[PostProcessBuild(1000 )]
		public static void PostProcessBuildAttribute( BuildTarget target, string pathToBuildProject ) {
			if (target == BuildTarget.iOS) {
				string projectPath = PBXProject.GetPBXProjectPath(pathToBuildProject);

				PBXProject pbxProject = new PBXProject();
				pbxProject.ReadFromFile(projectPath);
				string[] targetGuids = new string[2]
					{ pbxProject.GetUnityMainTargetGuid(), pbxProject.GetUnityFrameworkTargetGuid() };

				pbxProject.SetBuildProperty(targetGuids, "ENABLE_BITCODE", "NO");
				pbxProject.WriteToFile (projectPath);
			}
		}
	}
}
#endif
*/