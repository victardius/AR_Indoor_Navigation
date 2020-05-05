using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// The pathmanager in the scene that keeps track of the traversed path.
    /// </summary>
    [SerializeField]
    private PathManager pathManager = null;

    /// <summary>
    /// The vision arc used to determine if the user is looking at a node or not.
    /// Should always be between 0 and 1.
    /// </summary>
    [SerializeField]
    [Range(0,1)]
    private float visionArc = 0.7f;

    /// <summary>
    /// The audio clip played when the next node is seen according to the <see cref="visionArc"/>.
    /// </summary>
    [SerializeField]
    private AudioClip seeingClip = null;

    /// <summary>
    /// The audio clip played when the next node is not seen according to the <see cref="visionArc"/>.
    /// </summary>
    [SerializeField]
    private AudioClip notSeeingClip = null;

    /// <summary>
    /// Slider to control the volume.
    /// </summary>
    [SerializeField]
    private Slider volumeSlider = null;

    /// <summary>
    /// Should return true when the next node is seen.
    /// </summary>
    private bool SeeingNode = true;

    /// <summary>
    /// Gets the next node on the path according to the <see cref="pathManager.CurrentObjective"/>.
    /// </summary>
    private GameObject nextNode { get { return pathManager.CurrentObjective; } }

    /// <summary>
    /// The direction of the next node compared to the device.
    /// </summary>
    private Vector3 directionToPlayerCamera = Vector3.zero;

    /// <summary>
    /// The current audio volume.
    /// </summary>
    private float vol = 0;

    /// <summary>
    /// Should return true while audio clips are being changed and fading in or out.
    /// </summary>
    private bool playing = false;

    /// <summary>
    /// The volume on the <see cref="AudioListener"/> object.
    /// </summary>
    private float listenerVolume = 0;

    /// <summary>
    /// Gets the current position of the device being used.
    /// </summary>
    public static Transform PlayerPos { get; private set; }

    /// <summary>
    /// Initiates all variables that need to be initiated.
    /// </summary>
    private void Start()
    {
        AudioSource source = nextNode.GetComponentInChildren<AudioSource>();
        vol = source.volume;

        volumeSlider.value = 1;
        listenerVolume = AudioListener.volume;

        PlayerPos = transform;
    }
    
    /// <summary>
    /// Plays audio clips depending on if nodes are seen or not.
    /// </summary>
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

    /// <summary>
    /// Changes the audio clip and fades out the old one before fading in the new one over
    /// half a second.
    /// </summary>
    /// <param name="clip">The clip being faded in to.</param>
    /// <returns>The enumerator being used.</returns>
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
