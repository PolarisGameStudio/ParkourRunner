using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Adinmo;

///////////////////////////////////////////////////////////////////
// This class shows an example of how callbacks can be used
// to know when the textures have been applied.
//
// This simple example hides the image until it is ready
//
///////////////////////////////////////////////////////////////////
public class AdinmoBanner : MonoBehaviour {
	private const string _1to1_Key    = "pk_e8f4b332415648d9803dc21b868e8cc2";
	private const string _1to2_Key    = "pk_4c7790a20c4e4c549e4bef59bee0d0f3";
	private const string _16to9_Key   = "pk_fb038d68afdc4063bb9b0c8f350c2493";
	private const string _2to1_Key    = "pk_6a20317b485e43eebc0b28bdc69d6970";
	private const string _2_35to1_Key = "pk_917c5409bbd749bd8a90781b0ba47237";

	[SerializeField] private Size          BannerSize;
	[SerializeField] private AdinmoTexture m_imageToReplace;
	public                   bool          m_bHideObjectUntilReady = true;

	private MeshRenderer _bannerMesh;


	private void Awake() {
		m_imageToReplace.m_placementKey = GetKey(BannerSize);
	}


	private string GetKey (Size size) {
		switch (size) {
			case Size._1to1:    return _1to1_Key;
			case Size._1to2:    return _1to2_Key;
			case Size._16to9:   return _16to9_Key;
			case Size._2to1:    return _2to1_Key;
			case Size._2_35to1: return _2_35to1_Key;
		}

		return string.Empty;
	}


	///////////////////////////////////////////////////////////////////
	// Register callbacks
	///////////////////////////////////////////////////////////////////
	void Start () {
		// Global callback
		AdinmoManager.SetOnReadyCallback( OnAllTexturesDownloaded );

		// Callback per image if desired
		m_imageToReplace.SetOnReadyCallback( OnTextureReplace );
		m_imageToReplace.SetOnFailCallback( OnTextureFail );

		// Optionally hide the image until it is ready to be shown
		if (m_bHideObjectUntilReady) {
			_bannerMesh = m_imageToReplace.GetComponent<MeshRenderer>();
			_bannerMesh.enabled = false;
			// m_imageToReplace.gameObject.SetActive( false );
		}
	}


	///////////////////////////////////////////////////////////////////
	// Once all textures have finished downloading
	///////////////////////////////////////////////////////////////////
	void ShowImage() {
		if (m_bHideObjectUntilReady)
			_bannerMesh.enabled = true;
			// m_imageToReplace.gameObject.SetActive( true );
	}


	///////////////////////////////////////////////////////////////////
	// Once all textures have finished downloading
	///////////////////////////////////////////////////////////////////
	void OnAllTexturesDownloaded( string msg ) {
		Debug.Log("Ready: " + msg );

		ShowImage();
	}


	///////////////////////////////////////////////////////////////////
	// Once this 
	///////////////////////////////////////////////////////////////////
	void OnTextureReplace( AdinmoTexture t ) {
		Debug.Log("This texture has been replaced.");

		ShowImage();
	}


	///////////////////////////////////////////////////////////////////
	// Once all textures have finished downloading
	///////////////////////////////////////////////////////////////////
	void OnTextureFail( AdinmoTexture t ) {
		Debug.Log("Texture not replaced -- Original texture will be seen");

		ShowImage();
	}


	///////////////////////////////////////////////////////////////////
	// Show an adinmo dialog
	///////////////////////////////////////////////////////////////////
	public void OnShowDialog() {
		AdinmoManager.ShowDialog( MyDialogDoneCallback );
	}


	///////////////////////////////////////////////////////////////////
	// Callback for when dialog is done
	///////////////////////////////////////////////////////////////////
	void MyDialogDoneCallback( string message ) {
		Debug.Log( "Dialog Done: " + message );
	}


	public enum Size {
		_1to1,
		_1to2,
		_16to9,
		_2to1,
		_2_35to1,
	}
}