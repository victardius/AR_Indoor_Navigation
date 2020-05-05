using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Keypoints that represent major turns or stops on a path. The <see cref="PathManager"/>
/// uses these as its path while every keypoint keeps track of its following nodes.
/// </summary>
public class KeyPoint : MonoBehaviour
{
    /// <summary>
    /// The type of keypoint this is for example left turn or right.
    /// </summary>
    [SerializeField]
    private KeyPointType type = KeyPointType.Start;

    /// <summary>
    /// The nodes following this keypoint.
    /// </summary>
    [SerializeField]
    private List<GameObject> nodes = null;

    /// <summary>
    /// The next keypoint after this one on the path.
    /// </summary>
    [SerializeField]
    private KeyPoint nextPoint = null;

    /// <summary>
    /// The desired space between nodes.
    /// </summary>
    [SerializeField]
    private float nodeSpacing = 2.5f;

    /// <summary>
    /// The prefab that represents a node on the path.
    /// </summary>
    [SerializeField]
    private Object nodePrefab = null;

    /// <summary>
    /// Gets the list of nodes after this keypoint.
    /// </summary>
    public List<GameObject> Nodes { get { return nodes; } }

    /// <summary>
    /// Gets the type of keypoint this belongs to.
    /// </summary>
    public KeyPointType Type { get { return type; } }

    /// <summary>
    /// The position of this object.
    /// </summary>
    private Vector3 position = Vector3.zero;

    /// <summary>
    /// Generates nodes up to the next keypoint.
    /// </summary>
#if UNITY_EDITOR
    public void GenerateNodes()
    {
        transform.LookAt(nextPoint.transform);
        position = transform.position;
        Vector3 nextPosition;
        Vector3 movement;

        if (nodes.Count > 0)
        {
            foreach(GameObject go in nodes)
            {
                DestroyImmediate(go);
            }
            nodes.Clear();
        }

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
            node.transform.rotation = transform.rotation;
            node.transform.parent = transform;
            nodes.Add(node);
        }
        while (!position.Equals(nextPoint.transform.position));
    }
#endif
}

/// <summary>
/// Enum representing different types of nodes.
/// </summary>
public enum KeyPointType
{
    Start,
    Left,
    Right,
    Forward,
    End
}
