using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor class to create a editor button that will generate nodes between 
/// <see cref="KeyPoint"/>s. This should be run from <see cref="KeyPoint"/>
/// objects.
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(KeyPoint))]
public class GenerateNodeButton : Editor
{
    /// <summary>
    /// The function that generates nodes from 
    /// <see cref="KeyPoint.GenerateNodes"/> when pressed.
    /// </summary>
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