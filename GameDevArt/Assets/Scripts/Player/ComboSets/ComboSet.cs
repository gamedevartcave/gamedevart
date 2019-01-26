using UnityEngine;

namespace CityBashers
{
	[CreateAssetMenu(fileName = "New Combo set", menuName = "Player/Combo set", order = 1)]
	public class ComboSet : ScriptableObject
	{
		public string[] AdditiveCombos;
		public string[] AdditiveAnimationName;
		public string[] ClearableCombos;
		public string[] ClearableAnimationName;
	}
}
