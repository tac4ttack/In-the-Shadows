using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sync/SyncInt")]
public class SyncInt : ScriptableObject {

	[SerializeField] private int baseValue = 1;

	public int runValue = 0;

	private void OnEnable()
	{
		Reset();
	}
	public void Reset()
	{
		runValue = baseValue;
	}
}
