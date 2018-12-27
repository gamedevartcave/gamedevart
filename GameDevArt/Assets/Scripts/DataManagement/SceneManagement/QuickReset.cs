using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickReset : MonoBehaviour 
{
	public void ReloadScene ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}