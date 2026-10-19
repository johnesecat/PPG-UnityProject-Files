using System;
using UnityEngine;

public class DebugTargetDamageMeter : MonoBehaviour, Messages.IShot
{
	private DisplayBehaviour d;

	private void Awake()
	{
		d = GetComponent<DisplayBehaviour>();
	}

	public void Shot(Shot shot)
	{
		d.Value = Math.Round(shot.damage, 3).ToString();
		d.UpdateDisplay();
		ModAPI.Notify(d.Value);
	}
}
