using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public bool active = false;
	public int costToBuild = 3;
	public int costOfProduct = 2;

	public Transform hollowCoin;

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
				Instantiate(hollowCoin, new Vector3(transform.position.x, buildingBounds.max.y + 1 + (1*i), 0), Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
