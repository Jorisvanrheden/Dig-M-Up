using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PlayerPhysics : MonoBehaviour 
{
	public LayerMask collisionMask;

	private BoxCollider collider;
	private Vector3 s;
	private Vector3 c;

	private float skin = 0.005f;

	[HideInInspector]
	public bool grounded;
	[HideInInspector]
	public bool movementStoped;

	Ray ray;
	RaycastHit hit;


	void Start()
	{
		collider = GetComponent<BoxCollider>();
		s = collider.size;
		c = collider.center;

	}

	public void Move(Vector2 moveAmount)
	{
		float deltaY = moveAmount.y;
		float deltaX = moveAmount.x;
		Vector2 p = transform.position;

		grounded = false;

		for(int i = 0; i < 3; i++)
		{
			float dir = Mathf.Sign (deltaY);
			float x = (p.x + c.x - s.x/2) + s.x/2 * i;
			float y = p.y + c.y + s.y/2 * dir;

			ray = new Ray(new Vector2(x,y), new Vector2(0,dir));

			Debug.DrawRay(ray.origin, ray.direction);

			if(Physics.Raycast(ray, out hit, Mathf.Abs(deltaY) + skin, collisionMask))
			{

				if(hit.transform.gameObject.layer != 2){
					float dst = Vector3.Distance (ray.origin, hit.point);
					
					if(dst > skin)
					{
						deltaY = dst * dir - skin * dir;
					}
					else
					{
						deltaY = 0;
					}
					grounded = true;
					break;
				}
			}
		}

		movementStoped = false;
		for(int i = 0; i < 3; i++)
		{
			float dir = Mathf.Sign (deltaX);
			float x = p.x + c.x + s.x/2 * dir;
			float y = p.y + c.y - s.y/2  + s.y/2 * i;
			
			ray = new Ray(new Vector2(x,y), new Vector2(dir,0));
			
			Debug.DrawRay(ray.origin, ray.direction);
			
			if(Physics.Raycast(ray, out hit, Mathf.Abs(deltaX) + skin, collisionMask))
			{

				if(hit.transform.gameObject.layer != 2){
					float dst = Vector3.Distance (ray.origin, hit.point);
					
					if(dst > skin)
					{
						deltaX = dst * dir - skin * dir;
					}
					else
					{
						deltaX = 0;
					}
					movementStoped = true;
					break;
				}

			}
		}


//		if(!grounded && !movementStoped)
//		{
//			Vector3 playerDir = new Vector3(deltaX, deltaY);
//			Vector3 o = new Vector3(p.x + c.x + s.x/2 * Mathf.Sign (deltaX), p.y + c.y + s.y/2 * Mathf.Sign(deltaY));
//			Debug.DrawRay(o, playerDir.normalized);
//
//			ray = new Ray(o, playerDir.normalized);
//
//			if(Physics.Raycast(ray, Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY),collisionMask))
//			{
//				grounded = true;
//				deltaY = 0;
//			}
//		}


		Vector2 finalTransform = new Vector2(deltaX, deltaY);
		transform.Translate(finalTransform);

	}
}
