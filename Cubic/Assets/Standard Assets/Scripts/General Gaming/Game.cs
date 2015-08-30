using UnityEngine;
using System.Collections;

[System.Serializable]
public class Game
{
		public static Game current;
		public Character heroMekessi;
	
		public Game ()
		{
				heroMekessi = new Character ();
		}
}
