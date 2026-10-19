using UnityEngine;

[NotDocumented]
public class AntennaBehaviour : MonoBehaviour
{
	public Vector2 CorrectUp = Vector2.up;

	public float Fuzziness = 0.15f;

	public float GetSignalStrength()
	{
		float f = Vector2.Dot(CorrectUp, base.transform.right);
		f = Mathf.Abs(f);
		return Mathf.Pow(1f - Mathf.Pow(f, 1f + Fuzziness), 4f * (1f + Fuzziness)).NaNFallback();
	}
}
