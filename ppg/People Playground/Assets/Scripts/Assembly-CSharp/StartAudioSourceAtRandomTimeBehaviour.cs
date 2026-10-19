using UnityEngine;

public class StartAudioSourceAtRandomTimeBehaviour : MonoBehaviour
{
	public AudioSource AudioSource;

	private void Start()
	{
		AudioSource.time = AudioSource.clip.length * Random.value;
	}
}
