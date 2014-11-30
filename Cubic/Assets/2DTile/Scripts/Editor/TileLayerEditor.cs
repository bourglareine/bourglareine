using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(TileLayer))]

public class TileLayerEditor : Editor
{
	TileLayer layer;

	private TileMarker marker;
	private Vector2 tmpTileSize;
	private Vector2 tmpOffset;
	private Material tmpMat;
	private int tmpPrefabIndex;
	private int tmpLevel;
	private bool tmpBoxCollider;
	private bool tmpTrigger;
	private bool tmpMakeObject;
	private MonoScript tmpScript;
	private Vector3 mouseHitPos;
	private bool mouseDrag;
	private bool mouseDown;
	public static Texture2D boxTex;
	public static Texture2D markerTex;
	

	public override void OnInspectorGUI()
	{
		//Properties
		layer.showProperties = EditorGUILayout.Foldout(layer.showProperties, "Properties");
		if(layer.showProperties)
		{
			GUILayout.BeginVertical("box");
				layer.layerSize = EditorGUILayout.Vector2Field("Layer Size: ", layer.layerSize);
				tmpLevel = layer.level;
				layer.level = EditorGUILayout.IntField("Level :",layer.level);
				if(GUILayout.Button("Clear Layer"))
				{
					ClearLayer();
				}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginVertical("box");
				tmpMat = layer.mat;
				GUILayout.BeginHorizontal();
					GUILayout.Label("Material");
					layer.mat = (Material)EditorGUILayout.ObjectField(layer.mat, typeof(Material),false);
				GUILayout.EndHorizontal();
				tmpTileSize = layer.tileSize;
				layer.tileSize = EditorGUILayout.Vector2Field("Tiling:", layer.tileSize);
				tmpOffset = layer.offset;
				layer.offset = EditorGUILayout.Vector2Field("Offset:", layer.offset);
				tmpBoxCollider = layer.boxCollider;
				layer.boxCollider = EditorGUILayout.Toggle("BoxCollider", layer.boxCollider);
				if (layer.boxCollider)
				{
					tmpTrigger = layer.trigger;
					layer.trigger = EditorGUILayout.Toggle("Trigger", layer.trigger);
				}
				tmpMakeObject = layer.makeObject;
				layer.makeObject = EditorGUILayout.Toggle("Object", layer.makeObject);
				tmpScript = layer.script;
				GUILayout.BeginHorizontal();
					GUILayout.Label("Script");
			try {
				layer.script = (MonoScript)EditorGUILayout.ObjectField(layer.script, typeof(MonoScript),false);
			} catch (System.Exception) {
				
			}
					
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
				
	
			if (tmpTileSize != layer.tileSize)
			{UsePrefab(false);UpdateMarker();}
			if (tmpOffset != layer.offset)
			{UsePrefab(false);UpdateMarker();}
			if (tmpMat != layer.mat)
			{UsePrefab(false);UpdateMarker();}
			if (tmpBoxCollider != layer.boxCollider)
			{UsePrefab(false);UpdateMarker();}
			if (tmpTrigger != layer.trigger)
			{UsePrefab(false);UpdateMarker();}
			if (tmpMakeObject != layer.makeObject)
			{UsePrefab(false);UpdateMarker();}
			if (tmpScript != layer.script)
			{UsePrefab(false);UpdateMarker();}
			
			if (tmpLevel != layer.level)
			{
				UpdateTileLevelPos();
			}
		}
		
		//Prefab
		layer.showPrefab = EditorGUILayout.Foldout(layer.showPrefab, "Template");
		if(layer.showPrefab)
		{
			GUILayout.BeginVertical("box");
				tmpPrefabIndex = layer.prefabIndex;
				if (layer.prefabs > 0)
				{
					GUILayout.BeginHorizontal();
						GUILayout.Label("Template");
						layer.prefabIndex = EditorGUILayout.Popup(layer.prefabIndex, layer.prefabsName);
					GUILayout.EndHorizontal();
					if (tmpPrefabIndex != layer.prefabIndex)
					{
						UsePrefab(true);
						UpdateMarker();
					}
				}
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("Add"))
				{
					AddPrefab();
				}
				//if(GUILayout.Button("R"))
				//{
				//	RemovePrefab2();
				//}
				if (layer.prefabs > 0 && layer.prefabIndex != -1)
				{
					if(GUILayout.Button("Remove"))
					{
						RemovePrefab();
					}
					if(GUILayout.Button("Modify"))
					{
						ModifyPrefab();
					}
				}				
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		} 
		
		
		
