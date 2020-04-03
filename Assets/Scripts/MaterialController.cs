using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer renderer = null;

    private bool moving = false;

    public void SetMaterial(Material mat)
    {
        renderer.material = mat;
    }

    private void Update()
    {
        float playerY = PlayerManager.PlayerPos.position.y;
        float nodeY = transform.position.y;
        float verticalPlayerDistance = Mathf.Abs(nodeY - playerY);
        if (verticalPlayerDistance > 0.5 && !moving)
        {
            moving = true;
        }
        else if(verticalPlayerDistance < 0.1)
        {
            moving = false;
        }

        if (moving)
        {
            float moveDistance = 0.5f;
            if (playerY < nodeY)
                moveDistance *= -1;
            nodeY += moveDistance * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, nodeY, transform.position.z);
        }
    }
}
