using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Adinmo;


public class AdinmoDialog : AdinmoDialogBase {
	
	public bool m_bLocalizedText = true;

	public GameObject m_choiceDialog;
	public Text m_choiceText;
	public Image m_choiceImage0;
	public Image m_choiceImage1;

	public GameObject m_recallDialog;
	public Text m_recallText;
	public Image m_recallImage0;
    public Image m_recallImage1;
    public Image m_recallImage2;
    public Image m_recallImage3;

	
    ///////////////////////////////////////////////////////////////////////////
    public void OnRecall( int choice )
    {
        AdinmoManager.SetBrandRecall(choice);
		AdinmoManager.CycleTextures();
    }


    ///////////////////////////////////////////////////////////////////////////
    public void OnRecall0()
    {
		OnRecall(0);
    }
	

    ///////////////////////////////////////////////////////////////////////////
    public void OnRecall1()
    {
		OnRecall(1);
    }


    ///////////////////////////////////////////////////////////////////////////
    public void OnRecall2()
    {
		OnRecall(2);
    }


     ///////////////////////////////////////////////////////////////////////////
    public void OnRecall3()
    {
		OnRecall(3);
    }

		
    ///////////////////////////////////////////////////////////////////////////
    public void ReplaceRecallTextures( AdinmoImageData.ImageData [] ids )
    {
		m_recallImage0.overrideSprite = ids[0].imageSprite;
		m_recallImage1.overrideSprite = ids[1].imageSprite;
		m_recallImage2.overrideSprite = ids[2].imageSprite;
		m_recallImage3.overrideSprite = ids[3].imageSprite;
    }

	///////////////////////////////////////////////////////////////////////////
	void Start () 
    {	
		if (m_choiceDialog == null)
			Debug.LogWarning( "AdinmoDialog Choice Dialog needs to be set in inspector");

		if (m_recallDialog == null)
			Debug.LogWarning( "AdinmoDialog Recal Dialog needs to be set in inspector");
	}


	///////////////////////////////////////////////////////////////////////////
	public override void Show( DialogDoneCallback pfn )
	{
		base.Show( pfn );
		
		// Make sure we have enough choices and we are online
		if (AdinmoManager.CanShowDialog())
			SetupDialog();
	}


	///////////////////////////////////////////////////////////////////////////
	public override void Hide( string msg )
	{
		base.Hide( msg );
	}

    
	///////////////////////////////////////////////////////////////////////////
    public void OnChoice( int choice )
    {
        AdinmoManager.SetBrandChoice(choice);
	}


    ///////////////////////////////////////////////////////////////////////////
    public void OnChoice1()
    {
		OnChoice(1);
    }

    ///////////////////////////////////////////////////////////////////////////
    public void OnChoice0()
    {
		OnChoice(0);
    }
	

    ///////////////////////////////////////////////////////////////////////////
    public void ReplaceChoiceTextures( AdinmoImageData.ImageData [] imgDatas )
    {
		if (imgDatas == null)
			return;

		if (m_choiceImage0 == null || m_choiceImage1 == null)
		{
			Debug.Log("AdinmoBrandChoiceDialog needs image0 and image1 set");
			return;
		}

		m_choiceImage0.overrideSprite = imgDatas[0].imageSprite;
		m_choiceImage1.overrideSprite = imgDatas[1].imageSprite;
    }

	///////////////////////////////////////////////////////////////////////////
    void SetupDialog()
    {

		bool bShowChoice = (m_dialogType == AdinmoDialogBase.BrandDialogType.Choice);
			
		m_choiceDialog.gameObject.SetActive( bShowChoice );
		m_recallDialog.gameObject.SetActive( !bShowChoice );

		if (bShowChoice)
			ReplaceChoiceTextures( GetRandomBrandChoices() );
		else
			ReplaceRecallTextures( GetRandomBrandRecalls() );

		if (m_bLocalizedText)
		{
			if (bShowChoice)
				m_choiceText.text = GetLocalizedText();
			else
				m_recallText.text = GetLocalizedText();
		}
	}

}
