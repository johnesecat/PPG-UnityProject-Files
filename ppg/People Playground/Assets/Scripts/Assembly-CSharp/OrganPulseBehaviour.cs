using UnityEngine;

public class OrganPulseBehaviour : MonoBehaviour
{
	public CirculationBehaviour circulationController;

	public Renderer renderer;

	private void Update()
	{
		renderer.material.SetFloat("_PulsatingStrength", circulationController.TotalLiquidAmount);
		renderer.material.SetFloat("_PulsatingSpeed", circulationController.TotalLiquidAmount + 1f);
	}
}
