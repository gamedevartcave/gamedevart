using UnityEngine;
using UnityEditor;

namespace CityBashers
{
	[CustomEditor (typeof (PlayerController))]
	public class PlayerControllerEditor : Editor
	{
		private PlayerController playerController;
		
		void OnEnable ()
		{
			playerController = (PlayerController)target;
		}

		public override void OnInspectorGUI ()
		{
			if (GUILayout.Button ("Reset position"))
			{
				playerController.OverridePosition (playerController.startingPoint.position);
			}

			DrawDefaultInspector ();
		}
	}
}
