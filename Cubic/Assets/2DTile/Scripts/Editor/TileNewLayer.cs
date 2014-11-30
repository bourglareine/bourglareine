using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileNewLayer : EditorWindow
{
	[MenuItem("GameObject/Create Other/2D Layer")]
	public static void Init()
	{
		for (int i = 1; i > 0; i++)
		{
			if (GameObject.Find("Layer " + i.ToString()) == null)
			{
				GameObject go = new GameObject("Layer " + i.ToString());
				go.AddComponent<TileLayer>();
				go.GetComponent<TileLayer>().level = i - 1;
				i = -1;
			}
		}
	}
}
