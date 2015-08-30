		using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class Player : MonoBehaviour
{
	private float move;

	public float maxSpeed = 20f;
	bool facingRight = true;
	public float jumpForce = 700;
	bool doubleJump = false;
	bool buttonJumpReleased = false;

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
	bool nearWall = false;
	bool? leftwall;
	//Nombre de frames Ã  pendant laquelles le dÃ©placement horizontal dÃ©pends de la direction du saut mural
	//Quand la variable=0/24 -> Inactif / Quand la variable entre 0 et 24 (1secondes)-> Gestion auto
	int freezeHorizontalMovementAfterWallJump = 0;

	bool isJumpPressed = false;

	Animator anim;


	void Start ()
	{
		anim = GetComponent<Animator> ();
		Advertisement.Initialize ("34788");
		if (Advertisement.isReady ()) {
			Advertisement.Show ();
		}
	}

	/// <summary>
	/// Update (graphic) this instance.
	/// </summary>
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.M)) {
			GameControl gameControl = GameObject.FindObjectOfType<GameControl> ();
			gameControl.game.SaveCount++;
			SaveLoad.Save (0);
		}

		nearWall = nearWalled ();
		ancientwalled = walled;
		walled = ((leftwall.HasValue && leftwall.Value && Input.GetAxis ("Horizontal") < 0) || (leftwall.HasValue && !leftwall.Value && Input.GetAxis ("Horizontal") > 0)) && (nearWall);
				
		Debug.Log ("nearwall:" + nearWall + " walled:" + walled);
		if (!nearWall) {
			if ((move > 0 && !facingRight) || (move < 0 && facingRight))
				Flip ();
		} else {
			if (!ancientwalled && walled)
				Flip ();
		}
		//Gestion des sauts
		isJumpPressed = Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.JoystickButton0);
		if (isJumpPressed && buttonJumpReleased) {
			if (nearWall) {
				//Gestion de saut mural
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (leftwall.Value == true ? 10f : -10f, 4f);
				GetComponent<Rigidbody2D> ().AddForce (new Vector2 (leftwall.Value == true ? 5f : -5f, jumpForce * 3.0f), ForceMode2D.Force);
				walled = true;
				Flip ();
				anim.SetBool ("walled", walled);
				walled = false;
				freezeHorizontalMovementAfterWallJump++;
				Debug.Log ("saut mural");
				return;
			} else if (!doubleJump) {
				//Gestion de saut non mural
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, 0);
				GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, !grounded ? jumpForce / 1.5f : jumpForce), ForceMode2D.Impulse);
				doubleJump = true;
				Debug.Log ("saut non mural");
			}
			buttonJumpReleased = false;
		} else {
			if (Input.GetKeyDown (KeyCode.X) && !grounded) {
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -5f);
			}
			if (walled) {
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, GetComponent<Rigidbody2D> ().velocity.y / 1.5f);
			}
			buttonJumpReleased = true;
		}
		updateAnimations (false, grounded);
	}

	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void FixedUpdate ()
	{
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		if (grounded) {
			doubleJump = false;
		}
		if (freezeHorizontalMovementAfterWallJump > 0 && freezeHorizontalMovementAfterWallJump < 48) {
			freezeHorizontalMovementAfterWallJump++;
			if (leftwall.HasValue)
				move = leftwall.Value == true ? 0.5f : -0.5f;
			leftwall = null;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (move * maxSpeed, GetComponent<Rigidbody2D> ().velocity.y);
		} else {
			freezeHorizontalMovementAfterWallJump = 0;
			move = tresholdMove (Input.GetAxis ("Horizontal"));
			if (move != 0) {
				string lo = "l";
				lo.Clone ();
			}
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (move * maxSpeed * (doubleJump ? 0.4f : 1), GetComponent<Rigidbody2D> ().velocity.y);
		}
		updateFixedAnimations ();
		updateAnimations (walled, grounded);
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
		anim.SetFloat ("vSpeed", GetComponent<Rigidbody2D> ().velocity.y);
		anim.SetFloat ("speed", Mathf.Abs (move));
	}

	/// <summary>
	/// Detects the walled state.
	/// </summary>
	/// <returns><c>true</c>, if walled was used, <c>false</c> otherwise.</returns>
	/// <param name="walled">If set to <c>true</c> walled.</param>
	private bool nearWalled ()
	{
		bool isnearwall = false;
		if (!grounded) {
			_wallcollider2d = Physics2D.OverlapCircle (wallCheck.position, wallRadius, whatIsWall);
			if (_wallcollider2d) {
				leftwall = (_wallcollider2d.bounds.center.x - transform.position.x) < 0;
				//Limits of the edges
				float yOffset = _wallcollider2d.bounds.size.y / 2 - this.GetComponent<Collider2D> ().bounds.size.y / 2;
				//Debug.Log((_wallcollider2d.gameObject.transform.position.y - transform.position.y <= yOffset).ToString());
				isnearwall = (_wallcollider2d.gameObject.transform.position.y - transform.position.y <= yOffset);
			} 
		}
		return isnearwall;
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (freezeHorizontalMovementAfterWallJump > 0) {
			move = 0f;
		}
		freezeHorizontalMovementAfterWallJump = 0;
	}
}