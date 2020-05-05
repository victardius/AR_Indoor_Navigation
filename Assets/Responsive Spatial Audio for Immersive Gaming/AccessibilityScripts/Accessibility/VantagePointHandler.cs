using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Class for orienting the player to a viewdio in vicinity using orientation dependent beep
/// </summary>
public class VantagePointHandler : MonoBehaviour {

	private AudioSource beeper;
	private AudioClip beepClip;
	private VantagePoint trackedVantagePoint;
	private bool forwards = true;
	private bool finishedLock = false;
    private GameObject go;

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
        Dispatch.handlers.Remove(typeof(Actions.Accessibility.VantagePointLocked));

        Dispatch.registerHandler(typeof(Actions.Accessibility.UserEnteredVantagePoint), this.OnVantagePointEnter);
        Dispatch.registerHandler(typeof(Actions.Accessibility.UserExitedVantagePoint), this.OnVantagePointExit);
        Dispatch.registerHandler(typeof(Actions.Accessibility.VantagePointLocked), this.OnVantagePointLocked);
    }
    void Start () {

		go = GameObject.FindGameObjectWithTag(AppConstants.Tags.VANTAGE_POINT_BEEPER_AUDIOSOURCE);
		if (go == null)
		{
			throw new System.NullReferenceException("Could not find a gameObject with tag: " + AppConstants.Tags.VANTAGE_POINT_BEEPER_AUDIOSOURCE);
		}
		beeper = go.GetComponent<AudioSource>();
		if(beeper == null)
		{
			throw new System.NullReferenceException("Could not find an audioSource gameObject with tag: " + AppConstants.Tags.VANTAGE_POINT_BEEPER_AUDIOSOURCE);
		}

		beepClip = Resources.Load<AudioClip>(AppConstants.Resources.Audio.Clips.BEEP);
		if(beepClip == null)
		{
			throw new System.NullReferenceException("Could not find resource at: " + AppConstants.Resources.Audio.Clips.BEEP);
		}

		Dispatch.registerHandler(typeof(Actions.Accessibility.UserEnteredVantagePoint), this.OnVantagePointEnter);
		Dispatch.registerHandler(typeof(Actions.Accessibility.UserExitedVantagePoint), this.OnVantagePointExit);
		Dispatch.registerHandler(typeof(Actions.Accessibility.VantagePointLocked), this.OnVantagePointLocked);
	}

	private AppState OnVantagePointEnter(Actions.Base action, AppState state)
	{
		var action_ = action as Actions.Accessibility.UserEnteredVantagePoint;
		if(action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: "
			+ GetType().ToString()
			//+ " method: " + GetType().DeclaringMethod.ToString()
			+ " on GameObject: " + gameObject.name);
		}

		this.finishedLock = false;
		this.trackedVantagePoint = action_.vantagePoint;
		var fwd = this.trackedVantagePoint.transform.forward;
		var dot = Vector3.Dot(Camera.main.transform.forward, fwd);
		this.forwards = dot > 0;
        beeper.loop = true;
        beeper.clip = beepClip;
        beeper.gameObject.transform.position = Camera.main.transform.position + action_.vantagePoint.transform.forward;
        beeper.Play();
        
        
		return state;
	}

	private AppState OnVantagePointExit(Actions.Base action, AppState state)
	{
		this.finishedLock = false;
        state.currentVantagePoint = null;
		this.trackedVantagePoint = null;
        beeper.Stop();
        

		return state;
	}

	private AppState OnVantagePointLocked(Actions.Base action, AppState state)
	{
		this.finishedLock = true;
        
        beeper.Stop();
        
        if (this.forwards)
		{
			ScreenReader.Say(this.trackedVantagePoint.list[0].text);
		}
		else
		{
			ScreenReader.Say(this.trackedVantagePoint.list[1].text);
		}
		return state;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.B))
		{
			this.forwards = !this.forwards;
			if(this.finishedLock)
			{
				if (this.forwards)
				{
					ScreenReader.Say(this.trackedVantagePoint.list[0].text);
				}
				else
				{
					ScreenReader.Say(this.trackedVantagePoint.list[1].text);
				}
			}
		}
		if( !this.finishedLock && this.trackedVantagePoint != null )
		{
            beeper.gameObject.transform.position = Camera.main.transform.position + this.trackedVantagePoint.transform.forward;
            

			// get the deviation in the user's gaze
			var fwd = this.trackedVantagePoint.transform.forward;
			var dot = Vector3.Dot(Camera.main.transform.forward, this.forwards ? fwd: -fwd);

            beeper.pitch = 1 + dot / 6;
            

            if (dot > 0.9f)
			{
				Dispatch.dispatch(new Actions.Accessibility.VantagePointLocked());
			}
		}
	}
}
