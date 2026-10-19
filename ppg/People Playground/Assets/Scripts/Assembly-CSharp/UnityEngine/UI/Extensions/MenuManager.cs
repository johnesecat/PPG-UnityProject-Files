using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Menu Manager")]
	[DisallowMultipleComponent]
	public class MenuManager : MonoBehaviour
	{
		[SerializeField]
		private Menu[] menuScreens;

		[SerializeField]
		private int startScreen;

		private Stack<Menu> menuStack = new Stack<Menu>();

		public Menu[] MenuScreens
		{
			get
			{
				return menuScreens;
			}
			set
			{
				menuScreens = value;
			}
		}

		public int StartScreen
		{
			get
			{
				return startScreen;
			}
			set
			{
				startScreen = value;
			}
		}

		public static MenuManager Instance { get; set; }

		private void Start()
		{
			Instance = this;
			if (MenuScreens.Length > StartScreen)
			{
				GameObject go = CreateInstance(MenuScreens[StartScreen].name);
				OpenMenu(go.GetMenu());
			}
			else
			{
				Debug.LogError("Not enough Menu Screens configured");
			}
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		public GameObject CreateInstance(string MenuName)
		{
			return Object.Instantiate(GetPrefab(MenuName), base.transform);
		}

		public void CreateInstance(string MenuName, out GameObject menuInstance)
		{
			GameObject prefab = GetPrefab(MenuName);
			menuInstance = Object.Instantiate(prefab, base.transform);
		}

		public void OpenMenu(Menu menuInstance)
		{
			if (menuStack.Count > 0)
			{
				if (menuInstance.DisableMenusUnderneath)
				{
					foreach (Menu item in menuStack)
					{
						item.gameObject.SetActive(value: false);
						if (item.DisableMenusUnderneath)
						{
							break;
						}
					}
				}
				Canvas component = menuInstance.GetComponent<Canvas>();
				if (component != null)
				{
					Canvas component2 = menuStack.Peek().GetComponent<Canvas>();
					if (component2 != null)
					{
						component.sortingOrder = component2.sortingOrder + 1;
					}
				}
			}
			menuStack.Push(menuInstance);
		}

		private GameObject GetPrefab(string PrefabName)
		{
			for (int i = 0; i < MenuScreens.Length; i++)
			{
				if (MenuScreens[i].name == PrefabName)
				{
					return MenuScreens[i].gameObject;
				}
			}
			throw new MissingReferenceException("Prefab not found for " + PrefabName);
		}

		public void CloseMenu(Menu menu)
		{
			if (menuStack.Count == 0)
			{
				Debug.LogErrorFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
			}
			else if (menuStack.Peek() != menu)
			{
				Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
			}
			else
			{
				CloseTopMenu();
			}
		}

		public void CloseTopMenu()
		{
			Menu menu = menuStack.Pop();
			if (menu.DestroyWhenClosed)
			{
				Object.Destroy(menu.gameObject);
			}
			else
			{
				menu.gameObject.SetActive(value: false);
			}
			foreach (Menu item in menuStack)
			{
				item.gameObject.SetActive(value: true);
				if (item.DisableMenusUnderneath)
				{
					break;
				}
			}
		}

		private void Update()
		{
			if (UIExtensionsInputManager.GetKeyDown(KeyCode.Escape) && menuStack.Count > 0)
			{
				menuStack.Peek().OnBackPressed();
			}
		}
	}
}
