using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	public GameObject pointer;
	private GameObject _pointer;

	private Color color;
	private Color fadeColor;

	private PlayerController player;

	private Renderer renderer;

	private float radius = 0;
	private float controllerRadius;

	public Texture controllerTexture;
	public Texture pointerTextureBlue;
	public Texture pointerTextureRed;
	private Texture pointerTextureToUse;

	private float pointerRadius = 100;
	private float controllerRadius2 = 600;

	private GameObject playerChild;

	private int speed = 200;




	void Awake(){

		controllerRadius = gameObject.transform.localScale.x / 2;

		radius = controllerRadius;

		pointerTextureToUse = pointerTextureBlue;
	}

	// Use this for initialization
	void Start () {

		_pointer = Instantiate (pointer, transform.position, Quaternion.identity) as GameObject;
		_pointer.GetComponent<Renderer> ().enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (player != null) {

		}
		if (_pointer != null) {

			Vector3 pointerScreen = Camera.main.WorldToScreenPoint(transform.position);
			Vector3 mousePos = Input.mousePosition;
			Vector2 differenceVec = new Vector2((mousePos.x - pointerScreen.x), (mousePos.y - pointerScreen.y));

			if(Mathf.Abs(differenceVec.normalized.x)>Mathf.Abs(differenceVec.normalized.y)){
				player.setDirection(new Vector2(Mathf.Sign(differenceVec.x),0));
			}
			else{
				player.setDirection(new Vector2(0,Mathf.Sign(differenceVec.y)));
			}

			/*
			if(mousePos.x > pointerScreen.x)
			{
				playerChild.transform.eulerAngles = new Vector3(0, 90, 180);
			}
			else
			{
				playerChild.transform.eulerAngles = new Vector3(0, -90, 180);
			}
			*/

			//Change position of the ball based on where you click inside the controller panel

			radius = differenceVec.magnitude/90;
			if(radius>controllerRadius-0.8f)radius = controllerRadius-0.8f;


			float cos = (mousePos.x - pointerScreen.x)/differenceVec.magnitude;
			float sin = (mousePos.y - pointerScreen.y)/differenceVec.magnitude;

			float xPos = cos*radius;
			float yPos = sin*radius;

			Vector3 pointerPos = new Vector3(transform.position.x + xPos, transform.position.y + yPos, transform.position.z);
			_pointer.transform.position = pointerPos;

			if(Vector2.Distance(pointerScreen, Input.mousePosition)< 300)
			{
				if (Input.GetMouseButton (0)) {
					player.controlPlayer ();
				}
				else {
					player.stopPlayer();
				}

				pointerTextureToUse = pointerTextureBlue;

				if(pointerRadius < 100){
					pointerRadius += Time.deltaTime*speed;
				}
				else{
					pointerRadius = 100;
				}


			}
			else
			{
				player.stopPlayer ();

				pointerTextureToUse = pointerTextureRed;

				if(pointerRadius > 60){
					pointerRadius -= Time.deltaTime*speed;
				}
				else{
					pointerRadius = 60;
				}



			}
		}
	}

	void OnGUI(){
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		Vector3 pointerPos = Camera.main.WorldToScreenPoint (_pointer.transform.position);
		GUI.DrawTexture (new Rect (pos.x - controllerRadius2/2, Screen.height-pos.y - controllerRadius2/2	, controllerRadius2, controllerRadius2), controllerTexture, ScaleMode.StretchToFill, true, 10.0f);
		GUI.DrawTexture (new Rect (pointerPos.x - 45, Screen.height-pointerPos.y - 40, pointerRadius, pointerRadius), pointerTextureToUse, ScaleMode.StretchToFill, true, 10.0f);

	}

//	void OnMouseOver(){
//		renderer.material.color = new Color (Color.blue.r, Color.blue.g, Color.blue.b, 0.1f);
//		if (Input.GetMouseButton (0)) {
//			player.controlPlayer ();
//		}
//		else {
//			player.stopPlayer();
//		}
//
//	}
//	void OnMouseExit(){
//		player.stopPlayer ();
//		renderer.material.color = fadeColor;
//	}

	public void setPlayer(PlayerController _player){
		player = _player;
		playerChild = GameObject.Find("player_model");
	}

	public void DestroyThis(){
		Destroy (_pointer.gameObject);
		Destroy (this);
	}
}