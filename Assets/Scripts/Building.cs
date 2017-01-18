using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	private Building buildingScript;

	public Blackboard blackboard;

	public bool isMainHouse = false;
	public bool isFishingRodHouse = false;
	public bool isHarpoonHouse = false;
	public bool isFishingSpot = false;
	public bool isSignalFire = false;
	public bool isLadder = false;

	public GameObject fishingRodHouse;
	public GameObject harpoonHouse;
	private float houseZdist = 8.0f;

	// Player sensing
	private bool playerPresent = false;

	public bool active = false;
	public int costToBuild = 3;
	public int costOfItem = 2;

	// fishingRodHouse prefabs
	public GameObject fishingRod;
	private List<GameObject> fishingRodList = new List<GameObject>();
	private int numRods = 0;

	// general prefabs
	public GameObject hollowCoin;
	public GameObject coin;
	public int coinsAdded = 0;
	public bool paid = false;

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
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void PayCoin ()
	{
		if (!active) {
			if (coinsAdded < costToBuild) {
				AddCoin ();
				if (coinsAdded == costToBuild) {
					active = true;
					paid = true;

					Debug.Log ("Paid in full... remove cost indicators");

					RemoveHollowCoins ();
					RemoveCoins ();

					// after building show the item cost without having to re-enter trigger
					if (costOfItem > 0 && playerPresent) {
						ShowCostOfItem ();
					}

					// add new houses if main house
					if (isMainHouse) {
						AddFishingRodHouse ();
						AddHarpoonHouse ();
					}

				}
			}
		} else if (active) {
			if (coinsAdded < costOfItem) {
				AddCoin();
				if (coinsAdded == costOfItem) {
					paid = true;
					if (isFishingRodHouse){
						GameObject clone = Instantiate(fishingRod, new Vector3(transform.position.x + (1*numRods), transform.position.y, 0), Quaternion.identity) as GameObject;
						clone.GetComponent<Item> ().buildingScript = buildingScript;
						fishingRodList.Add(clone);
						numRods += 1;
					}
					if (isSignalFire && paid){
						inactiveFireModel.SetActive(false);
						activeFireModel.SetActive(true);
						blackboard.activeSignalFire = true;
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
		float coinYPos = transform.position.y;
		if (isLadder) {
			coinYPos = blackboard.ladderCoinYPos;
		} else {
			coinYPos = buildingBounds.max.y;
		}
			
		if (hollowCoinList.Count <= 0 || hollowCoinList == null) {
			for (int i = 0; i < costOfItem; i++) {
				GameObject clone = Instantiate (hollowCoin, new Vector3 (transform.position.x, coinYPos + 1 + (1 * i), 0), Quaternion.identity) as GameObject;
				hollowCoinList.Add (clone);
			}
		} else {
			for (int i = 0; i < hollowCoinList.Count; i++) {
				hollowCoinList[i].SetActive(true);
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
		GameObject clone = Instantiate(fishingRodHouse, new Vector3(transform.position.x - buildingBounds.max.x,transform.position.y, houseZdist), Quaternion.identity) as GameObject;
	}
	void AddHarpoonHouse(){
		GameObject clone = Instantiate(harpoonHouse, new Vector3(transform.position.x + buildingBounds.max.x,transform.position.y, houseZdist), Quaternion.identity) as GameObject;
	}

	public void RemoveFishingRod(){
		fishingRodList.RemoveAt(0);
		numRods -= 1;
	}

	// Ladder
	void ChargeLadder(){
		Debug.Log ("Add credits to ladder");
		numToClimbLadder += 1;
	}

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
