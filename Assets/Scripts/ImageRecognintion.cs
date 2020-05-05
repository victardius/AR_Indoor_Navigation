using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Resets position that is set depending on identified images in <see cref="IReferenceImageLibrary"/>.
/// </summary>
public class ImageRecognintion : MonoBehaviour
{
    /// <summary>
    /// The <see cref="GameObject"/> that represents the device and its facing.
    /// </summary>
    [SerializeField]
    [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
    Camera m_WorldSpaceCanvasCamera;

    /// <summary>
    /// The <see cref="PathManager"/> that keeps track of the current path being traversed.
    /// </summary>
    [SerializeField]
    private PathManager pathManager = null;

    /// <summary>
    /// The image tracking manager from the ARFoundation package.
    /// </summary>
    ARTrackedImageManager m_TrackedImageManager;

    /// <summary>
    /// The current point used for comparisons to the image library.
    /// </summary>
    private int currentPoint = -1;

    /// <summary>
    /// Assigns the <see cref="ARTrackedImageManager"/>.
    /// </summary>
    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    /// <summary>
    /// Adds <see cref="OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs)"/> to the tracked image 
    /// events.
    /// </summary>
    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    /// <summary>
    /// Removes <see cref="OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs)"/> from the tracked image 
    /// events.
    /// </summary>
    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    /// <summary>
    /// Updates positions related to an identified image.
    /// </summary>
    /// <param name="trackedImage"></param>
    void UpdateInfo(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            int pointIndex = int.Parse(trackedImage.referenceImage.name.Substring(0, 2));
            
            if (currentPoint != pointIndex)
            {
                pathManager.KeyPoints[pointIndex].transform.position += m_WorldSpaceCanvasCamera.transform.localPosition;
                pathManager.KeyPoints[pointIndex].transform.rotation *= m_WorldSpaceCanvasCamera.transform.localRotation;
                currentPoint = pointIndex;
            }
        }
    }

    /// <summary>
    /// Runs <see cref="UpdateInfo(ARTrackedImage)"/> when a tracked image is found.
    /// </summary>
    /// <param name="eventArgs"></param>
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
            UpdateInfo(trackedImage);
    }
}

