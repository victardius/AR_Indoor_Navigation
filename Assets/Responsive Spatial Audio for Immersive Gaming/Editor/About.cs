using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class About : EditorWindow
{
    GUIStyle style = new GUIStyle();
    Vector2 scrollPos;
    string t = " Mainstream video games are predominantly inaccessible to people who are blind or of low vision (PBLV) \nexcluding them from both the enjoyment of such games and participation in diverse communities built \naround such games.\n We have built a new interaction toolkit called Responsive Spatial Audio for Immersive Gaming, developed \naround spatial audio technology, that enables PBLV to enjoy playing mainstream video games. Responsive \nSpatial Audio for Immersive Gaming was developed at Microsoft Research India by Manohar Swaminathan \nand his team.\n Manohar Swaminathan is a senior researcher who is part of the Technologies for Emerging Markets group \nat MSR India. His primary research interests are in virtual and augmented reality technologies. Currently, \nhe is focused on applying these for empowering people with disabilities. His other interest is in the use of \nIoT and cloud for large scale impact in emerging markets. Prior to joining MSR, Manohar’s career has \nspanned the worlds of research, academia, and entrepreneurship.";



   [MenuItem("Tools/Responsive Spatial Audio/About")]

    static void Init()
    {
        GetWindowWithRect(typeof(About), new Rect(0,0, 624f, 250f));
    }


    void OnGUI()
    {

        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        GUILayout.Space(5);
        GUILayout.Label(" Responsive Spatial Audio for Immersive Gaming, a Microsoft Garage Project", style);
        GUILayout.Space(5);
        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUI.skin.button.wordWrap = true;
        EditorStyles.label.wordWrap = true;
        GUILayout.Label(t);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();

    }
}
