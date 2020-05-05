using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DeleteResponsiveSpatialAudio : Editor {

	[MenuItem("Tools/Responsive Spatial Audio/Uninstall", false, 800)]
    static void deleteReSAC()
    {
        AssetDatabase.DeleteAsset("Assets/Responsive Spatial Audio for Immersive Gaming");
        AssetDatabase.DeleteAsset("Assets/Resources/Responsive Spatial Audio for Immersive Gaming");
    }
}
