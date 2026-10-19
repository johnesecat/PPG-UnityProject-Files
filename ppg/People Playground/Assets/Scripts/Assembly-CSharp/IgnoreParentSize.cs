using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class IgnoreParentSize : MonoBehaviour
{
	public Vector2 DesiredSize = Vector2.one;

	private GameObject oldParent;

	private void Start()
	{
		if (!base.transform.parent)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		oldParent = base.transform.parent.gameObject;
		ActOnDestroy orAddComponent = base.transform.parent.gameObject.GetOrAddComponent<ActOnDestroy>();
		if (orAddComponent.DestroyWith == null)
		{
			orAddComponent.DestroyWith = new GameObject[1] { base.gameObject };
		}
		else if (!orAddComponent.DestroyWith.Contains(base.gameObject))
		{
			orAddComponent.DestroyWith = orAddComponent.DestroyWith.Append(base.gameObject).ToArray();
		}
		ParentConstraint component = base.gameObject.GetComponent<ParentConstraint>();
		component.AddSource(new ConstraintSource
		{
			sourceTransform = base.transform.parent,
			weight = 1f
		});
		component.constraintActive = true;
		base.transform.SetParent(null);
		base.transform.localScale = DesiredSize;
	}

	private void Update()
	{
		if (!oldParent)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
