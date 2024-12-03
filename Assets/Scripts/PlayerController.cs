using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Rigidbody2D rBody;
	
	public float acceleration = 20;
	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		rBody.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * acceleration, rBody.velocity.y));
	}
}
