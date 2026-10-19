using UnityEngine;

public class GasTankBehaviour : FlaskBehaviour
{
	public override Vector2 Limits => new Vector2(0f, 20f);

	public override bool AllowsOverflow => false;
}
