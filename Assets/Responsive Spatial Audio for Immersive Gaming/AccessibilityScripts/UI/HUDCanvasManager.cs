using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDCanvasManager : MonoBehaviour {
    public AudioSource birdMusicSource;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Dispatch.handlers.Remove(typeof(Actions.User.MenuKeyPressed));

        Dispatch.registerHandler(typeof(Actions.User.MenuKeyPressed), this.MenuKeyPressed);
    }
    // Use this for initialization

    void Start () {
        //find child gameobject called "MenuPanel" and set it false
        gameObject.transform.Find("MenuPanel").gameObject.SetActive(false);
        Dispatch.registerHandler(typeof(Actions.User.MenuKeyPressed), this.MenuKeyPressed);

    }

    AppState MenuKeyPressed(Actions.Base action, AppState state)
    {
        var action_ = action as Actions.User.MenuKeyPressed;
        if (action_ == null)
        {
            throw new System.ArgumentException("Incorrect action routed to: "
                + GetType().ToString()
                //+ " method: " + GetType().DeclaringMethod.ToString()
                + " on GameObject: " + gameObject.name);
        }

        state.isMenuDisplayed = !state.isMenuDisplayed;
        gameObject.transform.Find("MenuPanel").gameObject.SetActive(state.isMenuDisplayed);
        //if(state.isMenuDisplayed)
        //    ScreenReader.Announcer.Say("Menu");

        return state;
    }
}
