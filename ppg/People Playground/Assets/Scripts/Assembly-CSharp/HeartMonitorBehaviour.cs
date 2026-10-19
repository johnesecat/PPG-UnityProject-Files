using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class HeartMonitorBehaviour : MonoBehaviour, Messages.IUse
{
	[ReadOnly]
	public LimbBehaviour ConnectedLimb;

	[SkipSerialisation]
	public AudioSource BeepSource;

	[SkipSerialisation]
	[HideInInspector]
	public bool IsConnected;

	[SkipSerialisation]
	public string NoWireAttachedText = "Attach Blood Wire";

	[SkipSerialisation]
	public TextMeshPro HeartRateText;

	[SkipSerialisation]
	public LineRenderer LineRenderer;

	[SkipSerialisation]
	public float Amplitude = 0.2f;

	[SkipSerialisation]
	public Vector2 LineBounds = new Vector2(0f, 0.8f);

	public bool Activated;

	private const int historyResolution = 20;

	private readonly float[] history = new float[20];

	private const float historyInterval = 0.041666668f;

	private float t;

	private float t2;

	private readonly Vector3[] vertices = new Vector3[20];

	private float currentBPM;

	private void Awake()
	{
		Vector2 vector = new Vector2(LineBounds.x, 0f);
		Vector2 vector2 = new Vector2(LineBounds.y, 0f);
		for (int i = 0; i < 20; i++)
		{
			float num = (float)i / 19f;
			vertices[i] = Vector3.Lerp(vector, vector2, num);
			history[i] = 0f;
		}
	}

	private void Start()
	{
		LineRenderer.positionCount = 20;
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}

	private void UpdateActivation()
	{
		if (Activated)
		{
			HeartRateText.enabled = true;
			return;
		}
		BeepSource.Stop();
		LineRenderer.enabled = false;
		HeartRateText.enabled = false;
	}

	private void Update()
	{
		if (!Activated)
		{
			return;
		}
		LineRenderer.enabled = IsConnected;
		if (!IsConnected)
		{
			HeartRateText.text = NoWireAttachedText;
			if (BeepSource.isPlaying)
			{
				BeepSource.Stop();
			}
		}
		else
		{
			float deltaTime = Time.deltaTime;
			t += deltaTime;
			t2 += deltaTime;
			Measure();
		}
	}

	private void OnEnable()
	{
		UpdateActivation();
	}

	private void OnDisable()
	{
		Activated = false;
		UpdateActivation();
	}

	private void SetLineVertices()
	{
		for (int i = 0; i < 20; i++)
		{
			vertices[i].y = Mathf.Clamp(history[i], -1f, 1f) * Amplitude;
		}
		LineRenderer.SetPositions(vertices);
	}

	private void Measure()
	{
		if (t < 0.041666668f)
		{
			return;
		}
		t = 0f;
		float num = Mathf.Lerp(currentBPM, ConnectedLimb.NodeBehaviour.IsConnectedToRoot ? ConnectedLimb.Person.Heartbeat : 0f, 3f - Mathf.Exp(-0.01f * Time.deltaTime));
		float num2 = 60f / num;
		HeartRateText.text = $"{Mathf.RoundToInt(num)} BPM";
		bool flag = Mathf.RoundToInt(num) <= 0;
		if (flag && !BeepSource.isPlaying)
		{
			BeepSource.Play();
		}
		if (t2 > num2)
		{
			t2 = 0f;
			if (!BeepSource.isPlaying && !flag)
			{
				BeepSource.Play();
			}
			AddToHistory(Random.Range(0.8f, 1f) * Mathf.Clamp01(ConnectedLimb.CirculationBehaviour.BloodFlow));
		}
		else
		{
			if (BeepSource.isPlaying && !flag)
			{
				BeepSource.Stop();
			}
			AddToHistory(Random.value * 0.02f);
		}
	}

	private void AddToHistory(float value)
	{
		for (int i = 0; i < 19; i++)
		{
			history[i] = history[i + 1];
		}
		history[19] = value;
		SetLineVertices();
	}
}
