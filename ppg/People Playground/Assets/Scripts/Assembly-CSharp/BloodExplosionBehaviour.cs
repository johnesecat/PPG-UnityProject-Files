using UnityEngine;

public class BloodExplosionBehaviour : MonoBehaviour
{
	public ParticleSystem Debris1;

	public ParticleSystem Debris2;

	public ParticleSystem ExtraChunks;

	public FancyBloodSplatController FancyBloodSplatController;

	public void SetColor(Color color)
	{
		set(Debris1, color);
		set(Debris2, color);
		set(ExtraChunks, color);
		if ((bool)FancyBloodSplatController)
		{
			FancyBloodSplatController.DecalColourMultiplier = color;
		}
		void set(ParticleSystem ps, Color color)
		{
			ParticleSystem.MainModule main = ps.main;
			main.startColor = color;
		}
	}
}
