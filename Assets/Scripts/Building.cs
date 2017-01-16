using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	public GameObject fishingRodHouse;
	public GameObject harpoonHouse;
	private float houseZdist = 8.0f;

	public bool active = false;
	public int costToBuild = 3;
	public int costOfItem = 2;

	public GameObject hollowCoin;
	public GameObject coin;
	public int coinsAdded = 0;
	private bool paid = false;

	private List<GameObject> hollowCoinList = new List<GameObject>();
	private List<GameObject> coinList = new List<GameObject>();

	// building bounds
	private BoxCollider collider;
	private Bounds buildingBounds;

	// Use this for initialization
	void Start ()
	{
		collider = gameObject.GetComponent<BoxCollider>();
		buildingBounds = collider.bounds;

		if (!active && costToBuild > 0) {
			for (int i = 0; i < costToBuild; i++) {
				GameObject clone = Instantiate(hollowCoin, new Vector3(transform.position.x, buildingBounds.max.y + 1 + (1*i), 0), Quaternion.identity) as GameObject;
				Debug.Log("clone: " + clone);
				hollowCoinList.Add(clone);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}



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

					// add new houses
					AddFishingRodHouse();
					AddHarpoonHouse();
				}
			}
		} else if (active) {
			Debug.Log("Add a coin? " + coinsAdded);
			Debug.Log("cost of item: " + costOfItem);
			if (coinsAdded < costOfItem) {
				Debug.Log("Add a coin??");
				AddCoin();
				Debug.Log("Add a coin?????");
				if (coinsAdded == costOfItem) {
					paid = true;

					Debug.Log ("Paid in full... create Item");

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

	void RemoveHollowCoins(){
		for (int i = 0; i < hollowCoinList.Count; i++) {
			Destroy (hollowCoinList [i]);
			if (i == (hollowCoinList.Count - 1)) {
				hollowCoinList.Clear ();
			}
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

	void AddFishingRodHouse(){
		GameObject clone = Instantiate(fishingRodHouse, new Vector3(transform.position.x - buildingBounds.max.x,transform.position.y, houseZdist), Quaternion.identity) as GameObject;
	}
	void AddHarpoonHouse(){
		GameObject clone = Instantiate(harpoonHouse, new Vector3(transform.position.x + buildingBounds.max.x,transform.position.y, houseZdist), Quaternion.identity) as GameObject;
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
