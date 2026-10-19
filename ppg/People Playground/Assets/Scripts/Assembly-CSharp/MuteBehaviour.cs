using UnityEngine;

[RequireComponent(typeof(PhysicalBehaviour))]
public class MuteBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public AudioSource[] Sources;

	public bool Muted;

	private void Start()
	{
		GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(new ContextMenuButton("muteAudioSource", () => (!Muted) ? "Mute" : "Unmute", "Toggle sounds", delegate
		{
			SetMute(!Muted);
		}));
		SetMute(Muted);
	}

	public void SetMute(bool v)
	{
		Muted = v;
		if (Sources != null && Sources.Length != 0)
		{
			AudioSource[] sources = Sources;
			for (int i = 0; i < sources.Length; i++)
			{
				sources[i].enabled = !v;
			}
		}
	}
}
