using UnityEngine;      
using System.Collections;

public class Player : MonoBehaviour {

	public float maxSpeed=20f;
	bool facingRight = true;
	public float jumpForce =700;
	bool doubleJump = false;

	public Transform groundCheck;
	public LayerMask whatIsGround;
	bool grounded=false;
	float groundRadius =0.2f;
	
	public Transform wallCheck;
	public LayerMask whatIsWall;
	bool walled=false;
	bool ancientwalled;
	float wallRadius =0.52f;


	Animator anim;


	void Start()
	{
		anim = GetComponent<Animator> ();
	}

	void Update()
	{
		if ((grounded || !doubleJump) && (Input.GetKeyDown (KeyCode.Space)|| Input.GetKeyDown(KeyCode.JoystickButton0))) 
		{
			anim.SetBool ("ground",false);

            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
			rigidbody2D.AddForce (new Vector2(0,!grounded?jumpForce/1.5f:jumpForce),ForceMode2D.Impulse);

			if (!doubleJump && !grounded)
			{doubleJump=true;}
		}
	}

	void FixedUpdate()
	{

		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		ancientwalled= walled;
		if (!grounded) {
						walled = Physics2D.OverlapCircle (wallCheck.position, wallRadius, whatIsWall);
						if (walled) {
								doubleJump = false;
				if(!ancientwalled){
					Flip();
				}
						}
		} else {walled=false;
				}
		anim.SetBool ("walled", walled);
		anim.SetBool("ground", grounded);

		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);

		if (grounded) {
			doubleJump=false;}

		// if (!grounded) return; // non maniabilité du joueur en air  

		float move = Input.GetAxis ("Horizontal");
		if (move <= -0.5) {
						move = -0.5f;
				} else if (move > -0.5f && move <= -0.1f) {
						move = -0.2f;
				} else if (move > -0.1f && move <= 0.1f) {
						move = 0f;
				} else if (move > 0.1f && move <= 0.5f) {
						move = 0.2f;
		} else {move=0.5f;
				}
		anim.SetFloat("speed", Mathf.Abs(move));
		// "Opérateur ternaire"(cf.google pour le terme):a partie du code : doubleJumpe?0.4f:1) est équivalente à : if(doubleJump){valeur=0.4}else{valeur=1} et valeur est injectée directement dans le code 

			rigidbody2D.velocity = new Vector2 (move * maxSpeed*(doubleJump?0.4f:1), rigidbody2D.velocity.y);

		if (move > 0 && !facingRight) 
						Flip ();
				else if (move < 0 && facingRight)
						Flip ();

	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}