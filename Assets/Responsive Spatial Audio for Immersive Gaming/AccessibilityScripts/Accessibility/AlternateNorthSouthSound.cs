using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateNorthSouthSound : MonoBehaviour {
    public GameObject player;
    AudioSource directionAudio;
    string northSound;
    string southSound;
    [Range(0.5f, 4.0f)]
    public float waitBetweenSounds;
    [Range(0.0f,1.0f)]
    public float volume;
    public float distance = 1;
    public int loop_times = 2;
    public int loop_number;
    GameObject announcer;
    [SerializeField]
    AudioClip north_sound, south_sound;
	// Use this for initialization
	void Start () {
        directionAudio = GetComponent<AudioSource>();
        northSound = "Audio/Clips/beep";
        southSound = "Audio/Clips/clink";
        announcer = GameObject.FindGameObjectWithTag(AppConstants.Tags.ANNOUNCER_AUDIOSOURCE);
        waitBetweenSounds = 2.0f;
        volume = 0.5f;
        loop_number = 0;
        north_sound = (AudioClip)Resources.Load(northSound);
        south_sound = (AudioClip)Resources.Load(southSound);
    }
	
    IEnumerator NorthCompass()
    {
        directionAudio.clip = north_sound;
        directionAudio.volume = volume;
        transform.localPosition = new Vector3(0, 0, 1 * distance);
        directionAudio.Play();
        yield return new WaitForSeconds(waitBetweenSounds);
        StartCoroutine("SouthCompass");

    }

    IEnumerator SouthCompass()
    {
        directionAudio.clip = south_sound;
        directionAudio.volume = volume;
        transform.localPosition = new Vector3(0, 0, -1*distance);
        directionAudio.Play();
        yield return new WaitForSeconds(waitBetweenSounds);
        loop_number += 1;
        StartCoroutine("NorthCompass");
    }


    IEnumerator VerbalDirectionHelper()
    {
        StartCoroutine("SayNorth");
        yield return 1;
    }

    IEnumerator SayNorth()
    {
        announcer.transform.localPosition = new Vector3(0, 0, 5f);
        ScreenReader.Say("North");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("SayEast");
    }

    IEnumerator SayEast()
    {
        announcer.transform.localPosition = new Vector3(5f, 0, 0);
        ScreenReader.Say("East");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("SaySouth");
    }

    IEnumerator SaySouth()
    {
        announcer.transform.localPosition = new Vector3(0, 0, -5f);
        ScreenReader.Say("South");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("SayWest");
    }

    IEnumerator SayWest()
    {
        announcer.transform.localPosition = new Vector3(-5f, 0, 0);
        ScreenReader.Say("West");
        yield return new WaitForSeconds(0.5f);
        announcer.transform.localPosition = new Vector3(0, 0, 0);
    }
    // Update is called once per frame
    void Update () {
        transform.parent.position = player.transform.position;
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            loop_number = 0;
            StartCoroutine("NorthCompass");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            StartCoroutine("VerbalDirectionHelper");
        }

        if (loop_number>loop_times)
        {
            StopCoroutine("NorthCompass");
            StopCoroutine("SouthCompass");
        }
    }
}
