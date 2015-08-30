using UnityEngine;

/// <summary>
/// Title screen script
/// </summary>
public class MenuScript : MonoBehaviour
{
		public GameObject loadingImage;
	
		public void LoadScene (string target)
		{
				switch (target) {
				case "Quit":
						Application.Quit ();
						break;
				case "Option":
						break;
				case "LoadGame":
						SaveLoad.Load (0);
						break;
				default:
						Application.LoadLevel (target);
						break;
				}
				//loadingImage.SetActive (true);
		}
}