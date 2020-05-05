using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EntityType
{
    NPC_FRIENDLY, // squadmate
    NPC_ENEMY,    // enemy soldier
    NPC_NATURE,   // bunny
    OBSTACLE,     // table, mountain
    ITEM,         // sword, potion
    AMBIENT,      // nice view from here
    BUILDING,     // watch tower
}

[System.Serializable]
public struct EntityDescription
{
    [SerializeField]
    public string key;
    [SerializeField]
    public string text;
}

public class AccessibilityMetadata : MonoBehaviour
{
    [SerializeField]
    public List<EntityDescription> descriptions = new List<EntityDescription> {
        new EntityDescription() { key="Basic", text=""},
        new EntityDescription(){key="Advanced", text=""}
    };

    public EntityType entityType;
    public bool customMaxVisibleDistance = false;
    public float maxVisibleDistance = AppConstants.Parameters.DEFAULT_MAX_VISIBLE_DISTANCE;

    public int Hash
    {
        get
        {
            int ret = 0;
            foreach (var d in descriptions)
            {
                ret = ret ^ d.GetHashCode();
            }
            return ret;
        }
    }

    private void Start()
    {
        //Precache TTS with Default voice (David) to save overhead runtime 
        TextToSpeechManager.GetClipAndCache(GetBasicDescription(), PromptRate.Medium);
    }
    public string GetBasicDescription()
    {
        if (descriptions.Count > 1)
        {

            return descriptions[0].text;
        }
        else
        {
            throw new System.MissingMemberException(
                "No data was available for basic description on: "
                + gameObject.name);
        }
    }
    public void SetBasicDescription(EntityDescription d)
    {
        if (descriptions.Count > 1)
        {
            descriptions[0] = d;
        }
        else
        {
            throw new System.MissingMemberException(
                "No data was available for basic description on: "
                + gameObject.name);
        }
    }
}
