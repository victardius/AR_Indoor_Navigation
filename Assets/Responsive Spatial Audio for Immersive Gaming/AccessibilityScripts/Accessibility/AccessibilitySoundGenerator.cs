using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[AddComponentMenu("Responsive Spatial Audio/Accessibility Component")]

[RequireComponent(typeof(AccessibilityMetadata))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider))]
public class AccessibilitySoundGenerator : MonoBehaviour
{
    private AccessibilityMetadata metadata;
    public AudioSource audioSource;
    private string activeText = "";
    private readonly PromptRate rate = PromptRate.Medium;
    public bool isBeeping = false;
    private bool metadataIsValid = true;
    private MeshRenderer rend;
    private readonly MeshRenderer childRend;
    private NavMeshAgent agent;
    Vector3 player;
    Vector3 fwd;
    Vector3 obj_vctr_frm_plyer;
    float dot;
    public void Start()
    {
        agent = FindObjectOfType<NavMeshAgent>();
        metadata = GetComponent<AccessibilityMetadata>();
        audioSource = GetComponent<AudioSource>();
        rend = gameObject.GetComponent<MeshRenderer>();
        audioSource.spatialize = true;
        audioSource.loop = false;

        if (metadata.descriptions == null)
        {
            metadataIsValid = false;
            throw new System.NullReferenceException(gameObject.name);
        }

        if (metadata.descriptions.Count < 2)
        {
            metadataIsValid = false;
            throw new System.ArgumentException("Provide at least 2 descriptions to: " + gameObject.name);
        }
    }

    public bool IsPickupable()
    {
        if (gameObject.GetComponent<PickupAble>() != null)
            return true;
        return false;
    }


    //public void ChangeVolume(Vector3 player_pos, float bodyscanRadius)
    //{
    //    float distToPlayer = Vector3.Distance(player_pos, gameObject.transform.position);
    //    audioSource.volume = (-0.9f / bodyscanRadius) * distToPlayer + 1; //linear rolloff
    //    //audioSource.volume = 1.0f / (1.0f + distToPlayer - 1.0f); // logarithmic rolloff
    //        //volume = 1.0f / (1.0f + rolloffFactor * (distance - 1.0f));
    //
    // }
    public void PlayBasicDescription(PromptRate rate= PromptRate.Medium, float delay = 0.0f, TTSVoice voice = TTSVoice.DAVID)
    {
        if (metadataIsValid)
        {
            var text = metadata.descriptions[0].text;
            if (activeText != text)
            {
                //audioSource.clip = TextToSpeechManager.GetClipAndCache(text, voice: voice);
                //activeText = text;
                TextToSpeechManager.GetClipAndCache(text, voice: voice, rate: rate, callback: (clip) =>
                {
                    audioSource.clip = clip;
                    //UnityEngine.Debug.Log("Inside callback " + text);
                    activeText = text;
                    audioSource.PlayDelayed(delay);
                });
            }
            else
                audioSource.PlayDelayed(delay);

        }
    }

    public float GetBasicDescriptionLength(PromptRate rate)
    {
        if (metadataIsValid)
        {
            var text = metadata.descriptions[0].text;
            var clip = TextToSpeechManager.GetClipAndCache(text, rate);
            if (clip != null)
            {
                return clip.length;
            }
            else
            {
                return text.Length / 10.0f; //total hack
            }
        }
        return -1.0f;
    }

    public void PlayAdvancedDescription(TTSVoice voice = TTSVoice.DAVID)
    {
        if (metadataIsValid)
        {
            var text = metadata.descriptions[1].text;
            if (activeText != text)
            {
                //audioSource.clip = TextToSpeechManager.GetClipAndCache(text, voice: voice);
                //activeText = text;
                TextToSpeechManager.GetClipAndCache(text, voice: voice, rate: rate, callback: (clip) =>
                {
                    audioSource.clip = clip;
                    activeText = text;
                });
            }
            audioSource.Play();
        }
    }



    public void BeepUntilNear()
    {
        if (isBeeping == true)
        {
            isBeeping = false;
            audioSource.loop = false;
            audioSource.pitch = 1;
        }
        else if (activeText != "__BEEPU" || !audioSource.isPlaying)
        {
            activeText = "__BEEPU";
            audioSource.clip = Resources.Load(AppConstants.Resources.Audio.Clips.BEEP) as AudioClip;
            audioSource.loop = true;
            isBeeping = true;
            audioSource.Play();
        }
    }
    public void Update()
    {
        if (Vector3.Distance(Camera.main.transform.position, this.gameObject.transform.position) < AppConstants.Parameters.ITEM_IN_PICKUPABLE_RANGE_DISTANCE)
        {
            if (isBeeping == true)
            {
                isBeeping = false;
                audioSource.loop = false;
                audioSource.pitch = 1;
                ScreenReader.Announcer.Say("Reached " + this.gameObject.GetComponent<AccessibilityMetadata>().GetBasicDescription());
                agent.gameObject.SetActive(false);
            }
            //if (rend != null)
            //{
            //  rend.enabled = true;
            //  //Debug.Log("here"+rend.GetComponentInParent<AccessibilityMetadata>().GetBasicDescription());
            //  if (rend.transform.childCount > 0)
            //  {
            //    childRend = rend.transform.GetChild(0).GetComponent<MeshRenderer>();
            //    if (childRend != null)
            //    {
            //        childRend.enabled = true;
            //    }
            //  }
            //  //ScreenReader.Announcer.Say("Reached " + this.gameObject.GetComponent<AccessibilityMetadata>().GetBasicDescription());
            //}
        }
        else if (isBeeping == true)
        {
            fwd = Camera.main.transform.forward;//Camera.main.transform.rotation * Vector3.forward;
            player = Camera.main.transform.position;
            obj_vctr_frm_plyer = this.transform.position - player;
            dot = Vector3.Dot(fwd, obj_vctr_frm_plyer);
            audioSource.pitch = 1 + dot / 6;
        }
    }
}
