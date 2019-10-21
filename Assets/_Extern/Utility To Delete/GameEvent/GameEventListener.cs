using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	public GameEvent Event;
	public UnityEvent response;

	private void OnEnable()
	{
		if (!Event)
		{
			Debug.Log("ERROR :  forgot to set Event to " + this.gameObject);
			return;
		}
		Event.RegisterListener(this);
	}
	private void OnDisable()
	{
		if (Event)
		{
			Event.UnregisterListener(this);
		}
	}
	public void OnEventRaised()
	{
		response.Invoke();
	}
}
