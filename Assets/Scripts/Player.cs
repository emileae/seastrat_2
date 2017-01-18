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

	// Buildings
	private bool atMainHouse = false;
	private Building buildingScript;

	// Coin-ing
	public int coinsInHand = 0;
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
			}else if (atMainHouse){
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
		GameObject go = col.gameObject;
		if (go.tag == "Ladder") {
			rb.useGravity = false;
			onLadder = true;
		}
		// payable layer  == 9
		if (go.layer == 9) {
			Debug.Log ("At a building.... so money can move");
			buildingScript = go.GetComponent<Building> ();
			canPayCoin = true;
			payTarget = go;
			if (buildingScript.isMainHouse) {
				atMainHouse = true;
				// if main house is already activated
				if (buildingScript.active) {
					canPayCoin = false;
				}
			}
		}
	}
	void OnTriggerExit (Collider col)
	{
		GameObject go = col.gameObject;
		if (go.tag == "Ladder") {
			rb.useGravity = true;
			onLadder = false;
		}
		if (go.layer == 9) {
			Debug.Log ("Left a building.... so money can't move");
			buildingScript = null;
			canPayCoin = false;
			payTarget = null;
			if (buildingScript != null) {
				if (buildingScript.isMainHouse) {
					atMainHouse = false;
				}
			}
		}
	}

	IEnumerator PayCoin(){
		yield return new WaitForSeconds(0.2f);
		coinActive = false;
		Debug.Log("Pay a coin");
		payTarget.GetComponent<Building>().PayCoin();
	}
	IEnumerator CollectCoin ()
	{
		yield return new WaitForSeconds (0.2f);
		coinActive = false;
		if (buildingScript != null) {
			int coinsCollected = buildingScript.CollectCoin ();
			coinsInHand += coinsCollected;

		}
	}
}
