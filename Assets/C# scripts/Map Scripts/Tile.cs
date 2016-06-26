using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	
	public Vector3 pos;
	
	private Renderer renderer;
	private Color color;
	
	private int value;
	
	private float fadeValue = 1;
	
	public bool visible = false;
	private float fadeSpeed = 0.25f;
	
	private int health;
	
	private int type;

	private bool hudEnabled = false;
	public bool found = false;

	private Transform Child;

	public Material[] Textures;


	
	void Awake(){
		renderer = GetComponent<Renderer> ();
		//cache starting color for switching on and off
		color = renderer.material.color;
		renderer.material.color = Color.black;
		//renderer.material = Textures[Random.Range(0,5)];

		
		
	}
	
	// Use this for initialization
	void Start () {

		visible = false;
		//Debug.Log ("type = " + type);
		renderer.material.color = Color.black;


	}
	
	// Update is called once per frame
	void Update () {
		if (visible) {
			renderer.material.color = new Color (color.r*fadeValue, color.g*fadeValue, color.b*fadeValue);
			fadeValue -= fadeSpeed * Time.deltaTime;
			if(fadeValue <= 0)
			{
				fadeValue = 0;
				visible = false;
			}
		}
		if(type > 4 && type < 21)
		{
			if(!hudEnabled){
				Child.gameObject.SetActive(visible);
			}
			else 
			{
				Child.gameObject.SetActive(true);
			}
		}
	}
	
	/*
	void OnGUI(){
		Vector3 position = Camera.main.WorldToScreenPoint (transform.position);
		GUI.TextField (new Rect (position.x, Screen.height-position.y, 50, 50), health.ToString());
	}
	*/
	
	public void Hide(){
		
		renderer.material.color = Color.black;
	}

	public void setHUD(){
		hudEnabled = true;
		renderer.material.color = new Color (color.r*0.2f, color.g*0.2f, color.b*0.2f);
	}

	public void foundTile(){
		renderer.material.color = new Color (color.r*1.0f, color.g*1.0f, color.b*1.0f);
		found = true;
	}
	
	public void setVisible(){
		visible = false;
		renderer.material.color = color;
	}
	
	public void Show(){
		visible = true;
		fadeValue = 1;
		renderer.material.color = color;
	}
	
	public void setPoints(int points){
		value = points;
	}
	
	public int getPoints(){
		return value;
	}
	
	public void setID(int _type){
		type = _type;

		if(type > 4 && type < 21)
		{
			Child = this.gameObject.transform.GetChild(0);
		}
		if(type == 1 || type == 2 || type == 3 || type == 4)
		{
			renderer.material = Textures[Random.Range(0,5)];
		}

	}
	
	public int getID(){
		return type;
	}
	
	public void decreaseHealth(int amount){
		health -= amount;
	}
	
	public void setHealth(int _health){
		health = _health;
	}
	
	public int getHealth(){
		return health;
	}
	
	
}
