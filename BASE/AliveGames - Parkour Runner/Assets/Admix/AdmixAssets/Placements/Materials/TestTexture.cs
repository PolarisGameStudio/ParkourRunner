using System.Linq;
using UnityEngine;

public class TestTexture : MonoBehaviour {
	void Start() {
		var mr       = GetComponent<MeshRenderer>();
		var material = mr.material;

		var width  = 600;
		var height = 500;
		var txt    = new Texture2D(width, height, TextureFormat.ARGB32, false);
		var colors = Enumerable.Repeat(new Color32(255, 0, 0, 255), width * height);
		txt.SetPixels32(colors.ToArray());
		txt.Apply();

		material.mainTexture = txt;
	}
}