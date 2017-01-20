using UnityEngine;
using System.Collections;

public class Boat : MonoBehaviour {

	public Blackboard blackboard;

	public float speed = 3.0f;
	public Vector3 direction = Vector3.right;
	public float speedModifier = 1;
	public bool stop = false;
	public bool reachedDock = false;

	// NPC passengers
	private bool spawningNPC = false;
	private NPC npcScript;
	public GameObject NPC;
	public int passengers = 3;

	// Use this for initialization
	void Start () {
		if (blackboard == null) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard>();
		}
		// set boat position
		transform.position = new Vector3(transform.position.x, blackboard.seaYPos, blackboard.platformBounds[0].min.z);

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
					if (!spawningNPC && passengers > 0) {
						spawningNPC = true;
						StartCoroutine(SpawnNPC ());
					}
				}
			}
			// if going left and signal fire activated before reaching dock
			else if (direction == -Vector3.right && transform.position.x > blackboard.dock.transform.position.x)
			{
				if (reachedDock) {
					stop = true;
					if (!spawningNPC && passengers > 0) {
						spawningNPC = true;
						StartCoroutine(SpawnNPC ());
					}
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

	IEnumerator SpawnNPC(){

		yield return new WaitForSeconds (2.0f);

		Debug.Log ("Instantiate NPC!!!");
		float xPos = blackboard.platformBounds [0].max.x;
		float yPos = blackboard.platformBounds [0].max.y;
		GameObject targetMainHouse = blackboard.platformMainHouses [0];
		GameObject clone = Instantiate(NPC, new Vector3(xPos, yPos, 0), Quaternion.identity) as GameObject;

		passengers -= 1;
		spawningNPC = false;

		if (passengers <= 0) {
			// sink the ship!!!
			Destroy (gameObject);
			blackboard.ExtinguishSignalFire ();
		}

	}



}
