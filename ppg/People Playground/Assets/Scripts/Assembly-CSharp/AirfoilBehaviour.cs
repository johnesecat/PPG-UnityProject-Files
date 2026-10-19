using System;
using System.Linq;
using UnityEngine;

public class AirfoilBehaviour : MonoBehaviour
{
	[Serializable]
	public struct WingType
	{
		public string Name;

		public Sprite Sprite;

		public bool Lifts;

		public float LiftMultiplier;

		public float DragMultiplier;
	}

	[SkipSerialisation]
	public SpriteRenderer SpriteRenderer;

	[SkipSerialisation]
	public WingType[] WingTypes;

	[HideInInspector]
	public string CurrentWingTypeName;

	[HideInInspector]
	[SkipSerialisation]
	public WingType CurrentWingType;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	private float Lift => CurrentWingType.LiftMultiplier * EffectiveVelocity;

	private float EffectiveVelocity => Vector3.ProjectOnPlane(PhysicalBehaviour.rigidbody.velocity, base.transform.up.normalized).magnitude * Mathf.Abs(base.transform.localScale.x);

	private float Drag => 1f + CurrentWingType.DragMultiplier * (1f - Mathf.Abs(Vector2.Dot(PhysicalBehaviour.rigidbody.velocity.normalized, base.transform.right)));

	private void Awake()
	{
		if (WingTypes.Length != 0)
		{
			CurrentWingTypeName = WingTypes[0].Name;
		}
	}

	private void Start()
	{
		SetWingType(GetWingType(CurrentWingTypeName));
		WingType[] wingTypes = WingTypes;
		for (int i = 0; i < wingTypes.Length; i++)
		{
			WingType wing = wingTypes[i];
			ContextMenuButton contextMenuButton = new ContextMenuButton("setWingTo" + wing.Name, "Convert to " + wing.Name, "Convert to " + wing.Name, delegate
			{
				SetWingType(wing);
			});
			contextMenuButton.Condition = () => CurrentWingTypeName != wing.Name;
			ContextMenuButton item = contextMenuButton;
			PhysicalBehaviour.ContextMenuOptions.Buttons.Add(item);
		}
	}

	private WingType GetWingType(string name)
	{
		return WingTypes.FirstOrDefault((WingType w) => w.Name == name);
	}

	private void FixedUpdate()
	{
		Vector2 vector = base.transform.up;
		float num = base.transform.localScale.x * base.transform.localScale.y;
		float magnitude = PhysicalBehaviour.rigidbody.velocity.magnitude;
		Vector2 rhs = ((magnitude == 0f) ? Vector2.right : (PhysicalBehaviour.rigidbody.velocity / magnitude));
		float num2 = Vector2.Dot(vector, rhs) * -1f;
		if (CurrentWingType.Lifts)
		{
			float num3 = Vector2.Dot(base.transform.right, rhs);
			PhysicalBehaviour.rigidbody.AddForce(CurrentWingType.DragMultiplier * num3 * num * magnitude * vector);
		}
		PhysicalBehaviour.rigidbody.AddForce(CurrentWingType.DragMultiplier * num2 * Mathf.Abs(num) * magnitude * vector);
	}

	public void SetWingType(WingType wingType)
	{
		if ((bool)wingType.Sprite)
		{
			CurrentWingType = wingType;
			CurrentWingTypeName = wingType.Name;
			SpriteRenderer.sprite = wingType.Sprite;
			PhysicalBehaviour.RefreshOutline();
		}
	}
}
