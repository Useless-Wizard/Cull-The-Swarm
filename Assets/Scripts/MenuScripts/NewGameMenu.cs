﻿
using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnityEngine.SceneManagement;

public class NewGameMenu : MonoBehaviour
{
	//PUBLIC
	public InputField mNameField;
	public Button mCreateNewGameButton;

	public ScreenFade mScreenFader;

	//PRIVATE
	private string mName;

	private MainMenuEventHandler mMainMenu;
	private SavedGameManager mSavedGameManager;

//--------------------------------------------------------------------------------------------

	void Start()
	{
		mName = "";

		mNameField.onValueChanged.AddListener( delegate{ handleTextEditValueChanged(); } );

		mMainMenu = GetComponentInParent<MainMenuEventHandler>();
		mSavedGameManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SavedGameManager>();
	}

//--------------------------------------------------------------------------------------------

	public void handleTextEditValueChanged()
	{
		//save the text, remove spaces
		mName = mNameField.text;
		mName = mName.Replace(" ", "");

		//the create new game button is enabled if the text was nonempty
		mCreateNewGameButton.interactable = mName != "";
	}

//--------------------------------------------------------------------------------------------

	public void handleCreateNewGameButtonClicked(){ StartCoroutine(handleCreateNewGameButtonClickedHelper()); }
	private IEnumerator handleCreateNewGameButtonClickedHelper()
	{
		//if the name is nonempty...
		if(mName != "")
		{
			//create a new game object, load worldmap if successful
			if(mSavedGameManager.createNewGame(mName))
			{
				Debug.Log("LOADING WORLD MAP");

				yield return mScreenFader.FadeToBlack();
				SceneManager.LoadScene((int)SceneIndex.WORLD_MAP);
				yield return null;
			}
		}

		handleBackButtonClicked();
		yield return null;
	}
	

//--------------------------------------------------------------------------------------------

	public void handleBackButtonClicked()
	{
		//clear the text edit, disable the new game panel
		mNameField.text = "";
		gameObject.SetActive(false);

		mMainMenu.toggleButtons();
	}
}
