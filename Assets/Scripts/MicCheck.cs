using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class MicCheck : MonoBehaviour
{
    private AudioSource audioSource = null;

    [SerializeField]
    private Image checkMark = null;

    private void Awake()
    {
        Permission.RequestUserPermission(Permission.Microphone);
    }
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 100, 22050);
        audioSource.loop = true;
        while(!(Microphone.GetPosition(null) > 0)) {}
        audioSource.Play();
    }

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
