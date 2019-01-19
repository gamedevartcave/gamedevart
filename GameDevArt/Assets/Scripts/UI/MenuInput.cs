using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Events;

namespace CityBashers
{
	public class MenuInput : MonoBehaviour
	{
		public static MenuInput Instance { get; private set; }
		public MenuControls menuControls;

		private Vector2 Nav;
		public float navMin = 0.75f;

		public float ScrollRate = 0.25f;
		private float nextScroll;

		public UnityEvent OnScrollUp;
		public UnityEvent OnScrollDown;
		public UnityEvent OnScrollLeft;
		public UnityEvent OnScrollRight;
		public UnityEvent OnConfirm;
		public UnityEvent OnBack;

		void Awake()
		{
			Instance = this;
		}

		private void OnEnable()
		{
			menuControls.Menu.Nav.performed += HandleNav;
			menuControls.Menu.Nav.Enable();

			menuControls.Menu.Confirm.performed += HandleConfirm;
			menuControls.Menu.Confirm.Enable();

			menuControls.Menu.Back.performed += HandleBack;
			menuControls.Menu.Back.Enable();
		}

		private void OnDestroy()
		{
			menuControls.Menu.Nav.performed -= HandleNav;
			menuControls.Menu.Nav.Disable();

			menuControls.Menu.Confirm.performed -= HandleConfirm;
			menuControls.Menu.Confirm.Disable();

			menuControls.Menu.Back.performed -= HandleBack;
			menuControls.Menu.Back.Disable();
		}

		void HandleNav(InputAction.CallbackContext context)
		{
			Nav = context.ReadValue<Vector2>();
		}

		void HandleConfirm(InputAction.CallbackContext context)
		{
			OnConfirm.Invoke();
		}

		void HandleBack(InputAction.CallbackContext context)
		{
			OnBack.Invoke();
		}

		private void Update()
		{
			CheckNav();
		}

		void CheckNav()
		{
			if (Time.unscaledTime > nextScroll)
			{
				if (GameController.Instance != null)
				{
					if (GameController.Instance.isPaused)
					{
						return;
					}
				}

				else

				{

					// Move up
					if (Nav.y > navMin)
					{
						OnScrollUp.Invoke();
						nextScroll = Time.unscaledTime + ScrollRate;
						Nav = Vector2.zero;
						return;
					}

					// Move down
					if (Nav.y < -navMin)
					{
						OnScrollDown.Invoke();
						nextScroll = Time.unscaledTime + ScrollRate;
						Nav = Vector2.zero;
						return;
					}

					// Move left
					if (Nav.x < -navMin)
					{
						OnScrollLeft.Invoke();
						nextScroll = Time.unscaledTime + ScrollRate;
						Nav = Vector2.zero;
						return;
					}

					// Move right
					if (Nav.x > navMin)
					{
						OnScrollRight.Invoke();
						nextScroll = Time.unscaledTime + ScrollRate;
						Nav = Vector2.zero;
						return;
					}
				}
			}
		}
	}
}