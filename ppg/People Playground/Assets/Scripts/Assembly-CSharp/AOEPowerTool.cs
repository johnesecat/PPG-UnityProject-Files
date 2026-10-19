using UnityEngine;

public abstract class AOEPowerTool : ToolBehaviour
{
	protected Collider2D[] buffer = new Collider2D[256];

	protected LayerMask layers;

	protected Vector2 mouseMovement;

	protected Vector2 oldMousePos;

	private GameObject effectObject;

	private ParticleSystem effect;

	private bool hasEffect;

	public virtual float DampeningRadius => 5f;

	public virtual float Radius => 15f;

	public virtual float Force => 550f;

	public virtual float MaxForce => 150f;

	public virtual float TorqueForce => 5f;

	public virtual float MouseMoveInfluence => 45f;

	private void Awake()
	{
		layers = LayerMask.GetMask("Debris", "Objects", "CollidingDebris");
		effectObject = CreateEffectObject();
		hasEffect = effectObject;
		if (hasEffect)
		{
			effect = effectObject.GetComponent<ParticleSystem>();
		}
	}

	private void FixedUpdate()
	{
		mouseMovement = MouseMoveInfluence * ((Vector2)Global.main.MousePosition - oldMousePos);
		oldMousePos = Global.main.MousePosition;
		Debug.DrawLine(Global.main.MousePosition, (Vector2)Global.main.MousePosition + mouseMovement, Color.red);
	}

	private void Update()
	{
		if (hasEffect)
		{
			effectObject.transform.position = Global.main.MousePosition;
		}
	}

	public override void OnFixedHold()
	{
		if (DialogBox.IsAnyDialogboxOpen || Global.ActiveUiBlock || Global.main.UILock || TriggerEditorBehaviour.IsBeingEdited)
		{
			return;
		}
		int num = Physics2D.OverlapCircleNonAlloc(Global.main.MousePosition, Radius, buffer, layers);
		for (int i = 0; i < num; i++)
		{
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(buffer[i].transform, out var value))
			{
				HandleObject(value);
			}
		}
	}

	protected abstract void HandleObject(PhysicalBehaviour phys);

	protected virtual float GetFalloff(float intensity, float sqrDistance, float max)
	{
		return Mathf.Min(max, 4f * intensity / sqrDistance + mouseMovement.sqrMagnitude * 0.05f);
	}

	protected virtual GameObject CreateEffectObject()
	{
		return null;
	}

	public override void OnSelect()
	{
		if (hasEffect)
		{
			effect.Play();
		}
	}

	public override void OnDeselect()
	{
		if (hasEffect)
		{
			effect.Stop();
		}
	}

	public override void OnHold()
	{
	}

	public override void OnToolChosen()
	{
	}

	public override void OnToolUnchosen()
	{
	}
}
