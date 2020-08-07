using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ParkourRunner.Scripts.Player {
	public class HeadDismember : MuscleDismember {
		private const string HelmetKey = "Helmet";

		public List<GameObject> Helmets;

		private GameObject _helmet, _helmetInstance;
		private bool       _helmetIsDismembered;
		private float      _dismemberHelmetTime;


		private void Start() {
			Helmets.ForEach(h => h.SetActive(false));

			var helmetIndex = (int) MainMenuAndShop.Helmets.Helmets.CurrentHelmetType;
			_helmet = helmetIndex > 0 ? Helmets[helmetIndex - 1] : null;
			_helmet?.SetActive(true);
		}


		public override void DismemberMuscleRecursive() {
			if (_helmet) {
				if (!_helmetIsDismembered) {
					var hTransform = _helmet.transform;
					_helmetInstance = Instantiate(_helmet, hTransform.position, hTransform.rotation);

					_helmetInstance.GetComponent<SphereCollider>().enabled = true;
					_helmetInstance.GetComponent<Rigidbody>().isKinematic  = false;

					_helmet.SetActive(false);
					_helmetIsDismembered = true;
					_dismemberHelmetTime = Time.time;
					return;
				}

				// Бессмертие для головы 1 сек с момента потери шлема
				if (Time.time - _dismemberHelmetTime < 1) {
					return;
				}
			}

			base.DismemberMuscleRecursive();
		}


		protected override void Heal() {
			_helmetIsDismembered = false;
			if(_helmet) _helmet.SetActive(true);
			if (_helmetInstance) Destroy(_helmetInstance);

			base.Heal();
		}
	}
}