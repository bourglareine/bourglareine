using UnityEngine;
using System.Collections;
using System;
using GameUtils.Utils;
using UnityEditor;


public class GameControl : MonoBehaviour
{
	public  Game game;

	public static GameControl gameControl;
	// Use this for initialization
	void Awake ()
	{
		if (gameControl == null) {
			DontDestroyOnLoad (gameObject);
			gameControl = this;
			gameControl.game = new Game ();
			gameControl.game.levelOrArea = "";
			gameControl.game.level = 1;
		} else {
			Destroy (gameObject);
		}
	}

	void Update ()
	{
		if (Application.loadedLevelName.Contains ("Stage")) {
			gameControl.game.timePlayed += Time.deltaTime;
		}
	}

	void OnLevelWasLoaded (int level)
	{
		if (Application.loadedLevelName.Contains ("Stage")) {
			gameControl.game.levelOrArea = Application.loadedLevelName;
		}
		
	}
}
 