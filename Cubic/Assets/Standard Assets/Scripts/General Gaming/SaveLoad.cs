using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.IO;
using System; 


public class SaveLoad
{
		/// <summary>
		/// The saved games.
		/// </summary>
		public static List<Game> savedGames = new List<Game> (){new Game(),new Game(),new Game()};
		//J'ai mis exprès le nombre de sauvegarde limité à 4.	
		/// <summary>
		/// The saving slot.
		/// </summary>
		public static int savingSlot = 0;

		/// <summary>
		/// Save the game in the  i-slot.
		/// </summary>
		/// <param name="i">The index.</param>
		public static void Save (int i)
		{
				if (i > 3) {
						throw new ArgumentException ("Invalid slot savegame.");
				}
				savedGames [i] = Game.current;
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
						SaveLoad.savedGames = (List<Game>)bf.Deserialize (file);
						file.Close ();
				}
				savingSlot = i;
		}
}