using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GameUtils.Utils;
using System;


public static class SaveLoad
{

	public static List<Game> savedGames = new List<Game> (){new Game(),new Game(),new Game()};
			
	/// <summary>
	/// Save the game in the  i-slot.
	/// </summary>
	/// <param name="i">The index.</param>
	public static void Save (int i)
	{
		if (i > 3) {
			throw new ArgumentException ("Invalid slot savegame.");
		}
		GameControl control = GameObject.FindObjectOfType<GameControl> ();
		savedGames [i] = control.game;
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames" + i + ".gd");
		bf.Serialize (file, SaveLoad.savedGames);
		file.Close ();
	}	
	
	/// <summary>
	/// Load the game in the i-slot.
	/// </summary>
	/// <param name="i">The index.</param>
	public static void Load (int i)
	{
		if (File.Exists (Application.persistentDataPath + "/savedGames" + i + ".gd")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/savedGames" + i + ".gd", FileMode.Open);
			try {
				SaveLoad.savedGames = (List<Game>)bf.Deserialize (file);
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}

			file.Close ();
		}
	}
}
