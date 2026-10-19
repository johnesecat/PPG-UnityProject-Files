using UnityEngine;

[ExecuteAlways]
[SkipSerialisation]
public class NixieTubeBehaviour : MonoBehaviour
{
	public SpriteRenderer[] DigitRenderers;

	private MaterialPropertyBlock properties;

	public void SetValue(int number)
	{
		if (properties == null)
		{
			properties = new MaterialPropertyBlock();
		}
		int digitCount = (int)Mathf.Floor(Mathf.Log10(number)) + 1;
		for (int j = 0; j < DigitRenderers.Length; j++)
		{
			SpriteRenderer spriteRenderer = DigitRenderers[j];
			if (!(spriteRenderer == null))
			{
				int num = getDigit(number, j - (DigitRenderers.Length - digitCount));
				spriteRenderer.GetPropertyBlock(properties);
				properties.SetFloat(ShaderProperties.Get("_DigitIndex"), num);
				spriteRenderer.SetPropertyBlock(properties);
			}
		}
		int getDigit(int x, int i)
		{
			if (digitCount < i || float.IsInfinity(digitCount))
			{
				return 0;
			}
			i++;
			return (int)Mathf.Floor((float)x / Mathf.Pow(10f, digitCount - i) % 10f);
		}
	}
}
