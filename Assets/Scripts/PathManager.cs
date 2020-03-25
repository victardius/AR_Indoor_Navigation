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

    [SerializeField]
    private VoicelineManager voiceline = null;

    [SerializeField]
    private Material activeMaterial = null;

    [SerializeField]
    private Material disabledMaterial = null;

    private KeyPoint currentPoint = null;
    private List<GameObject> objectives = null;
    private int currentPointIndex = 0;

    public GameObject CurrentObjective { get { if (currentObjective < objectives.Count) return objectives[currentObjective]; else return objectives[0]; } }
    public KeyPoint[] KeyPoints { get { return keyPoints; } }

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

    private void FixedUpdate()
    {
        if (currentObjective < objectives.Count)
        {
            //if (!objectives[currentObjective].activeSelf)
            //{
            //    objectives[currentObjective].GetComponent<MaterialController>().SetMaterial(activeMaterial);
            //    objectives[currentObjective].GetComponentInChildren<AudioSource>().Play();
            //}
            if (Vector3.Distance(objectives[currentObjective].transform.position, player.position) < detectionDistance)
            {
                objectives[currentObjective++].SetActive(false);
                objectives[currentObjective].GetComponent<MaterialController>().SetMaterial(activeMaterial);
                objectives[currentObjective].GetComponentInChildren<AudioSource>().Play();
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
                ActivateObjectives();
            }

            voiceline.PlayVoiceline(currentPoint.Type);
        }
    }

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
