using UnityEngine;

public class TextureSwapTest : MonoBehaviour
{
	public Texture2D skin;

	public Texture2D flesh;

	public Texture2D bone;

	public float scale = 1f;

	public Color bruiseColor;

	public Color bruiseColor2;

	public Color bruiseColor3;

	public Color bloodColor;

	public Color rottenColor;

	private void Start()
	{
		PersonBehaviour component = GetComponent<PersonBehaviour>();
		component.SetBodyTextures(skin, flesh, bone, scale);
		component.SetBruiseColor(bruiseColor.br(), bruiseColor.bg(), bruiseColor.bb());
		component.SetSecondBruiseColor(bruiseColor2.br(), bruiseColor2.bg(), bruiseColor2.bb());
		component.SetThirdBruiseColor(bruiseColor3.br(), bruiseColor3.bg(), bruiseColor3.bb());
		component.SetBloodColour(bloodColor.br(), bloodColor.bg(), bloodColor.bb());
		component.SetRottenColour(rottenColor.br(), rottenColor.bg(), rottenColor.bb());
	}
}
