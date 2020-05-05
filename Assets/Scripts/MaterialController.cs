using UnityEngine;

/// <summary>
/// Sets material of nodes and controls their vertical positioning to never
/// go further than half a meter from the phone position.
/// </summary>
public class MaterialController : MonoBehaviour
{
    /// <summary>
    /// The mesh renderer used to change materials.
    /// </summary>
    [SerializeField]
    private MeshRenderer meshRenderer = null;

    /// <summary>
    /// Should return true when the node is moving and false when it is not moving.
    /// </summary>
    private bool moving = false;

    /// <summary>
    /// Sets the material to what is set in the parameter on the <see cref="meshRenderer"/>.
    /// </summary>
    /// <param name="mat">The material to be set to.</param>
    public void SetMaterial(Material mat)
    {
        meshRenderer.material = mat;
    }

    /// <summary>
    /// Changes position of the object to never go further than half a meter from the device verically.
    /// So that objects somewhat align with the height of the device.
    /// </summary>
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
