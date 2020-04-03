using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    private PathManager pathManager = null;

    [SerializeField]
    [Range(0,1)]
    private float visionArc = 0.7f;

    [SerializeField]
    private AudioClip seeingClip = null;

    [SerializeField]
    private AudioClip notSeeingClip = null;

    [SerializeField]
    private Slider volumeSlider = null;

    private bool SeeingNode = true;
    private GameObject nextNode { get { return pathManager.CurrentObjective; } }
    private Vector3 directionToPlayerCamera = Vector3.zero;
    private float vol = 0;
    private bool playing = false;
    private float listenerVolume = 0;

    public static Transform PlayerPos { get; private set; }

    private void Start()
    {
        AudioSource source = nextNode.GetComponentInChildren<AudioSource>();
        vol = source.volume;

        volumeSlider.value = 1;
        listenerVolume = AudioListener.volume;

        PlayerPos = transform;
    }

    private void Update()
    {
        directionToPlayerCamera = transform.position - nextNode.transform.position;
        float dotProduct = Vector3.Dot(directionToPlayerCamera.normalized, transform.forward);
        AudioSource source = nextNode.GetComponentInChildren<AudioSource>();

        if (SeeingNode)
        {
            if (!source.clip.Equals(seeingClip) && !playing)
            {
                StartCoroutine(ChangeClip(seeingClip));
            }

            if (dotProduct > -visionArc)
            {
                if (!source.clip.Equals(notSeeingClip) && !playing)
                {
                    StartCoroutine(ChangeClip(notSeeingClip));
                }
                SeeingNode = false;
                //First number is lenght, second is strength from 0 to 255 then it repeats so every value has to be a pair.
                long[] timings = { 500, 200, 500, 100, 500, 200 };
                Vibration.CreateWaveform(timings, -1);
            }
        }
        else if (dotProduct < -visionArc)
        {
            SeeingNode = true;
        }

        AudioListener.volume = listenerVolume * volumeSlider.value;
    }

    IEnumerator ChangeClip(AudioClip clip)
    {
        playing = true;
        float time = 0;
        AudioSource source = nextNode.GetComponentInChildren<AudioSource>();
        
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            source.volume = vol * (Mathf.Clamp01(1 - time / 0.5f));
            yield return null;
        }

        source.clip = clip;
        time = 0f;
        source.volume = vol;
        source.Play();

        playing = false;
    }
}
