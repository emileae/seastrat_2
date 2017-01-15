using UnityEngine;
using System.Collections;

public class Blackboard : MonoBehaviour {

	public GameObject platform1;
	public GameObject platform2;
	public GameObject platform3;

	public float platform1TopPos;
	public float platform2TopPos;
	public float platform3TopPos;

	// Use this for initialization
	void Start () {
		platform1TopPos = platform1.GetComponent<MeshRenderer> ().bounds.max.y;
		platform2TopPos = platform2.GetComponent<MeshRenderer> ().bounds.max.y;
		platform3TopPos = platform3.GetComponent<MeshRenderer> ().bounds.max.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
