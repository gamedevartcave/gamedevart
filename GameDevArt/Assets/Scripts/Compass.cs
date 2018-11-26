using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
	public Transform rotRef;
	public RawImage compassRawImage;
	public Vector2 offset;
	public float constantOffset;
	public float damping = 1;

	void Update () 
	{
		offset = new Vector2 (
			rotRef.eulerAngles.y * damping + constantOffset,
			0
		);

		var uv = compassRawImage.uvRect;
		uv.position = offset;
		compassRawImage.uvRect = uv;
	}
}
