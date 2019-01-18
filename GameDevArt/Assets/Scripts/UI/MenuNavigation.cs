using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CityBashers
{
	public class MenuNavigation : MonoBehaviour 
	{
		public static MenuNavigation activeMenu { get; protected set; }
		[ReadOnlyAttribute] public bool isActiveMenu;

		// Button assets
		public GameObject backButton;
		public Selectable firstSelectable;
		[ReadOnlyAttribute] public Selectable currentSelectable;
		public Selectable[] buttons;

		// Misc
		private CanvasGroup canvasGroup;
		public PointerEventData eventData;

		void Awake ()
		{
			DontDestroyOnLoadInit.Instance.OnInitialize.AddListener (OnInitialize);
		}

		void OnEnable ()
		{
			if (activeMenu == this)
			{
				ButtonEnter (currentSelectable);
			}
		}

		void OnInitialize ()
		{
			FetchComponents ();
			AddListeners();
		}

		void FetchComponents ()
		{
			if (eventData == null) eventData = new PointerEventData (EventSystem.current);
			if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup> ();
			if (currentSelectable == null) currentSelectable = firstSelectable;
		}

		void AddListeners()
		{
			MenuInput.instance.OnScrollUp.AddListener(OnScrollUp);
			MenuInput.instance.OnScrollDown.AddListener(OnScrollDown);
			MenuInput.instance.OnScrollLeft.AddListener(OnScrollLeft);
			MenuInput.instance.OnScrollRight.AddListener(OnScrollRight);

			MenuInput.instance.OnConfirm.AddListener(OnConfirm);
			MenuInput.instance.OnBack.AddListener(OnBack);
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

		void OnConfirm()
		{
			CheckActiveMenu();

			if (isActiveMenu)
			{
				SubmitButton(currentSelectable);
			}
		}

		void OnBack()
		{
			CheckActiveMenu();

			if (isActiveMenu)
			{
				BackButton();
			}
		}

		void CheckActiveMenu ()
		{
			// Bail out if not fully visible.
			if (GameController.instance == null)
			{
				return;
			} 

			else
			
			{
				// Is not active menu.
				if (GameController.instance.activeMenu != this)
				{
					// There is an active menu.
					if (GameController.instance.activeMenu != null)
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
