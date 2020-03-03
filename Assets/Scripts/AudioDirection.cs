using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DirectionalVoiceline", menuName = "Voicelines")]
public class AudioDirection : ScriptableObject
{
    [SerializeField]
    private AudioClip clip = null;

    [SerializeField]
    private KeyPointType type = KeyPointType.End;

    public AudioClip Voiceline { get; }

    public KeyPointType Type { get; }
}
