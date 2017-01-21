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
	private bool attacking = false;

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
			if (!attacking) {
				attacking = true;
				StartCoroutine (Attack ());
			}
		}

		if (transform.position.y >= blackboard.seaYPos) {
			rise = false;
			direction = Vector3.zero;
		}

		transform.Translate(direction * speed * Time.deltaTime);

	}

	IEnumerator Attack(){
		yield return new WaitForSeconds(3.0f);

		Debug.Log ("Attack!!!!!!");
		attacking = false;
	}

}
