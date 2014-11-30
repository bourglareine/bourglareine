using UnityEngine;
using System.Collections;

public class TileMarker : MonoBehaviour
{
	public GameObject outline;
	
	public static TileMarker CreateMarker(TileLayer layer,Texture _outline)
	{
		Vector2 _tileSize = new Vector2();
		if (layer.usePrefab)
		{
			_tileSize = layer.prefabsTileSize[layer.prefabIndex];
		}
		else
		{
			_tileSize = layer.tileSize;
		}
		
		
		
		GameObject go = new GameObject("Marker");
		go.transform.parent = layer.transform;
		go.transform.localScale = new Vector3(0.5f * _tileSize.y, 0.5f * _tileSize.x, 0.5f);
		Quaternion quaternion = go.transform.localRotation;
		quaternion.eulerAngles = new Vector3(0, 180, 90);
		go.transform.localRotation = quaternion;

		TileMarker marker = go.AddComponent<TileMarker>();
		go.AddComponent<MeshRenderer>();
		
		Material mat = new Material(Shader.Find("2DTile"));
		go.GetComponent<MeshRenderer>().material = new Material(mat);
		
		go.AddComponent<MeshFilter>();
		go.GetComponent<MeshFilter>().mesh = NewMesh();

		GameObject goOut = new GameObject("Outline");
		goOut.transform.parent = layer.transform;
		goOut.transform.localScale = new Vector3(0.5f * _tileSize.y, 0.5f * _tileSize.x, 0.5f);
		Quaternion quaternionOut = go.transform.localRotation;
		quaternionOut.eulerAngles = new Vector3(0, 180, 90);
		goOut.transform.localRotation = quaternionOut;

		goOut.AddComponent<MeshRenderer>();
		
		Material matOut = new Material(Shader.Find("2DTile"));
		goOut.GetComponent<MeshRenderer>().material = new Material(matOut);
		goOut.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = _outline;
		
		goOut.AddComponent<MeshFilter>();
		goOut.GetComponent<MeshFilter>().mesh = NewMesh();
		goOut.transform.parent = go.transform;
		goOut.transform.position = new Vector3(go.transform.position.x,go.transform.position.y,-0.1f);
		go.GetComponent<TileMarker>().outline = goOut;
		return marker;
	}
	
	public void UpdateMarker(Material _mat,Vector2 _tileSize,Vector2 _offset)
	{
		GetComponent<MeshRenderer>().sharedMaterial.mainTexture = _mat.mainTexture;
		float a = renderer.sharedMaterial.mainTexture.width / 32;
		float offset = 1 / a;
		renderer.sharedMaterial.mainTextureScale = new Vector2(offset * _tileSize.x, offset * _tileSize.y);
		
		float tO1 = 1 - (offset * (_offset.y + 1));
		float tO2 = -offset + offset * (_tileSize.y);
		renderer.sharedMaterial.mainTextureOffset = new Vector2(offset * _offset.x, tO1 - tO2);
		
		transform.localScale = new Vector3(0.5f * _tileSize.y, 0.5f * _tileSize.x, 0.5f);
		//outline.transform.localScale = new Vector3(0.5f * layer.tileSize.y, 0.5f * layer.tileSize.x, 0.5f);
	}
	public void UpdateOutline(Texture _tex)
	{
		outline.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = _tex;
	}
	
	public void Destroy()
	{
		DestroyImmediate(gameObject);
	}
	
	static Mesh NewMesh()
	{
		float size = 1;
		Mesh m = new Mesh();
	    m.name = "2dmesh";
		m.vertices = new Vector3[] {new Vector3(-size, -size, 0.01f),new Vector3(size, -size, 0.01f),new Vector3(size, size, 0.01f),new Vector3(-size, size, 0.01f)};
	    m.uv = new Vector2[] {new Vector2 (0, 0),new Vector2 (0, 1),new Vector2(1, 1),new Vector2 (1, 0)};
		m.triangles = new int[] {0, 1, 2, 0, 2, 3};
	    m.RecalculateNormals();
		return m;
	}
}
