using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CheckItems : MonoBehaviour {

    private GameObject sword;
    private GameObject shield;
    private GameObject player;
    public GameObject bagUI;
    private BagDisplayReducer clearInventory;
    private AudioSource trumpet;
    private bool hasEntered = true;
    
	// Use this for initialization
	void Start () {
        trumpet = GetComponent<AudioSource>();
        player = GameObject.Find("AccessibleFPSController");
        clearInventory = bagUI.GetComponent<BagDisplayReducer>();

    }

    // Update is called once per frame
    void Update () {
		if ((Vector3.Distance(player.transform.position, gameObject.transform.position) < 2.0f) && hasEntered)
        {
            hasEntered = false;
            sword = GameObject.Find("Sword");
            shield = GameObject.Find("Shield");


            print(gameObject.name);
            if (sword == null && shield == null)
            {
                trumpet.Play(0);
                ScreenReader.Say("Weapons found, time to save Ally from the Helt");
                Dispatch.state = AppState.initial;
                //ToDo Fadein + Fadeout
                Invoke("LoadLevel", 5);

            }

            else if (shield != null && sword)
            {
                ScreenReader.Say("You must find a shield to protect yourself");
            }

            else
            {
                ScreenReader.Say("There must be weapons somewhere, you have to keep looking");
            }
        }

        if(Vector3.Distance(player.transform.position, gameObject.transform.position)>10.0f)
        {
            hasEntered = true;
        }
	}


    void LoadLevel()
    {
        Instantiate(Resources.Load("Prefabs/Sword/swordPrefab"));
        Instantiate(Resources.Load("Prefabs/Shield/Shield"));
        player.transform.position = new Vector3(-50, 6.36f, -49.89f);
        Vector3 rot = new Vector3(0, 18.14f, 0);
        player.transform.rotation = Quaternion.Euler(rot);
        if (clearInventory != null)
        {
            while (clearInventory.iconList.Count > 0)
            {
                clearInventory.DeleteHolder();
            }
        }
        hasEntered = true;
        
    }
}
