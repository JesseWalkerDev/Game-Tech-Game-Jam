using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	public Rigidbody2D rigidBody;
	public SpriteRenderer spriteRenderer;
	public Sprite idle;
	public Sprite walk1;
	public Sprite walk2;
	public Sprite jump;
	
	public float acceleration = 0.6f;
	public float maxSpeed = 6f;
	public float jumpForce = 8f;
	public float groundDrag = 0.92f;
	public float gravityForce = 0.7f;
	public float maxFallSpeed = 16f;
	public int maxJumpTime = 15;
	public int gravityReverseCoolDown = 5;
	
	
	private Vector2 respawnPoint;
	private int gravityReverseTime;
	private bool reverseGravity = false;
	private int jumpTime = 0;
	private bool jumping = false;
	private bool grounded = false;
	
	
	void Start()
	{
		respawnPoint = transform.position;
	}
	
	void Update()
	{
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		
		// Sprite
		if (grounded)
			if (horizontalInput != 0)
				if (Time.realtimeSinceStartup * 6 % 2 < 1)
					spriteRenderer.sprite = walk1;
				else
					spriteRenderer.sprite = walk2;
			else
				spriteRenderer.sprite = idle;
		else
			spriteRenderer.sprite = jump;
		
		// Sprite direction
		if (horizontalInput > 0)
			spriteRenderer.flipX = false;
		if (horizontalInput < 0)
			spriteRenderer.flipX = true;
		spriteRenderer.flipY = reverseGravity;
	}
	
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
			if (reverseGravity)
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, -jumpForce);
			else
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
		if (reverseGravity)
			rigidBody.velocity = new(rigidBody.velocity.x, Mathf.Min(maxFallSpeed, rigidBody.velocity.y + gravityForce));
		else
			rigidBody.velocity = new(rigidBody.velocity.x, Mathf.Max(-maxFallSpeed, rigidBody.velocity.y - gravityForce));
		gravityReverseTime --;
	}
	
	void OnCollisionStay2D(Collision2D collision)
    {
        grounded = false;
		foreach (ContactPoint2D contact in collision.contacts)
        {
			if (reverseGravity)
				if (contact.normal.y < 0.5)
					grounded = true;
			else
				if (contact.normal.y > 0.5)
					grounded = true;
		}
		grounded = true;
	}
	
	void OnCollisionExit2D(Collision2D collision)
	{
		grounded = false;
	}
	
	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Hazard"))
		{
			rigidBody.position = respawnPoint;
			rigidBody.velocity = Vector2.zero;
			reverseGravity = false;
			gravityReverseTime = gravityReverseCoolDown;
		}
		else if (collider.CompareTag("Gravity Toggle") && gravityReverseTime <= 0)
		{
			jumping = false;
			reverseGravity = !reverseGravity;
			gravityReverseTime = gravityReverseCoolDown;
		}
		else if (collider.CompareTag("Level End"))
		{
			int id = collider.gameObject.GetComponent<LevelEnd>().IdOfNextScene;
			Debug.Log("id");
			SceneManager.LoadScene(id);
		}
	}
}
