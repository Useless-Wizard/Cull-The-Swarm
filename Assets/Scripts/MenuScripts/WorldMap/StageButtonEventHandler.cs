﻿
using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnityEngine.SceneManagement;

public class StageButtonEventHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	//PUBLIC
	public SceneIndex sceneIndex;

	public bool isUnlocked;

	//PRIVATE
	private WorldMapEventHandler mParentEventHandler;

//--------------------------------------------------------------------------------------------

	void Start()
	{
		//get the worldmap canvas event handler
		mParentEventHandler = GetComponentInParent<WorldMapEventHandler>();
	}

//--------------------------------------------------------------------------------------------

	public void handleButtonClicked()
	{
		if(mParentEventHandler != null)
		{
			//call to the canvas -- sets the selected level to this button's index
			mParentEventHandler.handleStageButtonClicked(sceneIndex);
		}
	}

//--------------------------------------------------------------------------------------------

	public void OnSelect(BaseEventData eventData)
	{
		handleButtonMouseOver();
	}

	public void handleButtonMouseOver()
	{
		if(mParentEventHandler != null)
		{
			//call to the canvas -- sets the selected level to this button's index
			mParentEventHandler.handleStageButtonMouseOver(sceneIndex);
		}
	}

//--------------------------------------------------------------------------------------------

	public void OnDeselect(BaseEventData eventData)
	{
		handleButtonMouseExit();
	}

	public void handleButtonMouseExit()
	{
		if(mParentEventHandler != null)
		{
			//call to the canvas -- sets the selected level to this button's index
			mParentEventHandler.handleStageButtonMouseExit(sceneIndex);
		}
	}
}
