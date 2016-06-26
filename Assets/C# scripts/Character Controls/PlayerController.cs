using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : MonoBehaviour 
{
	public float gravity = 20;
	public float speed = 8;
	public float acceleration = 30;
	public float jumpHeight = 10;

	private int drillPower = 60;
	private float amountFuel = 100;
	private float totalFuel = 100;

	private float currentSpeed;
	private float targetSpeed;
	private Vector2 amountToMove;

	private Vector2 direction = new Vector2 (0,-1);

	private PlayerPhysics playerPhysics;
	//public GameObject leftDrill;
	public GameObject rightDrill;

	private GameManager gameManager;

	public Color color1 = Color.blue;
	public Color color2 = Color.gray;
	
	private bool bgChanged = false;
	public Camera cam;

	private float fuelTimer = 60;
	private bool flipGravity = false;
	private Vector3 flippedPos;
	private Vector3 currentPos;
	private float xDif;
	private float xIter = 0;

	private GameObject playerChild;
	private Vector3 childPos;
	
	void Start () 
	{

		playerChild = GameObject.Find("player_model");
		childPos = playerChild.transform.localPosition;
		//playerChild.transform.eulerAngles = new Vector3(0, 90, 180);

		playerPhysics = GetComponent<PlayerPhysics>();
		gameManager = GameObject.Find ("Game").GetComponent<GameManager> ();
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();

	}
	
	void Update () 
	{
		if(playerPhysics.movementStoped)	
		{
			targetSpeed = 0;
			currentSpeed = 0;
		}
		
		if (targetSpeed != 0) {
			useFuel(1);
		}

		if(playerPhysics.grounded)
		{
			amountToMove.y = 0;
			if(Input.GetButtonDown("Jump"))
			{
				amountToMove.y = jumpHeight;
			}
		}

		rightDrill.transform.Rotate (Vector3.back * -800*Time.deltaTime);
		//leftDrill.transform.Rotate (Vector3.back * 500*Time.deltaTime);
		
		amountToMove.x = currentSpeed;
		amountToMove.y -= gravity * Time.deltaTime;
		
		//cap the falling speed for unrealistic falling speeds
		if(amountToMove.y < -10)amountToMove.y = -10;

		if (flipGravity) {

			int jumpHeight = 4;

			if(transform.eulerAngles.z > 180 || transform.eulerAngles.z == 0){
				transform.Rotate(Vector3.back*100*Time.deltaTime);
				
			}

			//get xvalue between negative number and positive number
			float xPos = xIter + (xDif/2 - xDif);
			float yPos = -Mathf.Pow(xPos,2);

			float tempY = -yPos - Mathf.Pow(xDif/2 - xDif,2);

			transform.position = new Vector3(currentPos.x + -xIter, currentPos.y + tempY*jumpHeight, transform.position.z);

			xIter += 1.0f*Time.deltaTime;

			if(xIter > xDif/2){
				if(tempY*jumpHeight > -0.3f*jumpHeight){
					flipGravity = false;
				}
			}

			if(xIter >= xDif){
				flipGravity = false;
			}

			//changing background back to 'above ground' level
			cam.backgroundColor = color1;
			bgChanged = true;
	
			amountToMove = new Vector2(0,0);
		}

		playerPhysics.Move(amountToMove * Time.deltaTime);

		gameManager.setDirection (direction);

		if(bgChanged == false)
		{
			if(transform.position.y < 99)
			{
				ChangeBGColor();
				//bgChanged = true;
			}
		}
		if(gameManager.getTimer() > 5.0f)
		{
			ResetValeus();
		}

	
	}
	private void ChangeBGColor()
	{
		
		if((transform.position.y - 88) / 10 <= 1.0f)
		{
			//cam.backgroundColor = Color.Lerp(color2, color1, (transform.position.y - 88) / 10);
		}
	}
	
	
	public void controlPlayer(){
		Vector3 screenPos = Camera.main.WorldToScreenPoint (transform.position);
		if (amountFuel > 0) {
			//digging below
			if (Input.mousePosition.x < screenPos.x + 20 && Input.mousePosition.x > screenPos.x - 20 &&
			    Input.mousePosition.y < screenPos.y - 20 && Input.mousePosition.y > screenPos.y - 500){

				
				Vector2 tempPos = new Vector2((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)-1);
				gameManager.drillTile(tempPos, drillPower);
				
				useFuel(3);
				
				direction = new Vector2(0,-1);
				//turn player down
				//playerChild.transform.eulerAngles = new Vector3(90, 180, -180);
				//playerChild.transform.localPosition = new Vector3(playerChild.transform.localPosition.x, 0.2f, playerChild.transform.localPosition.z);
			}
			//digging to the side
			if (Input.mousePosition.x > screenPos.x + 20) {
				targetSpeed = 1 * speed;
				
				currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);
				
				Vector2 tempPos = new Vector2((int)Mathf.Round(transform.position.x)+1, (int)Mathf.Round(transform.position.y));
				gameManager.drillTile(tempPos, drillPower);
				
				useFuel(3);
				
				direction = new Vector2(1,0);
				//turn player to right
				playerChild.transform.eulerAngles = new Vector3(0, 90, 180);
				playerChild.transform.localPosition = childPos;
			} 
			else if (Input.mousePosition.x < screenPos.x - 20) {
				targetSpeed = -1 * speed;
				currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);
				
				Vector2 tempPos = new Vector2((int)Mathf.Round(transform.position.x)-1, (int)Mathf.Round(transform.position.y));
				gameManager.drillTile(tempPos, drillPower);
				
				useFuel(3);
				
				direction = new Vector2(-1,0);
				//turn player to left
				playerChild.transform.eulerAngles = new Vector3(0, -90, 180);
				playerChild.transform.localPosition = childPos;
			}	
			else{
				targetSpeed = 0;
				currentSpeed = targetSpeed;
			}
		}

	}

	public void stopPlayer(){
		currentSpeed = 0;
		targetSpeed = 0;
	}

	private void useFuel(float amount){
		amountFuel -= amount*Time.deltaTime;
		if(amountFuel<0) amountFuel = 0;
	}

	public void refuel(float amount){
		amountFuel += amount;
		if(amountFuel>totalFuel) amountFuel = totalFuel;
	}

	public void setDirection(Vector2 dir){
		direction = dir;
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

	public void IncreaseDrilPower()
	{
		if(gameManager.TimerCoolDown >= 30.0f)
		{
			gameManager.SetTimer();
			drillPower = 60;
		}
	}

	public void negativeGravity(){
		gravity = 0;
		flipGravity = true;
		currentPos = transform.position;
		gameManager.removeController ();
		flippedPos = new Vector3 (transform.position.x+2, transform.position.y, transform.position.z);

		xDif = Mathf.Abs(transform.position.x - flippedPos.x);

		playerChild.transform.eulerAngles = new Vector3(90, 180, -180);
	}

	public float getFuel(){
		return amountFuel;
	}

	public void setTargetSpeed(float _speed){
		targetSpeed = _speed;
	}
	
	public void ResetValeus()
	{
		drillPower = 35;
	}
}
