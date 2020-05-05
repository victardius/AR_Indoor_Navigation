using UnityEngine;

/// <summary>
/// Manages voicelines and plays them.
/// </summary>
public class VoicelineManager : MonoBehaviour
{
    /// <summary>
    /// The audio source listening to the audio.
    /// </summary>
    private AudioSource audioSource = null;

    /// <summary>
    /// List of <see cref="ScriptableObject"/>s that holds types and voiceline clips.
    /// </summary>
    [SerializeField]
    private AudioDirection[] directionalVoicelines = null;

    /// <summary>
    /// Initiates the <see cref="audioSource"/> variable.
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays a voiceline belonging to a type in <see cref="directionalVoicelines"/>.
    /// </summary>
    /// <param name="type">The type of keypoint that the voiceline should belong to.</param>
    public void PlayVoiceline(KeyPointType type)
    {
        foreach (AudioDirection direction in directionalVoicelines)
        { 
            if (direction.Type == type)
            {
                audioSource.PlayOneShot(direction.Voiceline);
                return;
            }
        }
    }
}
