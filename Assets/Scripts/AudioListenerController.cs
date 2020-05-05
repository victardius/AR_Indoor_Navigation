using UnityEngine;

/// <summary>
/// Controls the rotation of the <see cref="AudioListener"/> <see cref="GameObject"/>
/// so that it will disregard rotations up or down and will play audio as if the device 
/// is always being held up.
/// </summary>
public class AudioListenerController : MonoBehaviour
{
    /// <summary>
    /// The transform of the <see cref="GameObject"/> that represents the device where 
    /// the app is being run.
    /// </summary>
    [SerializeField]
    private Transform aRCamera = null;

    /// <summary>
    /// Updates position to ignore X rotations (up and down rotation).
    /// </summary>
    void Update()
    {
        Quaternion rotation = aRCamera.rotation;
        rotation.x = Quaternion.identity.x;
        transform.rotation = rotation;
        transform.position = aRCamera.position;
    }
}
