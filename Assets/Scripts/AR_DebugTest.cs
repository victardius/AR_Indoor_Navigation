using UnityEngine;
using UnityEngine.UI;

public class AR_DebugTest : MonoBehaviour
{
    [SerializeField]
    private Transform cube = null;

    private Text text = null;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (text != null)
            text.text = "X: " + cube.transform.position.x + "\nY: " + cube.transform.position.y + "\nZ: " + cube.transform.position.z;
    }
}
