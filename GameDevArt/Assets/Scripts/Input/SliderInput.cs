using UnityEngine;
using UnityEngine.UI;

namespace CityBashers
{
	[RequireComponent(typeof(Slider))]
	public class SliderInput : MonoBehaviour
	{
		/// <summary>
		/// Checks if selectable is currently selected.
		/// </summary>
		private bool active;
		/// <summary>
		/// How much slider value moves on input per second.
		/// </summary>
		public float moveAmount = 1;
		/// <summary>
		/// Parent menu navigation to determine if active.
		/// </summary>
		public MenuNavigation menuNav;

		/// <summary>
		/// The slider UI.
		/// </summary>
		private Slider slider;

		/// <summary>
		/// The selectable.
		/// </summary>
		private Selectable selectable;

		void Awake()
		{
			DontDestroyOnLoadInit.Instance.OnInitialize.AddListener(OnInitialize);
		}

		void Start()
		{
			slider = GetComponent<Slider>();
			selectable = GetComponent<Selectable>();
		}

		void OnInitialize()
		{
			AddListeners();
		}

		void AddListeners()
		{
			MenuInput.instance.OnScrollLeft.AddListener(OnScrollLeft);
			MenuInput.instance.OnScrollRight.AddListener(OnScrollRight);
		}

		void OnScrollLeft()
		{
			CheckActiveMenu();

			if (active == true)
			{
				slider.value -= moveAmount;
			}
		}

		void OnScrollRight()
		{
			CheckActiveMenu();

			if (active == true)
			{
				slider.value += moveAmount;
			}
		}

		void CheckActiveMenu()
		{
			if (menuNav.isActiveMenu == true)
			{
				if (menuNav.currentSelectable == selectable)
				{
					if (active == false)
					{
						active = true;
					}
				}

				else

				{
					if (active == true)
					{
						active = false;
					}
				}
			}
		}
	}
}
