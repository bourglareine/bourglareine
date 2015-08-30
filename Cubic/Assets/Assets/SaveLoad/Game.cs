using UnityEngine;
using System.Collections;
using System;

namespace GameUtils.Utils
{
	[Serializable]
	public class Game
	{
		public float timePlayed;
		public int level;
		public int SaveCount;
		public int health;
		public int mana;
		public  string levelOrArea;
	}
}
