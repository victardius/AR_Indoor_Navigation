using System.Collections;
using System;
using System.Collections.Generic;
using Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnInput : MonoBehaviour {

    public EventSystem eventSystem;
    public GameObject selectedObject;

    private bool buttonSelected;
    private void OnEnable()
    {
        ScreenReader.Announcer.Say(gameObject.GetComponentInChildren<Text>().text);
    }
    void Update()
    {
        if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            GameObject go = eventSystem.currentSelectedGameObject;
            Debug.Log(go);
            if(go.GetComponent<Button>() != null)
            {
                string ButtonText = go.GetComponentInChildren<Text>().text;
                Debug.Log(ButtonText);
                ScreenReader.Announcer.Say(ButtonText);
            }
            
        }
    }

    private void OnDisable()
    {
        buttonSelected = false;
    }
}
