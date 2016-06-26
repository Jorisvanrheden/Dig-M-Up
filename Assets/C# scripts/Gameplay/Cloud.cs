using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.right*1*Time.deltaTime);

		if (transform.position.x > 40) {
			transform.position = new Vector3(-30, transform.position.y, transform.position.z);
		}
	}
}
