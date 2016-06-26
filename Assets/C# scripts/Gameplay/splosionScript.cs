using UnityEngine;
using System.Collections;

public class splosionScript : MonoBehaviour 
{
	private float Timer;
	// Use this for initialization
	void Start () 
	{
		Timer = 2.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Timer -= Time.deltaTime;
		if(Timer <= 0.0f)
		{
			Destroy(this.gameObject);
		}
	}
}
