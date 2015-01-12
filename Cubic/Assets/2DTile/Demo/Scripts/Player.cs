	using UnityEngine;      
using System.Collections;

public class Player : MonoBehaviour
{
		private float move ;

		public float maxSpeed = 20f;
		bool facingRight = true;
		public float jumpForce = 700;
		bool doubleJump = false;

		public Transform groundCheck;
		public LayerMask whatIsGround;
		bool grounded = false;
		float groundRadius = 0.2f;
				
		public Transform wallCheck;
		public LayerMask whatIsWall;
		bool walled = false;
		bool ancientwalled;
		float wallRadius = 0.52f;
		Collider2D _wallcollider2d;
		bool leftwall;
		//Nombre de frames à pendant laquelles le déplacement horizontal dépends de la direction du saut mural
		//Quand la variable=0/24 -> Inactif / Quand la variable entre 0 et 24 (1secondes)-> Gestion auto
		int freezeHorizontalMovementAfterWallJump = 0;

		bool isJumpPressed = false;

		Animator anim;


		void Start ()
		{
				anim = GetComponent<Animator> ();
		}

		/// <summary>
		/// Update (graphic) this instance.
		/// </summary>
		void Update ()
		{
				walled = isWalled (walled);
				if (!walled) {
						if ((move > 0 && !facingRight) || (move < 0 && facingRight)) 
								Flip ();
				}
				//Gestion des sauts
				isJumpPressed = Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.JoystickButton0);
				//Appui sur le bouton saut.
				if ((grounded || !doubleJump || walled) && isJumpPressed) {
						grounded = false;
						//Gestion de saut mural
						if (walled && move != 0) {
								rigidbody2D.velocity = new Vector2 (leftwall ? 10f : -10f, 4f);
								rigidbody2D.AddForce (new Vector2 (leftwall ? 5 : -5f, jumpForce), ForceMode2D.Force);
								walled = false;
								anim.SetBool ("walled", walled);
								freezeHorizontalMovementAfterWallJump++;
								Debug.Log ("move:" + rigidbody2D.velocity.x);
						} else if (!walled) {
								//Gestion de saut non mural
								rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, 0);
								rigidbody2D.AddForce (new Vector2 (0, !grounded ? jumpForce / 1.5f : jumpForce), ForceMode2D.Impulse);
								if (!doubleJump && !grounded) {
										doubleJump = true;
								}
								freezeHorizontalMovementAfterWallJump = 0;
						}
				} else if (walled && ((leftwall && move < 0) || (!leftwall && move > 0))) {
						//Amortissement de la chute
						rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, rigidbody2D.velocity.y / 1.5f);
				}
				updateAnimations (walled, grounded);
		}

		/// <summary>
		/// Fixeds the update.
		/// </summary>
		void FixedUpdate ()
		{
				if (freezeHorizontalMovementAfterWallJump > 0 && freezeHorizontalMovementAfterWallJump < 48) {
						freezeHorizontalMovementAfterWallJump++;
						move = leftwall ? tresholdMove (Mathf.Abs (Input.GetAxis ("Horizontal"))) : tresholdMove (-Mathf.Abs (Input.GetAxis ("Horizontal")));
				} else {
						freezeHorizontalMovementAfterWallJump = 0;
						move = tresholdMove (Input.GetAxis ("Horizontal"));
				}
				grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
				if (grounded) {
						doubleJump = false;
				}
				rigidbody2D.velocity = new Vector2 (move * maxSpeed * (doubleJump ? 0.4f : 1), rigidbody2D.velocity.y);
				updateFixedAnimations ();
		}

		/// <summary>
		/// Flip this instance.
		/// </summary>
		void Flip ()
		{
				facingRight = !facingRight;
				Vector3 theScale = transform.localScale;
				theScale.x *= -1;
				transform.localScale = theScale;
		}

		/// <summary>
		/// Tresholds the move.
		/// </summary>
		/// <returns>The move.</returns>
		/// <param name="move">Move.</param>
		private float tresholdMove (float move)
		{
				if (move <= -0.5) {
						move = -0.5f;
				} else if (move > -0.5f && move <= -0.1f) {
						move = -0.2f;
				} else if (move > -0.1f && move <= 0.1f) {
						move = 0f;
				} else if (move > 0.1f && move <= 0.5f) {
						move = 0.2f;
				} else {
						move = 0.5f;
				}
				return move;
		}

		/// <summary>
		/// Updates the animations.
		/// </summary>
		/// <param name="walled">If set to <c>true</c> walled.</param>
		/// <param name="grounded">If set to <c>true</c> grounded.</param>
		/// <param name="speed">Speed.</param>
		/// <param name="move">Move.</param>
		private void updateAnimations (bool walled, bool grounded)
		{
				anim.SetBool ("ground", grounded);
				anim.SetBool ("walled", walled);
		}

		/// <summary>
		/// Updates the fixed animations.
		/// </summary>
		private void updateFixedAnimations ()
		{
				anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
				anim.SetFloat ("speed", Mathf.Abs (move));
		}

		/// <summary>
		/// Detects the walled state.
		/// </summary>
		/// <returns><c>true</c>, if walled was used, <c>false</c> otherwise.</returns>
		/// <param name="walled">If set to <c>true</c> walled.</param>
		private bool isWalled (bool walled)
		{
				bool ancientwalled = walled;
				if (!grounded) {
						_wallcollider2d = Physics2D.OverlapCircle (wallCheck.position, wallRadius, whatIsWall);
						if (_wallcollider2d) {
								leftwall = (_wallcollider2d.bounds.center.x - transform.position.x) < 0;
								walled = (leftwall && move < 0) || (!leftwall && move > 0);
								if (walled && !ancientwalled && ((leftwall && !facingRight) || (!leftwall && facingRight))) {
										Flip ();
								}
						}
				} else {
						walled = false;
				}
				return walled;
		}
}