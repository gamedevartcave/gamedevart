using UnityEngine;

namespace CityBashers
{
	public class MainMenuManager : MonoBehaviour 
	{
		Vector3 delta;
		Vector3 lastPos;

		void Start () 
		{
			//Cursor.visible = true;
			//Cursor.lockState = CursorLockMode.None;
		}

		private void Update()
		{
			delta = Input.mousePosition - lastPos;

			if (delta.sqrMagnitude > 1)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				//Debug.Log("Unlocked");
			}

			//Debug.Log("delta X : " + delta.x);
			//Debug.Log("delta Y : " + delta.y);
			lastPos = Input.mousePosition;
		}
	}
}
