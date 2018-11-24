using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour 
{
	public static MenuNavigation activeMenu { get; protected set; }

	// Button index
	[ReadOnlyAttribute] public int buttonIndex;
	public int startButtonIndex;

	public bool wrapAround;

	// Scrolling
	public float scrollSpeed;
	private float nextScroll;

	// Button assets
	public GameObject backButton;
	public GameObject[] buttons;

	// Misc
	private CanvasGroup canvasGroup;
	public PlayerActions playerActions;
	public PointerEventData eventData;

	void OnEnable ()
	{
		buttonIndex = startButtonIndex;
		activeMenu = this;
	}

	void Start ()
	{
		playerActions = InControlActions.instance.playerActions;
		eventData = new PointerEventData (EventSystem.current);
		canvasGroup = GetComponent<CanvasGroup> ();
	}

	public void SetButtonIndex (int index)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			ButtonExit (i);
		}

		buttonIndex = index;
		ButtonEnter (buttonIndex);
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
				if (buttonIndex > 0)
				{
					ButtonExit (buttonIndex);
					buttonIndex--;
					ButtonEnter (buttonIndex);
				} 

				else

				{
					if (wrapAround == true)
					{
						ButtonExit (buttonIndex);
						buttonIndex = buttons.Length - 1;
						ButtonEnter (buttonIndex);
					}
				}
					
				nextScroll = Time.unscaledTime + scrollSpeed;
			}
		}

		// DOWN
		if (playerActions.Down.Value > 0.5f)
		{
			if (Time.unscaledTime > nextScroll)
			{
				if (buttonIndex < buttons.Length - 1)
				{
					ButtonExit (buttonIndex);
					buttonIndex++;
					ButtonEnter (buttonIndex);
				} 

				else

				{
					if (wrapAround == true)
					{
						ButtonExit (buttonIndex);
						buttonIndex = 0;
						ButtonEnter (buttonIndex);
					}
				}
					
				nextScroll = Time.unscaledTime + scrollSpeed;
			}
		}

		if (playerActions.Submit.WasPressed)
		{
			SubmitButton (buttonIndex);
		}

		if (playerActions.Back.WasPressed)
		{
			BackButton ();
		}
	}

	// Invoke Pointer Enter (BaseEventData) from EventTrigger.
	void ButtonEnter (int index)
	{
		ExecuteEvents.Execute (buttons [buttonIndex], eventData, ExecuteEvents.pointerEnterHandler);
	}

	// Invoke Pointer Exit (BaseEventData) from EventTrigger.
	void ButtonExit (int index)
	{
		ExecuteEvents.Execute (buttons [buttonIndex], eventData, ExecuteEvents.pointerExitHandler);
	}

	// Get current active button and send OnClick() event.
	void SubmitButton (int index)
	{
		ExecuteEvents.Execute (buttons [buttonIndex], eventData, ExecuteEvents.pointerClickHandler);
	}

	// Execute OnClick() event at any button index.
	void BackButton ()
	{
		ExecuteEvents.Execute (backButton, eventData, ExecuteEvents.pointerClickHandler);
	}
}
