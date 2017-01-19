using UnityEngine;
using System.Collections;

public class Boat : MonoBehaviour {

	public Blackboard blackboard;

	public float speed = 3.0f;
	public Vector3 direction = Vector3.right;
	public float speedModifier = 1;
	public bool stop = false;
	public bool reachedDock = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckSignalFire();

		if (stop) {
			speedModifier = 0.0f;
		} else {
			speedModifier = 1.0f;
		}

		transform.Translate(direction * speed * speedModifier * Time.deltaTime);
	}

	void CheckSignalFire ()
	{
		if (blackboard.activeSignalFire) {
			Debug.Log("Active signal fire!!!!!!!");
			// if going right adn signal fire activated before reaching dock
			if (direction == Vector3.right && transform.position.x < blackboard.dock.transform.position.x) {
				Debug.Log("In correct position, so can stop at dock!@!@!@!@!");
				if (reachedDock) {
					stop = true;
				}
			}
			// if going left and signal fire activated before reaching dock
			else if (direction == -Vector3.right && transform.position.x > blackboard.dock.transform.position.x)
			{
				if (reachedDock) {
					stop = true;
				}
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		GameObject go = col.gameObject;
		if (go.tag == "Dock") {
			reachedDock = true;
		}
	}
	void OnTriggerExit (Collider col)
	{
		GameObject go = col.gameObject;
		if (go.tag == "Dock") {
			reachedDock = false;
		}
	}

}
