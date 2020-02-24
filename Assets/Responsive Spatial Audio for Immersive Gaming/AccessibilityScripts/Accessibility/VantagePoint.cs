using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Actions;
using UnityEngine;
using UnityEngine.SceneManagement;

using FPC = UnityStandardAssets.Characters.FirstPerson.FirstPersonController;

/// <summary>
/// Class for storing metadata associated with vantage point and triggering vantage point description playback
/// </summary>
[System.Serializable]
public struct VantagePointDescription
{
	[SerializeField]
	public string key; // just a descrptive name for this instance
	[SerializeField]
	public string text;// what you want it to say
}

public class VantagePoint : MonoBehaviour {

	[SerializeField]
	public List<VantagePointDescription> list;



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
        Dispatch.handlers.Remove(typeof(Actions.Accessibility.UserEnteredVantagePoint));
        Dispatch.handlers.Remove(typeof(Actions.Accessibility.UserExitedVantagePoint));

        Dispatch.registerHandler(typeof(Actions.Accessibility.UserEnteredVantagePoint), VantagePoint.UserEnteredVantagePoint);
        Dispatch.registerHandler(typeof(Actions.Accessibility.UserExitedVantagePoint), VantagePoint.UserExitedVantagePoint);
    }
    public void Start()
	{
		Dispatch.registerHandler(typeof(Actions.Accessibility.UserEnteredVantagePoint), VantagePoint.UserEnteredVantagePoint);
		Dispatch.registerHandler(typeof(Actions.Accessibility.UserExitedVantagePoint), VantagePoint.UserExitedVantagePoint);
		foreach (var d in list)
		{
			TextToSpeechManager.GetClipAndCache(d.text);
		}
	}

	private static AppState UserEnteredVantagePoint(Base action, AppState state)
	{
		var action_ = action as Actions.Accessibility.UserEnteredVantagePoint;
		if(action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: UserEnteredVantagePoint in static member");
		}

		state.currentVantagePoint = action_.vantagePoint;

		// Play a sound from the announcer channel that indicates that a vantage point is available
		ScreenReader.Announcer.Say("Vantage point available");

		return state;
	}

	private static AppState UserExitedVantagePoint(Base action, AppState state)
	{
		var action_ = action as Actions.Accessibility.UserExitedVantagePoint;
		if (action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: UserExitedVantagePoint in static member");
		}

		state.currentVantagePoint = null;

		return state;
	}

	public void OnTriggerEnter(Collider other)
	{
		var player = other.gameObject.GetComponent<FPC>();
		if (player != null)
		{
			Dispatch.dispatch(new Actions.Accessibility.UserEnteredVantagePoint(this));
		}
	}

	public void OnTriggerExit(Collider other)
	{
		var player = other.gameObject.GetComponent<FPC>();
		if (player != null)
		{
			Dispatch.dispatch(new Actions.Accessibility.UserExitedVantagePoint());
		}
	}
}
