using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
public class ImageRecognintionTest : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
    Camera m_WorldSpaceCanvasCamera;

    [SerializeField]
    private Text debugText = null;

    [SerializeField]
    private PathManager pathManager = null;

    ARTrackedImageManager m_TrackedImageManager;

    private int currentPoint = -1;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void FixedUpdate()
    {
        debugText.text = m_WorldSpaceCanvasCamera.transform.position + " ";
    }

    void UpdateInfo(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            Vector3 position = Vector3.zero;
            int pointIndex = int.Parse(trackedImage.referenceImage.name.Substring(0, 2));
            //switch (pointIndex)
            //{
            //    case 0:
            //        position = pathManager.KeyPoints[pointIndex].transform.position;
            //        break;
            //    case 1:
            //        position = new Vector3(0, 0, 5);
            //        break;
            //}
            //transform.position = pathManager.KeyPoints[pointIndex].transform.position;
            //transform.rotation = pathManager.KeyPoints[pointIndex].transform.rotation;
            //m_WorldSpaceCanvasCamera.transform.localPosition = Vector3.zero;
            if (currentPoint != pointIndex)
            {
                pathManager.KeyPoints[pointIndex].transform.position += m_WorldSpaceCanvasCamera.transform.localPosition;
                pathManager.KeyPoints[pointIndex].transform.rotation *= m_WorldSpaceCanvasCamera.transform.localRotation;
                currentPoint = pointIndex;
            }
        }
    }

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

