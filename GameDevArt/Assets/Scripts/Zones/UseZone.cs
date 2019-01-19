using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace CityBashers
{
	public class UseZone : MonoBehaviour 
	{
		[ReadOnlyAttribute] public bool canUse;
		public bool denyUse;
		[ReadOnlyAttribute] public bool used;
		public bool oneOffUse;
		public bool autoUse;
		public bool toggleUse;

		[Header ("Timer use")]
		public bool timerUse;
		public bool active;
		public float activeTime;

		[Header ("Audio")]
		public AudioSource useSound;
		public AudioSource useEndSound;
		public AudioSource useDenySound;

		[Space (10)]
		public UnityEvent UseEvent;
		public UnityEvent OnUseEnded;
		public UnityEvent OnUseDeny;

		void Start ()
		{
			PlayerController.Instance.OnUse.AddListener (Use);
		}

		void OnTriggerEnter (Collider other)
		{
			if (other == PlayerController.Instance.playerCol)
			{
				canUse = true;

				if (autoUse == true && denyUse == false)
				{
					Use ();
				} 

				if (denyUse == true)
				{
					OnUseDeny.Invoke ();
				}
			}
		}

		void OnTriggerExit (Collider other)
		{
			if (other == PlayerController.Instance.playerCol)
			{
				canUse = false;
			}
		}

		public void Use ()
		{
			if (denyUse == false)
			{
				if (toggleUse == false)
				{
					if (oneOffUse == true)
					{
						if (used == false)
						{
							if (canUse == true)
							{
								if (timerUse == true)
								{
									if (active == false)
									{
										UseEvent.Invoke ();
										StartCoroutine (Timer ());
										active = true;
										Debug.Log ("One off timer use.");
									}
								} 

								else // Normal use.
								
								{
									UseEvent.Invoke ();
									Debug.Log ("Normal one off use.");
								}

								used = true;
							}

							else // Cannot use.
							
							{
								//OnUseDeny.Invoke ();
							}
						}
					} 

					else // Can use as many times as you like.
					
					{
						if (canUse == true)
						{
							if (timerUse == true)
							{
								if (active == false)
								{
									UseEvent.Invoke ();
									StartCoroutine (Timer ());
									active = true;
									Debug.Log ("Normal timer use.");
								}
							} 

							else // Normal use.
							
							{
								UseEvent.Invoke ();
								Debug.Log ("Normal use.");
							}
						} 

						else // Cannot use.
						
						{
							//OnUseDeny.Invoke ();
						}
					}
				} 

				else // Toggle use.
				
				{
					if (canUse == true)
					{
						if (active == false)
						{
							UseEvent.Invoke ();
							Debug.Log ("Toggle use on.");
						}

						if (active == true)
						{
							OnUseEnded.Invoke ();
							Debug.Log ("Toggle use off.");
						}

						active = !active;
					} 

					else // Cannot use.
					
					{
						//OnUseDeny.Invoke ();
					}
				}
			}
		}

		IEnumerator Timer ()
		{
			yield return new WaitForSeconds (activeTime);
			OnUseEnded.Invoke ();
			active = false;
		}
	}
}
