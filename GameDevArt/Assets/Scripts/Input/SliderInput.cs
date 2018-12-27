using UnityEngine;
using UnityEngine.UI;
using InControl;

namespace CityBashers
{
	[RequireComponent (typeof (Slider))]
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

		public PlayerActions playerActions;

		void Awake ()
		{
			DontDestroyOnLoadInit.Instance.OnInitialize.AddListener (OnInitialize);
		}

		void Start ()
		{
			slider = GetComponent<Slider> ();
			selectable = GetComponent<Selectable> ();
		}

		void OnInitialize ()
		{
			FetchComponents ();
		}

		void FetchComponents ()
		{
			if (playerActions == null) playerActions = InControlActions.instance.playerActions;
		}

		void Update ()
		{
			if (menuNav.isActiveMenu == true)
			{
				if (menuNav.currentSelectable == selectable)
				{
					if (active == false)
					{
						active = true;
					}
				
					if (playerActions.Move.Value.x != 0)
					{
						slider.value += moveAmount * playerActions.Move.Value.x * Time.unscaledDeltaTime;
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
