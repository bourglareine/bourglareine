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
		    public bool IsGrounded = false;
		    public bool Jumping = false;
		    public bool inAir = false;
		    public float airControl = 0.5f;
			    bool facingRight=true;


		    // 2 - Store the movement
		    private Vector2 movement;
		    private Vector2 force;
		    private Vector2 actualForce;
		    private Vector2 upTranform2d = new Vector2(0, 1.0f);
		    private Vector2 jumpVector;
			private RaycastHit2D hit;

				    void FixedUpdate()
					    {
					        Movement();
					    }

		    void Movement()
		    {
		float move = Input.GetAxis ("Horizontal");
				rigidbody2D.velocity= new Vector2(move*speed.x,rigidbody2D.velocity.y);
		if (move > 0 && !facingRight) {
						Flip ();
				} else if (move < 0 && facingRight) {
				Flip ();
				}
		    }

	void Flip(){
		facingRight = !facingRight;
		transform.localScale = new Vector3 (transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
		}
		}