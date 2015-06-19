using UnityEngine;

/// <summary>
/// Title screen script
/// </summary>
public class MenuScript : MonoBehaviour
{
		public GameObject loadingImage;
	
		public void LoadScene (string target)
		{
				//loadingImage.SetActive (true);
				Application.LoadLevel (target);
		}
}