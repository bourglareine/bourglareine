using UnityEngine;
using UnityEditor;
using System.Collections;

public class TilePrefabEditor : EditorWindow
{
	TileLayer layer;
	TileLayerEditor editor;
	
	public string preName = "Name";
	public Material mat;
	public Vector2 tileSize;
	public Vector2 offset;
	public bool boxCollider;
	public bool trigger;
	public bool makeObject;
	public MonoScript script;
	public static Texture2D boxTex;
	public int type;

	public void Setup(TileLayer _layer,TileLayerEditor _editor,int _type)
	{
		layer = _layer;
		editor = _editor;
		type = _type;
		
		if (type == 0)
		{
			this.title = "Modify";
			preName = layer.prefabsName[layer.prefabIndex];
			mat = layer.prefabsMat[layer.prefabIndex];
			tileSize = layer.prefabsTileSize[layer.prefabIndex];
			offset = layer.prefabsOffset[layer.prefabIndex];
			boxCollider = layer.prefabBoxCollider[layer.prefabIndex];
			trigger = layer.prefabTrigger[layer.prefabIndex];
			script = layer.prefabScript[layer.prefabIndex];
			makeObject = layer.prefabMakeObject[layer.prefabIndex];
		}
		else if (type == 1)
		{
			this.title = "Add";
			mat = layer.mat;
			tileSize = layer.tileSize;
			offset = layer.offset;
			boxCollider = layer.boxCollider;
			trigger = layer.trigger;
			script = layer.script;
			makeObject = layer.makeObject;
		}

		boxTex = new Texture2D(1,1);
		boxTex.SetPixel(0,0,new Color(0,0.5f,1f,0.4f));
		boxTex.Apply();
	}

	void OnGUI()
	{
		//Name
		GUILayout.BeginHorizontal();
			GUILayout.Label("Name:",GUILayout.Width(50));
			preName = GUILayout.TextField(preName, 15);
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
				GUILayout.Label("Material");
				mat = (Material)EditorGUILayout.ObjectField(mat, typeof(Material),false);
			GUILayout.EndHorizontal();
			tileSize = EditorGUILayout.Vector2Field("Tiling:", tileSize);
			offset = EditorGUILayout.Vector2Field("Offset:", offset);
			boxCollider = EditorGUILayout.Toggle("Use BoxCollider", boxCollider);
			if (boxCollider)
			{
				trigger = EditorGUILayout.Toggle("Trigger", trigger);
			}
			makeObject = EditorGUILayout.Toggle("Object", makeObject);
			GUILayout.BeginHorizontal();
				GUILayout.Label("Script");
				try {
					script = (MonoScript)EditorGUILayout.ObjectField(script, typeof(MonoScript),false);
				} catch (System.Exception) {}				
			GUILayout.EndHorizontal();
		GUILayout.EndHorizontal();
		
		

		if (type == 0)
		{
			GUILayout.BeginHorizontal();
				if(GUILayout.Button("Done"))
				{
					ModifyPrefab();
				}
				if(GUILayout.Button("Cancel"))
				{
					this.Close();
				}
			GUILayout.EndHorizontal();
		}
		else if (type == 1)
		{
			if(GUILayout.Button("Add"))
			{
				AddPrefab();
			}
		}
		
		Repaint();
		//Draw
		if (mat != null)
		{
			Rect rect = GUILayoutUtility.GetLastRect();
			float top = rect.yMax + 5;
			GUI.DrawTexture(new Rect(5,top,mat.mainTexture.width,mat.mainTexture.height), mat.mainTexture);
			
			GUIStyle style = new GUIStyle(GUI.skin.customStyles[0]);
			style.normal.background = boxTex;
			GUI.Box(new Rect(5 + 32 * offset.x,top + 32 * offset.y,32 * tileSize.x,32 * tileSize.y),"",style);
			
			Event current = Event.current;
			Vector2 tS = new Vector2(current.mousePosition.x - 5,current.mousePosition.y - top);
			if(current.type == EventType.mouseDown && current.button == 0) 
			{
				if (tS.x > 0 && tS.y > 0 && (int)tS.x / 32 < mat.mainTexture.width / 32 && (int)tS.y / 32 < mat.mainTexture.width / 32)
				{
					offset = new Vector2((int)tS.x / 32,(int)tS.y / 32);
					tileSize = new Vector2(1,1);
				}
			}
			if(current.type == EventType.mouseDrag && current.button == 0) 
			{
				if (tS.x > 0 && tS.y > 0 && (int)tS.y / 32 + 1 - offset.y > 0 && (int)tS.x / 32 + 1 - offset.x > 0)
				{
					float x = tileSize.x;
					float y = tileSize.y;
					if ((int)tS.x / 32 < mat.mainTexture.width / 32)
					{
						x = (int)tS.x / 32 + 1 - offset.x;
					}
					if ((int)tS.y / 32 < mat.mainTexture.width / 32)
					{
						y = (int)tS.y / 32 + 1 - offset.y;
					}
					
					tileSize = new Vector2(x,y);
				}
			}
		}
		
		
	}
	
