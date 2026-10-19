using UnityEngine;

public class CompilerServerBehaviour : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Object.DontDestroyOnLoad(this);
		CompilerClient.InitialiseConfig();
		CompilerClient.Start();
	}

	private void OnApplicationQuit()
	{
		CompilerClient.Dispose();
	}
}
