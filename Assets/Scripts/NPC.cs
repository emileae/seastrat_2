using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	// NPC types
	public GameObject unemployedModel;
	public GameObject fishermanModel;
	public GameObject harpoonModel;

	public Blackboard blackboard;

	private bool haveItem = false;// haveItem --> employed / active i.e. can't do anything else
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
	public Building mainHouseScript;

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
	private bool isSelling = false;
	public bool atMainHouse = false;
	private bool offloading = false;

	//fisherman
	private GameObject homeFishingSpot;
	private bool atFishingSpot = false;
	private bool goFish = false;
	private bool isFishing = false;
	public int numFish = 0;

	// Ladder
	private bool onLadder = false;
	public bool climbLadder = false;

	// Use this for initialization
	void Start () {
		collider = gameObject.GetComponent<BoxCollider> ();
		npcBounds = collider.bounds;

		if (currentPlatform != null) {
			platformScript = currentPlatform.GetComponent<Platform>();
			platformIndex = platformScript.platformIndex;
			mainHouse = blackboard.platformMainHouses[platformIndex];
			mainHouseScript = mainHouse.GetComponent<Building> ();
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

		// going to fishing spot to catch fish
		// goFish = NPC's task is to fish isFishing = is bus catching a fish at the fishing spot
		if (isFisherman) {
			FishermanLogic();
		}

		if (!haveItem && onLadder) {
			LadderLogic ();
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

		// if payable and a waypoint... i.e. fishing spot, farm, boat building, ladderPayment point or whatever
		if (go.layer == 9) {
			buildingScript = go.GetComponent<Building> ();
			if (isFisherman) {
				if (buildingScript.isFishingSpot) {
					atFishingSpot = true;
				}
				if (homeFishingSpot == null) {
					if (isFisherman && buildingScript.isFishingSpot && buildingScript.active && !buildingScript.occupied) {
						buildingScript.occupied = true;
						homeFishingSpot = go;
						stop = true;
						goFish = true;
					}
				} else {
					Debug.Log ("should STOP!!!");
					// if moving over home fishing spot and have no fish, then go fishing
					if (go == homeFishingSpot && numFish <= 0 && isFisherman) {
						Debug.Log ("STOP!!!");
						stop = true;
						goFish = true;
					}
				}
			}

			if (buildingScript.isMainHouse) {
				atMainHouse = true;
				Debug.Log("Passing Main House..........");
			}
			if (buildingScript.isLadder) {
				Debug.Log("Check if NPC should climb ladder..........");
				CheckLadderAvailability (buildingScript);
			}
			if (go.tag == "Ladder") {
				onLadder = true;// now on actual ladder, not just ladder payment area
			}
		}
	}

	void OnTriggerExit(Collider col){
		GameObject go = col.gameObject;
		if (go.layer == 9) {
			buildingScript = go.GetComponent<Building> ();

			if (isFisherman) {
				if (buildingScript.isFishingSpot) {
					atFishingSpot = false;
				}
				// if leaving home fishing spot
				if (buildingScript.isFishingSpot && buildingScript.active && go == homeFishingSpot) {
					stop = false;
//					goFish = false;
				}
			}

			if (buildingScript.isMainHouse) {
				atMainHouse = false;
				Debug.Log("Leaving Main House..........");
			}
		}
	}

	void MoveTowardsTarget(GameObject target){
		if (target.transform.position.x > transform.position.x) {
			direction = Vector3.right;
		} else {
			direction = -Vector3.right;
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
		yield return new WaitForSeconds(1.0f);
		numFish += 1;
		isFishing = false;
		Debug.Log("Caught a fish");
	}
	IEnumerator Sell(){
		Debug.Log ("going to market");
		yield return new WaitForSeconds(1.0f);
		numFish -= 1;
		Debug.Log ("numFish: " + numFish);
		if (mainHouseScript != null) {
			mainHouseScript.DepositCoin();
		}
		isSelling = false;
	}

	void ReturnToFishingSpot(){
		Debug.Log ("returning to fishing spot");
		//reverse direction and start moving
		if (homeFishingSpot.transform.position.x > mainHouse.transform.position.x) {
			direction = Vector3.right;
		} else {
			direction = -Vector3.right;
		}
		goFish = true;
		stop = false;
		speedModifier = 1;
	}

	void FishermanLogic(){
		if (goFish && !isFishing && atFishingSpot) {
			if (numFish >= blackboard.fishermanCapacity) {
				Debug.Log ("Take Fish to market.....");
				stop = false;
				speedModifier = 1.0f;
				isFishing = false;
				goFish = false;// stop fishing and go sell
				if (transform.position.x > mainHouse.transform.position.x) {
					direction = -Vector3.right;
				} else {
					direction = Vector3.right;
				}
			} else {
				isFishing = true;
				StartCoroutine (GoFish ());
			}

		}

		// going to sell fish
		if (atMainHouse) {
			if (numFish > 0) {
				stop = true;
				speedModifier = 0;
				if (!isSelling) {
					isSelling = true;
					StartCoroutine (Sell ());
				}
			} else if (numFish <= 0) {
				Debug.Log ("Go back to fishing spot");
				ReturnToFishingSpot ();
			}
		}
	}

	void CheckLadderAvailability(Building buildingScript){
		if (!haveItem && buildingScript.numToClimbLadder > 0) {
			Debug.Log ("Go to climb ladder");
			climbLadder = true;
			buildingScript.numToClimbLadder -= 1;
			if (buildingScript.Ladder != null) {
				MoveTowardsTarget (buildingScript.Ladder);
			}
		}
	}
	void LadderLogic(){
		Debug.Log ("execute ladder logic");
		if (climbLadder) {
			stop = true;
		}
	}

}
