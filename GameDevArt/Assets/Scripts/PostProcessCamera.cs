using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessCamera : MonoBehaviour 
{
	public static PostProcessCamera instance { get; private set; }

	public PostProcessLayer postProcessLayer;

	void Awake ()
	{
		instance = this;

		if (postProcessLayer == null)
		{
			postProcessLayer = GetComponent<PostProcessLayer> ();
		}
	}
}
