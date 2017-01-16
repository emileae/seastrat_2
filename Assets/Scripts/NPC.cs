using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	// NPC types
	public GameObject unemployedModel;
	public GameObject fishermanModel;
	public GameObject harpoonModel;

	public Blackboard blackboard;

	private bool haveItem = false;
	private GameObject item;
	private Item itemScript;

	// NPC occupation
	private bool isFisherman = false;
	private bool isHarpoonman = false;

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

	void OnTriggerEnter(Collider col){
		// layer 10 == item layer
		if (col.gameObject.layer == 10 && !haveItem) {
			Debug.Log ("Pick up the item!!!!!");
			item = col.gameObject;
			PickUpItem ();
		}
	}

	void OnTriggerExit(Collider col){
		
	}

	void PickUpItem(){
		Debug.Log ("Picking up the item");
		itemScript = item.GetComponent<Item> ();
		if (itemScript.fishingRod) {
			isFisherman = true;
			unemployedModel.SetActive (false);
			fishermanModel.SetActive (true);
			haveItem = true;
			// Destroy the rod
			itemScript.buildingScript.RemoveFishingRod();
			Destroy(item);
		}
	}
}
