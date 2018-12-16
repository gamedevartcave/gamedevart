using UnityEngine;
using UnityEngine.UI;
using InControl;

namespace CityBashers
{
	public class SliderInput : MonoBehaviour 
	{
		[ReadOnlyAttribute] public bool active;
		public float moveAmount = 1;
		[Space (10)]
		public MenuNavigation menuNav;
		[Space (10)]
		public Slider slider;
		public Selectable selectable;

		public PlayerActions playerActions;

		void Awake ()
		{
			DontDestroyOnLoadInit.Instance.OnInitialize.AddListener (OnInitialize);
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
