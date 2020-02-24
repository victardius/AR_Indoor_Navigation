using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

	AudioMixer mixer;

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

        Dispatch.handlers.Remove(typeof(Actions.User.UserEnteredSpace));

        Dispatch.registerHandler(typeof(Actions.User.UserEnteredSpace), this.UserEnteredSpace);
    }
    void Start()
    {
		mixer = Resources.Load<AudioMixer>(AppConstants.Resources.Audio.AudioMixer);

		if(mixer == null)
		{
			throw new NullReferenceException("Could not find audiomixer prefab at: Resources/" + AppConstants.Resources.Audio.AudioMixer);
		}

		Dispatch.registerHandler(typeof(Actions.User.UserEnteredSpace), this.UserEnteredSpace);
	}

	private AppState UserEnteredSpace(Base action, AppState state)
	{
		var action_ = action as Actions.User.UserEnteredSpace;
		if (action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: "
				+ GetType().ToString()
				//+ " method: " + GetType().DeclaringMethod.ToString()
				+ " on GameObject: " + gameObject.name);
		}

		switch(action_.buildingName)
		{
			case AppConstants.SpaceNames.BLACKSMITH:
			case AppConstants.SpaceNames.DOCK:
				mixer.SetFloat("AmbientVolume", -20.0f);
				break;
			case AppConstants.SpaceNames.WORLD:
			case AppConstants.SpaceNames.UNSPECIFIED:
			default:
				mixer.SetFloat("AmbientVolume", 1.0f);
				break;
		}

		return state;
	}
    

}
