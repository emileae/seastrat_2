using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {

	private float gravitySpeed = -3.0f;
	public float walkSpeed = 2.0f;
	private bool grounded = false;
	public LayerMask groundLayer;

	// ground check
	public Transform isGrounded;
	private float groundCheckRadius = 0.2f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		float inputH = Input.GetAxisRaw ("Horizontal");
		float inputV = Input.GetAxisRaw ("Vertical");

		float xMove = inputH;
		float zMove = inputV;

		// gravity
		grounded = Physics.CheckSphere (isGrounded.position, groundCheckRadius, groundLayer);
		if (!grounded) {
			Debug.Log ("Should not be falling");
			zMove += gravitySpeed;
		} else {
			zMove = 0f;
		}

		Vector3 velocityDirection = new Vector3 (xMove, 0, zMove);

		transform.Translate(velocityDirection * walkSpeed * Time.deltaTime);

	}

	void FixedUpdate(){

	}
}
