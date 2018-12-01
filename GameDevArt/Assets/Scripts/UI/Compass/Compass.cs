using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
	public Transform rotRef;
	public RawImage compassRawImage;
	public Vector2 offset;
	public float constantOffset = 0.25f; // For north.
	public float damping = 1;
	private Rect uv;

	void Update () 
	{
		offset = new Vector2 (rotRef.eulerAngles.y * damping + constantOffset, 0);
		uv = compassRawImage.uvRect;
		uv.position = offset;
		compassRawImage.uvRect = uv;
	}
}