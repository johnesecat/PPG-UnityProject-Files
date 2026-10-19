using UnityEngine;

public class LayerSerialisationBehaviour : MonoBehaviour
{
	public int Layer = -1;

	private void Start()
	{
		if (Layer == -1)
		{
			Layer = base.gameObject.layer;
		}
		else
		{
			base.gameObject.SetLayer(Layer);
		}
	}
}
