using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
	public Transform targetTransform;
	public float snappiness = 0.1f;
	
	private Vector2 target;
	
	// Start is called before the first frame update
	void Start()
	{
		target = targetTransform.position;
	}

	// Update is called once per frame
	void Update()
	{
		target = new(
			Mathf.Clamp(target.x, targetTransform.position.x - 2f, targetTransform.position.x + 2f),
			Mathf.Clamp(target.y, targetTransform.position.y - 1.5f, targetTransform.position.y + 4f)
		);
		
		transform.position = new(
			Mathf.Lerp(transform.position.x, target.x, snappiness),
			Mathf.Lerp(transform.position.y, target.y, snappiness),
			transform.position.z
		);
	}
}
