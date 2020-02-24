using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gets the reference to the audiosource attached to the player. This is used for viewdio metadata sounds, viewdio orientation beeps, on bump metadata callouts.
/// </summary>
public class ScreenReader : MonoBehaviour {

    static GameObject[] announcers__;
	public static AudioChannel Announcer;
	static ScreenReader()
	{
		announcers__ = GameObject.FindGameObjectsWithTag(AppConstants.Tags.ANNOUNCER_AUDIOSOURCE);
		if(announcers__.Length == 0)
		{
			Debug.LogError("No object with announcer tag (" + AppConstants.Tags.ANNOUNCER_AUDIOSOURCE + ") found");
		}
		else if (announcers__.Length > 1)
		{
			Debug.LogError("Multiple objects with announcer tag (" + AppConstants.Tags.ANNOUNCER_AUDIOSOURCE + ") found");
		}
		try
		{
            Announcer = new AudioChannel(announcers__[0].GetComponent<AudioSource>());
		}
		catch (System.ArgumentNullException e)
		{
			Debug.LogError(e.Message + " on: " + announcers__[0].name);
		}
	}

	public static void Say(string text, TTSVoice voice = TTSVoice.DAVID, PromptRate rate = PromptRate.Medium)
	{
        try
        {
            Announcer.Say(text);
        }
        catch
        {
            announcers__ = GameObject.FindGameObjectsWithTag(AppConstants.Tags.ANNOUNCER_AUDIOSOURCE);
            Announcer = new AudioChannel(announcers__[0].GetComponent<AudioSource>());
            Announcer.Say(text);
        }


    }
}



public class AudioChannel
{
	private AudioSource source;

	public AudioChannel(AudioSource source)
	{
		if(source == null)
		{
			throw new System.ArgumentNullException("AudioChannel was initialized with null");
		}
		this.source = source;
	}

	public void Say(string words)
	{
		// This layer of abstraction is not needed for now but in the future there may be extra features such as a priority queue
		source.Say(words);
	}

	public float Beep(Vector3 position)
	{
		var clip = Resources.Load<AudioClip>(AppConstants.Resources.Audio.Clips.BEEP);
		AudioSource.PlayClipAtPoint(clip, position);
		return clip.length;
	}
}

public static class Extensions
{
	public static void Say(this AudioSource source, string words)
	{
        try
        {
            source.clip = TextToSpeechManager.GetClipAndCache(words, PromptRate.Medium);
            source.Play();
        }
        catch
        {
            source = GameObject.FindObjectOfType<AudioSource>();
            source.clip = TextToSpeechManager.GetClipAndCache(words, PromptRate.Medium);
            source.Play();
        }
    }
}