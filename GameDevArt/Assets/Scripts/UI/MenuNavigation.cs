using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CityBashers
{
	public class MenuNavigation : MonoBehaviour 
	{
		public static MenuNavigation activeMenu { get; protected set; }

		// Scrolling
		public float scrollSpeed;
		private float nextScroll;

		// Button assets
		public GameObject backButton;
		public Selectable firstSelectable;
		[ReadOnlyAttribute] public Selectable currentSelectable;
		public Selectable[] buttons;

		// Misc
		private CanvasGroup canvasGroup;
		public PlayerActions playerActions;
		public PointerEventData eventData;

		void Awake ()
		{
			DontDestroyOnLoadInit.Instance.OnInitialize.AddListener (OnInitialize);
			activeMenu = this;
			this.enabled = false;
		}

		void OnEnable ()
		{
			activeMenu = this;
			ButtonEnter (currentSelectable);
		}

		void OnInitialize ()
		{
			FetchComponents ();
		}

		void FetchComponents ()
		{
			if (eventData == null) eventData = new PointerEventData (EventSystem.current);
			if (playerActions == null) playerActions = InControlActions.instance.playerActions;
			if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup> ();
			if (currentSelectable == null) currentSelectable = firstSelectable;
		}
			
		public void SetButtonIndex (int index)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				if (index != i)
				{
					ButtonExit (buttons [i]);
				}
			}
				
			ButtonEnter (buttons [index]);
		}

		void Update ()
		{
			// Bail out if not fully visible.
			if (canvasGroup.alpha != 1 && 
				GameController.instance.activeMenu != this && 
				GameController.instance.activeMenu != null)
			{
				return;
			}

			// UP
			if (playerActions.Up.Value > 0.5f)
			{
				if (Time.unscaledTime > nextScroll)
				{
					if (currentSelectable.FindSelectableOnUp () != null)
					{
						ButtonExit (currentSelectable);
						ButtonEnter (currentSelectable.FindSelectableOnUp ());
						currentSelectable = currentSelectable.FindSelectableOnUp ();					
						nextScroll = Time.unscaledTime + scrollSpeed;
					}
				}
			}

			// DOWN
			if (playerActions.Down.Value > 0.5f)
			{
				if (Time.unscaledTime > nextScroll)
				{
					if (currentSelectable.FindSelectableOnDown () != null)
					{
						ButtonExit (currentSelectable);
						ButtonEnter (currentSelectable.FindSelectableOnDown ());
						currentSelectable = currentSelectable.FindSelectableOnDown ();					
						nextScroll = Time.unscaledTime + scrollSpeed;
					}
				}
			}

			// LEFT
			if (playerActions.Left.Value > 0.5f)
			{
				if (Time.unscaledTime > nextScroll)
				{
					if (currentSelectable.FindSelectableOnLeft () != null)
					{
						ButtonExit (currentSelectable);
						ButtonEnter (currentSelectable.FindSelectableOnLeft ());
						currentSelectable = currentSelectable.FindSelectableOnLeft ();					
						nextScroll = Time.unscaledTime + scrollSpeed;
					}
				}
			}

			// RIGHT
			if (playerActions.Right.Value > 0.5f)
			{
				if (Time.unscaledTime > nextScroll)
				{
					if (currentSelectable.FindSelectableOnRight () != null)
					{
						ButtonExit (currentSelectable);
						ButtonEnter (currentSelectable.FindSelectableOnRight ());
						currentSelectable = currentSelectable.FindSelectableOnRight ();					
						nextScroll = Time.unscaledTime + scrollSpeed;
					}
				}
			}

			// SUBMIT
			if (playerActions.Submit.WasPressed)
			{
				SubmitButton (currentSelectable);
			}

			// BACK
			if (playerActions.Back.WasPressed)
			{
				BackButton ();
			}
		}

		// Invoke Pointer Enter (BaseEventData) from EventTrigger.
		void ButtonEnter (Selectable _selectable)
		{
			ExecuteEvents.Execute (_selectable.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
		}

		// Invoke Pointer Exit (BaseEventData) from EventTrigger.
		void ButtonExit (Selectable _selectable)
		{
			ExecuteEvents.Execute (_selectable.gameObject, eventData, ExecuteEvents.pointerExitHandler);
		}

		// Get current active button and send OnClick() event.
		void SubmitButton (Selectable _selectable)
		{
			ExecuteEvents.Execute (_selectable.gameObject, eventData, ExecuteEvents.pointerClickHandler);
		}

		// Execute OnClick() event at any button index.
		void BackButton ()
		{
			ExecuteEvents.Execute (backButton, eventData, ExecuteEvents.pointerClickHandler);
		}
	}
}
