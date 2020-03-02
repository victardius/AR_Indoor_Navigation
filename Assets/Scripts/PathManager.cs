using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathManager : MonoBehaviour
{
    private int currentObjective = 0;

    [SerializeField]
    private List<KeyPoint> keyPoints = null;

    [SerializeField]
    private float detectionDistance = 2f;

    [SerializeField]
    private Text text = null;

    [SerializeField]
    private Transform player = null;

    [SerializeField]
    private GameObject keyPointPrefab = null;

    private KeyPoint currentPoint = null;
    private List<GameObject> objectives = null;
    private int currentPointIndex = 0;

    public GameObject CurrentObjective { get { if (currentObjective < objectives.Count) return objectives[currentObjective]; else return objectives[0]; } }
    public bool DestinationReached { get; private set; } = false;

    private void Awake()
    {
        if (keyPoints == null)
            keyPoints = new List<KeyPoint>();

        foreach (KeyValuePair<VectorInfo, KeyPointType> pair in PlayerManager.PositionInfo.Points)
        {
            GameObject point = Instantiate(keyPointPrefab);
            point.transform.position = pair.Key.Position;
            point.GetComponent<KeyPoint>().Type = pair.Value;
            keyPoints.Add(point.GetComponent<KeyPoint>());
        }

        try
        {
            currentPoint = keyPoints[currentPointIndex];
        }catch(NullReferenceException e)
        {
            Debug.Log(e);
        }

        objectives = currentPoint.Nodes;
    }

    private void FixedUpdate()
    {
        if (currentObjective < objectives.Count)
        {
            if (!objectives[currentObjective].activeSelf)
                objectives[currentObjective].SetActive(true);
            if (Vector3.Distance(objectives[currentObjective].transform.position, player.position) < detectionDistance)
            {
                objectives[currentObjective++].SetActive(false);
            }
        }
        else if (currentPointIndex < keyPoints.Count - 1)
        {
            currentPoint = keyPoints[++currentPointIndex];

            string message = "";

            switch (currentPoint.Type)
            {
                //Play a keypoint type voiceline aswell
                case KeyPointType.End:
                    message = "You have reached the destination";
                    DestinationReached = true;
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
                currentObjective = 0;
                objectives = currentPoint.Nodes;
            }
        }
    }
}
