using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Reflection;


namespace Adinmo
{
	public class AdinmoCompatibility : MonoBehaviour {

		// Prevent dead-stripping
		public void KeepStrippedSymbols()
		{
			UnityWebRequest r = new UnityWebRequest();
			#if UNITY_2017_2_OR_NEWER
				r.SendWebRequest();
			#else
				r.Send();
			#endif
		
		}
	}

}