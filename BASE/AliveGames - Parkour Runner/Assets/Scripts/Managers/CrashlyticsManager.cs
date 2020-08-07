using System.Collections;
using Firebase.Crashlytics;
using UnityEngine;

namespace Managers {
	public class CrashlyticsManager : MonoBehaviour {
		private void Start () {
// #if UNITY_EDITOR
			// return;
// #endif
			// Initialize Firebase
			Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
				var dependencyStatus = task.Result;
				if (dependencyStatus == Firebase.DependencyStatus.Available) {
					// Create and hold a reference to your FirebaseApp,
					// where app is a Firebase.FirebaseApp property of your application class.
					// Crashlytics will use the DefaultInstance, as well;
					// this ensures that Crashlytics is initialized.
					Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

					// Set a flag here for indicating that your project is ready to use Firebase.
				}
				else {
					Debug.LogError(System.String.Format(
														"Could not resolve all Firebase dependencies: {0}",
														dependencyStatus));
					// Firebase Unity SDK is not safe to use here.
				}
			});

			// Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
			// StartCoroutine(Crash());
		}


		private IEnumerator Crash() {
			yield return new WaitForSeconds(15f);
			StartCoroutine(Crash());
			Crashlytics.LogException(new System.Exception("test exception"));
			throw new System.Exception("test exception please ignore");
		}
	}
}