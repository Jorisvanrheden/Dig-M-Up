using UnityEngine;
using System.Collections;

public class ObjectsFloat : MonoBehaviour 
{
	private Vector3 startpos;
	float lerpTime = 2f;
	float currentLerpTime;

	void Start () 
	{
		startpos = transform.position;
	}
	
	void Update () 
	{

		currentLerpTime += Time.deltaTime;
		if (currentLerpTime > lerpTime) 
		{
			currentLerpTime = lerpTime;
		}
		transform.Rotate(Vector3.up, Time.deltaTime * 100);
		float perc = currentLerpTime / lerpTime;
		Vector3 temp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width*0.01f, Screen.height - 25,5));
		transform.position = Vector3.Lerp(startpos, temp, perc);
		if(perc >= 1)
		{
			Destroy(transform.gameObject);
		}
	}
}
