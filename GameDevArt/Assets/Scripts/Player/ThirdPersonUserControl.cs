using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Events;
using System.Collections;
using CityBashers;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
		public static ThirdPersonUserControl instance { get; private set; }
        
		private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object




		private PlayerActions playerActions;

		void Awake ()
		{
			instance = this;
			this.enabled = false;
		}

        private void Start()
        {
			playerActions = InControlActions.instance.playerActions;

            // get the third person character.
			// This should never be null due to require component.
            m_Character = GetComponent<ThirdPersonCharacter>();
        }
    }
}
