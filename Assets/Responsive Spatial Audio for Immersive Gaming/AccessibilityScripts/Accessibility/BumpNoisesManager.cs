using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates temporary audio sources and plays through this list of audio sources, placing these audio source where the player bumps with a collider
/// </summary>
public class BumpNoisesManager : MonoBehaviour {

	private GameObject emitterPrefab;
	private LinkedList<AudioSource> sources = new LinkedList<AudioSource>();
	private List<AudioClip> bump_clips = new List<AudioClip>();
	private int clip_id = 0;
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
        Dispatch.handlers.Remove(typeof(Actions.User.OnBump));

        Dispatch.registerHandler(typeof(Actions.User.OnBump), this.OnBump);
    }
    // Use this for initialization
    void Start () {
        
		emitterPrefab = PrefabCache.getPrefab(AppConstants.Resources.Audio.EmptySpatialEmitter);
		foreach(var clip_name in AppConstants.Resources.Audio.Clips.BUMPS)
		{
			var clip = Resources.Load(clip_name) as AudioClip;
			if(clip == null)
			{
				throw new System.NullReferenceException("Could not find clip: " + AppConstants.Resources.Audio.Clips.BUMPS);
			}
			bump_clips.Add(clip);
		}


		for(int i = 0; i < 10; i++)
		{
			sources.AddLast(MakeEmitter());
		}

		Dispatch.registerHandler(typeof(Actions.User.OnBump), this.OnBump);
	}
    
	private AudioSource MakeEmitter()
	{
        return Instantiate(emitterPrefab).GetComponent<AudioSource>();
  	}

	private AudioSource GetFreeAudioSource()
	{
		AudioSource src = null;
        src = sources.First();
        sources.RemoveFirst();
		sources.AddLast(src);
		return src;
	}
    IEnumerator WaitforHalfSecond(ControllerColliderHit hit)
    {
        yield return new WaitForSeconds(0.5f);
        var obj = hit.collider.gameObject.GetComponent<AccessibilitySoundGenerator>();
        if (obj != null)
        {
            obj.PlayBasicDescription();
            obj.audioSource.loop = false;
        }
    }

    /// <summary>
    /// Maintain a circular queue of audiosources
    /// When event comes in, we check if the audiosource is busy
    /// If free we take it out and append at the end after starting to play
    /// If busy then we instantiate a new one and queue it
    /// </summary>

    AppState OnBump(Actions.Base action, AppState state)
	{
		var action_ = action as Actions.User.OnBump;
		if(action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: "
			+ GetType().ToString()
			//+ " method: " + GetType().DeclaringMethod.ToString()
			+ " on GameObject: " + gameObject.name);
		}

		// Maintain a circular queue of audiosources
		// When event comes in, we check if the audiosource is busy
		// If free we take it out and append at the end after starting to play
		// If busy then we instantiate a new one and queue it

		AudioSource src = GetFreeAudioSource();

		if (src != null)
		{
			src.clip = bump_clips[clip_id];
			src.pitch = 1 + Random.Range(-0.1f, 0.1f);
            src.volume = AppConstants.Resources.Audio.BumpVolume;
            src.Play();
			src.gameObject.transform.position = action_.hit.point;
            clip_id = (clip_id + 1) % bump_clips.Count;
		}

        var obj = action_.hit.collider.gameObject.GetComponent<AccessibilitySoundGenerator>();
        if (obj != null)
        {
            //obj.audioSource.transform.position = action_.hit.point;
            obj.PlayBasicDescription();
            obj.audioSource.loop = false;
        }
        //StartCoroutine(WaitforHalfSecond(action_.hit));

        return state; 
	}
}
