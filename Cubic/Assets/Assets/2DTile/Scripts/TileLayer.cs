using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileLayer : MonoBehaviour
{
	public Vector2 layerSize = new Vector2(20,10);
	public Material mat;
	public Vector2 tileSize = new Vector2(1,1);
	public Vector2 offset = new Vector2(0,0);
	public int level = 0;
	public bool boxCollider = false;
	public bool trigger = false;
	public bool makeObject;
	public MonoScript script = null;
	public bool usePrefab;
	public bool showProperties = true;
	public bool showPrefab = false;
	
	public int prefabs = 0;
    public int prefabIndex = 0;
	public int[] prefabSize;
	public string[] prefabsName;
	public Material[] prefabsMat;
	public Vector2[] prefabsTileSize;
	public Vector2[] prefabsOffset;
	public bool[] prefabBoxCollider;
	public bool[] prefabTrigger;
	public MonoScript[] prefabScript;
	public bool[] prefabMakeObject;
	
	public GameObject tiles;
	public GameObject objects;
	
	void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.white;
        Gizmos.DrawLine(new Vector3(0,0,level), new Vector3(layerSize.x,0,level));
		Gizmos.DrawLine(new Vector3(0,0,level), new Vector3(0,layerSize.y,level));
		Gizmos.DrawLine(new Vector3(layerSize.x,0,level), new Vector3(layerSize.x,layerSize.y,level));
		Gizmos.DrawLine(new Vector3(0,layerSize.y,level), new Vector3(layerSize.x,layerSize.y,level));
		
		for (int i = 1; i < layerSize.x; i++)
		{
			Gizmos.color = Color.grey;
        	Gizmos.DrawLine(new Vector3(i,0,level), new Vector3(i,layerSize.y,level));
		}
		for (int i = 1; i < layerSize.y; i++)
		{
			Gizmos.color = Color.grey;
        	Gizmos.DrawLine(new Vector3(0,i,level), new Vector3(layerSize.x,i,level));
		}
    }
}