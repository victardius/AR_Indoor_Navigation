using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

public class AccessibleExtensionForEditor : Editor {


    

    /// <summary>
    /// Instantiates Accessible FPS Controller. This also instantiates the components it depends on if they're not already in the scene. 
    /// The level is set ot 11 to show this option along with the existing components in GameObject option in the menu.
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/GameObject/Accessible FPS Controller", false, 11)]
    static void SpawnAccessibleController()
    {
        if (!GameObject.Find("Responsive Spatial Audio Logo"))
        {
            GameObject logo = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/Responsive Spatial Audio Logo"));
            logo.name = "Responsive Spatial Audio Logo";
        }
        GameObject navMeshSurface = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/NavMeshSurface"));
        //Changing the name from NavMeshSurface (clone) to NavMeshSurface
        navMeshSurface.name = "NavMeshSurface";

        //Getting the reference to the sceneview camera
        SceneView.lastActiveSceneView.Repaint();

        Vector3 spawnPos = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, SceneView.lastActiveSceneView.camera.nearClipPlane+10.0f));

        //If NavAgent is already in the scene we won't instantiate another NavAgent
        if (!GameObject.Find("NavAgent"))
        {
            GameObject navAgent = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/NavAgent"), spawnPos, Quaternion.identity);
            navAgent.name = "NavAgent";
        }
        //Instantiating Accessible FPS Controller
        GameObject accessibleFPS = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/AccessibleFPSController"), spawnPos, Quaternion.identity);
        accessibleFPS.name = "AccessibleFPSController";

        //Destroying the default in game camera since the camera attached to Accessible FPS Controller will be the main camera now
        DestroyImmediate(GameObject.Find("Camera"));
        SceneView.lastActiveSceneView.Repaint();
    }

    
    /// <summary>
    /// Instantiates Audio Compass gameobject at the centre of the editor screen.
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/GameObject/Audio Compass", false, 11)]
    static void SpawnAudioCompass()
    {
        if (!GameObject.Find("Responsive Spatial Audio Logo"))
        {
            GameObject logo = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/Responsive Spatial Audio Logo"));
            logo.name = "Responsive Spatial Audio Logo";
        }
        //Getting the reference to the sceneview camera
        Vector3 spawnPos = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, SceneView.lastActiveSceneView.camera.nearClipPlane + 10.0f));
        GameObject audio_compass = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/AudioCompass"), spawnPos, Quaternion.identity);
        audio_compass.name = "AudioCompass";
    }

    
    /// <summary>
    /// Instantiates a VantagePoint at the centre of the screen. VantagePoint requires a ViewdioManage, it gets instantiated if one doesn't exist in the screen.
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/GameObject/Vantage Point", false, 11)]
    static void SpawnVantagePoint()
    {
        if (!GameObject.Find("Responsive Spatial Audio Logo"))
        {
            GameObject logo = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/Responsive Spatial Audio Logo"));
            logo.name = "Responsive Spatial Audio Logo";
        }
        //Getting the reference to the sceneview camera
        Vector3 spawnPos = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, SceneView.lastActiveSceneView.camera.nearClipPlane + 10.0f));
        GameObject vantagePoint = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/VantagePoint"), spawnPos, Quaternion.identity);
        vantagePoint.name = "VantagePoint";
        if (!GameObject.Find("VantagePointHandler"))
        {
            GameObject vantagePointHandler = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/VantagePointHandler"));
            vantagePointHandler.name = "VantagePointHandler";
        }
    }


    /// <summary>
    /// Instantiates a viewdio handler if one doesn't exist in the scene.
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/GameObject/Vantage Point Handler", false, 11)]
    static void SpawnVantagePointHandler()
    {
        if (!GameObject.Find("Responsive Spatial Audio Logo"))
        {
            GameObject logo = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/Responsive Spatial Audio Logo"));
            logo.name = "Responsive Spatial Audio Logo";
        }
        //VantagePoint handler 
        if (!GameObject.Find("VantagePointHandler"))
        {
            GameObject viewdioHandler = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/VantagePointHandler"));
            viewdioHandler.name = "VantagePointHandler";
        }
    }

    /// <summary>
    /// Instantiates a Bump Noises manager if one doesn't exist in the scene.
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/GameObject/Bump Noises Manager", false, 11)]
    static void SpawnBumpNoisesManager()
    {
        if (!GameObject.Find("Responsive Spatial Audio Logo"))
        {
            GameObject logo = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/Responsive Spatial Audio Logo"));
            logo.name = "Responsive Spatial Audio Logo";
        }
        if (!GameObject.Find("BumpNoisesManager"))
        {
            GameObject bumpManager = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/BumpNoisesManager"));
            bumpManager.name = "BumpNoisesManager";
        }
    }

    /// <summary>
    /// Instantiates a Nav Agent if sn Accessible FPS Controller exists in the scene and Nav Agent doesn't already exist in the scene.
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/GameObject/Nav Agent", false, 11)]
    static void SpawnNavAgent()
    {
        if (!GameObject.Find("Responsive Spatial Audio Logo"))
        {
            GameObject logo = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/Responsive Spatial Audio Logo"));
            logo.name = "Responsive Spatial Audio Logo";
        }
        //Getting the reference to the sceneview camera
        Vector3 spawnPos = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, SceneView.lastActiveSceneView.camera.nearClipPlane + 10.0f));
        if(GameObject.Find("AccessibleFPSController"))
        {
            spawnPos = GameObject.Find("AccessibleFPSController").transform.position;
        }
        if (!GameObject.Find("NavAgent"))
        {
            GameObject navAgent = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/NavAgent"), spawnPos, Quaternion.identity);
            navAgent.name = "NavAgent";
        }
    }

    /// <summary>
    /// Instantiates an Inventory UI in the scene.
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/GameObject/Inventory UI", false, 11)]
    static void SpawnInventoryUI()
    {
        if (!GameObject.Find("Responsive Spatial Audio Logo"))
        {
            GameObject logo = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/Responsive Spatial Audio Logo"));
            logo.name = "Responsive Spatial Audio Logo";
        }
        GameObject hud = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/HUDCanvas"));
        hud.name = "HUDCanvas";
    }

    /// <summary>
    /// Instantiates a Music Manager in the scene.
    /// </summary>
    [MenuItem("Tools/Responsive Spatial Audio/GameObject/Music Manager", false, 11)]
    static void SpawnMusicManager()
    {
        if (!GameObject.Find("Responsive Spatial Audio Logo"))
        {
            GameObject logo = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/Responsive Spatial Audio Logo"));
            logo.name = "Responsive Spatial Audio Logo";
        }
        GameObject musicManager = (GameObject)Instantiate(Resources.Load("Responsive Spatial Audio for Immersive Gaming/Prefabs/Accessible Objects/MusicManager"));
        musicManager.name = "MusicManager";
    }
}
