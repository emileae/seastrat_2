using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	private Building buildingScript;

	public bool isMainHouse = false;
	public bool isFishingRodHouse = false;
	public bool isHarpoonHouse = false;
	public bool isFishingSpot = false;

	public GameObject fishingRodHouse;
	public GameObject harpoonHouse;
	private float houseZdist = 8.0f;

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
	private bool paid = false;

	// depositing coins for player to take
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

	// Use this for initialization
	void Start ()
	{
		buildingScript = gameObject.GetComponent<Building> ();
		collider = gameObject.GetComponent<BoxCollider> ();
		buildingBounds = collider.bounds;

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
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	// this code is replicated in HotSpot.cs
	public void PayCoin ()
	{
		if (!active) {
			if (coinsAdded < costToBuild) {
				AddCoin();
				if (coinsAdded == costToBuild) {
					active = true;
					paid = true;

					Debug.Log ("Paid in full... remove cost indicators");

					RemoveHollowCoins ();
					RemoveCoins ();

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

					Debug.Log ("Paid in full... create Item");
					if (isFishingRodHouse){
						GameObject clone = Instantiate(fishingRod, new Vector3(transform.position.x + (1*numRods), transform.position.y, 0), Quaternion.identity) as GameObject;
						clone.GetComponent<Item> ().buildingScript = buildingScript;
						fishingRodList.Add(clone);
						numRods += 1;
					}

					RemoveHollowCoins ();
					RemoveCoins ();
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
		if (hollowCoinList.Count <= 0 || hollowCoinList == null) {
			for (int i = 0; i < costOfItem; i++) {
				GameObject clone = Instantiate (hollowCoin, new Vector3 (transform.position.x, buildingBounds.max.y + 1 + (1 * i), 0), Quaternion.identity) as GameObject;
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

	void RemoveCoins(){
		for (int i = 0; i < coinList.Count; i++) {
			Destroy(coinList[i]);
			if (i == (coinList.Count - 1)) {
				coinList.Clear ();
			}
		}
		coinsAdded = 0;
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

	void OnTriggerEnter (Collider col)
	{
		GameObject go = col.gameObject;
		if (go.tag == "Player") {
			Debug.Log ("Show the cost to build object....");
			if (active) {
				ShowCostOfItem ();
			}
		}
	}
	void OnTriggerExit (Collider col)
	{
		GameObject go = col.gameObject;
		if (go.tag == "Player") {
			Debug.Log ("Hide the cost to build object....");
			if (!active) {
				if (!paid && coinsAdded > 0) {
					RemoveCoins();
				}
			} else {
				HideCostOfItem ();
			}
		}
	}

}
