using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	public Player playerScript;

	public bool activeSignalFire = false;
	public GameObject dock;

	public List<GameObject> platforms = new List<GameObject>();
	public List<Platform> platformScripts = new List<Platform>();
	public List<float> platformTopPos = new List<float>();
	public List<Bounds> platformBounds = new List<Bounds>();
	public List<GameObject> platformMainHouses = new List<GameObject>();

	// Buildings
	// 		Ladder
	public float ladderCoinYPos = 6.0f;


	// fisherman
	public int fishermanCapacity = 5;

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < platforms.Count; i++) {
			platformTopPos.Add(platforms[i].GetComponent<MeshRenderer> ().bounds.max.y);
		}
		for (int i = 0; i < platforms.Count; i++) {
			platformScripts.Add(platforms[i].GetComponent<Platform> ());
		}
		for (int i = 0; i < platforms.Count; i++) {
			platformMainHouses.Add(platformScripts[i].mainHouse);
		}
		for (int i = 0; i < platforms.Count; i++) {
			platformBounds.Add(platformScripts[i].GetComponent<BoxCollider>().bounds);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
