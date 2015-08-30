using UnityEngine;
using GameUtils.Utils;
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
			GameControl control = GameObject.FindObjectOfType<GameControl> ();
			control.game = SaveLoad.savedGames [0];
			Application.LoadLevel (control.game.levelOrArea);
			break;
		default:
			Application.LoadLevel (target);
			break;
		}
		//loadingImage.SetActive (true);
	}
}