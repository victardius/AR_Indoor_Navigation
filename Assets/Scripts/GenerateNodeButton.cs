using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(KeyPoint))]
public class GenerateNodeButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        KeyPoint myScript = (KeyPoint)target;
        if (GUILayout.Button("Build Object"))
        {
            myScript.GenerateNodes();
        }
    }
}
#endif