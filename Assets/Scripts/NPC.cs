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

	// information about the NPCs platform
	public GameObject currentPlatform;
	private Platform platformScript;
	private int platformIndex = 0;
	public GameObject mainHouse;

	private BoxCollider collider;
	private Bounds npcBounds;

	// interact with buildings
	private Building buildingScript;

	// walking
	private Vector3 direction = Vector3.right;
	public LayerMask groundLayer;
	public float walkSpeed = 2.0f;
	public float speedModifier = 1.0f;
	private bool stop = false;

	// going to mainhouse
	private bool atMainHouse = false;
	private bool offloading = false;

	//fisherman
	private bool goFish = false;
	private bool isFishing = false;
	private int numFish = 0;

	// Use this for initialization
	void Start () {
		collider = gameObject.GetComponent<BoxCollider> ();
		npcBounds = collider.bounds;

		if (currentPlatform != null) {
			platformScript = currentPlatform.GetComponent<Platform>();
			platformIndex = platformScript.platformIndex;
			mainHouse = blackboard.platformMainHouses[platformIndex];
			transform.position = new Vector3 (transform.position.x, blackboard.platformTopPos[platformIndex], transform.position.z);
		}
	}

	// Update is called once per frame
	void Update ()
	{

		if (!Physics.CheckSphere (leftSensor.transform.position, 0.5f, groundLayer)) {
			Debug.Log ("Left Sphere edge");
			direction = Vector3.right;
		}
		if (!Physics.CheckSphere (rightSensor.transform.position, 0.5f, groundLayer)) {
			Debug.Log ("Right Sphere edge");
			direction = -Vector3.right;
		}

		if (stop) {
			speedModifier = 0;
		}
		if (goFish && !isFishing) {
			if (numFish >= blackboard.fishermanCapacity) {
				Debug.Log ("Take Fish to market.....");
				stop = false;
				isFishing = false;
				if (transform.position.x > mainHouse.transform.position.x) {
					direction = -Vector3.right;
				} else {
					direction = Vector3.right;
				}
				if (atMainHouse) {
					stop = true;
					speedModifier = 0;
					Debug.Log ("At the market to sell " + numFish + " fish.");
					StartCoroutine (Sell ());
				} else {
					speedModifier = 1;
				}
			} else {
				isFishing = true;
				StartCoroutine (GoFish ());
			}
		}

		transform.Translate(direction * walkSpeed * speedModifier * Time.deltaTime);
	}

	void OnTriggerEnter (Collider col)
	{
		GameObject go = col.gameObject;
		// layer 10 == item layer
		if (go.layer == 10 && !haveItem) {
			Debug.Log ("Pick up the item!!!!!");
			item = go;
			PickUpItem ();
		}

		// if payable and a waypoint... i.e. fishing spot, farm, boat building or whatever
		if (go.layer == 9) {
			Debug.Log ("Can do something here");
			buildingScript = go.GetComponent<Building> ();
			if (isFisherman && buildingScript.isFishingSpot && buildingScript.active) {
				stop = true;
				goFish = true;
			}

			if (buildingScript.isMainHouse) {
				atMainHouse = true;
				Debug.Log("Passing Main House..........");
			}
		}
	}

	void OnTriggerExit(Collider col){
		GameObject go = col.gameObject;
		if (go.layer == 9) {
			buildingScript = go.GetComponent<Building> ();
			if (isFisherman && buildingScript.isFishingSpot && buildingScript.active) {
				stop = false;
//				goFish = true;
			}

			if (buildingScript.isMainHouse) {
				atMainHouse = false;
				Debug.Log("Leaving Main House..........");
			}
		}
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

	IEnumerator GoFish(){
		yield return new WaitForSeconds(2.0f);
		numFish += 1;
		isFishing = false;
		Debug.Log("Caught a fish");
	}
	IEnumerator Sell(){
		yield return new WaitForSeconds(10.0f);
		numFish += 1;
		isFishing = false;
		Debug.Log("Caught a fish");
	}

}
