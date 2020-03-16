using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = Microphone.Start(null, true, 100, 22050);
        audio.loop = true;
        while(!(Microphone.GetPosition(null) > 0)) {}
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
