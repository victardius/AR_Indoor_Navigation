using UnityEngine;
using UnityEngine.UI;

public class AR_DebugTest : MonoBehaviour
{
    [SerializeField]
    private Transform cube = null;

    [SerializeField]
    private Transform arSessionOrigin = null;

    [SerializeField]
    private Transform cam = null;

    private Text text = null;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (text != null)
        {
            text.text = "SessionX: " + arSessionOrigin.transform.position.x + "\nSessionY: " + arSessionOrigin.transform.position.y + "\nSessionZ: " + arSessionOrigin.transform.position.z + "\nCubeX: " + cube.transform.position.x + "\nCubeY: " + cube.transform.position.y + "\nCubeZ: " + cube.transform.position.z + "\nCameraX: " + cam.transform.position.x + "\nCameraY: " + cam.transform.position.y + "\nCameraZ: " + cam.transform.position.z;
        }
    }
}
