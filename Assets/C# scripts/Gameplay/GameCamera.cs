using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour 
{
	private Transform target;

	Vector2 position;

	private float cameraSpeed;
	private float cruiseSpeed = 0;
	private float catchUpSpeed = 0;

	private float yOffset = 0;
	private bool smoothCatchup = false;	
	
	public void SetTarget(Transform t)
	{
		cameraSpeed = cruiseSpeed;

		target = t;

		position.x = t.transform.position.x;
		position.y = t.transform.position.y;
	}

	void LateUpdate()
	{

		if(target)
		{
				yOffset += cameraSpeed*Time.deltaTime;

				if(target.position.y - transform.position.y < -3){
					smoothCatchup = true;
					
				}
				
				if(smoothCatchup){
					//float x = IncrementTowards(transform.position.x, target.position.x, trackSpeed);
					
					//float y = IncrementTowards(transform.position.y, target.position.y + 1, trackSpeed);
					cameraSpeed = catchUpSpeed;
					if(Mathf.Abs(target.position.y - transform.position.y) < 1){
						smoothCatchup = false;
						cameraSpeed = cruiseSpeed;
					}
				}
				transform.position = new Vector3(position.x, position.y - yOffset, transform.position.z);

		}
	}

	public void disable(){
		cameraSpeed = 0;
		cruiseSpeed = 0;
	}

	public void enable(){
		cruiseSpeed = 0.35f;
		catchUpSpeed = 6.0f;
		cameraSpeed = cruiseSpeed;

	}
	
	private float IncrementTowards(float n, float target, float a)
	{
		if (n == target)
		{
			return n;
		}
		else
		{
			float dir = Mathf.Sign (target - n);
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign (target - n))? n: target;
		}
	}
}
