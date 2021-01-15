using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;




////////////////////////////////////////////////////////////////////////////////////
// We create our own namespace to not overlap with your app
////////////////////////////////////////////////////////////////////////////////////
namespace Adinmo {


	public delegate void EventCallback( AdinmoTexture t );

	////////////////////////////////////////////////////////////////////////////////////
	public class AdinmoTexture : AdinmoReplace {
		

		// Implementation
		private Image m_image = null;
		private EventCallback m_onReadyCallback = null;
		private EventCallback m_onFailCallback = null;
		private GameObject m_debugBorder = null;
		private Image m_debugBorderImage = null;
		private static bool s_bShowMessageFromServer = true;


		////////////////////////////////////////////////////////////////////////////////////
		void Start() 
		{
			ShowMessageFromServer();
			Init();
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////
		// Allow the server to print a debug message to the editor window just once.
		// This message is usually a notification that the SDK is oboslete
		////////////////////////////////////////////////////////////////////////////////////
		void ShowMessageFromServer()
		{
			if (s_bShowMessageFromServer)
			{
				s_bShowMessageFromServer = false;
				
				string message = AdinmoManager.GetMessageFromServer();
				if (message != null && message != "")
					Debug.Log( message );				
			}
		}


		////////////////////////////////////////////////////////////////////////////////////
		protected override void ApplyTextureToImage(AdinmoImageData.ImageData id, Texture2D t)
		{
			// Remember the original texture
            if (m_image.sprite != null)
			    m_originalTexture = m_image.sprite.texture;

			// Overrride the texture with the downloaded one
			if (id.imageSprite == null)
				id.imageSprite = Sprite.Create( t, new Rect(0, 0, t.width, t.height), new Vector2(.5f, .5f), 100.0f, 0, SpriteMeshType.FullRect );

			m_image.overrideSprite = id.imageSprite; 
		}


		////////////////////////////////////////////////////////////////////////////////////
		public override bool IsImageActive()
		{
			return m_image.isActiveAndEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////
		public override void Init() 
		{
			// Get the image component
			m_image = GetComponent<Image>();
			if (m_image != null)
				m_orignalImage = m_image.gameObject;
			else
				m_orignalImage = null;

			base.Init();
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void SetOnReadyCallback( EventCallback pfn )
		{
			m_onReadyCallback = pfn;
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void SetOnFailCallback( EventCallback pfn )
		{
			m_onFailCallback = pfn;
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected override void OnReady()
		{
			// Let the owner know that this is ready with the downloaded texture
			if (m_onReadyCallback != null)
				m_onReadyCallback( this );
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected override void OnFail()
		{
			// Let the owner know that this is ready with the downloaded texture
			if (m_onFailCallback != null)
				m_onFailCallback( this );
		}


		////////////////////////////////////////////////////////////////////
		protected override void SetRendererEnabled( bool bEnable )
		{
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			if (sr != null)
				sr.enabled = bEnable;
			else
			{
				MeshRenderer mr = GetComponent<MeshRenderer>();
				if (mr != null)
					mr.enabled = bEnable;

				else
				{
					Image i = GetComponent<Image>();
					if (i != null)
						i.enabled = bEnable;
				}
			}
		}


		////////////////////////////////////////////////////////////////////////////////////
		// Start drawing a red or green border around the image to say whether it is big enough
		////////////////////////////////////////////////////////////////////////////////////
		public override void UpdateDebugging( Color c )
		{		
			base.UpdateDebugging( c );

			if (m_debugBorder != null && m_debugBorderImage != null)
			{
				DuplicateParentWidthHeight();
				m_debugBorderImage.color = c;
			}
		}


		////////////////////////////////////////////////////////////////////////////////////
		// Start drawing a red or green border around the image to say whether it is big enough
		////////////////////////////////////////////////////////////////////////////////////
		public override void StartDebugging()
		{		
			base.StartDebugging();

			// Quads and Sprites are handled by the base class
			Image i = GetComponent<Image>();
			if (i == null)
				return;

			GameObject borderPrefab = (GameObject)Resources.Load("AdinmoDebugImageBorder", typeof(GameObject));

			if (borderPrefab == null)
				return;

			m_debugBorder = Instantiate(borderPrefab, transform );
			m_debugBorderImage = m_debugBorder.GetComponent<Image>();

			DuplicateParentWidthHeight();
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void DuplicateParentWidthHeight()
		{
			RectTransform rt = GetComponent<RectTransform>();
			if (rt == null)
				return;
			
			RectTransform newRt = m_debugBorder.GetComponent<RectTransform>();
			if (newRt == null)
				return;
			newRt.sizeDelta = new Vector2( rt.rect.width, rt.rect.height );
		}
			
		////////////////////////////////////////////////////////////////////////////////////
		// Stop drawing a red or green border around the image to say whether it is big enough
		////////////////////////////////////////////////////////////////////////////////////
		public override void StopDebugging()
		{		
			base.StopDebugging();

			if (m_debugBorder)
				Destroy(m_debugBorder);
		}


	}

}
