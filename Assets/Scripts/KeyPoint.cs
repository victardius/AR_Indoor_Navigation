using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPoint : MonoBehaviour
{
    [SerializeField]
    private KeyPointType type = KeyPointType.Start;

    [SerializeField]
    private GameObject[] nodes = null;

    public GameObject[] Nodes { get { return nodes; } }
    public KeyPointType Type { get { return type; } }
}

public enum KeyPointType
{
    Start,
    Left,
    Right,
    Forward,
    End
}
