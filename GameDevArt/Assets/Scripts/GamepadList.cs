using UnityEngine;
using UnityEngine.Experimental.Input;

public class GamepadList : MonoBehaviour
{
    public static GamepadList Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		FetchGamepads();

		InputSystem.onDeviceChange +=
		(device, change) =>
		{
			if (change == InputDeviceChange.Added)
			{
				/* New Device */
				Debug.Log("Device added");
			}

			else if (change == InputDeviceChange.Disconnected)
			{
				/* Device got unplugged */
				Debug.Log("Device disconnected");
			}

			else if (change == InputDeviceChange.Reconnected)
			{
				/* Plugged back in */
				Debug.Log("Device reconnected");
			}
		
			else if (change == InputDeviceChange.Removed)
			{
				/* Remove from input system entirely; by default, devices stay in the system once discovered */
				Debug.Log("Device removed");
			}
		};
	}

	void FetchGamepads()
	{
		var allGamepads = Gamepad.all;
		//Debug.Log(allGamepads);
	}
}
