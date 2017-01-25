using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	private Building buildingScript;

	public Blackboard blackboard;

	public bool isMainHouse = false;
	public bool isBank = false;
	public bool isFishingRodHouse = false;
	public bool isHarpoonHouse = false;
	public bool isFishingSpot = false;
	public bool isSignalFire = false;
	public bool isLadder = false;

	// Platform - the building's platform index
	public LayerMask groundLayer;// this is probably optional to set, it was only used for trying to automatically detect platform on Start()
	public int platformIndex;// set this when building in instantiated

	public GameObject fishingRodHouse;
	public GameObject harpoonHouse;
	private float houseZdist = 8.0f;

	// Player sensing
	private bool playerPresent = false;

	public bool payable = true;// all buildings start off payable so that player can pay to be built, then become unpayable when waiting for builder
	public bool active = false;
	public int costToBuild = 3;
	public int costOfItem = 2;

	// mainHouse prefabs
	public GameObject hammer;
	private List<GameObject> hammerList = new List<GameObject>();
	private int numHammers = 0;

	// fishingRodHouse prefabs
	public GameObject fishingRod;
	private List<GameObject> fishingRodList = new List<GameObject>();
	private int numRods = 0;

	// harpoonHouse prefabs
	public GameObject harpoon;
	private List<GameObject> harpoonList = new List<GameObject>();
	private int numHarpoons = 0;

	// general prefabs
	public GameObject hollowCoin;
	public GameObject coin;
	public int coinsAdded = 0;
	public bool paid = false;
	public bool waitingForBuilder = false;

	// depositing coins for player to take
	public bool hasInitialCoins = false;
	public int coinsDeposited = 0;

	// money
	private List<GameObject> hollowCoinList = new List<GameObject>();
	private List<GameObject> coinList = new List<GameObject>();
	private List<GameObject> depositedCoinList = new List<GameObject>();

	// building bounds
	private BoxCollider collider;
	private Bounds buildingBounds;

	// occupied by NPC - in case of fishing spot  & prob hunting spot, farms & maybe others
	public bool occupied = false;

	// Signal Fire
	public GameObject activeFireModel;
	public GameObject inactiveFireModel;

	// Ladder
	public int numToClimbLadder = 0;
	public GameObject Ladder;

	// Use this for initialization
	void Start ()
	{
		buildingScript = gameObject.GetComponent<Building> ();
		collider = gameObject.GetComponent<BoxCollider> ();
		buildingBounds = collider.bounds;

		if (blackboard == null) {
			blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		}

		if (!active && costToBuild > 0) {
			for (int i = 0; i < costToBuild; i++) {
				GameObject clone = Instantiate (hollowCoin, new Vector3 (transform.position.x, buildingBounds.max.y + 1 + (1 * i), 0), Quaternion.identity) as GameObject;
				hollowCoinList.Add (clone);
			}
		}

		// initial coins
		if (coinsDeposited > 0) {
			for (int i = 0; i < coinsDeposited; i++) {
				GameObject clone = Instantiate (coin, new Vector3 (transform.position.x, buildingBounds.min.y - (1 * i), -3.1f), Quaternion.identity) as GameObject;
				depositedCoinList.Add (clone);
			}
			hasInitialCoins = true;
		}

		if (isSignalFire) {
			if (blackboard.signalFire == null) {
				blackboard.signalFire = gameObject;
				blackboard.SetSignalFireScript ();
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void PayCoin ()
	{
		Debug.Log (".-.-.-Building PayCoin()...");
		if (!active) {
			if (coinsAdded < costToBuild) {
				AddCoin ();
				if (coinsAdded == costToBuild) {
//					active = true;
					// TODO
					// trigger a function to call a builder to activate (active=true) building
					waitingForBuilder = true;
					paid = true;
					payable = false;

					Debug.Log ("Paid in full... remove cost indicators");

					RemoveHollowCoins ();
					RemoveCoins ();

					// after building show the item cost without having to re-enter trigger
					if (costOfItem > 0 && playerPresent) {
						ShowCostOfItem ();
					}

					// add new houses if main house
					// TODO
					// add the active build areas, but need a builder to actually build them

					// mainHouse doesn't need a builder
					if (isMainHouse) {
						active = true;
						waitingForBuilder = false;
						payable = true;
						AddFishingRodHouse ();
						AddHarpoonHouse ();
					} else {
						CallBuilder();
					}

				}
			}
		} else if (active) {
			if (coinsAdded < costOfItem) {
				Debug.Log ("Adding a coin---- building");
				AddCoin();
				if (coinsAdded == costOfItem) {
					paid = true;

					// ITEM SPAWNING HERE

					if (isMainHouse){
						GameObject clone = Instantiate(hammer, new Vector3(transform.position.x + (1*numHammers), transform.position.y, 0), Quaternion.identity) as GameObject;
						clone.GetComponent<Item> ().buildingScript = buildingScript;
						hammerList.Add(clone);
						numHammers += 1;
					}
					if (isFishingRodHouse){
						GameObject clone = Instantiate(fishingRod, new Vector3(transform.position.x + (1*numRods), transform.position.y, 0), Quaternion.identity) as GameObject;
						clone.GetComponent<Item> ().buildingScript = buildingScript;
						fishingRodList.Add(clone);
						numRods += 1;
					}
					if (isHarpoonHouse){
						GameObject clone = Instantiate(harpoon, new Vector3(transform.position.x + (1*numHarpoons), transform.position.y, 0), Quaternion.identity) as GameObject;
						clone.GetComponent<Item> ().buildingScript = buildingScript;
						harpoonList.Add(clone);
						numHarpoons += 1;
					}
					if (isSignalFire && paid){
						ActivateSignalFire ();
					}
					if (isLadder) {
						ChargeLadder ();
					}

					RemoveHollowCoins ();
					RemoveCoins ();

					// after building show the item cost without having to re-enter trigger
					if (costOfItem > 0 && playerPresent) {
						ShowCostOfItem ();
					}
				}
			}
		}
	}

	// builder calls this function to 'build'
	void CallBuilder ()
	{
		if (blackboard.builders.Count > 0) {
			for (int i = 0; i < blackboard.builders.Count; i++) {
				if (blackboard.builders [i].platformIndex == platformIndex) {
					Debug.Log ("Call the Builder...");
					blackboard.builders [i].AddBuildingToList (gameObject);
				}
			}
		} else {
			blackboard.AddGameObjectToList(gameObject, blackboard.buildingsWaitingForBuilder);
		}
	}
	public void ActivateBuilding(){
		payable = true;
		waitingForBuilder = false;
		active = true;
	}

	void AddCoin(){
		GameObject clone = Instantiate (coin, new Vector3 (transform.position.x, buildingBounds.max.y + 1 + (1 * coinsAdded), -0.1f), Quaternion.identity) as GameObject;
		coinList.Add (clone);
		coinsAdded += 1;
	}

	public void DepositCoin(){
		// coins need to be positioned at z=-3 to be visible... fix this with design magic-ing
		GameObject clone = Instantiate (coin, new Vector3 (transform.position.x, buildingBounds.min.y - (1 * coinsDeposited), -3.1f), Quaternion.identity) as GameObject;
		depositedCoinList.Add (clone);
		coinsDeposited += 1;
	}

	public int CollectCoin ()
	{
		if (coinsDeposited > 0) {
			// once initial coins are gone then reset to original state of hasInitialCoins -> false
			// the last coin is collected when coinsDeposited == 1 here
			if (hasInitialCoins && coinsDeposited <= 1) {
				hasInitialCoins = false;
			}
			Destroy (depositedCoinList [depositedCoinList.Count - 1]);// destroy game object first
			depositedCoinList.RemoveAt ((depositedCoinList.Count - 1));// after destroying game objet then remove from list
			coinsDeposited -= 1;
			return 1;
		}else{
			return 0;
		}
	}

	void RemoveHollowCoins(){
		for (int i = 0; i < hollowCoinList.Count; i++) {
			Destroy (hollowCoinList [i]);
			if (i == (hollowCoinList.Count - 1)) {
				hollowCoinList.Clear ();
			}
		}
	}

	void ShowCostOfItem ()
	{
		if (active) {
			float coinYPos = transform.position.y;
			if (isLadder) {
				coinYPos = transform.position.y + blackboard.ladderCoinYPos;
			} else {
				coinYPos = buildingBounds.max.y;
			}
			
			if (hollowCoinList.Count <= 0 || hollowCoinList == null) {
				for (int i = 0; i < costOfItem; i++) {
					GameObject clone = Instantiate (hollowCoin, new Vector3 (transform.position.x, coinYPos + (1 * i), 0), Quaternion.identity) as GameObject;
					hollowCoinList.Add (clone);
				}
			} else {
				for (int i = 0; i < hollowCoinList.Count; i++) {
					hollowCoinList [i].SetActive (true);
				} 
			}
		}
	}
	void HideCostOfItem ()
	{
		for (int i = 0; i < hollowCoinList.Count; i++) {
			hollowCoinList[i].SetActive(false);
		}
	}

	void RemoveCoins ()
	{
		Debug.Log ("coinList.Count: " + coinList.Count);
		for (int i = 0; i < coinList.Count; i++) {
			Destroy (coinList [i]);
			if (i == (coinList.Count - 1)) {
				coinList.Clear ();
			}
		}

		if (!paid && coinsAdded > 0) {
			Debug.Log ("coinsAdded: " + coinsAdded);
			for (int i = 0; i < coinsAdded; i++) {
				// return coin to player
				blackboard.playerScript.coinsInHand += 1;
			}
		}

		coinsAdded = 0;
		// finally if all coins have been returned or item is paid for then set paid to false
		paid = false;
	}

	void AddFishingRodHouse(){
		GameObject clone = Instantiate(fishingRodHouse, new Vector3(transform.position.x - buildingBounds.size.x,transform.position.y, houseZdist), Quaternion.identity) as GameObject;
	}
	void AddHarpoonHouse(){
		GameObject clone = Instantiate(harpoonHouse, new Vector3(transform.position.x + buildingBounds.size.x,transform.position.y, houseZdist), Quaternion.identity) as GameObject;
	}

	// ITEMS PICKED UP HERE

	public void RemoveHammer(){
		hammerList.RemoveAt(0);
		numHammers -= 1;
	}
	public void RemoveFishingRod(){
		fishingRodList.RemoveAt(0);
		numRods -= 1;
	}
	public void RemoveHarpoon(){
		harpoonList.RemoveAt(0);
		numHarpoons -= 1;
	}

	// Ladder
	void ChargeLadder(){
		Debug.Log ("Add credits to ladder----------");
		numToClimbLadder += 1;
	}

	// Signal Fire
	public void ActivateSignalFire(){
		inactiveFireModel.SetActive(false);
		activeFireModel.SetActive(true);
		blackboard.activeSignalFire = true;
	}
	public void DeactivateSignalFire(){
		inactiveFireModel.SetActive(true);
		activeFireModel.SetActive(false);
		blackboard.activeSignalFire = false;
	}

	// TRIGGERS

	void OnTriggerEnter (Collider col)
	{
		GameObject go = col.gameObject;
		if (go.tag == "Player") {
			playerPresent= true;
			if (active) {
				Debug.Log ("Show cost of item........");
				ShowCostOfItem ();
			}
		}
	}
	void OnTriggerExit (Collider col)
	{
		GameObject go = col.gameObject;
		if (go.tag == "Player") {
			Debug.Log ("Hide the cost to build object....");
			playerPresent = false;
//			if (!active) {
//				if (!paid && coinsAdded > 0) {
//					RemoveCoins();
//				}
//			} else {
//				HideCostOfItem ();
//			}
			if (!paid && coinsAdded > 0) {
				RemoveCoins ();
			}
			if (active) {
				HideCostOfItem ();
			}
		}
	}

}
