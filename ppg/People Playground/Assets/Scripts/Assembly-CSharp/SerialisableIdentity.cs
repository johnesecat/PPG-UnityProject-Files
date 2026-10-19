using System;
using NaughtyAttributes;
using UnityEngine;

public class SerialisableIdentity : MonoBehaviour
{
	public Guid UniqueIdentity;

	[SkipSerialisation]
	[Label("ID")]
	public string IdentityForInspector;

	public void Regenerate()
	{
		UniqueIdentity = Guid.NewGuid();
		IdentityForInspector = UniqueIdentity.ToString();
	}
}
