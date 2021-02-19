using System;
using UnityEngine;

namespace Managers.Advertising {
	public class EnableAdObject : MonoBehaviour {
		private void Start() {
			if(!AdManager.EnableAds) gameObject.SetActive(false);
		}
	}
}