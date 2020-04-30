using System;
using UnityEngine;

namespace Managers.Advertising.Admix {
	public class AdmixLevelBlock : MonoBehaviour {
		public Transform Banner1X2Transform;
		public Transform Banner6X5Transform;
		public Transform Banner32X5Transform;
		public Transform Video4X3Transform;
		public Transform Video16X9Transform;


		private void Start() {
			AdmixAdBundleController.ActivateBundle(Banner1X2Transform, Banner6X5Transform, Banner32X5Transform,
													Video4X3Transform, Video16X9Transform);

			if (Banner1X2Transform) Banner1X2Transform.gameObject.SetActive(false);
			if (Banner6X5Transform) Banner6X5Transform.gameObject.SetActive(false);
			if (Banner32X5Transform) Banner32X5Transform.gameObject.SetActive(false);
			if (Video4X3Transform) Video4X3Transform.gameObject.SetActive(false);
			if (Video16X9Transform) Video16X9Transform.gameObject.SetActive(false);
		}
	}
}