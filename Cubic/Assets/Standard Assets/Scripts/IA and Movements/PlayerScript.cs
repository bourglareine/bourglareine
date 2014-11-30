using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
	public float speed = 3.0F;
	
	void Update()
	{
		CharacterController controller = GetComponent<CharacterController>();
		Vector3 forward = transform.TransformDirection(Vector3.right);
		float curSpeed = speed * Input.GetAxis("Horizontal");
		controller.SimpleMove(forward * curSpeed);
	}
}
