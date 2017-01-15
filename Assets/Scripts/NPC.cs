using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	public Blackboard blackboard;

	public GameObject leftSensor;
	public GameObject rightSensor;

	public GameObject currentPlatform;

	private BoxCollider collider;
	private Bounds npcBounds;

	// walking
	private Vector3 direction = Vector3.right;
	public LayerMask groundLayer;
	public float walkSpeed = 2.0f;

	// Use this for initialization
	void Start () {
		collider = gameObject.GetComponent<BoxCollider> ();
		npcBounds = collider.bounds;

		if (currentPlatform != null) {
			transform.position = new Vector3 (transform.position.x, blackboard.platform1TopPos, transform.position.z);
		}
	}

	// Update is called once per frame
	void Update () {

		if (!Physics.CheckSphere (leftSensor.transform.position, 0.5f, groundLayer)) {
			Debug.Log ("Left Sphere edge");
			direction = Vector3.right;
		}
		if (!Physics.CheckSphere (rightSensor.transform.position, 0.5f, groundLayer)) {
			Debug.Log ("Right Sphere edge");
			direction = -Vector3.right;
		}

		transform.Translate(direction * walkSpeed * Time.deltaTime);
	}
}
