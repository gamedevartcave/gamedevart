/// Credit NemoKrad (aka Charles Humphrey) / valtain
/// Sourced from - http://www.randomchaos.co.uk/SoftAlphaUIMask.aspx
/// Updated by valtain - https://bitbucket.org/ddreaper/unity-ui-extensions/pull-requests/33
/// Modified to accept a gradient and generates a texture for it. / Andrew Spalato - http://www.andrewspalato.ml

namespace UnityEngine.UI.Extensions
{
    [ExecuteInEditMode]
    [AddComponentMenu ("UI/Effects/Extensions/SoftMaskScript")]
    public class SoftMaskScript : MonoBehaviour
    {
        [Tooltip ("The area that is to be used as the container.")]
        public RectTransform MaskArea;
		public Graphic graphic;
		Material mat; // Material used for the soft mask shader.

		// Rect stuff.
		Canvas cachedCanvas = null;
		Transform cachedCanvasTransform = null;
		readonly Vector3[] m_WorldCorners = new Vector3 [4];
		readonly Vector3[] m_CanvasCorners = new Vector3 [4];

        [Tooltip ("Texture to be used to do the soft alpha")]
		public bool useInputTexture = true; // Uses input texture or gradient.
        public Texture AlphaMask; // Texture that the soft mask will use.
		public UnityEngine.Gradient AlphaGradient; // Gradient that a 1-Dimensional texture can be created from.
		public Vector2 gradientSize; // Image size (faster if uses power-of-2 width and height).
		private Texture2D gradientTexture; // Generated texture from gradient.
		private Color gradientEvaluate; // Evaluated value of gradient at any given point with range 0 - 1.

        [Tooltip ("At what point to apply the alpha min range 0-1")]
        [Range (0, 1)] public float CutOff = 0; // Value to start using alpha for mask, any lower is marked as 0 alpha.

        [Tooltip ("Implement a hard blend based on the Cutoff")]
        public bool HardBlend = false;

        [Tooltip ("Flip the masks alpha value")]
        public bool FlipAlphaMask = false;

        [Tooltip ("If a different Mask Scaling Rect is given, and this value is true, " +
        	"the area around the mask will not be clipped")]
        public bool DontClipMaskScalingRect = false;

        Vector2 maskOffset = Vector2.zero;
        Vector2 maskScale = Vector2.one;

        public void Start ()
        {
            if (MaskArea == null)
            {
                MaskArea = GetComponent<RectTransform> ();
            }

            var text = GetComponent<Text> ();
            
			if (text != null)
            {
                mat = new Material (Shader.Find ("UI Extensions/SoftMaskShader"));
                text.material = mat;
                cachedCanvas = text.canvas;
                cachedCanvasTransform = cachedCanvas.transform;
                
				// For some reason, having the mask control on the parent and disabled stops the mouse interacting
                // with the texture layer that is not visible.. Not needed for the Image.
				if (transform.parent.GetComponent<Mask> () == null)
				{
					transform.parent.gameObject.AddComponent<Mask> ();
				}

                transform.parent.GetComponent<Mask> ().enabled = false;
                return;
            }

			if (graphic == null)
			{
				graphic = GetComponent<Graphic> ();
			}

			else // Graphic is found.
            
			{
                mat = new Material (Shader.Find ("UI Extensions/SoftMaskShader"));
                graphic.material = mat;
                cachedCanvas = graphic.canvas;
                cachedCanvasTransform = cachedCanvas.transform;
            }

			if (useInputTexture == false)
			{
				CreateGradientTexture ();
			}

			if (cachedCanvas != null)
			{
				SetMask ();
			}
        }

		// Generates a texture based on the gradient property. Applies to alpha mask texture.
		public void CreateGradientTexture ()
		{
			gradientTexture = new Texture2D (Mathf.CeilToInt (gradientSize.x), Mathf.CeilToInt (gradientSize.y));

			// Note: Causes error when building (missing assembly reference for Texture2D).
			//gradientTexture.alphaIsTransparency = true; // Enable transparency so alpha effect works.

			// Loop through all pixels and evaluate points on the gradient.
			for (int y = 0; y < Mathf.CeilToInt (gradientSize.y); y++)
			{
				for (int x = 0; x < Mathf.CeilToInt (gradientSize.x); x++)
				{
					gradientEvaluate = AlphaGradient.Evaluate (x / gradientSize.x);
					gradientTexture.SetPixel (Mathf.CeilToInt (x), Mathf.CeilToInt (y), gradientEvaluate);
				}
			}

			// Serialize the texture and set it to the alpha value.
			gradientTexture.Apply (true, false);
			AlphaMask = gradientTexture;
			mat.SetTexture ("_AlphaMask", AlphaMask);
		}
	
		// Refreshes the mask for the material and rect.
        public void SetMask()
        {
            var worldRect = GetCanvasRect ();
            var size = worldRect.size;
            maskScale.Set (1.0f / size.x, 1.0f / size.y);
            maskOffset = -worldRect.min;
            maskOffset.Scale (maskScale);

            mat.SetTextureOffset ("_AlphaMask", maskOffset);
            mat.SetTextureScale ("_AlphaMask", maskScale);
            mat.SetTexture ("_AlphaMask", AlphaMask);

            mat.SetFloat ("_HardBlend", HardBlend ? 1 : 0);
            mat.SetInt ("_FlipAlphaMask", FlipAlphaMask ? 1 : 0);
            mat.SetInt ("_NoOuterClip", DontClipMaskScalingRect ? 1 : 0);
            mat.SetFloat ("_CutOff", CutOff);
        }

		// Gets canvas rect and applies corners.
        public Rect GetCanvasRect ()
        {
			if (cachedCanvas == null)
			{
				return new Rect ();
			}

            MaskArea.GetWorldCorners (m_WorldCorners);

			for (int i = 0; i < 4; ++i)
			{
				m_CanvasCorners [i] = cachedCanvasTransform.InverseTransformPoint (m_WorldCorners [i]);
			}

            return new Rect (
				m_CanvasCorners[0].x, m_CanvasCorners[0].y, 
				m_CanvasCorners[2].x - m_CanvasCorners[0].x, m_CanvasCorners[2].y - m_CanvasCorners[0].y);
        }
    }
}