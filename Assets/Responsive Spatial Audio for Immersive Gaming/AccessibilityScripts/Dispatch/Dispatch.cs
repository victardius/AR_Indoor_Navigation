using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

using ActionHandler = System.Func<Actions.Base, AppState, AppState>;
using System;

/// <summary>
/// Handles events updating the appstate as per the event occurance
/// </summary>
public static class Dispatch
{
	public static Dictionary<System.Type, List<ActionHandler>> handlers 
		= new Dictionary<System.Type, List<ActionHandler>>();
	public static AppState state = AppState.initial;

    
    public static void registerHandler(System.Type t, ActionHandler h) {

		if (!handlers.ContainsKey(t))
		{
			handlers[t] = new List<ActionHandler>();
		}
		if (!handlers[t].Contains(h))
		{
			handlers[t].Add(h); 
		}  
	}

    // Takes an action and passes it to every registered handler
	public static void dispatch(Actions.Base action)
	{
        //Uncomment the line below to see the events getting triggered
		//Debug.Log(action);
		System.Type t = action.GetType();
		if(handlers.ContainsKey(t))
		{
			foreach(ActionHandler handler in handlers[t])
			{
				state = handler(action, state);
			}
		}
	}

	internal static void registerHandler(Type type, Action resetView)
	{
		throw new NotImplementedException();
	}
}
