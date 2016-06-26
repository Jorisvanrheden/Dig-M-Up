using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubQuest : MonoBehaviour {

	private float xOffset = 0;
	private int width;
	private int buttonWidth = 475;
	private int buttonHeight;
	private int maxLayers;
	private int mapHeight;

	private int speed = 120;

	private Transform playerTransform;
	private int layer;

	private bool show = true;

	private int iterator = 0;
	public Texture2D[] images = new Texture2D[16];

	public List<int> collection1 = new List<int>();
	public List<int> collection2 = new List<int>();
	public List<int> collection3 = new List<int>();
	public List<int> collection4 = new List<int>();

	private List<int> currentCollection;	
	
	private MapGenerator mapGenerator;

	private float collectionDepth = -1.0f;

	public GameObject particles;
	private List<GameObject> sparkles;

	void Awake(){
		mapGenerator = GameObject.Find ("MapGenerator").GetComponent<MapGenerator> ();
	}

	// Use this for initialization
	void Start () {
		width = Screen.width;
		buttonHeight = buttonWidth / 2;



	}
	
	// Update is called once per frame
	void Update () {

		//move the panel to the left as an introduction
		if (show) {
			if (width + xOffset > width - buttonWidth) {
				xOffset -= speed * Time.deltaTime;
			}
			//check if you are close to the next layer
			if(mapHeight - playerTransform.position.y > (mapHeight/maxLayers)*layer - 1){
				collectionFailed();
			}
		} 
		//let it slide away to the right
		else if (!show) {
			xOffset += speed * Time.deltaTime;
			if (width + xOffset > width) {

				Destroy(this.gameObject);
			}
		}

		Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector2(0,0));
	}

	void OnGUI(){
		GUI.DrawTexture (new Rect (width + xOffset, 0, images[0].width/1.2f, images[0].height/1.5f), images [iterator]);
	}

	public void setInfo(Transform trans, int _layer, int _maxLayers, int _mapHeight){
		playerTransform = trans;
		layer = _layer;
		maxLayers = _maxLayers;
		mapHeight = _mapHeight;

		sparkles = new List<GameObject> ();

		if (layer == 1) {
			currentCollection = collection1;
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ipad")) {
				sparkles.Add(Instantiate(particles, go.transform.position, Quaternion.identity) as GameObject);
			}
		} 
		else if (layer == 2) {
			currentCollection = collection2;
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("Cannon")) {
				sparkles.Add(Instantiate(particles, go.transform.position, Quaternion.identity) as GameObject);
			}
		}
		else if (layer == 3) {
			currentCollection = collection3;
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("Sword")) {
				sparkles.Add(Instantiate(particles, go.transform.position, Quaternion.identity) as GameObject);
			}
		}
		else if (layer == 4) {
			currentCollection = collection4;
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("Skull")) {
				sparkles.Add(Instantiate(particles, go.transform.position, Quaternion.identity) as GameObject);
			}
		}

		if (layer == 1) {
			iterator = 0;	
		} else {
			iterator = (layer-1)*4;
		}
	}

	private void collectionComplete(){
		print ("YOU GOTTHEM FUL POINTS BRAHHH NICE JOB");
		GameObject.Find ("Game").GetComponent<GameManager> ().collectionComplete ();
		show = false;

		foreach (GameObject sparkle in sparkles) {
			Destroy(sparkle);
		}
	}

	private void collectionFailed(){
		print ("GOTTA TRY HARDER NEXT TIME MAN");
		show = false;

		foreach (GameObject sparkle in sparkles) {
			Destroy(sparkle);
		}
	}

	public void AddItem(Tile tile){
		for (int i=0; i<currentCollection.Count; i++) {
			if(tile.getID() == currentCollection[i]){

				foreach (GameObject sparkle in sparkles) {
					if(sparkle.transform.position == tile.transform.position){
						Destroy(sparkle);
						sparkles.Remove(sparkle);
						break;
					}
				}

				iterator ++;
				currentCollection.RemoveAt(i);
				
				if(currentCollection.Count == 0){
					collectionComplete();
				}
				break;
			}
		}
	}
}
