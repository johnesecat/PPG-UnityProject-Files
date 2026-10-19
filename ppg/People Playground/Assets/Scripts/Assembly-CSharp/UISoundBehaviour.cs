using UnityEngine;
using UnityEngine.UI;

public class UISoundBehaviour : MonoBehaviour
{
	public static UISoundBehaviour Main;

	public AudioSource AudioSource;

	public AudioClip DefaultClip;

	public AudioClip WarningClip;

	public AudioClip ErrorClip;

	public AudioClip ScrollClip;

	private float timeOfLastScrollClip;

	private void Start()
	{
		Main = this;
		Refresh();
	}

	public static void Refresh()
	{
		if ((bool)Main)
		{
			Main.RefreshButtons();
		}
	}

	private void RefreshButtons()
	{
		Button[] array = Resources.FindObjectsOfTypeAll<Button>();
		foreach (Button button in array)
		{
			if (!button.CompareTag("NoSoundButton"))
			{
				try
				{
					button.onClick.RemoveListener(Blip);
				}
				catch
				{
				}
				button.onClick.AddListener(Blip);
			}
		}
		UnityEngine.UI.Toggle[] array2 = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Toggle>();
		foreach (UnityEngine.UI.Toggle toggle in array2)
		{
			try
			{
				toggle.onValueChanged.RemoveListener(playBlip);
			}
			catch
			{
			}
			toggle.onValueChanged.AddListener(playBlip);
		}
		Slider[] array3 = Resources.FindObjectsOfTypeAll<Slider>();
		foreach (Slider slider in array3)
		{
			try
			{
				slider.onValueChanged.RemoveListener(SliderCallback);
			}
			catch
			{
			}
			slider.onValueChanged.AddListener(SliderCallback);
		}
		void playBlip(bool _)
		{
			Blip();
		}
	}

	public void Blip()
	{
		AudioSource.PlayOneShot(DefaultClip);
	}

	public void Error()
	{
		AudioSource.PlayOneShot(ErrorClip);
	}

	public void Warning()
	{
		AudioSource.PlayOneShot(WarningClip);
	}

	public void Scroll()
	{
		timeOfLastScrollClip = Time.unscaledTime;
		AudioSource.PlayOneShot(ScrollClip);
	}

	private void SliderCallback(float v)
	{
		if (Time.unscaledTime - timeOfLastScrollClip > ScrollClip.length * 0.2f || !AudioSource.isPlaying)
		{
			Scroll();
		}
	}
}
