using UnityEngine;

public class FoliageBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public string ShaderPropertyName = "_BurnProgress";

	[SkipSerialisation]
	public SpriteRenderer Renderer;

	[SkipSerialisation]
	public bool SyncWithParentPhysicalBehaviour = true;

	public float BurnProgress;

	private MaterialPropertyBlock materialProperty;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		if (!Renderer)
		{
			Renderer = GetComponent<SpriteRenderer>();
		}
		materialProperty = new MaterialPropertyBlock();
		Renderer.GetPropertyBlock(materialProperty);
		if (SyncWithParentPhysicalBehaviour)
		{
			phys = GetComponentInParent<PhysicalBehaviour>();
		}
	}

	private void Start()
	{
		SetBurnProgressInternal(BurnProgress);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z + Random.value * 0.5f);
	}

	private void OnWillRenderObject()
	{
		if (SyncWithParentPhysicalBehaviour && (bool)phys)
		{
			SetBurnProgress(phys.BurnProgress);
		}
	}

	public void SetBurnProgress(float burnProgress)
	{
		if (!(Mathf.Abs(burnProgress - BurnProgress) < 0.01f))
		{
			SetBurnProgressInternal(burnProgress);
		}
	}

	private void SetBurnProgressInternal(float burnProgress)
	{
		materialProperty.SetFloat(ShaderProperties.Get(ShaderPropertyName), burnProgress);
		Renderer.SetPropertyBlock(materialProperty);
		BurnProgress = burnProgress;
		if (burnProgress >= 0.95f)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
