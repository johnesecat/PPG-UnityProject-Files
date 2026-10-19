using System;
using System.Linq;
using UnityEngine;

public class ThumbnailCreator : MonoBehaviour
{
	public float ThumbnailPadding = 1f;

	public static ThumbnailCreator Main;

	public Camera Camera;

	public Transform Center;

	public Transform Background;

	private GameObject[] gameObjects;

	private void Awake()
	{
		Main = this;
	}

	public Texture2D CreateThumbnail(ObjectState[] states, int resolution)
	{
		Texture2D whiteTexture = Texture2D.whiteTexture;
		try
		{
			gameObjects = ObjectStateConverter.Convert(states, Center.position);
			GameObject[] array = gameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].BroadcastMessage("Start", SendMessageOptions.DontRequireReceiver);
			}
			array = gameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].BroadcastMessage("Update", SendMessageOptions.DontRequireReceiver);
			}
			array = gameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].BroadcastMessage("FixedUpdate", SendMessageOptions.DontRequireReceiver);
			}
			array = gameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				Transform[] componentsInChildren = array[i].GetComponentsInChildren<Transform>();
				foreach (Transform transform in componentsInChildren)
				{
					if (transform.gameObject.layer != 15)
					{
						transform.gameObject.layer = Camera.gameObject.layer;
					}
				}
			}
			whiteTexture = TakePicture(resolution);
			array = gameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i]);
			}
			return whiteTexture;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			Camera.gameObject.SetActive(value: false);
		}
	}

	private Texture2D TakePicture(int resolution)
	{
		Camera.gameObject.SetActive(value: true);
		Vector2 vector = default(Vector2);
		vector.x = gameObjects.Min((GameObject g) => g.transform.position.x) - ThumbnailPadding;
		vector.y = gameObjects.Max((GameObject g) => g.transform.position.y) + ThumbnailPadding;
		Vector2 vector2 = default(Vector2);
		vector2.x = gameObjects.Max((GameObject g) => g.transform.position.x) + ThumbnailPadding;
		vector2.y = gameObjects.Min((GameObject g) => g.transform.position.y) - ThumbnailPadding;
		float num = Mathf.Max(Mathf.Abs(vector.x - vector2.x), Mathf.Abs(vector.y - vector2.y));
		Camera.orthographicSize = num / 2f;
		Background.localScale = Vector3.one * Camera.orthographicSize;
		RenderTexture temporary = RenderTexture.GetTemporary(resolution, resolution, 16);
		Camera.targetTexture = temporary;
		Camera.Render();
		RenderTexture.active = Camera.targetTexture;
		Texture2D texture2D = new Texture2D(resolution, resolution, TextureFormat.RGB24, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, resolution, resolution), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		Camera.targetTexture = null;
		RenderTexture.ReleaseTemporary(temporary);
		Camera.gameObject.SetActive(value: false);
		return texture2D;
	}
}
