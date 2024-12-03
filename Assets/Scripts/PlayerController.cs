using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Rigidbody2D rigidBody;
	
	public float acceleration = 0.6f;
	public float maxSpeed = 6f;
	public float jumpForce = 8f;
	public float groundDrag = 0.92f;
	public float gravityForce = 25f;
	public int maxJumpTime = 15;
	
	private int jumpTime = 0;
	private bool jumping = false;
	private bool grounded = false;
	
	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		
		// Horizontal movement
		rigidBody.velocity = new Vector2(
			Mathf.Clamp(rigidBody.velocity.x + horizontalInput * acceleration, -maxSpeed, maxSpeed),
			rigidBody.velocity.y);
		
		if (grounded && (horizontalInput == 0f || Mathf.Sign(horizontalInput) != Mathf.Sign(rigidBody.velocity.x)))
			// Drag if no horizontal input
			rigidBody.velocity = new Vector2(rigidBody.velocity.x * groundDrag, rigidBody.velocity.y);
		
		// Jump
		if (Input.GetKey(KeyCode.Space) && (grounded || jumping))
		{
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
			if (grounded)
				jumpTime = 0;
			else
				jumpTime ++;
			jumping = jumpTime < maxJumpTime;
		}
		else
			jumping = false;
		
		// Gravity
		rigidBody.AddForce(Vector2.down * gravityForce);
	}
	
	void OnCollisionStay2D(Collision2D collision)
    {
        grounded = false;
		foreach (ContactPoint2D contact in collision.contacts)
        {
			if (contact.normal.y >= 0.5)
                grounded = true;
		}
	}
	
	void OnCollisionExit2D(Collision2D collision)
	{
		grounded = false;
	}
}
