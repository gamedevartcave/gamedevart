using UnityEngine;
using UnityEngine.Events;

namespace CityBashers
{
	public class ParentToTransform : MonoBehaviour 
	{
		[Tooltip ("Finds GameObject by this string to parent to.")]
		public string ParentTransformName = "Instantiated";

		[Tooltip ("Parent to GameObject on Start?")]
		public bool OnStart = true;

		[Tooltip ("The parent Trasnform.")]
		private Transform ParentTransform;

		[Tooltip ("Event when parented")]
		public UnityEvent OnParented;

		void Awake () 
		{
			if (OnStart == true)
			{
				ParentNow ();
			}
		}

		// Finds GameObject at this time and parents to it.
		public void ParentNow ()
		{
			ParentTransform = GameObject.Find (ParentTransformName).transform;
			transform.SetParent (ParentTransform);
			transform.SetAsLastSibling ();
			OnParented.Invoke ();
		}
	}
}