using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DynamicAspect : MonoBehaviour {

    [Range(0.0f, 5.0f)]
    public float aspect;
    private Camera cam;

    public void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void Update()
    {
        if(cam != null && cam.aspect != aspect)
        {
            cam.aspect = aspect;
        }
    }
}
