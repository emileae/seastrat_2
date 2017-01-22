using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	// NPC types
	public GameObject unemployedModel;
	public GameObject fishermanModel;
	public GameObject harpoonModel;

	public Blackboard blackboard;

	public bool haveItem = false;// haveItem --> employed / active i.e. can't do anything else
	private GameObject item;
	private Item itemScript;

	// NPC occupation
	private bool isFisherman = false;
	private bool isHarpoonman = false;

	public GameObject leftSensor;
	public GameObject rightSensor;

	// PLATFORM information about the NPCs platform - if this changes then need to update the function that updates the paltform... lower down ins cript
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
	private GameObject Ladder = null;
	public bool onLadder = false;
	public bool climbLadder = false;

	// Use this for initialization
	void Start ()
	{
		collider = gameObject.GetComponent<BoxCollider> ();
		npcBounds = collider.bounds;

		if (blackboard == null) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}

		if (currentPlatform != null) {
			platformScript = currentPlatform.GetComponent<Platform> ();
			platformIndex = platformScript.platformIndex;
			mainHouse = blackboard.platformMainHouses [platformIndex];
			mainHouseScript = mainHouse.GetComponent<Building> ();
			Debug.Log ("NPC starting platform index: " + platformIndex);
			Debug.Log ("blackboard.platformTopPos.Count: " + blackboard.platformTopPos.Count);
			Debug.Log ("Starting NPC POS: " + blackboard.platformTopPos [platformIndex]);
			transform.position = new Vector3 (transform.position.x, blackboard.platformTopPos [platformIndex], transform.position.z);
		} else {
			UpdatePlatform (platformIndex);
		}

		if (mainHouse) {
			MoveTowardsTarget (mainHouse);
		}
	}

	// Update is called once per frame
	void Update ()
	{

	// climbLadder overrides this since ladder collider may have a small gap from platform
		if (!Physics.CheckSphere (leftSensor.transform.position, 0.5f, groundLayer) && !climbLadder) {
			Debug.Log ("Left Sphere edge");
			if (blackboard.pushMenRight) {
				direction += new Vector3 (0, -0.1f, 0);
			} else {
				direction = Vector3.right;
			}

		}
		if (!Physics.CheckSphere (rightSensor.transform.position, 0.5f, groundLayer) && !climbLadder) {
			Debug.Log ("Right Sphere edge");
			if (blackboard.pushMenRight) {
				direction += new Vector3 (0, -0.1f, 0);
			} else {
				direction = -Vector3.right;
			}
		}

		if (stop) {
			speedModifier = 0;
		} else {
			speedModifier = 1;
		}

		// going to fishing spot to catch fish
		// goFish = NPC's task is to fish isFishing = is bus catching a fish at the fishing spot
		if (isFisherman) {
			FishermanLogic();
		}

		if (!haveItem && onLadder) {
			Debug.Log ("!have item && onLAdder");
			LadderLogic ();
		}

		if (blackboard.pushMenRight) {
			direction -= new Vector3 (-0.5f, 0, 0);
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

		// Payable
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
				Debug.Log("Check if NPC should climb ladder.......... " + platformIndex);
				CheckLadderAvailability (buildingScript);
			}
		}

		// Ladder
		if (go.tag == "Ladder") {
			onLadder = true;// now on actual ladder, not just ladder payment area
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

	public void MoveTowardsTarget(GameObject target){
		if (target.transform.position.x > transform.position.x) {
			Debug.Log("target is on right..." + platformIndex);
			direction = Vector3.right;
		} else {
			Debug.Log("target is on left..." + platformIndex);
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

	void FishermanLogic ()
	{
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
				Debug.Log ("Go back to fishing spot if it exists");
				if (homeFishingSpot != null) {
					ReturnToFishingSpot ();
				}
			}
		}
	}

	void UpdatePlatform(int idx){
		platformIndex = idx;
		Debug.Log("idx: " + idx);
		Debug.Log("blackboard??? " + blackboard.platforms[idx]);
		currentPlatform = blackboard.platforms[idx];
		platformScript = blackboard.platformScripts[idx];
		mainHouse = blackboard.platformMainHouses[idx];
		mainHouseScript = mainHouse.GetComponent<Building>();
	}

	void CheckLadderAvailability(Building buildingScript){
		if (!haveItem && buildingScript.numToClimbLadder > 0) {
			Debug.Log ("Go to climb ladder " + platformIndex);
			climbLadder = true;
			buildingScript.numToClimbLadder -= 1;
			Ladder = buildingScript.Ladder;
			if (buildingScript.Ladder != null) {
				MoveTowardsTarget (buildingScript.Ladder);
			}
		}
	}
	void LadderLogic ()
	{
		Debug.Log ("execute ladder logic " + platformIndex);
		if (climbLadder && Ladder != null) {
			stop = true;
			Ladder ladderScript = Ladder.GetComponent<Ladder> ();
			int targetPlatformIndex = 0;

			bool goingUp = false;
			bool goingDown = false;

			if (platformIndex > ladderScript.bottomPlatformIndex) {
				Debug.Log ("Climb down ladder " + platformIndex);
				direction = -Vector3.up;
				stop = false;
				goingUp = false;
				goingDown = true;
				targetPlatformIndex = ladderScript.bottomPlatformIndex;
			} else {
				Debug.Log ("Climb up ladder " + platformIndex);
				direction = Vector3.up;
				stop = false;
				goingUp = true;
				goingDown = false;
				targetPlatformIndex = ladderScript.topPlatformIndex;
			}

			Debug.Log ("Target platform index: " + targetPlatformIndex);

			if (goingUp && transform.position.y >= blackboard.platformTopPos [targetPlatformIndex]) {
				ExitLadder(targetPlatformIndex);
			}else if (goingDown && transform.position.y <= blackboard.platformTopPos [targetPlatformIndex]){
				ExitLadder(targetPlatformIndex);
			}

		}
	}
	void ExitLadder (int targetPlatformIndex)
	{
		UpdatePlatform(targetPlatformIndex);
		Debug.Log ("Stop at target end of ladder platform index: " + targetPlatformIndex);
		onLadder = false;
		climbLadder = false;
		stop = true;
		// choose direction to move off ladder
		if (transform.position.x > blackboard.platformBounds [targetPlatformIndex].max.x) {
			direction = -Vector3.right;
			stop = false;
		} else if (transform.position.x < blackboard.platformBounds [targetPlatformIndex].min.x) {
			direction = Vector3.right;
			stop = false;
		} else {
			MoveTowardsTarget(mainHouse);
			stop = false;
		}
	}

}
