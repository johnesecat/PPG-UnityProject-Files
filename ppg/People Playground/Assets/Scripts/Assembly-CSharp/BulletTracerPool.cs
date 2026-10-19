public class BulletTracerPool : ObjectPoolBehaviour
{
	public static BulletTracerPool Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}
}
