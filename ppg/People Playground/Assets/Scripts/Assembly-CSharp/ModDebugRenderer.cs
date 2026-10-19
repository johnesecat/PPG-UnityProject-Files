using UnityEngine;

public class ModDebugRenderer : MonoBehaviour
{
	private Material mat;

	private Camera cam;

	private void Awake()
	{
		Shader shader = Shader.Find("Hidden/Internal-Colored");
		mat = new Material(shader);
		cam = Camera.main;
	}

	private void OnPostRender()
	{
		if (ModAPI.Draw.Actions.Count == 0)
		{
			return;
		}
		GL.PushMatrix();
		if (mat.SetPass(0))
		{
			GL.LoadProjectionMatrix(cam.projectionMatrix);
			GL.Color(Color.magenta);
			while (ModAPI.Draw.Actions.Count > 0)
			{
				ModAPI.Draw.Actions.Pop()();
			}
			GL.PopMatrix();
		}
	}
}
