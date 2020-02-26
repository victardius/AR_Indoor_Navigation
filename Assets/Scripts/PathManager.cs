using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathManager : MonoBehaviour
{
    private int currentObjective = 0;

    [SerializeField]
    private GameObject[] objectives = null;

    [SerializeField]
    private float detectionDistance = 2f;

    [SerializeField]
    private Text text = null;

    private void FixedUpdate()
    {
        if (currentObjective < objectives.Length)
        {
            if (!objectives[currentObjective].activeSelf)
                objectives[currentObjective].SetActive(true);
            if (Vector3.Distance(objectives[currentObjective].transform.position, transform.position) < detectionDistance)
            {
                objectives[currentObjective++].SetActive(false);
            }
        }
        else
        {
            text.text = "You have reached the destination!";
        }
    }
}
