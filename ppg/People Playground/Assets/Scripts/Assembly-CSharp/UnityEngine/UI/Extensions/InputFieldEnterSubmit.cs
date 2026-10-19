using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(InputField))]
	[AddComponentMenu("UI/Extensions/Input Field Submit")]
	public class InputFieldEnterSubmit : MonoBehaviour
	{
		[Serializable]
		public class EnterSubmitEvent : UnityEvent<string>
		{
		}

		public EnterSubmitEvent EnterSubmit;

		public bool defocusInput = true;

		private InputField _input;

		private void Awake()
		{
			_input = GetComponent<InputField>();
			_input.onEndEdit.AddListener(OnEndEdit);
		}

		public void OnEndEdit(string txt)
		{
			if (UIExtensionsInputManager.GetKeyDown(KeyCode.Return) || UIExtensionsInputManager.GetKeyDown(KeyCode.KeypadEnter))
			{
				EnterSubmit.Invoke(txt);
				if (defocusInput)
				{
					EventSystem.current.SetSelectedGameObject(null);
				}
			}
		}
	}
}
