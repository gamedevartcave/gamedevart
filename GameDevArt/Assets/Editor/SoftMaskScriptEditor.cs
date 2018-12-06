using UnityEngine;
using UnityEditor;
using UnityEngine.UI.Extensions;

[CustomEditor (typeof (SoftMaskScript))]
public class SoftMaskScriptEditor : Editor
{
	private SoftMaskScript softMask;

	void OnEnable ()
	{
		softMask = (SoftMaskScript)target;
	}

	public override void OnInspectorGUI ()
	{
		if (GUILayout.Button ("Generate gradient texture"))
		{
			softMask.CreateGradientTexture ();
			Debug.Log ("Created soft mask from gradient.");
		}

		DrawDefaultInspector ();
	}
}
