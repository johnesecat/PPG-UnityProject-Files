using UnityEngine;

public class BloodImpactBehaviour : MonoBehaviour
{
	public ParticleSystem MainSystem;

	public ParticleSystem Debris;

	public ParticleSystem Debris2;

	public ParticleSystem Trail;

	public ParticleSystem UnderWaterPuff;

	public ParticleSystem[] ParticleSystemsToAdjust;

	public FancyBloodSplatController FancyBloodSplatController;

	public void SetColor(Color color)
	{
		if (UserPreferenceManager.Current.GorelessMode)
		{
			color = Utils.ChangeRedToOrange(in color);
		}
		color.a = 0.4f;
		if ((bool)FancyBloodSplatController)
		{
			FancyBloodSplatController.DecalColourMultiplier = color;
		}
		if (ParticleSystemsToAdjust != null)
		{
			for (int i = 0; i < ParticleSystemsToAdjust.Length; i++)
			{
				set(ParticleSystemsToAdjust[i], color);
			}
		}
		void set(ParticleSystem ps, Color color)
		{
			ParticleSystem.MainModule main = ps.main;
			main.startColor = color;
		}
	}
}
