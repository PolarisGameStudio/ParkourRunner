using System;
using UnityEngine;

namespace DefaultNamespace.Menu.Shop {
	public class MaterialEmissionPulse : MonoBehaviour {
		[SerializeField] private Material Material;
		[SerializeField] private float MinimumColor, MaximumColor;
		[SerializeField] private float Speed = 1f;

		private static readonly int _emissionColor = Shader.PropertyToID("_EmissionColor");


		private void Update() {
			var normalizedSin = (Mathf.Sin(Time.time * Speed) + 1) / 2f;
			var value = Mathf.Lerp(MinimumColor, MaximumColor, normalizedSin);
			var color = new Color (value, value, value);
			Material.SetColor(_emissionColor, color);
		}
	}
}