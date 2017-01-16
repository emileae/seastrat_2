using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	public bool active = false;
	public int costToBuild = 3;
	public int costOfProduct = 2;

	public GameObject hollowCoin;
	public GameObject coin;
	public int coinsAdded = 0;

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
		if (coinsAdded < costToBuild) {
			GameObject clone = Instantiate (coin, new Vector3 (transform.position.x, buildingBounds.max.y + 1 + (1 * coinsAdded), -0.1f), Quaternion.identity) as GameObject;
			coinList.Add (clone);
			coinsAdded += 1;
			if (coinsAdded == costToBuild) {
				Debug.Log ("Paid in full... remove cost indicators");
				for (int i = 0; i < hollowCoinList.Count; i++) {
					Destroy(hollowCoinList[i]);
				}
				for (int i = 0; i < coinList.Count; i++) {
					Destroy(coinList[i]);
				}
			}
		}
	}

}
