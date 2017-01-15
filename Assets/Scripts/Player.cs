using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class Player : MonoBehaviour {

	private float gravitySpeed = -2.0f;
	public float walkSpeed = 2.0f;
	private bool grounded = false;
	public LayerMask groundLayer;

	// ground check
	public Transform isGrounded;
	private float groundCheckRadius = 0.2f;

	// ground raycast
	private Bounds playerbounds;
	private float rayLength;


	// Use this for initialization
	void Start () {
		playerbounds = gameObject.GetComponent<BoxCollider>().bounds;
		rayLength = playerbounds.size.y / 2 + 1.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{

//		float inputH = Input.GetAxisRaw ("Horizontal");
//		float inputV = Input.GetAxisRaw ("Vertical");
//
//		float xMove = inputH;
//		float zMove = inputV;
//
//		// gravity
////		grounded = Physics.CheckSphere (isGrounded.position, groundCheckRadius, groundLayer);
//		if (!grounded) {
//			Debug.Log ("Should not be falling");
//			zMove += gravitySpeed;
//		} else {
//			zMove = 0;
//		}
//
//		Vector3 velocityDirection = new Vector3 (xMove, 0, zMove);
//
//		transform.Translate(velocityDirection * walkSpeed * Time.deltaTime);

	}

	void FixedUpdate ()
	{
//		float inputH = Input.GetAxisRaw ("Horizontal");
//		float inputV = Input.GetAxisRaw ("Vertical");
//
//		float xMove = inputH;
//		float zMove = inputV;
//
//		// gravity
////		grounded = Physics.CheckSphere (isGrounded.position, groundCheckRadius, groundLayer);
//		if (!grounded) {
//			Debug.Log ("Should not be falling");
//			zMove += gravitySpeed;
//		} else {
//			zMove = 0;
//		}
//
//		Vector3 velocityDirection = new Vector3 (xMove, 0, zMove);
//
//		transform.Translate(velocityDirection * walkSpeed * Time.deltaTime);

		// Raycast

		float inputH = Input.GetAxisRaw ("Horizontal");
		float inputV = Input.GetAxisRaw ("Vertical");

		float xMove = inputH;
		float zMove = inputV;

		Vector3 dwn = transform.TransformDirection (-Vector3.forward);

		RaycastHit rayHit;
		bool hit = Physics.Raycast (transform.position, dwn, rayLength, groundLayer, out rayHit);

//		Debug.Log("Hit distance: " + hit.distance);

		Debug.DrawRay (transform.position, dwn * rayLength, Color.green);
		if (hit) {
			print ("There is something below the object!");
			grounded = false;
		}

		// gravity
		if (!grounded) {
			Debug.Log ("Should not be falling");
			zMove += gravitySpeed;
		} else {
			zMove = 0;
		}

		Vector3 velocityDirection = new Vector3 (xMove, 0, zMove);

		transform.Translate(velocityDirection * walkSpeed * Time.deltaTime);

	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.layer == 8) {
			grounded = true;
		}
	}
	void OnTriggerExit(Collider col){
		if (col.gameObject.layer == 8) {
			grounded = false;
		}
	}
}
