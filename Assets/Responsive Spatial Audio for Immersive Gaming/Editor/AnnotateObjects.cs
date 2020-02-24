using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Unity editor tool for mass annotating objects and setting them as pickables.
/// </summary>
[ExecuteInEditMode]
public class AnnotateObjects : EditorWindow
{
    string existingBasicDescription;
    int count;
    int numberOfObjectsToAnnotate;
    GameObject parentObject;
    List<GameObject> objectsToAnnotate = new List<GameObject>();
    Dictionary<GameObject, List<GameObject>> hierarchy = new Dictionary<GameObject, List<GameObject>>();
    Dictionary<string, List<GameObject>> activeSelection = new Dictionary<string, List<GameObject>>();
    [SerializeField]
    List<EntityDescription> basicDescriptionsKeysTexts = new List<EntityDescription>();
    GUIStyle style = new GUIStyle();
    string[] basicDescriptions = new string[1];
    EntityDescription d = new EntityDescription();
    bool[] pickables = new bool[1];
    bool isColliding = false;


    /// <summary>
    /// Shows the tool window when selected for Window->Annotate GameObjects menu
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/Annotate GameObjects", false, 12)]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnnotateObjects));
    }



    /// <summary>
    /// Checks for the colliding meshes and sets a flag to determine if the meshes are a part of a bigger logical object
    /// </summary>
    /// <param name="sameParentChildObjects"></param>
    void CheckCollision(List<GameObject> sameParentChildObjects)
    {
        foreach (GameObject game in sameParentChildObjects)
        {
            Renderer c = game.GetComponent<Renderer>();
            foreach (GameObject g in sameParentChildObjects)
            {
                Renderer meshToCheckWith = g.GetComponent<Renderer>();

                if (c != meshToCheckWith)
                {

                    if (c.bounds.Intersects(meshToCheckWith.bounds))
                    {

                        isColliding = true;
                        break;
                    }
                }
            }
        }

    }
    ///TODO: give labels to selected object showing if they are annotated



    /// <summary>
    /// Callback triggered when selection is changed in the Unity editor. Removes older references to basicDescriptions and pickable arrays
    /// </summary>
    private void OnSelectionChange()
    {
        objectsToAnnotate.Clear();

        basicDescriptions = new string[numberOfObjectsToAnnotate];
        pickables = new bool[numberOfObjectsToAnnotate];
    }




    /// <summary>
    /// Updates the window with current selection in the editor window, upper half shows the names of the selected game objects, the count of the same gameobject selected and lets the developer add basic description and changing them to pickable.
    /// The lower half shows the details of the selected game objects, their names, whether they are already accessible, existing basic description if they are and whether they are pickable or not.
    /// </summary>
    private void OnGUI()
    {
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Space(5);
        GUILayout.Label("Responsive Spatial Audio for Immersive Gaming", style);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.Label("Add Accessibility Metadata NOW!", EditorStyles.boldLabel);





        hierarchy.Clear();
        try
        {
            basicDescriptionsKeysTexts.Clear();
        }
        catch(System.Exception e)
        {
            Debug.LogError("ignore "+e.Message);
        }
        objectsToAnnotate.Clear();
        activeSelection.Clear();

        foreach (GameObject obj in Selection.gameObjects)
        {

            //for count of gameobjects of the same name
            if (activeSelection.ContainsKey(obj.name))
            {
                activeSelection[obj.name].Add(obj);
            }
            else
            {
                activeSelection.Add(obj.name, new List<GameObject> { obj });
            }

            //build a dictionary of parents and their children out of active selection
            if (obj.transform.parent)
                parentObject = obj.transform.parent.gameObject;
            else
                parentObject = obj;

            if (hierarchy.ContainsKey(parentObject))
            {
                hierarchy[parentObject].Add(obj);
            }
            else
            {
                hierarchy.Add(parentObject, new List<GameObject> { obj });
            }
        }

        foreach (GameObject obj in hierarchy.Keys)
        {
            //Check whether the child mesh of a parent are forming a logical connected object
            if (hierarchy[obj].Count > 1)
            {
                CheckCollision(hierarchy[obj]);
                if (objectsToAnnotate.IndexOf(obj) < 0 && isColliding)
                {
                    objectsToAnnotate.Add(obj);

                }
                else
                {
                    foreach (GameObject o in hierarchy[obj])
                    {
                        if (objectsToAnnotate.IndexOf(o) < 0)
                        {
                            objectsToAnnotate.Add(o);
                        }
                    }
                }

            }

            else
            {
                if (objectsToAnnotate.IndexOf(obj) < 0)
                {
                    objectsToAnnotate.Add(hierarchy[obj][0]);
                }
            }
            isColliding = false;
        }


        numberOfObjectsToAnnotate = objectsToAnnotate.Count;
        System.Array.Resize(ref basicDescriptions, numberOfObjectsToAnnotate);
        System.Array.Resize(ref pickables, numberOfObjectsToAnnotate);


        int counter = 0;

        for (int i = 0; i < objectsToAnnotate.Count; i++)
        {
            GUILayout.BeginHorizontal();
            foreach (GameObject o in hierarchy.Keys)
            {
                if (objectsToAnnotate[i].name == o.name)
                {
                    count = hierarchy[o].Count;
                }
            }
            if (count < 2)
            {
                count = activeSelection[objectsToAnnotate[i].name].Count;
            }
            EditorGUILayout.LabelField(objectsToAnnotate[i].name + "(" + count + ")");


            basicDescriptions[i] = EditorGUILayout.TextField("Basic Description", basicDescriptions[i]);
            pickables[i] = EditorGUILayout.Toggle("Pickable", pickables[i]);

            GUILayout.EndHorizontal();
            count = 0;
            counter++;
        }



        for (int i = 0; i < numberOfObjectsToAnnotate; i++)
        {
            d.key = "Basic";
            try
            {
                d.text = basicDescriptions[i];

            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
            try
            {
                basicDescriptionsKeysTexts.Add(d);

            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }

        }

        if (GUILayout.Button("Make Accessible!", GUILayout.MinHeight(60)))
        {

            for (int i = 0; i < numberOfObjectsToAnnotate; i++)
            {
                if (objectsToAnnotate[i].GetComponent<AccessibilitySoundGenerator>() == null)
                {
                    objectsToAnnotate[i].AddComponent<AccessibilitySoundGenerator>();
                    try
                    {
                        objectsToAnnotate[i].GetComponent<AccessibilityMetadata>().SetBasicDescription(basicDescriptionsKeysTexts[i]);

                    }
                    catch(System.Exception e)
                    {
                        Debug.Log(e.Message);
                    }


                }
                else
                {
                    try
                    {
                        objectsToAnnotate[i].GetComponent<AccessibilityMetadata>().SetBasicDescription(basicDescriptionsKeysTexts[i]);

                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(e.Message);
                    }

                }
                if (pickables[i] && objectsToAnnotate[i].GetComponent<PickupAble>() == null)
                {
                    objectsToAnnotate[i].AddComponent<PickupAble>();
                }
            }
            objectsToAnnotate.Clear();
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Gameobject Name", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Basic Description", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Is Pickable", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();


        foreach (GameObject obj in objectsToAnnotate)
        {

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(obj.name, EditorStyles.label);
            if (obj.GetComponent<AccessibilitySoundGenerator>())
            {
                existingBasicDescription = obj.GetComponent<AccessibilityMetadata>().GetBasicDescription();
                if (existingBasicDescription == "")
                {
                    existingBasicDescription = "no description";
                }

            }
            else
            {
                existingBasicDescription = "not accessible yet";

            }

            EditorGUILayout.LabelField(existingBasicDescription, EditorStyles.label);

            if (obj.GetComponent<PickupAble>())
            {
                EditorGUILayout.LabelField("True", EditorStyles.label);
            }
            else
            {
                EditorGUILayout.LabelField("False", EditorStyles.label);
            }
            GUILayout.EndHorizontal();
        }
    }
}

