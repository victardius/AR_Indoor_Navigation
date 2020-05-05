using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The path manager that keeps track of current objectives and points.
/// </summary>
public class PathManager : MonoBehaviour
{
    /// <summary>
    /// The current objecive or node where the user was at last.
    /// </summary>
    private int currentObjective = 0;

    /// <summary>
    /// A list of keypoints in the scene.
    /// </summary>
    [SerializeField]
    private KeyPoint[] keyPoints = null;

    /// <summary>
    /// The detection distance for nodes.
    /// </summary>
    [SerializeField]
    private float detectionDistance = 2f;

    /// <summary>
    /// The text object displaying subtitles for voice lines.
    /// </summary>
    [SerializeField]
    private Text text = null;

    /// <summary>
    /// The object or transform representing the device.
    /// </summary>
    [SerializeField]
    private Transform player = null;

    /// <summary>
    /// The object that manages voicelines and plays them when reaching <see cref="KeyPoints"/>.
    /// </summary>
    [SerializeField]
    private VoicelineManager voiceline = null;

    /// <summary>
    /// The material that represents an active status.
    /// </summary>
    [SerializeField]
    private Material activeMaterial = null;

    /// <summary>
    /// The material that represents an disabled status.
    /// </summary>
    [SerializeField]
    private Material disabledMaterial = null;

    /// <summary>
    /// The last key point reached.
    /// </summary>
    private KeyPoint currentPoint = null;

    /// <summary>
    /// A list of nodes up to the next keypoint.
    /// </summary>
    private List<GameObject> objectives = null;

    /// <summary>
    /// The index of the current <see cref="KeyPoint"/> in <see cref="KeyPoints"/>.
    /// </summary>
    private int currentPointIndex = 0;

    /// <summary>
    /// Gets the last node that was reached.
    /// </summary>
    public GameObject CurrentObjective { get { if (currentObjective < objectives.Count) return objectives[currentObjective]; else return objectives[0]; } }

    /// <summary>
    /// Gets the keypoints used in the scene.
    /// </summary>
    public KeyPoint[] KeyPoints { get { return keyPoints; } }

    /// <summary>
    /// Sets the <see cref="currentPoint"/> and acivates objectives.
    /// </summary>
    private void Awake()
    {
        try
        {
            currentPoint = keyPoints[currentPointIndex];
        }catch(NullReferenceException e)
        {
            Debug.Log(e);
        }

        ActivateObjectives();
    }

    /// <summary>
    /// Manages keypoints and nodes as well as plays feedback depending on current node and keypoint.
    /// </summary>
    private void FixedUpdate()
    {
        if (currentObjective < objectives.Count)
        {
            
            if (Vector3.Distance(objectives[currentObjective].transform.position, player.position) < detectionDistance)
            {
                objectives[currentObjective++].SetActive(false);
                if (currentObjective < objectives.Count)
                {
                    objectives[currentObjective].GetComponent<MaterialController>().SetMaterial(activeMaterial);
                    objectives[currentObjective].GetComponentInChildren<AudioSource>().Play();
                }
            }
        }
        else if (currentPointIndex < keyPoints.Length - 1)
        {
            currentPoint = keyPoints[++currentPointIndex];

            string message = "";

            switch (currentPoint.Type)
            {
                case KeyPointType.End:
                    message = "You have reached the destination";
                    break;
                case KeyPointType.Left:
                    message = "Turn left";
                    break;
                case KeyPointType.Right:
                    message = "Turn right";
                    break;
                case KeyPointType.Forward:
                    message = "Continue on straight ahead";
                    break;
            }

            text.text = message;

            if (currentPoint.Type != KeyPointType.End)
            {
                ActivateObjectives();
            }

            voiceline.PlayVoiceline(currentPoint.Type);
        }
    }

    /// <summary>
    /// Activates nodes following the current <see cref="KeyPoint"/>.
    /// </summary>
    private void ActivateObjectives()
    {
        objectives = currentPoint.Nodes;
        currentObjective = 0;

        foreach (GameObject gO in objectives)
        {
            gO.GetComponent<MaterialController>().SetMaterial(disabledMaterial);
            gO.SetActive(true);
        }

        if (objectives.Count > 0)
        {
            objectives[currentObjective].GetComponent<MaterialController>().SetMaterial(activeMaterial);
            objectives[currentObjective].GetComponentInChildren<AudioSource>().Play();
        }
    }
}
