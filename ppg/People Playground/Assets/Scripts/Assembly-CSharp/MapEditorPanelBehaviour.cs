using UnityEngine;

public class MapEditorPanelBehaviour : MonoBehaviour
{
	private void Update()
	{
		if ((bool)MapEditorGlobal.Instance)
		{
			MapBounds mapBounds = MapEditorGlobal.Instance.MapProperties.MapBounds;
			base.transform.localScale = mapBounds.GetSizeInUnits();
			base.transform.position = mapBounds.GetCenterInUnits();
		}
	}
}
