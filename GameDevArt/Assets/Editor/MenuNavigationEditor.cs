using UnityEditor;
using UnityEngine;

namespace CityBashers
{
	[CustomEditor(typeof(MenuNavigation))]
	public class MenuNavigationEditor : Editor
	{
		MenuNavigation mn;

		private void OnEnable()
		{
			mn = (MenuNavigation)target;
		}

		public override void OnInspectorGUI()
		{
			if (Application.isPlaying)
			{
				if (GUILayout.Button("Ping Active Menu"))
				{
					if (MenuNavigation.ActiveMenu != null)
					{
						EditorGUIUtility.PingObject(MenuNavigation.ActiveMenu);
					}
				}
			}

			DrawDefaultInspector();
		}
	}
}
