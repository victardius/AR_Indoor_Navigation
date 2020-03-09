using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KeyPoint : MonoBehaviour
{
    [SerializeField]
    private KeyPointType type = KeyPointType.Start;

    [SerializeField]
    private List<GameObject> nodes = null;

    [SerializeField]
    private KeyPoint nextPoint = null;

    [SerializeField]
    private float nodeSpacing = 2.5f;

    [SerializeField]
    private Object nodePrefab = null;

    public List<GameObject> Nodes { get { return nodes; } }
    public KeyPointType Type { get { return type; } }

    private Vector3 position = Vector3.zero;

#if UNITY_EDITOR
    public void GenerateNodes()
    {
        position = transform.position;
        Vector3 nextPosition;
        Vector3 movement;

        do
        {
            GameObject node = (GameObject)PrefabUtility.InstantiatePrefab(nodePrefab);
            nextPosition = nextPoint.transform.position - position;
            movement = nextPosition.normalized * nodeSpacing;

            if (movement.magnitude > nextPosition.magnitude)
            {
                position += nextPosition;
            }
            else
            {
                position += movement;
            }

            node.transform.position = position;
            node.transform.parent = transform;
            nodes.Add(node);
        }
        while (!position.Equals(nextPoint.transform.position));
    }
#endif
}

public enum KeyPointType
{
    Start,
    Left,
    Right,
    Forward,
    End
}
