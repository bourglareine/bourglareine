using UnityEngine;


/// <summary>
/// Player controller and behavior
/// </summary>
public class Player : MonoBehaviour
{
	/// <summary>
	/// 1 - The speed of the ship
	/// </summary>
	public Vector2 speed = new Vector2(50, 5);
	public float jumpSpeed = 500.0f;
	public bool IsGrounded= false;
	public bool Jumping = false;
	public bool inAir  = false;
	public float airControl = 0.5f;

	
	// 2 - Store the movement
	private Vector2 movement;
	private Vector2 force;
	private Vector2 actualForce;
	private Vector2 upTranform2d=new Vector2(0,1.0f);
	private Vector2 jumpVector;
	void Update()
	{

	}
	
	void FixedUpdate()
	{
		if (!IsGrounded){
			if (Physics.Raycast(transform.position, -transform.up, 
			                    gameObject.GetComponent<BoxCollider2D>().size.sqrMagnitude/(2 + 0.2f))){
				IsGrounded = true;
				Jumping = false;
				inAir = false;
			}
			else if (!inAir){
				inAir = true;
				jumpVector = movement;
			} 
		}  
		Movement ();
	}

	void Movement(){
		movement = new Vector2 (Input.GetAxis ("Horizontal") * Time.deltaTime * speed.x, 0);
		if (Input.GetButtonDown("Jump") &&IsGrounded){
			throw new UnityException("toto");
			Jumping = true;
			jumpVector=new Vector2(upTranform2d.x,upTranform2d.y*jumpSpeed);
			rigidbody2D.AddForce(jumpVector);
			inAir=true;
		}


		if (IsGrounded) {
						this.transform.Translate((movement.normalized * speed.x) *Time.deltaTime);
				}
		else if (Jumping || inAir){
			float airSpeed=speed.x*airControl;
			this.transform.Translate((movement.normalized * airSpeed) *Time.deltaTime);
		}
	}
}