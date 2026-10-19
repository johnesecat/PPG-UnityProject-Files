using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class JointBreakSoundBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public Joint2D[] Joints;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public AudioClip[] Clips;

	[SkipSerialisation]
	public UnityEvent OnBreak;

	private void Start()
	{
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		if (!base.enabled || !Joints.Contains(joint))
		{
			return;
		}
		if (Clips != null && Clips.Length != 0)
		{
			if (!AudioSource)
			{
				GetComponent<PhysicalBehaviour>().PlayClipOnce(Clips.PickRandom());
			}
			else
			{
				AudioSource.PlayOneShot(Clips.PickRandom());
			}
		}
		OnBreak.Invoke();
	}
}
