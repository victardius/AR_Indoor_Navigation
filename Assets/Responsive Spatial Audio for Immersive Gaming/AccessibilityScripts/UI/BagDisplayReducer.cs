using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BagDisplayReducer : MonoBehaviour {

    private GameObject bagItemImageHolder;
    private const float interIconSpacing = 30.0f;
    private const float panelPadding = 25.6f;
    public List<GameObject> iconList = new List<GameObject>();
	private Image m_backgroundImage;
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
        Dispatch.handlers.Remove(typeof(Actions.Inventory.BagItemUpdated));

        Dispatch.registerHandler(typeof(Actions.Inventory.BagItemUpdated), BagItemUpdated);
    }
    void Start()
    {
        bagItemImageHolder = Resources.Load(AppConstants.Resources.IconHolderPrefab) as GameObject;
        if(bagItemImageHolder == null)
        {
            throw new System.NullReferenceException("Resource not found: " + AppConstants.Resources.IconHolderPrefab);
        }
        Dispatch.registerHandler(typeof(Actions.Inventory.BagItemUpdated), BagItemUpdated);

		// Make the parent start out hidden
		//m_backgroundImage = GetComponent<Image>() as Image;
		//if(m_backgroundImage == null)
		//{
		//	throw new System.NullReferenceException("Could not find Image on gameObject: " + gameObject.name);
		//}
		gameObject.SetActive(false);
	}


    private void AddHolder()
    {
        var offset_y = transform.position.y;
        var offset_x = bagItemImageHolder.GetComponent<RectTransform>().anchoredPosition.x;
        var i = iconList.Count;
        var go = Instantiate(bagItemImageHolder, new Vector3(offset_x + panelPadding + i * interIconSpacing, offset_y, 0), Quaternion.identity, this.transform);
        if (go == null)
        {
            throw new System.NullReferenceException("Prefab was null " + AppConstants.Resources.IconHolderPrefab + "or failed to Instantiate");
        }
        iconList.Add(go);
    }

    public void DeleteHolder()
    {
        if (iconList.Any())
        {
            GameObject lastHolder = iconList.Last();
            iconList.RemoveAt(iconList.Count - 1);
            Destroy(lastHolder);
        }
    }

    AppState BagItemUpdated(Actions.Base action, AppState state)
    {
        var action_ = action as Actions.Inventory.BagItemUpdated;
        if (action_ == null)
        {
            throw new System.ArgumentException("Incorrect action routed to:"
                + GetType().ToString()
                + " on GameObject: " + gameObject.name);
        }

        // Assuming that no 0 count objects exist
        int bag_item_count = state.bag.Keys.Count;
        int max_items_ui = iconList.Count;

		if (bag_item_count == 0)
		{
			gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(true);
		}

		// add enough bagItemImageHolders to fit
		while (bag_item_count > max_items_ui)
        {
            AddHolder(); // append a new icon to the list
            max_items_ui++;
        }
        // destroy excess holders
        while(bag_item_count < max_items_ui)
        {
            DeleteHolder(); // pop off and destroy the last icon
            max_items_ui--;
        }

        int i = 0;
        foreach(var pair in state.bag)
        {
            if(i >= iconList.Count)
            {
                break;
            }
            var prefabKey = pair.Key;
            var prefab = PrefabCache.getPrefab(prefabKey).GetComponent<PickupAble>();
            var icon = PrefabCache.getPrefab(prefab.pathToImageIcon).GetComponent<RawImage>();

            iconList[i].GetComponent<RawImage>().texture = icon.texture;
            i++; 
           
        }
        return state;
    }
}
