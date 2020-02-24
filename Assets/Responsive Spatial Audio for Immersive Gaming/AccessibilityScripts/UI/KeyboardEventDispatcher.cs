using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEventDispatcher : MonoBehaviour {

	private void Start()
	{
		// ensure singleton
		var objs = GameObject.FindObjectsOfType<KeyboardEventDispatcher>();
		if (objs.Length > 1)
		{
			throw new System.Exception("Found more than one KeyboardEventDispatcher");
		}
	}

	private void Update()
	{
		if(Input.GetKeyDown(Actions.User.InteractKeyPressed.key))
		{
			Dispatch.dispatch(new Actions.User.InteractKeyPressed());
		}

		if (Input.GetKeyDown(Actions.Accessibility.PushBasicDescription.key))
		{
			Dispatch.dispatch(new Actions.Accessibility.PushBasicDescription());
		}

        if (Input.GetKeyDown(Actions.User.CaneKeyPressed.key))
        {
            Dispatch.dispatch(new Actions.User.CaneKeyPressed());
        }

        if (Input.GetKeyDown(Actions.Accessibility.PushBasicDescriptionWithRadius.key))
		{
			Dispatch.dispatch(new Actions.Accessibility.PushBasicDescriptionWithRadius());
		}

		if (Input.GetKeyDown(Actions.User.MenuKeyPressed.key))
		{
			Dispatch.dispatch(new Actions.User.MenuKeyPressed());
		}

		if (Input.GetKeyDown(Actions.User.SlowMotionKeyPressed.key))
		{
			Dispatch.dispatch(new Actions.User.SlowMotionKeyPressed());
		}

		if (Input.GetKeyDown(Actions.User.SelectKeyPressed.key))
		{
			Dispatch.dispatch(new Actions.User.SelectKeyPressed());
		}

		if (Input.GetKeyDown(Actions.User.SelectedItemDetailKeyPressed.key))
		{
			Dispatch.dispatch(new Actions.User.SelectedItemDetailKeyPressed());
		}

		if (Input.GetKey(Actions.User.ReachKeyPressed.key))
		{
			Dispatch.dispatch(new Actions.User.ReachKeyPressed());
		}

		if (Input.GetKeyDown(Actions.User.OrientationResetPressed.key))
		{
			Dispatch.dispatch(new Actions.User.OrientationResetPressed());
		}

		if (Input.GetKey(Actions.User.NorthHornPressed.key))
		{
			Dispatch.dispatch(new Actions.User.NorthHornPressed());
		}

        if (Input.GetKey(Actions.User.BlindMode.key))
        {
            Dispatch.dispatch(new Actions.User.BlindMode());
        }

        if(Input.GetKeyDown(Actions.User.IncBodyScanAngleKeyPressed.key))
        {
            Dispatch.dispatch(new Actions.User.IncBodyScanAngleKeyPressed());
        }

        if(Input.GetKeyDown(Actions.User.DecBodyScanAngleKeyPressed.key))
        {
            Dispatch.dispatch(new Actions.User.DecBodyScanAngleKeyPressed());
        }

        if (Input.GetKeyDown(Actions.User.IncRadiusKeyPressed.key))
        {
            Dispatch.dispatch(new Actions.User.IncRadiusKeyPressed());
        }

        if (Input.GetKeyDown(Actions.User.DecRadiusKeyPressed.key))
        {
            Dispatch.dispatch(new Actions.User.DecRadiusKeyPressed());
        }
    }
}