		Rect rect = GUILayoutUtility.GetLastRect();
		float top = rect.yMax;
		if (layer.mat != null)
		{
			GUI.DrawTexture(new Rect(5,top,layer.mat.mainTexture.width,layer.mat.mainTexture.height), layer.mat.mainTexture);
	
			GUIStyle style = new GUIStyle(GUI.skin.customStyles[0]);
			style.normal.background = boxTex;
			if (!layer.usePrefab)
			{
				GUI.Box(new Rect(5 + 32 * layer.offset.x,top + 32 * layer.offset.y,32 * layer.tileSize.x,32 * layer.tileSize.y),"",style);
			}			
	
			Event cE = Event.current;
			Vector2 tS = new Vector2(cE.mousePosition.x - 5,cE.mousePosition.y - top);
			if(Event.current.type == EventType.mouseDown && Event.current.button == 0) 
			{
				if (tS.x > 0 && tS.y > 0 && (int)tS.x / 32 < layer.mat.mainTexture.width / 32 && (int)tS.y / 32 < layer.mat.mainTexture.width / 32)
				{
					layer.offset = new Vector2((int)tS.x / 32,(int)tS.y / 32);
					layer.tileSize = new Vector2(1,1);
					
					UsePrefab(false);
					UpdateMarker();
				}
			}
			if(Event.current.type == EventType.mouseDrag && Event.current.button == 0) 
			{
				if (tS.x > 0 && tS.y > 0 && (int)tS.y / 32 + 1 - layer.offset.y > 0 && (int)tS.x / 32 + 1 - layer.offset.x > 0)
				{
					float x = layer.tileSize.x;
					float y = layer.tileSize.y;
					if ((int)tS.x / 32 < layer.mat.mainTexture.width / 32)
					{
						x = (int)tS.x / 32 + 1 - layer.offset.x;
					}
					if ((int)tS.y / 32 < layer.mat.mainTexture.width / 32)
					{
						y = (int)tS.y / 32 + 1 - layer.offset.y;
					}
					
					layer.tileSize = new Vector2(x,y);
				}
				UsePrefab(false);
				UpdateMarker();
			}
			if(Event.current.type == EventType.mouseUp && Event.current.button == 0) 
			{
				UsePrefab(false);
				UpdateMarker();
			}
		}
		
