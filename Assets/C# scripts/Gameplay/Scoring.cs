using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scoring : MonoBehaviour {

	public GameObject coin;
	private List<GameObject> coins = new List<GameObject>();

	private float totalScore;
	private float maxScore;
	private int scoreValue;
	private float coinSpawnTimer;

	private float maxWith = 400;

	private bool submitted = false;

	private HUD hud;

	private GameObject gConverter;
	public Texture2D converter;
	public GUIStyle converterStyle;

	// Use this for initialization
	void Start () {
		gConverter = GameObject.Find ("Cam/machine");
		gConverter.GetComponent<Renderer>().enabled = true;
		hud = GameObject.Find("HUD(Clone)").GetComponent<HUD>();
	}
	
	// Update is called once per frame
	void Update () {

		if (totalScore > 0) {
			totalScore -= Time.deltaTime * 5000;
			if (totalScore <= coinSpawnTimer - 500) {
				Vector3 pos = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2 + 200, 8));
				GameObject newCoin = Instantiate (coin, pos, Quaternion.Euler (-90, 0, 0)) as GameObject;

				int range = 50;
				float random = -1 * (range / 2) + Random.value * range;
				float random2 = -1 * (range / 2) + Random.value * range;
				newCoin.GetComponent<Rigidbody> ().AddRelativeForce (new Vector3 (random, 0, random2));

				coins.Add (newCoin);
				coinSpawnTimer = totalScore;
			}
			scoreValue = (int)totalScore;
		} 
		else {
			if(!submitted){
				Invoke("submit", 4.0f);
				submitted = true;
			}
		}
		if(totalScore<0)totalScore=0;
		hud.totalPoints = (int)totalScore;
	}

	private void submit(){
		GameObject.Find("Game").GetComponent<GameManager>().submitScore(coins.Count);
	}

	void OnGUI(){

		//GUI.DrawTexture(new Rect(Screen.width / 2 - 300, Screen.height / 2 + 50, 600,500), converter);
		GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 + 145, 600,500), coins.Count.ToString(), converterStyle);

	}

	public void setScore(int score, bool end){
		maxScore = score;
		totalScore = score;
		coinSpawnTimer = totalScore;
		if (end) {
			Physics.gravity = new Vector3(0,9.8f,0);
		}
		else Physics.gravity = new Vector3(0,-9.8f,0);
	}
}
