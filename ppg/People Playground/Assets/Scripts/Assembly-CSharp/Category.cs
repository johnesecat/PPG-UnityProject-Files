using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "PPG/Category")]
public class Category : ScriptableObject
{
	public Sprite Icon;

	[Multiline]
	public string Description;

	public bool HadPriorName;

	[ShowIf("HadPriorName")]
	public string PriorName;
}