		Event current = Event.current;
		if (current.Equals(Event.KeyboardEvent("h")))
		{
			FlipHor();
		}
		if (current.Equals(Event.KeyboardEvent("v")))
		{
			FlipVer();
		}
	}
	
	void OnSceneGUI()
	{
		UpdateHitPosition();
		MoveMarker();
		
		if (layer.mat != null)
		{
			Event current = Event.current;
			if (MouseOnLayer())
			{
				if(current.control) 
				{
					if (!layer.makeObject)
					{					
						UpdateOutline(Color.red);
						marker.UpdateOutline(markerTex);
						RemoveTile();
					}
				}
				else
				{
					UpdateOutline(Color.black);
					marker.UpdateOutline(markerTex);
				}
				if(current.shift && !mouseDown)
				{
					if (!layer.makeObject)
					{
						Draw();
					}
				}
				
				if (current.Equals(Event.KeyboardEvent("h")))
				{
					FlipHor();
				}
				if (current.Equals(Event.KeyboardEvent("v")))
				{
					FlipVer();
				}
				if (current.Equals(Event.KeyboardEvent("t")))
				{
					RotateTile();
				}
				
				if(current.type == EventType.mouseDown)
				{
					mouseDown = true;
				}
				if(current.type == EventType.mouseDrag)
				{
					mouseDrag = true;
				}
				if(current.type == EventType.mouseUp)
				{
					mouseDown = false;
					if(current.button == 0)
					{
						Draw();
					}
					else if(current.button == 1 && !mouseDrag)
					{
						if (layer.usePrefab)
						{
							if (layer.prefabMakeObject[layer.prefabIndex])
							{
								RemoveObject();
							}
							else
							{
								RemoveTile();
							}
						}
						else
						{
							if (layer.makeObject)
							{
								RemoveObject();
							}
							else
							{
								RemoveTile();
							}
						}
					}
					mouseDrag = false;
				}
			}
		}
		
		SceneGUI();
		//Resources.UnloadUnusedAssets();
	}
	
	void SceneGUI()
	{
		Handles.BeginGUI();
		GUI.Label(new Rect(10,Screen.height - 60,100,100),"H: Flip Horizontal");
		GUI.Label(new Rect(10,Screen.height - 75,100,100),"V: Flip Vertical");
		GUI.Label(new Rect(10,Screen.height - 90,100,100),"Ctrl: Erase");
		GUI.Label(new Rect(10,Screen.height - 105,100,100),"Shift: Draw");
		Handles.EndGUI();
	}
	
	void OnEnable() 
	{
		Tools.current = Tool.View;
		Tools.viewTool = ViewTool.FPS;
		layer = target as TileLayer;
		DestroyMarker();
		
		if(boxTex == null)
		{
			boxTex = new Texture2D(1,1);
			boxTex.SetPixel(0,0,new Color(0,0.5f,1f,0.4f));
			boxTex.Apply();
		}
		if(markerTex == null)
		{
			UpdateOutline(Color.black);
		}
		marker = TileMarker.CreateMarker(layer,markerTex);
		if (layer.mat != null)
		{
			UpdateMarker();
		}
		
		if (layer.tiles == null)
		{
			GameObject go = new GameObject("Tiles");
			go.transform.parent = layer.transform;
			go.transform.position = new Vector3(0,0,0);
			layer.tiles = go;
		}
		if (layer.objects == null)
		{
			GameObject go = new GameObject("Objects");
			go.transform.parent = layer.transform;
			go.transform.position = new Vector3(0,0,0);
			layer.objects = go;
		}
	}
	void OnDisable() 
	{
		DestroyMarker();
	}
	
	void Draw()
	{
		DrawTile();
	}
	
	void DrawTile()
	{
		Vector2 _tilesize = new Vector2();
		Vector2 _offset = new Vector2();
		Vector3 _pos = marker.transform.position;
		bool _makeObject = false;
		string _objectName = "object";
		if (layer.usePrefab)
		{
			_tilesize = layer.prefabsTileSize[layer.prefabIndex];
			_offset = layer.prefabsOffset[layer.prefabIndex];
			_makeObject = layer.prefabMakeObject[layer.prefabIndex];
			string[] _name = layer.prefabsName[layer.prefabIndex].Split(" "[0]);
			//_objectName = _name[1];
			
			_objectName = "";
			for (int i = 1; i < _name.Length; i++)
			{
				_objectName += " " + _name[i];
			}
		}
		else
		{
			_tilesize = layer.tileSize;
			_offset = layer.offset;
			_makeObject = layer.makeObject;
		}
		
		RemoveTile();
		
		float posX = 0;
		float posY = 0;
		
		float matPosX = 0;
		float matPosY = 0;
		
		float sX = marker.transform.localScale.x;
		float sY = marker.transform.localScale.y;
		
		bool hor = false;
		bool ver = false;
		
		GameObject goObject = null;
		
		if (_makeObject)
		{
			goObject = new GameObject(_objectName);
			goObject.transform.parent = layer.objects.transform;
			goObject.transform.position = marker.transform.position;
			
			bool _boxCollider = false;
			if (layer.usePrefab)
			{
				_boxCollider = layer.prefabBoxCollider[layer.prefabIndex];
			}
			else
			{
				_boxCollider = layer.boxCollider;
			}
			bool _trigger = false;
			if (layer.usePrefab)
			{
				_trigger = layer.prefabTrigger[layer.prefabIndex];
			}
			else
			{
				_trigger = layer.trigger;
			}
			MonoScript _script = null;
			if (layer.usePrefab)
			{
				_script = layer.prefabScript[layer.prefabIndex];
			}
			else
			{
				_script = layer.script;
			}
			if (_boxCollider)
			{
				goObject.AddComponent<BoxCollider>();
				goObject.GetComponent<BoxCollider>().size = new Vector3(1 * _tilesize.x,1 * _tilesize.y,3);
				
				goObject.GetComponent<BoxCollider>().isTrigger = _trigger;
			}
			if (_script != null)
			{
				goObject.AddComponent(_script.GetClass());
			}
		}
		
		for (int a = 0; a < _tilesize.x; a++)
		{
			posX = a + _pos.x - 0.5f * _tilesize.x;
			for (int b = 0; b < _tilesize.y; b++)
			{
				posY = b + _pos.y - 0.5f * _tilesize.y;
				
				if (sY < 0)
				{
					matPosX = _offset.x - a + _tilesize.x - 1;
					hor = true;
				}
				else
				{
					matPosX = _offset.x + a;
				}
				if (sX > 0)
				{
					matPosY = _offset.y - b + _tilesize.y - 1;
				}
				else
				{
					matPosY = _offset.y + b;
					ver = true;
				}	
				Material mat = TileMat(layer.mat,new Vector2(1,1),new Vector2(matPosX,matPosY));
				
				if (_makeObject)
				{
					DrawObject(new Vector3(posX,posY,0),mat,hor,ver).transform.parent = goObject.transform;
				}
				else
				{
					DrawTile(new Vector3(posX,posY,0),mat,hor,ver);
				}
			}
		}
	}
	
	void DrawTile(Vector3 _pos,Material _mat,bool _hor,bool _ver)
	{
		GameObject go = new GameObject("tile_" + _pos.x.ToString() + "_" + _pos.y.ToString());
		go.transform.parent = layer.tiles.transform;
		go.transform.position = new Vector3(_pos.x + 0.5f,_pos.y + 0.5f,layer.level);
		
		float x = 1;
		float y = 1;
		
		bool _boxCollider = false;
		if (layer.usePrefab)
		{
			_boxCollider = layer.prefabBoxCollider[layer.prefabIndex];
		}
		else
		{
			_boxCollider = layer.boxCollider;
		}
		bool _trigger = false;
		if (layer.usePrefab)
		{
			_trigger = layer.prefabTrigger[layer.prefabIndex];
		}
		else
		{
			_trigger = layer.trigger;
		}
		MonoScript _script = null;
		if (layer.usePrefab)
		{
			_script = layer.prefabScript[layer.prefabIndex];
		}
		else
		{
			_script = layer.script;
		}
		
		if (_ver)
		{
			x = -1;
		}
		if (_hor)
		{
			y = -1;
		}
		go.transform.localScale = new Vector3(x,y,1);

		go.transform.localRotation = marker.transform.localRotation;
		go.AddComponent<MeshRenderer>();
		go.GetComponent<MeshRenderer>().material = _mat;
		go.GetComponent<MeshRenderer>().sharedMaterial.shader = Shader.Find("2DTile");
		go.GetComponent<MeshRenderer>().castShadows = false;
		go.GetComponent<MeshRenderer>().receiveShadows = false;
		
		go.AddComponent<MeshFilter>();
		go.GetComponent<MeshFilter>().mesh = New2DMesh();
		
		if (_boxCollider)
		{
			go.AddComponent<BoxCollider>();
			go.GetComponent<BoxCollider>().size = new Vector3(1,1,3);
			
			go.GetComponent<BoxCollider>().isTrigger = _trigger;
		}
		if (_script != null)
		{
			go.AddComponent(_script.GetClass());
		}
	}

	GameObject DrawObject(Vector3 _pos,Material _mat,bool _hor,bool _ver)
	{
		GameObject go = new GameObject("tile_" + _pos.x.ToString() + "_" + _pos.y.ToString());
		go.transform.position = new Vector3(_pos.x + 0.5f,_pos.y + 0.5f,layer.level);
		
		float x = 1;
		float y = 1;

		if (_ver)
		{
			x = -1;
		}
		if (_hor)
		{
			y = -1;
		}
		go.transform.localScale = new Vector3(x,y,1);

		go.transform.localRotation = marker.transform.localRotation;
		go.AddComponent<MeshRenderer>();
		go.GetComponent<MeshRenderer>().material = _mat;
		go.GetComponent<MeshRenderer>().sharedMaterial.shader = Shader.Find("2DTile");
		go.GetComponent<MeshRenderer>().castShadows = false;
		go.GetComponent<MeshRenderer>().receiveShadows = false;
		
		go.AddComponent<MeshFilter>();
		go.GetComponent<MeshFilter>().mesh = New2DMesh();
		
		return go;
	}
	
	Mesh New2DMesh()
	{
		float _size = 0.5f;
		Mesh m = new Mesh();
	    m.name = "2dmesh";
		m.vertices = new Vector3[] {new Vector3(-_size, -_size, 0.01f),new Vector3(_size, -_size, 0.01f),new Vector3(_size, _size, 0.01f),new Vector3(-_size, _size, 0.01f)};
	    m.uv = new Vector2[] {new Vector2 (0, 0),new Vector2 (0, 1),new Vector2(1, 1),new Vector2 (1, 0)};
		m.triangles = new int[] {0, 1, 2, 0, 2, 3};
	    m.RecalculateNormals();
	    return m;
	}
	
	Material TileMat(Material _mat,Vector2 _tileSize,Vector2 _offset)
	{
		Material newMat = new Material(_mat);
		
		float a = newMat.mainTexture.width / 32;
		float offset = 1 / a;
		newMat.mainTextureScale = new Vector2(offset * _tileSize.x, offset * _tileSize.y);
		
		float tO1 = 1 - (offset * (_offset.y + 1));
		float tO2 = -offset + offset * (_tileSize.y);
		newMat.mainTextureOffset = new Vector2(offset * _offset.x, tO1 - tO2);
		
		return newMat;
	}
	
	void ClearLayer()
	{
		for(int i = 0; i < layer.tiles.transform.childCount; i++)
		{
			Transform tra = layer.tiles.transform.GetChild(i);
			DestroyImmediate(tra.gameObject);
			i--;
		}
		for(int i = 0; i < layer.objects.transform.childCount; i++)
		{
			Transform tra = layer.objects.transform.GetChild(i);
			DestroyImmediate(tra.gameObject);
			i--;
		}
	}
	
	bool TileOnPos(Vector3 _pos)
	{
		bool a = false;
		for(int i = 0; i < layer.transform.childCount; i++)
		{
			if(name[1].ToString() == _pos.x.ToString() && name[2].ToString() == _pos.y.ToString())
			{
				a = true;
				i = layer.transform.childCount;
			}
		}
		return a;
	}
	
	bool MouseOnLayer()
	{
		bool a = false;
		if(mouseHitPos.x > 0 && mouseHitPos.x < layer.layerSize.x && mouseHitPos.y > 0 && mouseHitPos.y < layer.layerSize.y) 
		{
			a = true;
		}
		return a;
	}
	
	void RemoveTile()
	{
		Vector2 _tilesize = new Vector2();
		Vector3 _pos = marker.transform.position;
		
		if (layer.usePrefab)
		{
			_tilesize = layer.prefabsTileSize[layer.prefabIndex];
		}
		else
		{
			_tilesize = layer.tileSize;
		}
		
		float posX = 0;
		float posY = 0;
			
		for (int a = 0; a < _tilesize.x; a++)
		{
			posX = a + _pos.x - 0.5f * _tilesize.x;
			
			for (int b = 0; b < _tilesize.y; b++)
			{
				posY = b + _pos.y - 0.5f * _tilesize.y;
				for(int i = 0; i < layer.tiles.transform.childCount; i++)
				{
					Transform tra = layer.tiles.transform.GetChild(i);
					string[] name = tra.name.Split("_"[0]);
					if(name.Length > 1 && name[1].ToString() == posX.ToString() && name[2].ToString() == posY.ToString())
					{
						DestroyImmediate(tra.gameObject);
						i--;
					}
				}
			}
		}
	}
	
	void RemoveObject()
	{
		Vector2 _tilesize = new Vector2();
		Vector3 _pos = marker.transform.position;
		
		if (layer.usePrefab)
		{
			_tilesize = layer.prefabsTileSize[layer.prefabIndex];
		}
		else
		{
			_tilesize = layer.tileSize;
		}
		
		float posX = 0;
		float posY = 0;
		
		
		for(int c = 0; c < layer.objects.transform.childCount; c++)
		{
			Transform traObject = layer.objects.transform.GetChild(c);
			for (int a = 0; a < _tilesize.x; a++)
			{
				posX = a + _pos.x - 0.5f * _tilesize.x;
				
				for (int b = 0; b < _tilesize.y; b++)
				{
					posY = b + _pos.y - 0.5f * _tilesize.y;
					for(int i = 0; i < traObject.childCount; i++)
					{
						Transform tra = traObject.GetChild(i);
						string[] name = tra.name.Split("_"[0]);
						if(name.Length > 1 && name[1].ToString() == posX.ToString() && name[2].ToString() == posY.ToString())
						{
							DestroyImmediate(tra.parent.gameObject);
							i = 10000;
							b = 10000;
							a = 10000;
							c = 10000;
							return;
						}
					}
				}
			}
		}		
	}
	
	void MoveMarker()
	{
		Vector2 tileSize = new Vector2();
		if (layer.usePrefab)
		{
			tileSize = layer.prefabsTileSize[layer.prefabIndex];
		}
		else
		{
			tileSize = layer.tileSize;
		}
		
		Vector3 pos = new Vector3((int)mouseHitPos.x + 0.5f * tileSize.x,(int)mouseHitPos.y + 0.5f * tileSize.y,(int)mouseHitPos.z);
		if (pos.x < 0.5f * tileSize.x)
		{
			pos = new Vector3(0.5f * tileSize.x,pos.y,pos.z);
		}
		else if (pos.x > layer.layerSize.x - 0.5f * tileSize.x)
		{
			pos = new Vector3(layer.layerSize.x - 0.5f * tileSize.x,pos.y,pos.z);
		}
		if (pos.y < 0.5f * tileSize.y)
		{
			pos = new Vector3(pos.x,0.5f * tileSize.y,pos.z);
		}
		else if (pos.y > layer.layerSize.y - 0.5f * tileSize.y)
		{
			pos = new Vector3(pos.x,layer.layerSize.y - 0.5f * tileSize.y,pos.z);
		} 
		marker.transform.position = new Vector3(pos.x,pos.y,layer.level - 0.05f);
	}
	
	void UpdateHitPosition()
	{
		Plane p = new Plane((this.target as MonoBehaviour).transform.TransformDirection(Vector3.forward), new Vector3(0,0,layer.level));
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Vector3 hit = new Vector3();
		   float dist;
		if (p.Raycast(ray, out dist))
			hit = ray.origin + ray.direction.normalized * dist;
		mouseHitPos = (this.target as MonoBehaviour).transform.InverseTransformPoint(hit);
	}
	
	void UpdateOutline(Color _color)
	{
		float r = _color.r;
		float g = _color.g;
		float b = _color.b;
		float a = 0.2f;
		markerTex = new Texture2D(1,1);
		markerTex.SetPixel(0,0,new Color(r,g,b,a));
		markerTex.Apply();
	}
	
	void UpdateTileLevelPos()
	{
		for(int i = 0; i < layer.tiles.transform.childCount; i++)
		{
			Transform tra = layer.tiles.transform.GetChild(i);
			string[] name = tra.name.Split("_"[0]);
			if(name[0] == "tile")
			{
				tra.position = new Vector3(tra.position.x,tra.position.y,layer.level);
			}
		}
	}
	
	public void UpdateMarker()
	{
		if (layer.usePrefab)
		{
			marker.UpdateMarker(layer.prefabsMat[layer.prefabIndex],layer.prefabsTileSize[layer.prefabIndex],layer.prefabsOffset[layer.prefabIndex]);
		}
		else
		{
			marker.UpdateMarker(layer.mat,layer.tileSize,layer.offset);	
		}
	}
	
	void DestroyMarker()
	{
		if (layer != null && layer.transform.childCount > 0)
		{
			for(int i = 0; i < layer.transform.childCount; i++)
			{
				Transform tra = layer.transform.GetChild(i);
				if(tra.name == "Marker")
				{
					DestroyImmediate(tra.gameObject);
					i--;
				}
			}
		}
	}
	
	public void UsePrefab(bool _use)
	{
		layer.usePrefab = _use;
		
		if (!_use)
		{
			 layer.prefabIndex = -1;
		}
	}
	
	void AddPrefab()
	{
		TilePrefabEditor window = TilePrefabEditor.CreateInstance<TilePrefabEditor>();
		window.Setup(layer,this,1);
		window.ShowUtility();
	}
	void RemovePrefab()
	{
		//Name
		string[] tmpPreName = layer.prefabsName;
		for (int i = layer.prefabIndex; i < layer.prefabsName.Length - 1; i++)
		{
			tmpPreName[i] = tmpPreName[i + 1];
		}
		layer.prefabsName = new string[tmpPreName.Length - 1];
		for (int i = 0; i < tmpPreName.Length - 1; i++)
		{
			layer.prefabsName[i] = tmpPreName[i];
		}
		
		//TileSize
		Vector2[] tmpPreTileSize = layer.prefabsTileSize;
		for (int i = layer.prefabIndex; i < layer.prefabsTileSize.Length - 1; i++)
		{
			tmpPreTileSize[i] = tmpPreTileSize[i + 1];
		}
		layer.prefabsTileSize = new Vector2[tmpPreTileSize.Length - 1];
		for (int i = 0; i < tmpPreTileSize.Length - 1; i++)
		{
			layer.prefabsTileSize[i] = tmpPreTileSize[i];
		}
		
		//Offset
		Vector2[] tmpPreOffset = layer.prefabsOffset;
		for (int i = layer.prefabIndex; i < layer.prefabsOffset.Length - 1; i++)
		{
			tmpPreOffset[i] = tmpPreOffset[i + 1];
		}
		layer.prefabsOffset = new Vector2[tmpPreOffset.Length - 1];
		for (int i = 0; i < tmpPreOffset.Length - 1; i++)
		{
			layer.prefabsOffset[i] = tmpPreOffset[i];
		}
		
		//Mat
		Material[] tmpPreMat = layer.prefabsMat;
		for (int i = layer.prefabIndex; i < layer.prefabsMat.Length - 1; i++)
		{
			tmpPreMat[i] = tmpPreMat[i + 1];
		}
		layer.prefabsMat = new Material[tmpPreMat.Length - 1];
		for (int i = 0; i < tmpPreMat.Length - 1; i++)
		{
			layer.prefabsMat[i] = tmpPreMat[i];
		}
		
		//BoxCollider
		bool[] tmpPreBoxCollider = layer.prefabBoxCollider;
		for (int i = layer.prefabIndex; i < layer.prefabBoxCollider.Length - 1; i++)
		{
			tmpPreBoxCollider[i] = tmpPreBoxCollider[i + 1];
		}
		layer.prefabBoxCollider = new bool[tmpPreBoxCollider.Length - 1];
		for (int i = 0; i < tmpPreBoxCollider.Length - 1; i++)
		{
			layer.prefabBoxCollider[i] = tmpPreBoxCollider[i];
		}
		
		//Trigger
		bool[] tmpPreTrigger = layer.prefabTrigger;
		for (int i = layer.prefabIndex; i < layer.prefabTrigger.Length - 1; i++)
		{
			tmpPreTrigger[i] = tmpPreTrigger[i + 1];
		}
		layer.prefabTrigger = new bool[tmpPreTrigger.Length - 1];
		for (int i = 0; i < tmpPreTrigger.Length - 1; i++)
		{
			layer.prefabTrigger[i] = tmpPreTrigger[i];
		}
		
		//MakeObject
		bool[] tmpPreMakeObject = layer.prefabMakeObject;
		for (int i = layer.prefabIndex; i < layer.prefabMakeObject.Length - 1; i++)
		{
			tmpPreMakeObject[i] = tmpPreMakeObject[i + 1];
		}
		layer.prefabMakeObject = new bool[tmpPreMakeObject.Length - 1];
		for (int i = 0; i < tmpPreMakeObject.Length - 1; i++)
		{
			layer.prefabMakeObject[i] = tmpPreMakeObject[i];
		}
		
		//MakeObject
		MonoScript[] tmpPreScript = layer.prefabScript;
		for (int i = layer.prefabIndex; i < layer.prefabScript.Length - 1; i++)
		{
			tmpPreScript[i] = tmpPreScript[i + 1];
		}
		layer.prefabScript = new MonoScript[tmpPreScript.Length - 1];
		for (int i = 0; i < tmpPreScript.Length - 1; i++)
		{
			layer.prefabScript[i] = tmpPreScript[i];
		}
		layer.prefabs--;

		if (layer.prefabIndex == layer.prefabs && layer.prefabs > 0)
		{
			layer.prefabIndex = layer.prefabs - 1;
		}
		if (layer.prefabs == 0)
		{
			UsePrefab(false);
		}
		else
		{
			UpdateMarker();
		}
	}
	void RemovePrefab2()
	{
		layer.prefabs = 0;
		layer.prefabsName = new string[0];
		layer.prefabsMat = new Material[0];
		layer.prefabsTileSize = new Vector2[0];
		layer.prefabsOffset = new Vector2[0];
		layer.prefabBoxCollider = new bool[0];
		layer.prefabTrigger = new bool[0];
		layer.prefabMakeObject = new bool[0];
		layer.prefabScript = new MonoScript[0];
		UsePrefab(false);
	}
	void ModifyPrefab()
	{
		TilePrefabEditor window = TilePrefabEditor.CreateInstance<TilePrefabEditor>();
		window.Setup(layer,this,0);
		window.ShowUtility();
	}
	
	void FlipHor()
	{
		marker.transform.localScale = new Vector3(marker.transform.localScale.x,-marker.transform.localScale.y,marker.transform.localScale.z);
	}
	void FlipVer()
	{
		marker.transform.localScale = new Vector3(-marker.transform.localScale.x,marker.transform.localScale.y,marker.transform.localScale.z);
	}
	void RotateTile()
	{
		//Transform tra = marker.transform;
		//marker.transform.eulerAngles = new Vector3(tra.eulerAngles.x,tra.eulerAngles.y,tra.eulerAngles.z + 90);	
	}
}