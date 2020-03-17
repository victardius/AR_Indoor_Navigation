using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer renderer = null;

    public void SetMaterial(Material mat)
    {
        renderer.material = mat;
    }
}
