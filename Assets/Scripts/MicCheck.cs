using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

/// <summary>
/// Class used to play back microphone input so that the user can hear their surrounding 
/// even with headphones.
/// </summary>
public class MicCheck : MonoBehaviour
{
    /// <summary>
    /// The audio source listening to the sound.
    /// </summary>
    private AudioSource audioSource = null;

    /// <summary>
    /// The ui checkmark that is changed depending on if the device should play back surroundings
    /// or not.
    /// </summary>
    [SerializeField]
    private Image checkMark = null;

    /// <summary>
    /// Requests permissions to user the microphone.
    /// </summary>
    private void Awake()
    {
        Permission.RequestUserPermission(Permission.Microphone);
    }
    
    /// <summary>
    /// Assigns variables and starts recording surrounding sounds for playback.
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 100, 22050);
        audioSource.loop = true;
        while(!(Microphone.GetPosition(null) > 0)) {}
        audioSource.Play();
    }

    /// <summary>
    /// Stops or resumes the microphone playback and changes the UI checkmark to display if the 
    /// function is enabled or not.
    /// </summary>
    public void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Microphone.End(null);
        }
        else
        {
            audioSource.clip = Microphone.Start(null, true, 100, 22050);
            while (!(Microphone.GetPosition(null) > 0)) { }
            audioSource.Play();
        }

        checkMark.enabled = audioSource.isPlaying;
    }
}
