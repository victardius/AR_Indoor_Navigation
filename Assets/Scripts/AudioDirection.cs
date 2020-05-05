using UnityEngine;

/// <summary>
/// A class that creates a <see cref="ScriptableObject"/> to hold variables and audio clips
/// related to a direction or <see cref="KeyPointType"/>. For example this could be used to
/// check if the scriptable object type is a <see cref="KeyPointType.End"/> and play and 
/// play an audio clip related to it if the user has reached a <see cref="KeyPoint"/> that 
/// is the end of a path.
/// </summary>
[CreateAssetMenu(fileName = "DirectionalVoiceline", menuName = "Voicelines")]
public class AudioDirection : ScriptableObject
{
    /// <summary>
    /// The audio clip related to this <see cref="KeyPointType"/>.
    /// </summary>
    [SerializeField]
    private AudioClip clip = null;

    /// <summary>
    /// The type of keypoint that the <see cref="AudioClip"/> should be related to.
    /// </summary>
    [SerializeField]
    private KeyPointType type = KeyPointType.End;

    /// <summary>
    /// Gets the audio clip related to this <see cref="KeyPointType"/>.
    /// </summary>
    public AudioClip Voiceline { get { return clip; } }

    /// <summary>
    /// Gets the <see cref="KeyPointType"/> this object belongs to.
    /// </summary>
    public KeyPointType Type { get { return type; } }
}
