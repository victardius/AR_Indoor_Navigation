using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathManager : MonoBehaviour
{
    private int currentObjective = 0;

    [SerializeField]
    private KeyPoint[] keyPoints = null;

    [SerializeField]
    private float detectionDistance = 2f;

    [SerializeField]
    private Text text = null;

    [SerializeField]
    private Transform player = null;

    private KeyPoint currentPoint = null;
    private GameObject[] objectives = null;
    private int currentPointIndex = 0;

    private void Awake()
    {
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
        if (currentObjective < objectives.Length)
        {
            if (!objectives[currentObjective].activeSelf)
                objectives[currentObjective].SetActive(true);
            if (Vector3.Distance(objectives[currentObjective].transform.position, player.position) < detectionDistance)
            {
                objectives[currentObjective++].SetActive(false);
            }
        }
        else if (currentPointIndex < keyPoints.Length - 1)
        {
            currentPoint = keyPoints[++currentPointIndex];

            string message = "";

            switch (currentPoint.Type)
            {
                //Play a keypoint type voiceline aswell
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
                objectives = currentPoint.Nodes;
                currentObjective = 0;
            }
        }
    }
}
