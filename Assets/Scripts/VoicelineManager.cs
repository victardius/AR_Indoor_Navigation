using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicelineManager : MonoBehaviour
{
    private AudioSource audioSource = null;

    [SerializeField]
    private AudioDirection[] directionalVoicelines = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
