using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	public Player playerScript;

	public bool activeSignalFire = false;

	public List<GameObject> platforms = new List<GameObject>();
	public List<Platform> platformScripts = new List<Platform>();
	public List<float> platformTopPos = new List<float>();
	public List<GameObject> platformMainHouses = new List<GameObject>();

//	public GameObject platform1;
//	public GameObject platformMainHouse1;
//	public GameObject platform2;
//	public GameObject platformMainHouse2;
//	public GameObject platform3;
//	public GameObject platformMainHouse3;
//
//	public float platform1TopPos;
//	public float platform2TopPos;
//	public float platform3TopPos;

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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
