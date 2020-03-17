using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool SeeingNode = true;
    private GameObject nextNode { get { return pathManager.CurrentObjective; } }
    private Vector3 directionToPlayerCamera = Vector3.zero;
    private float vol = 0;
    private bool playing = false;
    private Vibration vibration = null;

    private void Awake()
    {
        vibration = GetComponent<Vibration>();
    }

    private void Start()
    {
        AudioSource source = nextNode.GetComponentInChildren<AudioSource>();
        vol = source.volume;
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
