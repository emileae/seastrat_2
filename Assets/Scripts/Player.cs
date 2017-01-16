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

	// Coin-ing
	public GameObject payTarget;
	private bool canPayCoin = false;
	private bool canCollectCoin = false;
	public int coins = 0;
	private bool coinActive = false;


	// Use this for initialization
	void Start () {
		playerbounds = gameObject.GetComponent<BoxCollider>().bounds;
		rayLength = playerbounds.size.y / 2 + 2.0f;

		rb = gameObject.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float inputV = Input.GetAxisRaw ("Vertical");

		if (!onLadder && !coinActive && inputV < 0) {
			coinActive = true;
			if (canPayCoin) {
				StartCoroutine (PayCoin ());
			}else if (canCollectCoin){
				StartCoroutine (CollectCoin ());
			}
		}
	}

	void FixedUpdate ()
	{

		float inputH = Input.GetAxisRaw ("Horizontal");
		float inputV = Input.GetAxisRaw ("Vertical");

		float xMove = inputH;
		float zMove = inputV;

//		Vector3 dwn = transform.TransformDirection (-Vector3.forward);

//		Debug.DrawRay (transform.position, dwn * rayLength, Color.green);
		// Ladder
		if (onLadder) {
			zMove = inputV;
		} else {
			zMove = 0.0f;
		}

		Vector3 velocityDirection = new Vector3 (xMove, zMove, 0);

		transform.Translate(velocityDirection * walkSpeed * Time.deltaTime);

	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag == "Ladder") {
			rb.useGravity = false;
			onLadder = true;
		}
		if (col.gameObject.tag == "CollectCoin") {
			Debug.Log("Can collect coin.....");
			canCollectCoin = true;
			payTarget = col.gameObject;
		}
		// payable layer  == 9
		if (col.gameObject.layer == 9) {
			Debug.Log("Can pay coin.....");
			canPayCoin = true;
			payTarget = col.gameObject;
		}
	}
	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Ladder") {
			rb.useGravity = true;
			onLadder = false;
		}
		if (col.gameObject.tag == "CollectCoin") {
			canCollectCoin = false;
		}
		if (col.gameObject.layer == 9) {
			canPayCoin = false;
		}
	}

	IEnumerator PayCoin(){
		yield return new WaitForSeconds(0.2f);
		coinActive = false;
		Debug.Log("Pay a coin");
		payTarget.GetComponent<Building>().PayCoin();
	}
	IEnumerator CollectCoin(){
		yield return new WaitForSeconds(0.2f);
		coinActive = false;
		Debug.Log("Collect a coin");
//		payTarget.GetComponent<Building>().CollectCoin();
	}
}
