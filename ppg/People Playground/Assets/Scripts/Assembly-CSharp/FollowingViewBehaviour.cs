using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class FollowingViewBehaviour : MonoBehaviour
{
	public TextMeshProUGUI Text;

	private void Update()
	{
		if (!Global.main.CameraControlBehaviour.CurrentlyFollowing.Any())
		{
			return;
		}
		if (Global.main.CameraControlBehaviour.CurrentlyFollowing.Count == 1)
		{
			Text.text = Transform(Global.main.CameraControlBehaviour.CurrentlyFollowing[0].name);
			return;
		}
		Text.text = string.Join(", ", Global.main.CameraControlBehaviour.CurrentlyFollowing.Select((PhysicalBehaviour c) => Transform(c.name)).ToArray());
	}

	private string Transform(string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < name.Length; i++)
		{
			string value = name[i].ToString();
			if (char.IsUpper(name[i]) && i != 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(value);
		}
		stringBuilder.Replace("(Clone)", "");
		return stringBuilder.ToString();
	}
}
