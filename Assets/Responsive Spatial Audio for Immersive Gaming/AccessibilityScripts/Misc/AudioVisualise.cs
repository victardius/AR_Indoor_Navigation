using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class AudioVisualise : MonoBehaviour {
    #region Variables
    public Shader curShader;
    [Range(0,1)]
    public float leftRight = 1.0f;
    [Range(0, 1)]
    public float frontBack = 1.0f;
    [Range(0, 2)]
    public float pitch = 1.0f;
    public float noiseXSpeed = 100.0f;
    public float noiseYSpeed = 100.0f;
    public float randomValue = 0.0f;
    public Texture2D blendTex;
    public Texture2D noiseTex;
    private Vector3 targetDirection;
    private GameObject navAgent;
    private GameObject player;
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
    void Start ()
    {
        navAgent = GameObject.Find("NavAgent");
        player = GameObject.FindGameObjectWithTag("Player");
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        if (!curShader && !curShader.isSupported)
        {
            enabled = false;
        }
    }


    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (curShader != null)
        {
            ScreenMat.SetFloat("_LeftRight", leftRight);
            ScreenMat.SetFloat("_FrontBack", frontBack);
            ScreenMat.SetFloat("_Pitch", pitch);
            ScreenMat.SetTexture("_BlendTex", blendTex);
            ScreenMat.SetFloat("_NoiseXSpeed", noiseXSpeed);
            ScreenMat.SetFloat("_NoiseYSpeed", noiseYSpeed);
            ScreenMat.SetFloat("_RandomValue", randomValue);
            ScreenMat.SetTexture("_NoiseTex", noiseTex);

            Graphics.Blit(sourceTexture, destTexture, ScreenMat);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }


    // Update is called once per frame
    void Update ()
    {
        randomValue = Random.Range(-1f, 1f);
        targetDirection = navAgent.transform.position - player.transform.position;
        leftRight =  Vector3.Dot(targetDirection, player.transform.right)>0?1:0;
        frontBack = Mathf.Round(Vector3.Dot(targetDirection, player.transform.right)) == 0 && Mathf.Round(pitch) == 2 || Mathf.Ceil(pitch) == 2 ? 1 : 0;
        pitch = navAgent.GetComponent<AudioSource>().pitch;
    }

    void OnDisable()
    {
        if (screenMat)
        {
            DestroyImmediate(screenMat);
        }
    }


}
