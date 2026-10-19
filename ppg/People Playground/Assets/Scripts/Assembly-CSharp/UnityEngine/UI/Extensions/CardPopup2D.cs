namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Rigidbody))]
	public class CardPopup2D : MonoBehaviour
	{
		[SerializeField]
		private float rotationSpeed = 1f;

		[SerializeField]
		private float centeringSpeed = 4f;

		[SerializeField]
		private bool singleScene;

		private Rigidbody rbody;

		private bool isFalling;

		private Vector3 cardFallRotation;

		private bool fallToZero;

		private float startZPos;

		private void Start()
		{
			rbody = GetComponent<Rigidbody>();
			rbody.useGravity = false;
			startZPos = base.transform.position.z;
		}

		private void Update()
		{
			if (isFalling)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(cardFallRotation), Time.deltaTime * rotationSpeed);
			}
			if (fallToZero)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 0f, startZPos), Time.deltaTime * centeringSpeed);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * centeringSpeed);
				if (Vector3.Distance(base.transform.position, new Vector3(0f, 0f, startZPos)) < 0.0025f)
				{
					base.transform.position = new Vector3(0f, 0f, startZPos);
					fallToZero = false;
				}
			}
			if (base.transform.position.y < -4f)
			{
				isFalling = false;
				rbody.useGravity = false;
				rbody.velocity = Vector3.zero;
				base.transform.position = new Vector3(0f, 8f, startZPos);
				if (singleScene)
				{
					CardEnter();
				}
			}
		}

		public void CardEnter()
		{
			fallToZero = true;
		}

		public void CardFallAway(float fallRotation)
		{
			rbody.useGravity = true;
			isFalling = true;
			cardFallRotation = new Vector3(0f, 0f, fallRotation);
		}
	}
}
