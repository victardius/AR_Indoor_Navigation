using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum PickupType
{
	POTION,
	WEAPON,
    SWORD,
	FOOD
}


/// <summary>
/// Allows objects to be picked by the player.
/// The pickable object here is mandated to be made accessible by adding Accessibility Metadata and Sound Generator components, since pickable objects are said in a different voice in environment scan.
/// The pickable object should be a prefab.
/// </summary>
[AddComponentMenu("Responsive Spatial Audio/Pickable")]
/*
 * Allows objects to be picked up by the player
 */
[RequireComponent(typeof(AccessibilityMetadata))]
[RequireComponent(typeof(AccessibilitySoundGenerator))]
public class PickupAble : MonoBehaviour
{
    public float weight;
    public PickupType pickup_type;
    public AudioClip OnPickupSound;
    
    public string pathToPrefab;
    public string pathToImageIcon;

    private AccessibilityMetadata metadata;
    void Start()
    {
        if(pathToPrefab == null || pathToPrefab == "")
        {
            throw new System.ArgumentException("Received a null or empty prefab identifier on: " + gameObject.name);
        }
        if (metadata == null)
        {
            metadata = GetComponent<AccessibilityMetadata>(); 
        }
        
    }
    
    public class PickupAbleEqualityByType : IEqualityComparer<PickupAble>
    {
        public bool Equals(PickupAble x, PickupAble y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            var x_ = x.metadata.descriptions;
            var y_ = y.metadata.descriptions;

            if (x_.Count != y_.Count)
            {
                return false;
            }

            for (int i = 0; i < x_.Count; i++)
            {
                if (!x_[i].Equals(y_[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(PickupAble obj)
        {
            if (obj.metadata == null)
            {
                obj.metadata = obj.GetComponent<AccessibilityMetadata>();
            }
            return obj.metadata.Hash;
        }
    }
}
