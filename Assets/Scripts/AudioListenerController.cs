using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerController : MonoBehaviour
{
    [SerializeField]
    private Transform aRCamera = null;

    void Update()
    {
        Quaternion rotation = aRCamera.rotation;
        rotation.x = Quaternion.identity.x;
        transform.rotation = rotation;
        transform.position = aRCamera.position;
    }
}
