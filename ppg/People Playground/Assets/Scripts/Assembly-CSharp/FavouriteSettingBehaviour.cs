using System;
using UnityEngine;
using UnityEngine.UI;

[Obsolete]
public class FavouriteSettingBehaviour : MonoBehaviour
{
	private Transform QuickAccessContainer;

	public Color Unfavourite = Color.black;

	public Color Favourite = Color.yellow;

	public string Identity;

	public Image Image;

	private GameObject copy;
}
