using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Blackboard blackboard;

	public bool rise = false;
	public bool sink = false;
	public bool attack = false;

	// movement
	private Vector3 direction = Vector3.zero;
	private float speed = 0.0f;
	public float floatSpeed = 2.0f;
	public float sinkSpeed = 3.0f;

	// Attack
	private bool busyAttacking = false;
	private bool attacking = false;
	public float riseDistance = 5.0f;
	public float jumpSpeed = 6.0f;
	public float dropSpeed = 6.0f;

	// Use this for initialization
	void Start () {
		if (blackboard == null) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (rise) {
			direction = Vector3.up;
			speed = floatSpeed;
		}
		if (sink) {
			direction = -Vector3.up;
			speed = floatSpeed;
		}
		if (attack) {
			busyAttacking = true;
			Attack ();
			if (!attacking) {
				attacking = true;
				blackboard.pushMenRight = false;
				StartCoroutine (StartAttack ());
			}
		}

		if (transform.position.y >= blackboard.seaYPos && !busyAttacking) {
			rise = false;
			direction = Vector3.zero;
		}

		transform.Translate(direction * speed * Time.deltaTime);

	}

	void ResetPosition(){
		transform.position = new Vector3 (transform.position.x, blackboard.seaYPos, transform.position.z);
	}

	void Attack(){
		if (transform.position.y >= riseDistance) {
			direction = -Vector3.up;
			speed = dropSpeed;
		}

		if (transform.position.y < blackboard.seaYPos) {
			direction = Vector3.zero;
			// reset back to sea level... otherwise the direction will stay at VEctor3.zero
			ResetPosition ();
		}
	}

	IEnumerator StartAttack(){
		yield return new WaitForSeconds(3.0f);

		direction = Vector3.up;
		speed = jumpSpeed;

		blackboard.pushMenRight = false;
		StartCoroutine (MakeWave());


		Debug.Log ("Attack!!!!!!");
		attacking = false;
	}

	IEnumerator MakeWave(){
		yield return new WaitForSeconds(2.8f);

		blackboard.pushMenRight = true;
	}

}
