using UnityEngine;
using TMPro;
using System.IO;
using UnityEditor;

public class DisplayBuildNumber : MonoBehaviour 
{
	public TextMeshProUGUI buildNumberText;
	private string path;
	string versionText;

	void Start () 
	{
		path = Application.streamingAssetsPath + "/version.txt";
		versionText = CommonUtils.ReadTextFile (path);
		buildNumberText.text = "Build " + versionText + "(Windows x86_x64)";
	}
}
