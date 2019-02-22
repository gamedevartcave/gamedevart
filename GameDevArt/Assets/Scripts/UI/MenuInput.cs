using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Events;

namespace CityBashers
{
	public class MenuInput : MonoBehaviour
	{
		public static MenuInput Instance { get; private set; }
		[Tooltip ("Inout controls asset to use to measure nav.")]
		public MenuControls menuControls;

		private Vector2 Nav;
		[Tooltip ("Threshold input until nav is registered.")]
		public float navMin = 0.75f;

		[Tooltip ("Real time in seconds between one scroll to the next.")]
		public float ScrollRate = 0.25f;
		private float nextScroll;

		// Unity events on input performed.
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
			RegisterControls();
		}

		private void OnDestroy()
		{
			DeregisterControls();
		}

		#region Control event registration
		void RegisterControls()
		{
			menuControls.Menu.Nav.performed += HandleNav;
			menuControls.Menu.Nav.Enable();

			menuControls.Menu.Confirm.performed += HandleConfirm;
			menuControls.Menu.Confirm.Enable();

			menuControls.Menu.Back.performed += HandleBack;
			menuControls.Menu.Back.Enable();
		}

		void DeregisterControls()
		{
			menuControls.Menu.Nav.performed -= HandleNav;
			menuControls.Menu.Nav.Disable();

			menuControls.Menu.Confirm.performed -= HandleConfirm;
			menuControls.Menu.Confirm.Disable();

			menuControls.Menu.Back.performed -= HandleBack;
			menuControls.Menu.Back.Disable();
		}
		#endregion

		#region Input handles
		void HandleNav(InputAction.CallbackContext context)
		{
			Nav = context.ReadValue<Vector2>();
			CheckNav();
		}

		void HandleConfirm(InputAction.CallbackContext context)
		{
			OnConfirm.Invoke();
		}

		void HandleBack(InputAction.CallbackContext context)
		{
			OnBack.Invoke();
		}
		#endregion

		#region Navigation
		void CheckNav()
		{
			if (Time.unscaledTime <= nextScroll)
			{
				return;
			}

			if (PlayerController.Instance != null)
			{
				if (PlayerController.Instance.health <= 0)
				{
					DoNav();
					return;
				}
			}

			if (GameController.Instance != null)
			{
				if (GameController.Instance.isPaused == false)
				{
					return;
				}
			}

			DoNav(); // Got to this point, GameController Instance was not found, allow menu input.
		}

		// Invokes correct nav direction.
		void DoNav()
		{
			// Move up
			if (Nav.y > navMin)
			{
				OnScrollUp.Invoke();
				ResetNav();
				return;
			}

			// Move down
			if (Nav.y < -navMin)
			{
				OnScrollDown.Invoke();
				ResetNav();
				return;
			}

			// Move left
			if (Nav.x < -navMin)
			{
				OnScrollLeft.Invoke();
				ResetNav();
				return;
			}

			// Move right
			if (Nav.x > navMin)
			{
				OnScrollRight.Invoke();
				ResetNav();
				return;
			}
		}

		void ResetNav()
		{
			nextScroll = Time.unscaledTime + ScrollRate;
			Nav = Vector2.zero;
		}
		#endregion
	}
}