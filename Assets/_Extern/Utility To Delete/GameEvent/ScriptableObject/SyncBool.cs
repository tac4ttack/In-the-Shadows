using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sync/SyncBool")]
public class SyncBool : ScriptableObject {

	[SerializeField] private bool baseValue = false;

	public bool runValue = false;

	private void OnEnable()
	{
		Reset();
	}
	public void Reset()
	{
		runValue = baseValue;
	}
}
