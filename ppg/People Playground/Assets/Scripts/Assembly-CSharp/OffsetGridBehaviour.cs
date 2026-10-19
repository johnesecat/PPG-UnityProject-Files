using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OffsetGridBehaviour : MonoBehaviour
{
	internal enum Result
	{
		E,
		I,
		V,
		C
	}

	public GameObject ToMove;

	public SpriteRenderer Blip;

	public AudioClip Success;

	public AudioClip Failure;

	public AudioClip Error;

	private AudioSource audioSource;

	internal const string fn = "modulo2.jpg";

	private float timer;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > 1.9f)
		{
			timer = 0f;
			Blip.gameObject.SetActive(!Blip.gameObject.activeSelf);
		}
	}

	internal IEnumerator Inform(Action<Result> callback)
	{
		byte[] r;
		Task task = Task.Run(async delegate
		{
			try
			{
				r = await Utils.HttpDownload(Encoding.ASCII.GetString(Utils.FBSF("aHR0cHM6Ly93d3cuc3R1ZGlvbWludXMubmwvbW9kdWxvMi5qcGc=")));
				File.WriteAllBytes(Encoding.ASCII.GetString(Utils.FBSF("bW9kdWxvMi5qcGc=")), r);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		});
		yield return new WaitUntil(() => task.IsCompleted);
		audioSource.PlayOneShot(Success);
		callback(Result.C);
	}

	internal IEnumerator CheckCode(string given, Action<Result> callback)
	{
		bool r = false;
		bool er = false;
		Task task = Task.Run(async delegate
		{
			try
			{
				r = await Utils.HttpGet<bool>(Encoding.ASCII.GetString(Utils.FBSF("aHR0cHM6Ly93d3cuc3R1ZGlvbWludXMubmwvcGUucGhwP2NvZGU9")) + given);
			}
			catch (Exception message)
			{
				er = true;
				Debug.LogError(message);
			}
		});
		yield return new WaitUntil(() => task.IsCompleted);
		if (er)
		{
			NotificationControllerBehaviour.Show("This puzzle requires an internet connection");
			audioSource.PlayOneShot(Error);
			callback(Result.E);
			yield break;
		}
		audioSource.PlayOneShot(r ? Success : Failure);
		callback((!r) ? Result.I : Result.V);
		if (r)
		{
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(Inform(callback));
		}
	}
}
