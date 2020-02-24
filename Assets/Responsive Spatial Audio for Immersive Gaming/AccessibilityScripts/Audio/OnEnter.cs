using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OnEnter : MonoBehaviour {
	public AppConstants.SpaceNames spaceName = AppConstants.SpaceNames.UNSPECIFIED;

	public void Start()
	{
		if(spaceName == AppConstants.SpaceNames.UNSPECIFIED)
		{
			throw new System.ArgumentException("unspecified constant on gameObject: " + gameObject.name);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		var player = other.gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
		if (player != null)
		{
			Dispatch.dispatch(new Actions.User.UserEnteredSpace(this.spaceName)); 
		}
    }

    private void OnTriggerExit(Collider other)
    {
		var player = other.gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
		if (player != null)
		{
			Dispatch.dispatch(new Actions.User.UserEnteredSpace(AppConstants.SpaceNames.WORLD));
		}
	}
}
