using UnityEngine;
using System.Collections;

public class BreakCubeScript : MonoBehaviour 
{


	Color start;
	Color end;
	float t = 0.0f;
	public Renderer rend;

	void Start () 
	{
		rend = GetComponent<Renderer>();
		start = rend.material.color;
		end = new Color(start.r, start.g, start.b, 0.0f);
	}
	
	void Update () 
	{

		SelfDestruct();

	}

	private void SelfDestruct()
	{
		t += Time.deltaTime * 1.0f;
		rend.material.color = Color.Lerp(start, end, t);
		if(rend.material.color.a <= 0.0)
		{
			Destroy(gameObject);
		}
	}
}
