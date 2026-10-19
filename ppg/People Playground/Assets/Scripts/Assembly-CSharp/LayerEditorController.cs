using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LayerEditorController : MonoBehaviour
{
	public GameObject ToyboxToggleContainer;

	public GameObject Container;

	public TextMeshProUGUI OrderDisplay;

	public TextMeshProUGUI LayerDisplay;

	public List<PhysicalBehaviour> Targets = new List<PhysicalBehaviour>();

	public static LayerEditorController Instance { get; private set; }

	public bool IsOpen => Container.activeSelf;

	private void Awake()
	{
		Instance = this;
		Container.SetActive(value: false);
	}

	private void UpdateDisplays()
	{
		int? num = null;
		int? num2 = null;
		bool flag = false;
		bool flag2 = false;
		foreach (PhysicalBehaviour target in Targets)
		{
			if ((bool)target && (bool)target.spriteRenderer)
			{
				target.gameObject.GetOrAddComponent<SerialisedLayerBehaviour>();
				var (num3, num4) = SerialisedLayerBehaviour.GetSortingFor(target);
				if (!num.HasValue)
				{
					num = num3;
				}
				else if (num != num3)
				{
					flag = true;
				}
				if (!num2.HasValue)
				{
					num2 = num4;
				}
				else if (num2 != num4)
				{
					flag2 = true;
				}
			}
		}
		LayerDisplay.text = ((flag || !num.HasValue) ? "--" : num.ToString());
		OrderDisplay.text = ((flag2 || !num2.HasValue) ? "--" : num2.ToString());
	}

	public void Show()
	{
		Container.transform.position = GetTargetPos();
		Container.SetActive(Targets.Any((PhysicalBehaviour d) => d));
		if (Container.activeSelf)
		{
			UpdateDisplays();
		}
	}

	public void Hide()
	{
		Container.SetActive(value: false);
		Targets.Clear();
	}

	private void Update()
	{
		if (Global.ActiveUiBlock || DialogBox.IsAnyDialogboxOpen)
		{
			return;
		}
		if (InputSystem.Down("arrangeSelection"))
		{
			if (IsOpen)
			{
				Hide();
			}
			else
			{
				Targets.Clear();
				Targets.AddRange(SelectionController.Main.SelectedObjects);
				Show();
			}
		}
		if (InputSystem.Down("arrangeFront"))
		{
			BringToFront();
		}
		if (InputSystem.Down("arrangeBack"))
		{
			SendToBack();
		}
		if (InputSystem.Down("arrangeDown"))
		{
			MoveLayer(-1);
		}
		if (InputSystem.Down("arrangeUp"))
		{
			MoveLayer(1);
		}
		if (!IsOpen)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Hide();
		}
		if (Targets.Any())
		{
			if (Targets.All((PhysicalBehaviour d) => !d))
			{
				Container.SetActive(value: false);
			}
			else
			{
				Container.transform.position = GetTargetPos();
			}
		}
		else
		{
			Hide();
		}
	}

	private Vector2 GetTargetPos()
	{
		int num = 0;
		float num2 = 0f;
		float num3 = float.NegativeInfinity;
		foreach (PhysicalBehaviour relevantTarget in GetRelevantTargets())
		{
			if ((bool)relevantTarget)
			{
				num++;
				Vector3 position = relevantTarget.transform.position;
				num2 += position.x;
				num3 = Mathf.Max(num3, position.y);
			}
		}
		return Global.main.camera.WorldToScreenPoint(new Vector3(num2 / (float)num, num3, 0f));
	}

	public void RawOffsetLayer(bool sign)
	{
		foreach (PhysicalBehaviour relevantTarget in GetRelevantTargets())
		{
			if ((bool)relevantTarget && (bool)relevantTarget.spriteRenderer)
			{
				SerialisedLayerBehaviour orAddComponent = relevantTarget.gameObject.GetOrAddComponent<SerialisedLayerBehaviour>();
				var (layervalue, order) = SerialisedLayerBehaviour.GetSortingFor(relevantTarget);
				if (sign ? SortingLayers.GetLayerAbove(layervalue, out var layer) : SortingLayers.GetLayerUnder(layervalue, out layer))
				{
					orAddComponent.SetOrder(layer.id, order);
				}
			}
		}
		UpdateDisplays();
	}

	public void RawOffsetOrder(bool sign)
	{
		foreach (PhysicalBehaviour relevantTarget in GetRelevantTargets())
		{
			if ((bool)relevantTarget && (bool)relevantTarget.spriteRenderer)
			{
				SerialisedLayerBehaviour orAddComponent = relevantTarget.gameObject.GetOrAddComponent<SerialisedLayerBehaviour>();
				var (layerId, num) = SerialisedLayerBehaviour.GetSortingFor(relevantTarget);
				orAddComponent.SetOrder(layerId, num + (sign ? 1 : (-1)));
			}
		}
		UpdateDisplays();
	}

	public void MoveLayer(int delta)
	{
		if (delta != 0)
		{
			foreach (PhysicalBehaviour relevantTarget in GetRelevantTargets())
			{
				if ((bool)relevantTarget && (bool)relevantTarget.spriteRenderer)
				{
					relevantTarget.gameObject.GetOrAddComponent<SerialisedLayerBehaviour>().MoveLayer(delta);
				}
			}
		}
		UpdateDisplays();
	}

	private IEnumerable<PhysicalBehaviour> GetRelevantTargets()
	{
		if (!IsOpen)
		{
			return SelectionController.Main.SelectedObjects;
		}
		return Targets;
	}

	public void BringToFront()
	{
		foreach (PhysicalBehaviour relevantTarget in GetRelevantTargets())
		{
			if ((bool)relevantTarget && (bool)relevantTarget.spriteRenderer)
			{
				relevantTarget.gameObject.GetOrAddComponent<SerialisedLayerBehaviour>().BringToFront();
			}
		}
		UpdateDisplays();
	}

	public void SendToBack()
	{
		foreach (PhysicalBehaviour relevantTarget in GetRelevantTargets())
		{
			if ((bool)relevantTarget && (bool)relevantTarget.spriteRenderer)
			{
				relevantTarget.gameObject.GetOrAddComponent<SerialisedLayerBehaviour>().SendToBack();
			}
		}
		UpdateDisplays();
	}
}