	void AddPrefab()
	{
		if (mat == null)
		{
			this.Close();
			return;
		}
		layer.prefabs++;
		int tmpPrefabs = layer.prefabs;
		
		
		//Name
		string _name = "Tile: ";
		if (makeObject)
		{
			_name = "Object: ";
		}
		string[] tmpPrefabName = layer.prefabsName;
		layer.prefabsName = new string[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabsName[i] = tmpPrefabName[i];
		}
		layer.prefabsName[tmpPrefabs - 1] = _name + preName;
		
		//Size
		int[] tmpPrefabSize = layer.prefabSize;
		layer.prefabSize = new int[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabSize[i] = tmpPrefabSize[i];
		}
		layer.prefabSize[tmpPrefabs - 1] = 0;
		
		//Mat
		Material[] tmpPrefabMat = layer.prefabsMat;
		layer.prefabsMat = new Material[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabsMat[i] = tmpPrefabMat[i];
		}
		layer.prefabsMat[tmpPrefabs - 1] = mat;
		
		//TileSize
		Vector2[] tmpPrefabTileSize = layer.prefabsTileSize;
		layer.prefabsTileSize = new Vector2[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabsTileSize[i] = tmpPrefabTileSize[i];
		}
		layer.prefabsTileSize[tmpPrefabs - 1] = tileSize;
		
		
		//Offset
		Vector2[] tmpPrefabOffset = layer.prefabsOffset;
		layer.prefabsOffset = new Vector2[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabsOffset[i] = tmpPrefabOffset[i];
		}
		layer.prefabsOffset[tmpPrefabs - 1] = offset;
		
		
		//BoxCollider
		bool[] tmpPrefabBoxCollider = layer.prefabBoxCollider;
		layer.prefabBoxCollider = new bool[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabBoxCollider[i] = tmpPrefabBoxCollider[i];
		}
		layer.prefabBoxCollider[tmpPrefabs - 1] = boxCollider;
		
		//Trigger
		bool[] tmpPrefabTrigger = layer.prefabTrigger;
		layer.prefabTrigger = new bool[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabTrigger[i] = tmpPrefabTrigger[i];
		}
		layer.prefabTrigger[tmpPrefabs - 1] = script;
		
		//Script
		MonoScript[] tmpPrefabScript = layer.prefabScript;
		layer.prefabScript = new MonoScript[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabScript[i] = tmpPrefabScript[i];
		}
		if (script != null)
		{
			layer.prefabScript[tmpPrefabs - 1] = script;
		}
		
		//Object
		bool[] tmpPrefabMakeObject = layer.prefabMakeObject;
		layer.prefabMakeObject = new bool[tmpPrefabs];
		for (int i = 0; i < tmpPrefabs - 1; i++)
		{
			layer.prefabMakeObject[i] = tmpPrefabMakeObject[i];
		}
		layer.prefabMakeObject[tmpPrefabs - 1] = makeObject;
		
		layer.prefabIndex = layer.prefabs - 1;
		editor.UsePrefab(true);
		editor.UpdateMarker();
		
		this.Close();
	}
	
	void ModifyPrefab()
	{
		if (mat == null)
		{
			this.Close();
			return;
		}
		
		string[] _name = preName.Split(" "[0]);
		string a = "";
		if (_name[0] == "Tile:" || _name[0] == "Object:")
		{
			for (int i = 1; i < _name.Length; i++)
			{
				
				a += " " + _name[i];
			}
		}
		if (makeObject)
		{
			layer.prefabsName[layer.prefabIndex] = "Object:" + a;
		}
		else
		{
			layer.prefabsName[layer.prefabIndex] = "Tile:" + a;
		}
		
		layer.prefabsMat[layer.prefabIndex] = mat;
		layer.prefabsTileSize[layer.prefabIndex] = tileSize;
		layer.prefabsOffset[layer.prefabIndex] = offset;
		layer.prefabBoxCollider[layer.prefabIndex] = boxCollider;
		layer.prefabTrigger[layer.prefabIndex] = trigger;
		layer.prefabScript[layer.prefabIndex] = script;
		layer.prefabMakeObject[layer.prefabIndex] = makeObject;
		
		editor.UsePrefab(true);
		editor.UpdateMarker();
		this.Close();
	}
}
