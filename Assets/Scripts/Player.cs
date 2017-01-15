using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class Player : MonoBehaviour {

	// if using rigidbody
	private Rigidbody rb;

	//
	private float gravitySpeed = -50.0f;
	public float walkSpeed = 2.0f;
	private bool grounded = false;
	public LayerMask groundLayer;

	// ground check
	public Transform isGrounded;
	private float groundCheckRadius = 0.2f;

	// ground raycast
	private Bounds playerbounds;
	private float rayLength;

	// Ladder
	public bool onLadder = false;


	// Use this for initialization
	void Start () {
		playerbounds = gameObject.GetComponent<BoxCollider>().bounds;
		rayLength = playerbounds.size.y / 2 + 2.0f;

		rb = gameObject.GetComponent<Rigidbody> ();
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

//		RaycastHit rayHit;
//		bool hit = Physics.Raycast (transform.position, dwn, rayLength, groundLayer, out rayHit);

//		Debug.Log("Hit distance: " + hit.distance);

		Debug.DrawRay (transform.position, dwn * rayLength, Color.green);
//		if (hit) {
//			print ("There is something below the object!");
//			grounded = false;
//		}

//		RaycastHit hit;
//
//		if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
//			print ("Found an object - distance: " + hit.distance);
//			if (hit.distance <= (playerbounds.size.y / 2)) {
//				Debug.Log ("GROUNDEDDDDDD");
//				grounded = true;
//			} else {
//				grounded = false;
//			}
//		}
//
//		// gravity
//		if (!grounded) {
//			zMove += gravitySpeed * Time.deltaTime;
//		} else {
//			zMove = 0;
//		}
//
		// Ladder
		if (onLadder) {
			zMove = inputV;
		} else {
			zMove = 0.0f;
		}

		Vector3 velocityDirection = new Vector3 (xMove, 0, zMove);

		transform.Translate(velocityDirection * walkSpeed * Time.deltaTime);

	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag == "Ladder") {
			rb.useGravity = false;
			onLadder = true;
		}
	}
	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Ladder") {
			rb.useGravity = true;
			onLadder = false;
		}
	}
}
