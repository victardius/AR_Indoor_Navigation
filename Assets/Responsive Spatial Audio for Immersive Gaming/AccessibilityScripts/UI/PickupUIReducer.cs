using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Actions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PickupUIReducer : MonoBehaviour {
	Text text;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Dispatch.handlers.Remove(typeof(Actions.Inventory.HighlightItem));
        Dispatch.handlers.Remove(typeof(Actions.Inventory.HighlightOffItem));
        Dispatch.handlers.Remove(typeof(Actions.Inventory.BagIsOverweight));

        Dispatch.registerHandler(typeof(Actions.Inventory.HighlightItem), this.HighlightItem);
        Dispatch.registerHandler(typeof(Actions.Inventory.HighlightOffItem), this.HighlightOffItem);
        Dispatch.registerHandler(typeof(Actions.Inventory.BagIsOverweight), this.BagIsOverweight);
    }
    void Start () {
		text = GetComponent<Text>();
		if (text == null)
		{
			throw new System.NullReferenceException(
				this.GetType().ToString()
				+ " could not find sibling Text component on GameObject"
				+ gameObject.name);
		}
		
		Dispatch.registerHandler(typeof(Actions.Inventory.HighlightItem), this.HighlightItem);
		Dispatch.registerHandler(typeof(Actions.Inventory.HighlightOffItem), this.HighlightOffItem);
		Dispatch.registerHandler(typeof(Actions.Inventory.BagIsOverweight), this.BagIsOverweight);
	}

	AppState HighlightItem(Actions.Base action, AppState state)
	{
        var action_ = action as Actions.Inventory.HighlightItem;
		if(action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: " 
				+ GetType().ToString()
				//+ " method: " + GetType().DeclaringMethod.ToString()
				+ " on GameObject: " + gameObject.name);
		}

		var go = action_.pickup.gameObject;
		var dispString = go.GetComponent<AccessibilityMetadata>().GetBasicDescription();
        // GUI 
        this.text.text = string.Format("Press [{1}] to pickup", dispString, Actions.User.InteractKeyPressed.keystr);

        // AUI
        ScreenReader.Announcer.Say(this.text.text);
        return state;
	} 

	AppState HighlightOffItem(Actions.Base action, AppState state)
	{
		var action_ = action as Actions.Inventory.HighlightOffItem;
		if (action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: "
				+ GetType().ToString()
				+ " on GameObject: " + gameObject.name);
		}

		text.text = "";
		return state;
	}

	AppState BagIsOverweight(Actions.Base action, AppState state)
	{
		var action_ = action as Actions.Inventory.BagIsOverweight;
		if (action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: "
				+ GetType().ToString()
				+ " on GameObject: " + gameObject.name);
		}

		text.text = "Your bag is overweight. Remove or sell some items.";
		return state;
	}
}
