using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
	public float speed = 3;
	private Vector3 dir;
	
	void Update ()
	{
		if (Input.GetAxis("Horizontal") > 0.2f && transform.position.x < 35)
		{
			dir = new Vector3(1,0,0);
		}
		else if (Input.GetAxis("Horizontal") < -0.2f && transform.position.x > 15)
		{
			dir = new Vector3(-1,0,0);
		}
		else
		{
			dir = new Vector3(0,0,0);
		}
		transform.Translate(dir * speed * Time.deltaTime);
	}
}
