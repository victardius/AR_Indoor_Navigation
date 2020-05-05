using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CloseEyes : MonoBehaviour {

    #region Variables
    public Shader curShader;
    public float cutOff = 0.0f;
    float lerp = 0.1f;
    float speed = 4.0f;
    public bool blindMode = false;
    bool isFog = false;

    private Material screenMat;
    #endregion

    #region Properties
    Material ScreenMat
    {
        get
        {
            if (screenMat == null)
            {
                screenMat = new Material(curShader);
                screenMat.hideFlags = HideFlags.HideAndDontSave;
            }
            return screenMat;
        }
    }
    #endregion
    // Use this for initialization
    void Start () {
        
		if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        if(!curShader && !curShader.isSupported)
        {
            enabled = false;
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            blindMode = !blindMode;
        }
        if (blindMode)
        {
            if (cutOff < 1.001f)
                cutOff += lerp * Time.deltaTime * speed;
            else
                cutOff = 1.001f;
        }
        else
        {
            if (cutOff > 0f)
                cutOff -= lerp * Time.deltaTime * speed;
            else
                cutOff = 0f;
        }
        if (Input.GetKeyDown(KeyCode.U))
            isFog = !isFog;

       
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (curShader != null)
        {
            ScreenMat.SetFloat("_CutOff", cutOff);

            Graphics.Blit(sourceTexture, destTexture, ScreenMat);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }
}
