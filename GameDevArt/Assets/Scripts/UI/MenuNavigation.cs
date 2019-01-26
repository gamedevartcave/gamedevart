using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CityBashers
{
	public class MenuNavigation : MonoBehaviour 
	{
		// The current active menu reference.
		public static MenuNavigation ActiveMenu { get; set; }
		// Show we are the active menu.
		[ReadOnly] public bool isActiveMenu;

		// Button assets
		public GameObject backButton;
		public Selectable firstSelectable;
		[ReadOnly] public Selectable currentSelectable;
		public Selectable[] buttons;

		// Misc
		private CanvasGroup canvasGroup;
		public PointerEventData eventData;

		void Awake ()
		{
			// Set up delayed initialization.
			DontDestroyOnLoadInit.Instance.OnInitialize.AddListener (OnInitialize);
		}

		void OnEnable ()
		{
			// Automatically highlight the current selectable when the menu appears.
			if (ActiveMenu == this)
			{
				ButtonEnter (currentSelectable);
			}
		}

		void OnInitialize ()
		{
			FetchComponents ();
			AddListeners();
		}

		/// <summary>
		/// Help with initializing.
		/// </summary>
		void FetchComponents ()
		{
			if (eventData == null) eventData = new PointerEventData (EventSystem.current);
			if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup> ();
			if (currentSelectable == null) currentSelectable = firstSelectable;
		}

		/// <summary>
		/// Add listeners to menu input events rather than checking for them here.
		/// </summary>
		void AddListeners()
		{
			// Scroll events.
			MenuInput.Instance.OnScrollUp.AddListener(OnScrollUp);
			MenuInput.Instance.OnScrollDown.AddListener(OnScrollDown);
			MenuInput.Instance.OnScrollLeft.AddListener(OnScrollLeft);
			MenuInput.Instance.OnScrollRight.AddListener(OnScrollRight);

			// Button events.
			MenuInput.Instance.OnConfirm.AddListener(OnConfirm);
			MenuInput.Instance.OnBack.AddListener(OnBack);
		}
		
		/// <summary>
		/// Set custom button index, when the menu is active, it will select that button index.
		/// </summary>
		/// <param name="index"></param>
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

		/// <summary>
		/// Scroll up event.
		/// </summary>
		void OnScrollUp()
		{
			CheckActiveMenu();

			if (isActiveMenu)
			{
				if (currentSelectable.FindSelectableOnUp() != null)
				{
					ButtonExit(currentSelectable);
					ButtonEnter(currentSelectable.FindSelectableOnUp());
					currentSelectable = currentSelectable.FindSelectableOnUp();
				}
			}
		}

		/// <summary>
		/// Scroll down event.
		/// </summary>
		void OnScrollDown()
		{
			CheckActiveMenu();

			if (isActiveMenu)
			{
				if (currentSelectable.FindSelectableOnDown() != null)
				{
					ButtonExit(currentSelectable);
					ButtonEnter(currentSelectable.FindSelectableOnDown());
					currentSelectable = currentSelectable.FindSelectableOnDown();
				}
			}
		}

		/// <summary>
		/// Scroll left event.
		/// </summary>
		void OnScrollLeft()
		{
			CheckActiveMenu();

			if (isActiveMenu)
			{
				if (currentSelectable.FindSelectableOnLeft() != null)
				{
					ButtonExit(currentSelectable);
					ButtonEnter(currentSelectable.FindSelectableOnLeft());
					currentSelectable = currentSelectable.FindSelectableOnLeft();
				}
			}
		}

		/// <summary>
		/// Scroll right event.
		/// </summary>
		void OnScrollRight()
		{
			CheckActiveMenu();

			if (isActiveMenu)
			{
				if (currentSelectable.FindSelectableOnRight() != null)
				{
					ButtonExit(currentSelectable);
					ButtonEnter(currentSelectable.FindSelectableOnRight());
					currentSelectable = currentSelectable.FindSelectableOnRight();
				}		
			}
		}

		/// <summary>
		/// Confirm/Accept event.
		/// </summary>
		void OnConfirm()
		{
			CheckActiveMenu();

			if (isActiveMenu)
			{
				SubmitButton(currentSelectable);
			}
		}

		/// <summary>
		/// Back event.
		/// </summary>
		void OnBack()
		{
			CheckActiveMenu();

			if (isActiveMenu)
			{
				BackButton();
			}
		}

		/// <summary>
		/// Checks if we are the active menu.
		/// </summary>
		void CheckActiveMenu ()
		{
			// Bail out if not fully visible.
			if (GameController.Instance == null)
			{
				return;
			} 

			else
			
			{
				// Is not active menu.
				if (GameController.Instance.activeMenu != this)
				{
					// There is an active menu.
					if (GameController.Instance.activeMenu != null)
					{
						// Don't be active menu.
						if (isActiveMenu == true)
						{
							isActiveMenu = false;
						}

						return;
					}
				} 

				else // Is active menu already.
				
				{
					// Be active menu.
					if (isActiveMenu == false)
					{
						isActiveMenu = true;
					}
				}
			}
		}
			
		/// <summary>
		/// Invoke Pointer Enter (BaseEventData) from EventTrigger.
		/// </summary>
		/// <param name="_selectable">Selectable.</param>
		void ButtonEnter (Selectable _selectable)
		{
			ExecuteEvents.Execute (_selectable.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
		}
			
		/// <summary>
		/// Invoke Pointer Exit (BaseEventData) from EventTrigger.
		/// </summary>
		/// <param name="_selectable">Selectable.</param>
		void ButtonExit (Selectable _selectable)
		{
			ExecuteEvents.Execute (_selectable.gameObject, eventData, ExecuteEvents.pointerExitHandler);
		}

		/// <summary>
		/// Get current active button and send OnClick () event.
		/// </summary>
		/// <param name="_selectable">Selectable.</param>
		void SubmitButton (Selectable _selectable)
		{
			ExecuteEvents.Execute (_selectable.gameObject, eventData, ExecuteEvents.pointerClickHandler);
		}
			
		/// <summary>
		/// Execute OnClick () event at any button index.
		/// </summary>
		void BackButton ()
		{
			ExecuteEvents.Execute (backButton, eventData, ExecuteEvents.pointerClickHandler);
		}
	}
}
