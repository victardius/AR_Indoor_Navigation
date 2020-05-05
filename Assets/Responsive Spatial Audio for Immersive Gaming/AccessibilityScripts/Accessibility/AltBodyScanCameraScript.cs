using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Actions;
public class AltBodyScanCameraScript : MonoBehaviour {

    // Use this for initialization
    Camera bodyscancamera;
    public static Plane[] bodyscanPlanes;
    public static GameObject[] bodyscanPlaneGO;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Dispatch.handlers.Remove(typeof(Actions.Accessibility.BodyScanAngleChanged));
        Dispatch.registerHandler(typeof(Actions.Accessibility.BodyScanAngleChanged), ChangeAspectNfov);

    }

    void Start()
    {
        
        Dispatch.registerHandler(typeof(Actions.Accessibility.BodyScanAngleChanged), ChangeAspectNfov);
        bodyscancamera = GameObject.FindGameObjectWithTag("BodyScanCamera").GetComponent<Camera>();
        Dispatch.dispatch(new Actions.Accessibility.BodyScanAngleChanged());
    }
   private AppState ChangeAspectNfov(Base action, AppState state)
    {
        var phi = state.bodyScanHalfAngle * 2;
        var theta = 1f; 
        var aspect = 1f;
        try
        {
            theta = 2 * Mathf.Atan(3.0f * bodyscancamera.transform.localPosition.y / bodyscancamera.nearClipPlane);
            aspect = Mathf.Tan(phi / 2) / Mathf.Tan(theta / 2);
            bodyscancamera.aspect = aspect;
            bodyscancamera.fieldOfView = theta * 180 / Mathf.PI;
            bodyscancamera.farClipPlane = state.bodyScanRadius;
        }
        catch
        {
            bodyscancamera = GameObject.FindGameObjectWithTag("BodyScanCamera").GetComponent<Camera>();
            theta = 2 * Mathf.Atan(3.0f * bodyscancamera.transform.localPosition.y / bodyscancamera.nearClipPlane);
            aspect = Mathf.Tan(phi / 2) / Mathf.Tan(theta / 2);
            bodyscancamera.aspect = aspect;
            bodyscancamera.fieldOfView = theta * 180 / Mathf.PI;
            bodyscancamera.farClipPlane = state.bodyScanRadius;
        }
        
        return state;
    }

    
   }

