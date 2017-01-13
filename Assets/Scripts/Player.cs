using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {

	public float walkSpeed = 2.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float inputH = Input.GetAxisRaw("Horizontal");
		float inputV = Input.GetAxisRaw("Vertical");

		Debug.Log("inputH: " + inputH);
		Debug.Log("inputV: " + inputV);

		Vector3 velocityDirection = new Vector3(inputH, inputV, 0);

		transform.Translate(velocityDirection * walkSpeed * Time.deltaTime);

	}
}
