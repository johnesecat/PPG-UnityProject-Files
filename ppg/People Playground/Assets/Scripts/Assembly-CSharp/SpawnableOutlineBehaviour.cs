using System.Collections.Generic;
using UnityEngine;

public class SpawnableOutlineBehaviour : MonoBehaviour
{
	public GameObject RectPrefab;

	public static SpawnableOutlineBehaviour Instance;

	private List<SpriteRenderer> children = new List<SpriteRenderer>();

	private void Awake()
	{
		Instance = this;
	}

	public void SetOutline(ContraptionOutline outline)
	{
		int num = outline.Rectangles.Length;
		int count = children.Count;
		int num2 = 0;
		for (int i = 0; i < children.Count; i++)
		{
			SpriteRenderer spriteRenderer = children[i];
			if (num2 >= num)
			{
				spriteRenderer.gameObject.SetActive(value: false);
				continue;
			}
			RotatedRectangle rotatedRectangle = outline.Rectangles[num2];
			spriteRenderer.gameObject.SetActive(value: true);
			spriteRenderer.size = rotatedRectangle.Size;
			spriteRenderer.transform.localPosition = rotatedRectangle.Center;
			spriteRenderer.transform.localEulerAngles = new Vector3(0f, 0f, rotatedRectangle.AngleDegrees);
			num2++;
		}
		if (count < num)
		{
			for (num2 = count; num2 < num; num2++)
			{
				RotatedRectangle rotatedRectangle2 = outline.Rectangles[num2];
				GameObject obj = Object.Instantiate(RectPrefab, base.transform);
				SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
				obj.transform.localPosition = rotatedRectangle2.Center;
				obj.transform.localEulerAngles = new Vector3(0f, 0f, rotatedRectangle2.AngleDegrees);
				component.size = rotatedRectangle2.Size;
				children.Add(component);
			}
		}
	}
}
