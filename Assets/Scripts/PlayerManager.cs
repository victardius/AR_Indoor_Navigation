using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PathManager pathManager = null;

    [SerializeField]
    [Range(0,1)]
    private float visionArc = 0.7f;

    private bool SeeingNode = true;
    private GameObject nextNode { get { return pathManager.CurrentObjective; } }
    private Vector3 directionToPlayerCamera = Vector3.zero;
    private static PointPositionInfo positionInfo;

    public static PointPositionInfo PositionInfo
    {
        get
        {
            if (positionInfo == null)
            {
                positionInfo = new PointPositionInfo();
                PointPositionInfo.LoadSave();
            }
            return positionInfo;
        }
        set
        {
            positionInfo = value;
        }
    }

    private void FixedUpdate()
    {
        if (!pathManager.DestinationReached) 
        {
            directionToPlayerCamera = transform.position - nextNode.transform.position;
            float dotProduct = Vector3.Dot(directionToPlayerCamera.normalized, transform.forward);

            if (SeeingNode && dotProduct > -visionArc)
            {
                SeeingNode = false;
                Handheld.Vibrate();
            }
            else if (dotProduct < -visionArc)
            {
                SeeingNode = true;
            }
        }
    }

    public void SavePositionPoint(int pointType)
    {
        KeyPointType type = (KeyPointType)pointType;

        PositionInfo.AddKeyPoint(transform.position, type);

        PointPositionInfo.SaveGame();
    }
}
