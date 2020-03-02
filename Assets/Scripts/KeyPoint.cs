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
    public KeyPointType Type { get { return type; } set { type = value; } }

    private Vector3 position = Vector3.zero;

    private void Start()
    {
        if (Type != KeyPointType.End)
        {
            GenerateNodes();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Generate nodes")]
#endif
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
}

[System.Serializable]
public enum KeyPointType
{
    Start,
    Left,
    Right,
    Forward,
    End
}
