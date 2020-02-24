using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


// Names all the objects in the surrounding area 
/// <summary>
/// Names all objects in the frustum defined by BODY_SCAN_RADIUS and BODY_HALF_ANGLE constant.
/// While the names are playing the last played object can be selected. Once selected a navigation agent appears which emits a spatial audio beacon. The navigation agent finds the shortest path to the selected object and waits for the player to reach in its vicinity before moving closer to the selected object.
/// The body scan radius and body scan half angle can be changed using this script.
/// </summary>
public class EnvironmentScanner : MonoBehaviour
{
    private AccessibilitySoundGenerator currentlyPlayingObject;
    private Coroutine playingCoRoutine;
    private IEnumerable<AccessibilitySoundGenerator> playList_; // store to allow PlayDelayed arguments to pass through
    Dictionary<string, int> descriptionDictionary;
    private string description;
    private bool detailed_;
    GameObject bodyScanGO;
    Camera bodyScanCamera;
    Plane[] bodyScanPlanes;
    NavMeshSurface navSurface;
    private AudioVisualise audioVisualise;
    private NavMeshAgent agent;
    private GameObject agentChildGlowingOrb;
    private AccessibilityMetadata agentMetaData;
    private EntityDescription selectedObjDescription;
    private PromptRate rate;
    public static bool playedBasicDescription = false;

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
        Dispatch.handlers.Remove(typeof(Actions.Accessibility.PushBasicDescription));
        Dispatch.handlers.Remove(typeof(Actions.User.SelectKeyPressed));
        Dispatch.handlers.Remove(typeof(Actions.User.SelectedItemDetailKeyPressed));
        Dispatch.handlers.Remove(typeof(Actions.User.ReachKeyPressed));
        Dispatch.handlers.Remove(typeof(Actions.User.SignificantMovement));
        Dispatch.handlers.Remove(typeof(Actions.User.IncBodyScanAngleKeyPressed));
        Dispatch.handlers.Remove(typeof(Actions.User.DecBodyScanAngleKeyPressed));

    }


    /// <summary>
    /// Instantiating variables to get references to the camera to be used for bodyscan/environment scan, navsurface for generation of nav mesh at game start and the Nav Agent.
    /// </summary>
    private void Start()
    {
        Dispatch.registerHandler(typeof(Actions.Accessibility.PushBasicDescription), PushBodyScan);
        Dispatch.registerHandler(typeof(Actions.User.SelectKeyPressed), SelectKeyPressed);
        Dispatch.registerHandler(typeof(Actions.User.SelectedItemDetailKeyPressed), SelectedItemDetailKeyPressed);
        Dispatch.registerHandler(typeof(Actions.User.ReachKeyPressed), ReachKeyPressed);
        Dispatch.registerHandler(typeof(Actions.User.SignificantMovement), this.SignificantMovement);
        Dispatch.registerHandler(typeof(Actions.User.IncBodyScanAngleKeyPressed), IncBodyScanAngleKeyPressed);
        Dispatch.registerHandler(typeof(Actions.User.DecBodyScanAngleKeyPressed), DecBodyScanAngleKeyPressed);
        Dispatch.registerHandler(typeof(Actions.User.IncRadiusKeyPressed), IncRadiusKeyPressed);
        Dispatch.registerHandler(typeof(Actions.User.DecRadiusKeyPressed), DecRadiusKeyPressed);
        rate = PromptRate.Medium;
        bodyScanGO = GameObject.FindGameObjectWithTag("BodyScanCamera");
        navSurface = FindObjectOfType<NavMeshSurface>();
        navSurface.BuildNavMesh();
        bodyScanCamera = bodyScanGO.GetComponent<Camera>();
        descriptionDictionary = new Dictionary<string, int>();
        audioVisualise = FindObjectOfType<AudioVisualise>();
        agent = FindObjectOfType<NavMeshAgent>();
        agentChildGlowingOrb = agent.transform.GetChild(0).gameObject;
        agent.gameObject.SetActive(false);
        audioVisualise.enabled = false;
    }


    /// <summary>
    /// Keeping the agent at a set distance from the player until it reaches the selected object. Once reached the agent is disabled until another object is selected in Body Scan.
    /// </summary>
    private void Update()
    {
        if (Vector3.Distance(Camera.main.transform.position, agent.transform.position) > 4.0f)
        {
            agent.speed = 0;
        }
        else
        {
            agent.speed = 20;
            agent.acceleration = 20;
        }

        if (agent.GetComponent<AccessibilitySoundGenerator>().isBeeping)
        {
            agentChildGlowingOrb.SetActive(true);
            audioVisualise.enabled = true;
        }
        else
        {
            agentChildGlowingOrb.SetActive(false);
            audioVisualise.enabled = false;
        }



    }
    private void DrawRay(Vector3 rayStart, Vector3 rayDir)
    {
        Debug.DrawRay(rayStart, rayDir, Color.white, 50.0f, true);

    }


    /// <summary>
    /// If the player moved while Body Scan is playing the list of objects in Body Scan camera frustum, the playback is stopped by invoking CancelPlaybackIfPlaying method
    /// </summary>
    /// <param name="action">A list of predefined in game action which lead to change in the state of the game</param>
    /// <param name="state">The current state of the game</param>
    /// <returns>The existing state without changing</returns>
    private AppState SignificantMovement(Base action, AppState state)
    {
        CancelPlaybackIfPlaying();
        return state;
    }


    /// <summary>
    /// Speaks out all object identified in the bodyscan, stored in playList_ variable.
    /// Gets the basic description of the objects in the playList_ variable, if the object has a pickable component attached to it, it is spoken in a different tone.
    /// </summary>
    private IEnumerator PlayCoRoutine(float bodyscanRadius)
    {
        if (UnityEngine.XR.XRSettings.enabled)
        {
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(0.1f);
        }

        if (playList_ != null)
        {
            ScreenReader.Say("Scan Start");
            yield return new WaitForSeconds(1.5f);
            foreach (var obj in playList_)
            {
                var delay = obj.GetBasicDescriptionLength(rate);
                description = obj.GetComponent<AccessibilityMetadata>().GetBasicDescription();
                if (!descriptionDictionary.ContainsKey(description))
                {
                    descriptionDictionary.Add(description, 1);

                    if (delay > 0)
                    {
                        currentlyPlayingObject = obj;
                        

                        if (!this.detailed_)
                        {
                            if (obj.IsPickupable())
                                obj.PlayBasicDescription(rate, 0.0f, TTSVoice.ZIRA);
                            else
                            {
                                obj.PlayBasicDescription(rate,0.0f);
                            }
                            SetDestinationOfNavAgent(currentlyPlayingObject);
                        }
                        else
                        {
                            if (obj.IsPickupable())
                                obj.PlayAdvancedDescription(TTSVoice.ZIRA);
                            else
                                obj.PlayAdvancedDescription();
                        }
                        if (UnityEngine.XR.XRSettings.enabled)
                            yield return new WaitForSeconds(delay + 1.0f);
                        else
                            yield return new WaitForSeconds(delay + 0.5f);
                    }
                }
            }
            ScreenReader.Say("Scan End");
            currentlyPlayingObject = null;
        }

        descriptionDictionary.Clear();
    }


    /// <summary>
    /// Destination of the Nav Agent is set to the selected object. In addition to that it sets its metadata to the metadata of the selected object, activates itself.
    /// </summary>
    /// <param name="selectedObj"></param>
    private void SetDestinationOfNavAgent(AccessibilitySoundGenerator selectedObj)
    {

        selectedObjDescription = selectedObj.GetComponent<AccessibilityMetadata>().descriptions[0];
        agent.transform.position = Camera.main.transform.position + Camera.main.transform.forward*2;
        agent.gameObject.SetActive(true);
        agent.destination = selectedObj.GetComponentInParent<Transform>().position;
        agentMetaData = agent.GetComponent<AccessibilityMetadata>();
        agentMetaData.SetBasicDescription(selectedObjDescription);
        

    }


    private void PlaySet(IEnumerable<AccessibilitySoundGenerator> playList, float bodyscanRadius, bool detailed = false)
    {
        CancelPlaybackIfPlaying();
        this.playList_ = playList;
        this.detailed_ = detailed;
        if (playList.Count() == 0)
        {
            ScreenReader.Announcer.Say("Empty scan");
            return;
        }

        this.playingCoRoutine = StartCoroutine(PlayCoRoutine(bodyscanRadius));
    }

    public float SignedAngle(Vector3 a, Vector3 b)
    {
        float ang = Mathf.Acos(Vector3.Dot(a, b));
        Vector3 cro = Vector3.Cross(a, b);

        if (cro.y > 0)
        {
            return ang;
        }
        return -ang;
    }

    public bool CheckVisibility(Vector3 playerPos, GameObject collidingObject)
    {
        RaycastHit hit;
        BoxCollider b = collidingObject.GetComponent<BoxCollider>(); //retrieves the Box Collider of the GameObject called obj
        Vector3[] vertex = new Vector3[10];
        vertex[0] = collidingObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f);
        vertex[1] = collidingObject.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f);
        vertex[2] = collidingObject.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f);
        vertex[3] = collidingObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f);
        vertex[4] = collidingObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, -b.size.z) * 0.5f);
        vertex[5] = collidingObject.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, -b.size.z) * 0.5f);
        vertex[6] = collidingObject.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f);
        vertex[7] = collidingObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);
        vertex[8] = collidingObject.transform.TransformPoint(b.center + new Vector3(b.size.x, 0, -b.size.z) * 0.5f);//midpoint of two vertices (for wall intersections)
        vertex[9] = collidingObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, 0, -b.size.z) * 0.5f);//midpoint of two vertices (for wall intersections)
        Ray shotRay;
        Vector3 rayDirection;
        for (int i = -1; i < 10; i++)
        {
            if (i == -1)
                rayDirection = collidingObject.transform.position - playerPos;
            else
                rayDirection = vertex[i] - playerPos;
            //    Debug.Log("playerPos: " + playerPos + " rayDir: " + rayDirection);
            shotRay = new Ray(playerPos, rayDirection);
            if (Physics.Raycast(shotRay, out hit))
            {
                if (hit.collider.gameObject == collidingObject)
                {
                    //Debug.Log("in checkVisibility: " + collidingObject.GetComponent<AccessibilityMetadata>().GetBasicDescription() + "vertex[" + i + "]");

                    Debug.DrawRay(playerPos, rayDirection, Color.white, 1.0f, true);
                    return true;
                }

            }

        }

        return false;
    }

    public float DistanceInXZplane(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        var distance = a - b;
        float distanceInXZplane = distance.magnitude;
        return distanceInXZplane;
    }

    public Vector3 GetDeltaPosInXZplane(Vector3 a, Vector3 b)
    {
        var delta_pos = a - b;
        delta_pos.y = 0;
        return delta_pos;
    }



    public bool LiesAhead(Vector3 a, Vector3 b)
    {
        float ang = SignedAngle(a, b) * 180 / Mathf.PI;

        if (ang > 0)
            return ang <= 100;
        else
            return ang >= -100;
    }
    private AppState PushBodyScan(Base action, AppState state)
    {
        var action_ = action as Actions.Accessibility.PushBasicDescription;
        if (action_ == null)
        {
            throw new System.ArgumentException("Incorrect action routed to: "
                + GetType().ToString()
                //+ " method: " + GetType().DeclaringMethod.ToString()
                + " on GameObject: " + gameObject.name);
        }
        descriptionDictionary.Clear();

        if (this.currentlyPlayingObject != null)
        {
            if (currentlyPlayingObject.isBeeping == true)
            {
                currentlyPlayingObject.isBeeping = false;
                currentlyPlayingObject.GetComponentInParent<AudioSource>().pitch = 1;
                agent.gameObject.SetActive(false);
                currentlyPlayingObject.audioSource.loop = false;
                currentlyPlayingObject = null;
            }
        }

        if (!UnityEngine.XR.XRSettings.enabled)
            bodyScanPlanes = GeometryUtility.CalculateFrustumPlanes(bodyScanCamera);
        else
            bodyScanPlanes = AltBodyScanCameraScript.bodyscanPlanes;

        bool isInBounds = false;
        var objects = GameObject.FindObjectsOfType<AccessibilitySoundGenerator>().Select(x => x.gameObject);
        var player_fwd = Camera.main.transform.forward;
        var player_pos = Camera.main.transform.position;

        var nearby = from GameObject x in objects
                         //where GeometryUtility.TestPlanesAABB(bodyScanPlanes, x.GetComponent<Collider>().bounds)
                     where MathUtils.TestPlanesAABB(bodyScanPlanes, 1 | 2 | 4 | 8 | 16 | 32, x.GetComponent<Collider>(), out isInBounds)
                     let gen = x.GetComponent<AccessibilitySoundGenerator>()
                     where LiesAhead(player_fwd, (gen.transform.position - player_pos).normalized)
                     where CheckVisibility(player_pos, x)
                     orderby SignedAngle(player_fwd, (gen.transform.position - player_pos).normalized)
                     select gen;
        //foreach (var i in nearby)
        //{
        //    Debug.Log(i.GetComponent<AccessibilityMetadata>().GetBasicDescription());
        //}
        PlaySet(nearby, state.bodyScanRadius);
        return state;
    }


    public AppState IncRadiusKeyPressed(Actions.Base action, AppState state)
    {
        if (Mathf.Round(state.bodyScanRadius + AppConstants.Parameters.DELTA_BODY_SCAN_RADIUS) > 30)
        {
            ScreenReader.Announcer.Say("Cannot increase scan radius any further");
            return state;
        }
        state.bodyScanRadius += AppConstants.Parameters.DELTA_BODY_SCAN_RADIUS;
        if (!UnityEngine.XR.XRSettings.enabled)
            bodyScanCamera.farClipPlane = state.bodyScanRadius;
        else
            AltBodyScanCameraScript.bodyscanPlaneGO[5].transform.localPosition = new Vector3(AltBodyScanCameraScript.bodyscanPlaneGO[5].transform.localPosition.x, AltBodyScanCameraScript.bodyscanPlaneGO[5].transform.localPosition.y, state.bodyScanRadius);

        Debug.Log("Radius: " + state.bodyScanRadius);
        ScreenReader.Announcer.Say("Radius: " + state.bodyScanRadius + " metres");
        return state;
    }

    public AppState DecRadiusKeyPressed(Actions.Base action, AppState state)
    {
        if (Mathf.Round(state.bodyScanRadius - AppConstants.Parameters.DELTA_BODY_SCAN_RADIUS) < 0)
        {
            ScreenReader.Announcer.Say("Cannot decrease scan radius any further");
            return state;
        }
        state.bodyScanRadius -= AppConstants.Parameters.DELTA_BODY_SCAN_RADIUS;

        if (!UnityEngine.XR.XRSettings.enabled)
            bodyScanCamera.farClipPlane = state.bodyScanRadius;
        else
            AltBodyScanCameraScript.bodyscanPlaneGO[5].transform.localPosition = new Vector3(AltBodyScanCameraScript.bodyscanPlaneGO[5].transform.localPosition.x, AltBodyScanCameraScript.bodyscanPlaneGO[5].transform.localPosition.y, state.bodyScanRadius);


        // Debug.Log("Radius: " + state.bodyScanRadius);
        ScreenReader.Announcer.Say("Radius: " + state.bodyScanRadius + " metres");
        return state;
    }

    private void IncBodyScanAngle(float degrees)
    {
        AltBodyScanCameraScript.bodyscanPlaneGO[0].transform.Rotate(0, 0, -degrees / 2);
        AltBodyScanCameraScript.bodyscanPlaneGO[1].transform.Rotate(0, 0, degrees / 2);

    }
    public AppState IncBodyScanAngleKeyPressed(Actions.Base action, AppState state)
    {
        if (state.bodyScanHalfAngle == Mathf.PI / 360)
        {
            state.bodyScanHalfAngle += 4 * state.bodyScanHalfAngle;
        }
        else if (Mathf.Round((state.bodyScanHalfAngle + AppConstants.Parameters.DELTA_BODY_SCAN_HALF_ANGLE) * 2 * 180 / Mathf.PI) > 360)
        {
            ScreenReader.Announcer.Say("Cannot increase angle any further");
            return state;
        }
        else
        {
            state.bodyScanHalfAngle += AppConstants.Parameters.DELTA_BODY_SCAN_HALF_ANGLE;
        }
        if (UnityEngine.XR.XRSettings.enabled)
            IncBodyScanAngle(AppConstants.Parameters.DELTA_BODY_SCAN_HALF_ANGLE * 2 * 180 / Mathf.PI);

        Dispatch.dispatch(new Actions.Accessibility.BodyScanAngleChanged());
        ScreenReader.Announcer.Say("Scan angle: " + Mathf.Round(state.bodyScanHalfAngle * 2 * 180 / Mathf.PI) + "degrees");
        return state;
    }

    public AppState DecBodyScanAngleKeyPressed(Actions.Base action, AppState state)
    {
        if (Mathf.Abs(state.bodyScanHalfAngle - AppConstants.Parameters.DELTA_BODY_SCAN_HALF_ANGLE) * 2 * 180 / Mathf.PI < 1.0f)
        {
            state.bodyScanHalfAngle = Mathf.PI / 360;
        }
        else if (Mathf.Round((state.bodyScanHalfAngle - AppConstants.Parameters.DELTA_BODY_SCAN_HALF_ANGLE) * 2 * 180 / Mathf.PI) < 0)
        {
            ScreenReader.Announcer.Say("Cannot decrease angle any further");
            return state;
        }
        else
        {
            state.bodyScanHalfAngle -= AppConstants.Parameters.DELTA_BODY_SCAN_HALF_ANGLE;
        }
        if (UnityEngine.XR.XRSettings.enabled)
            IncBodyScanAngle(-AppConstants.Parameters.DELTA_BODY_SCAN_HALF_ANGLE * 2 * 180 / Mathf.PI);

        Dispatch.dispatch(new Actions.Accessibility.BodyScanAngleChanged());
        ScreenReader.Announcer.Say("Scan angle: " + Mathf.Round(state.bodyScanHalfAngle * 2 * 180 / Mathf.PI) + "degrees");
        return state;
    }

    public void CancelPlaybackIfPlaying()
    {
        if (this.playingCoRoutine != null)
        {
            StopCoroutine(this.playingCoRoutine);
        }
    }

    public AppState SelectKeyPressed(Actions.Base action, AppState state)
    {
        if (this.currentlyPlayingObject != null)
        {
            currentlyPlayingObject = agent.GetComponent<AccessibilitySoundGenerator>();

            if (currentlyPlayingObject.isBeeping == true)
            {
                currentlyPlayingObject.isBeeping = false;
                
                currentlyPlayingObject.GetComponentInParent<AudioSource>().pitch = 1;
                currentlyPlayingObject.audioSource.loop = false;
                currentlyPlayingObject = null;
                agent.gameObject.SetActive(false);
                return state;
            }
            CancelPlaybackIfPlaying();
            state.currentlySelected = this.currentlyPlayingObject;
            state.currentlySelected.PlayBasicDescription(rate, 0.0f);
            StartCoroutine(WaitforHalfSecond(state));
            // Dispatch.dispatch(new Actions.User.ReachKeyPressed());

        }
        return state;
    }

    IEnumerator WaitforHalfSecond(AppState state)
    {
        yield return new WaitForSeconds(1.5f);
        if (state.currentlySelected != null)
        {
            state.currentlySelected.BeepUntilNear();
        }

    }
    public AppState SelectedItemDetailKeyPressed(Actions.Base action, AppState state)
    {
        if (state.currentlySelected != null)
        {
            PlaySet(new List<AccessibilitySoundGenerator> { state.currentlySelected }, state.bodyScanRadius, true);
        }
        return state;
    }

    public AppState ReachKeyPressed(Actions.Base action, AppState state)
    {
        if (state.currentlySelected != null)
        {
            state.currentlySelected.BeepUntilNear();
        }
        return state;
    }

}
