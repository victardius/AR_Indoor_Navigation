using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryReducer : MonoBehaviour {

	public float capacity = 10;
	private AudioClip defaultPickupClip;

	void Start () {
		defaultPickupClip = Resources.Load(AppConstants.Resources.Audio.Clips.DEFAULT_PICKUP) as AudioClip;

		if(defaultPickupClip == null)
		{
			throw new System.NullReferenceException("Could not find: " + AppConstants.Resources.Audio.Clips.DEFAULT_PICKUP);
		}

		Dispatch.registerHandler(typeof(Actions.User.InteractKeyPressed), this.AddToBag);
		Dispatch.registerHandler(typeof(Actions.Inventory.ItemCameIntoRange), this.ItemCameIntoRange);
		Dispatch.registerHandler(typeof(Actions.Inventory.ItemWentOutOfRange), this.ItemWentOutofRange);
	}

	private void OnTriggerEnter(Collider collider)
	{
		var pickup = collider.gameObject.GetComponent<PickupAble>();

		if(pickup != null)
		{
			Dispatch.dispatch(new Actions.Inventory.ItemCameIntoRange(pickup));
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		var pickup = collider.gameObject.GetComponent<PickupAble>();

		if (pickup != null)
		{
			Dispatch.dispatch(new Actions.Inventory.ItemWentOutOfRange(pickup));
		}
	}

	private AppState ItemCameIntoRange(Actions.Base action, AppState state)
	{
		var action_ = action as Actions.Inventory.ItemCameIntoRange;
		if(action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: "
				+ GetType().ToString()
				//+ " method: " + GetType().DeclaringMethod.ToString()
				+ " on GameObject: " + gameObject.name);
		}

		if(!state.currentlyReachable.Contains(action_.pickup))
		{
			state.currentlyReachable.Add(action_.pickup);
			if(state.activePickup == null)
			{
				state.activePickup = action_.pickup;
				// dispatch so that GUI can display
				Dispatch.dispatch(new Actions.Inventory.HighlightItem(action_.pickup));
			}
		}

		return state;
	}


	private AppState ItemWentOutofRange(Actions.Base action, AppState state)
	{
		var action_ = action as Actions.Inventory.ItemWentOutOfRange;
		if (action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: "
				+ GetType().ToString()
				//+ " method: " + GetType().DeclaringMethod.ToString()
				+ " on GameObject: " + gameObject.name);
		}

		if (action_.pickup != null && state.currentlyReachable.Contains(action_.pickup))
		{
			state.currentlyReachable.Remove(action_.pickup);
			state.activePickup = null;
			// dispatch highlightoff
			Dispatch.dispatch(new Actions.Inventory.HighlightOffItem(action_.pickup));
		}

		if (state.currentlyReachable.Any())
		{
			// Dispatch available pickup
			var new_pickup = state.currentlyReachable.First();
			state.activePickup = new_pickup;
			// dispatch highlight on
			Dispatch.dispatch(new Actions.Inventory.HighlightItem(new_pickup));
		}

		return state;
	}

	// Given a reference to an object placed in the real world, places it into the internal bag
	private AppState AddToBag(Actions.Base action, AppState state)
	{
		var action_ = action as Actions.User.InteractKeyPressed;
		if (action_ == null)
		{
			throw new System.ArgumentException("Incorrect action routed to: "
				+ GetType().ToString()
				//+ " method: " + GetType().DeclaringMethod.ToString()
				+ " on GameObject: " + gameObject.name);
		}

		var pickup = state.activePickup;

		if (pickup != null)
		{

			// Can't add object if bag is overweight
			if (pickup.weight + state.weight < capacity)
			{
				// Add to the bag and increment the weight
				if (!state.bag.ContainsKey(pickup.pathToPrefab))
				{
                    state.bag.Add(pickup.pathToPrefab, 1);
                }
				else
				{
					state.bag[pickup.pathToPrefab] = state.bag[pickup.pathToPrefab] + 1;
				}
				state.weight += pickup.weight;

				if(pickup.OnPickupSound != null)
				{
					AudioSource.PlayClipAtPoint(pickup.OnPickupSound, Camera.main.transform.position);
				}
				else
				{
					AudioSource.PlayClipAtPoint(defaultPickupClip, Camera.main.transform.position);
				}

				// Need to manually do this because OnTriggerExit is not called automatically
				Dispatch.dispatch(new Actions.Inventory.ItemWentOutOfRange(pickup));
                Dispatch.dispatch(new Actions.Inventory.BagItemUpdated(pickup));
                // Remove all traces of the object
                Destroy(pickup.gameObject);
            }
			else
			{
				Dispatch.dispatch(new Actions.Inventory.BagIsOverweight());
			}
		}

		return state;
	}
}
