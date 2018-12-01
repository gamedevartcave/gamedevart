using UnityEngine;
using UnityEditor;

public class SaveWindow : EditorWindow
{
	static SaveAndLoadScript savescript;
	private GUIStyle style;

	[MenuItem ("Window/Save Editor")]
	public static void ShowSaveEditorWindow ()
	{
		GetWindow<SaveWindow> ("Save Editor");
	}

	[MenuItem ("Window/Open save folder location")]
	public static void ShowSaveLocationWindow ()
	{
		string savefolder = Application.persistentDataPath + "/";
		Application.OpenURL (savefolder);
		Debug.Log ("Opened save folder at " + savefolder);
	}

	void OnGUI ()
	{
		savescript = FindObjectOfType<SaveAndLoadScript> ();

		style = new GUIStyle (GUI.skin.button);
		style.stretchWidth = true;
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;

		GUILayout.Label ("Save and Load data management.", EditorStyles.boldLabel);

		if (GUILayout.Button ("Open save location"))
		{
			ShowSaveLocationWindow ();
		}

		GUILayout.Space (10);

		GUILayout.Label ("Player Data");

		GUILayout.BeginHorizontal ();

		// SAVE
		GUI.backgroundColor = new Color (0.4f, 0.9f, 1, 1);
		if (GUILayout.Button ("Save", style)) 
		{
			savescript.SavePlayerData ();
		}

		// LOAD
		if (Application.isPlaying == true)
		{
			GUI.backgroundColor = new Color (0.75f, 0.75f, 1, 1);
			if (GUILayout.Button ("Load", style))
			{
				savescript.LoadPlayerData ();
			}
		}

		// DELETE
		GUI.backgroundColor = new Color (1, 0.5f, 0.5f, 1);
		if (GUILayout.Button ("Delete", style)) 
		{
			savescript.DeletePlayerDataMain ();
		}

		// DELETE (EDITOR)
		if (GUILayout.Button ("Delete (Editor)", style)) 
		{
			savescript.DeletePlayerDataEditor ();
		}

		GUILayout.EndHorizontal ();

		GUILayout.Space (10);

		GUILayout.Label ("Settings Data");

		GUILayout.BeginHorizontal ();

		// SAVE
		GUI.backgroundColor = new Color (0.4f, 0.9f, 1, 1);
		if (GUILayout.Button ("Save", style)) 
		{
			savescript.SaveSettingsData ();
		}

		// LOAD
		if (Application.isPlaying == true) 
		{
			GUI.backgroundColor = new Color (0.75f, 0.75f, 1, 1);
			if (GUILayout.Button ("Load", style)) 
			{
				savescript.LoadSettingsData ();
			}
		}

		// DELETE
		GUI.backgroundColor = new Color (1, 0.5f, 0.5f, 1);
		if (GUILayout.Button ("Delete", style)) 
		{
			savescript.DeleteSettingsDataMain ();
		}

		// DELETE (EDITOR)
		if (GUILayout.Button ("Delete (Editor)", style)) 
		{
			savescript.DeleteSettingsDataEditor ();
		}

		GUILayout.EndHorizontal ();
	}
}