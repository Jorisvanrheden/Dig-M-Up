using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	private CameraSounds sounds;

	public GameObject player;
	private GameObject _player;
	private PlayerController PlayerContl;

	public Controller controller;
	private Controller _controller;

	private float depth = 0;
	private float maxDepth;

	public GameObject destructedTile;
	public GameObject destructedTileByBomb;
	public List<GameObject> objSpawn = new List<GameObject>();

	public GameCamera cam;
	public GameObject cameraObj;

	public HUD hud;
	private HUD _hud;
	private MapGenerator mapGenerator;
	
	private List<Tile> Tiles;
	private int[,] popMap;
	private Tile[,] tileArray;

	private int totalPoints = 0;
	private int scoreMultiplier = 1;
	private float multiplierTimer = 0;

	public TileScore tileScore;

	private Vector2 direction = new Vector2(0,-1);

	private bool endStarted = false;

	public float Timer = 0.0f;
	public float TimerCoolDown = 30.0f;

	public Scoring endScore;

	public SubQuest subQuest;
	private SubQuest currentSubquest;
	private float currentLayer;
	private int maxLayers = 4;
	private float lastLayer;

	public GameObject Splosion;

	public GUIStyle bombStyle;
	public GUIStyle bombUpgradeStyle;
	public GUIStyle bonusStyle;
	public GUIStyle fuelStyle;

	private int bombRange = 2;
	private int maxBombRange = 4;

	private int bombCost = 1000;
	private int bombUpgradeCost = 2000;
	private int bonusCost = 1000;
	private int fuelCost = 500;

	private bool endAnimation = false;
	private float fuelTimer = 60;

	public Texture2D FuelNormal;
	public Texture2D FuelRed;
	public Texture2D FuelDisable;
	public Texture2D bombNormal;
	public Texture2D bombDisable;
	public Texture2D bombUpgradeNormal;
	public Texture2D bombUpgradeDisable;
	public Texture2D bonusNormal;
	public Texture2D bonusDisable;
	public Texture2D buttonHolder;
	private float scalerY = 4.82f;
	private float scalerX = 3.04f;
	private int offsetx = 47;
	private int offset = 163;

	private int buttonWidth = 120;
	private int buttonHeight = 120;


	private Vector3 originPosition;
	private Quaternion originRotation;
	public float shake_decay;
	public float shake_intensity;

	private TutorialController tutContrl;

	private bool endBeforeFinish = false;

	void Awake(){
		sounds = Camera.main.GetComponent<CameraSounds> ();
	}

	void Start () 
	{
		Screen.SetResolution (1920, 1080, true);
		mapGenerator = GameObject.Find ("MapGenerator").GetComponent<MapGenerator> ();
		tutContrl = GetComponent<TutorialController>();
		maxDepth = mapGenerator.mapHeight;

		loadMap ();

		_player = Instantiate (player, new Vector3 (mapGenerator.mapWidth/2, mapGenerator.mapHeight + 2, 0), Quaternion.identity) as GameObject;

		_controller = Instantiate (controller, new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z-0.5f), Quaternion.Euler(90,0,0)) as Controller;
		_controller.transform.parent = _player.transform;
		_controller.setPlayer (_player.GetComponent<PlayerController>());

		cam = cameraObj.GetComponent<GameCamera>();
		cam.SetTarget(_player.transform);

		_hud = Instantiate (hud, transform.position, Quaternion.identity) as HUD;
		_hud.setInfo (_player.GetComponent<PlayerController>(), maxDepth);


		lastLayer = Mathf.Ceil ((mapGenerator.mapHeight - _player.transform.position.y) / (mapGenerator.mapHeight / maxLayers));
		PlayerContl = _player.GetComponent<PlayerController>();

		fuelStyle.normal.background = FuelNormal;

		buttonStateUpdate ();

	}

	void Update(){
		inRange ();
	
		if(Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(0);
		}

		if(PlayerContl.getFuel() < 10.0f)
		{
			tutContrl.AddFuel();
		}

		if (Camera.main.transform.position.y - _player.transform.position.y < -6.5f && Camera.main.transform.position.y - _player.transform.position.y > -10) {
			if (!endBeforeFinish) {
			
				Scoring _endScore = Instantiate(endScore, transform.position, Quaternion.identity) as Scoring;
				_endScore.setScore(totalPoints, false);
				cam.disable();
				_hud.count = false;
				endBeforeFinish = true;
			}
			
		}

		Timer += Time.deltaTime;
		TimerCoolDown += Time.deltaTime;

		if (Mathf.Ceil ((mapGenerator.mapHeight - _player.transform.position.y) / (mapGenerator.mapHeight / maxLayers)) != lastLayer) {
			{
				if(lastLayer < 4){

					SubQuest _subQuest = Instantiate (subQuest, transform.position, Quaternion.identity) as SubQuest;
					currentLayer = Mathf.Ceil ((mapGenerator.mapHeight - _player.transform.position.y) / (mapGenerator.mapHeight / maxLayers));
					lastLayer = currentLayer;
					
					_subQuest.setInfo (_player.transform, (int)currentLayer, maxLayers, mapGenerator.mapHeight);
					
					currentSubquest = _subQuest;

					//start running camera
					if(lastLayer == 1){
						cam.enable();
					}

					if(lastLayer == 3)
					{
						tutContrl.BombUsedCheck();
					}
					if(lastLayer == 2)
					{
						tutContrl.MisiionUsedCheck();
					}

				}
				else{
					if(!endStarted){
						//flipping around 
						_player.GetComponent<PlayerController> ().negativeGravity ();

						//rotate the camera
						endAnimation = true;

						_hud.count = false;
						endStarted = true;
						//gameOver();

					}

				}
			}
			
		}

		if (endAnimation) {
			if(cam.transform.eulerAngles.z > 180 || Camera.main.transform.eulerAngles.z == 0){
				cam.transform.Rotate(Vector3.back*100*Time.deltaTime);
			}
			else{
				cam.GetComponent<GameCamera>().disable();

				TileScore bonusScore = Instantiate(tileScore, _player.transform.position, Quaternion.identity) as TileScore;
				bonusScore.setScore(20000);

				Scoring _endScore = Instantiate(endScore, transform.position, Quaternion.identity) as Scoring;
				_endScore.setScore(totalPoints, true);

				endAnimation = false;
			}
		}


		if (multiplierTimer < 0) {
			multiplierTimer = 0;
			scoreMultiplier = 1;

			buttonStateUpdate();
		} 
		else if (multiplierTimer > 0) {
			multiplierTimer -= Time.deltaTime;
		}




		if (shake_intensity > 0){
			Camera.main.transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, cam.transform.position.y, Camera.main.transform.position.z);
			Camera.main.transform.rotation = new Quaternion(
				originRotation.x + Random.Range (-shake_intensity,shake_intensity) * .2f,
				originRotation.y + Random.Range (-shake_intensity,shake_intensity) * .2f,
				originRotation.z + Random.Range (-shake_intensity,shake_intensity) * .2f,
				originRotation.w + Random.Range (-shake_intensity,shake_intensity) * .2f);
			shake_intensity -= shake_decay;
		}
		else if(!endAnimation)
		{
			Camera.main.transform.rotation = cam.transform.rotation;
		}
		//Debug.Log ("camPos : " + Camera.main.transform.position);
		//Debug.Log ("camObj : " + cam.transform.position);
	}


	void Shake(){
		originPosition = Camera.main.transform.position;
		originRotation = Camera.main.transform.rotation;
		shake_intensity = .1f;
		shake_decay = 0.004f;
	}

	private void buttonStateUpdate(){
		if (totalPoints < bombCost) {
			bombStyle.normal.background = bombDisable;
		} else {
			bombStyle.normal.background = bombNormal;
		}

		if (totalPoints < bombUpgradeCost || bombRange >= maxBombRange) {
			bombUpgradeStyle.normal.background = bombUpgradeDisable;
		} else {
			if(bombRange<maxBombRange){
				bombUpgradeStyle.normal.background = bombUpgradeNormal;
			}
		}

		if (totalPoints < bonusCost || multiplierTimer > 0) {
			bonusStyle.normal.background = bonusDisable;
		} else {
			if(multiplierTimer == 0){
				bonusStyle.normal.background = bonusNormal;
			}
		}

		if (totalPoints < fuelCost) {
			fuelStyle.normal.background = FuelDisable;
		} else {
			fuelStyle.normal.background = FuelNormal;
		}
	}

	void OnGUI(){

		if (!endStarted && !endBeforeFinish) {
			//bomb
			//if (TimerCoolDown >= 10.0f) {
			if (GUI.Button (new Rect (offsetx, offset+bombNormal.height/scalerY*0, bombNormal.width/scalerX, bombNormal.height/scalerY), " ", bombStyle)) {
				if(totalPoints>=bombCost){
					Vector2 tempPos = new Vector2((int)Mathf.Round(_player.transform.position.x), (int)Mathf.Round(_player.transform.position.y));
					if(tempPos.y > 6)
					{
						Bomb (tempPos);
						sounds.playSound(0);
					}
				}
			}
			//}
			//bomb upgrade
			if (GUI.Button (new Rect (offsetx, offset+bombNormal.height/scalerY*1, bombUpgradeNormal.width/scalerX, bombUpgradeNormal.height/scalerY), " ", bombUpgradeStyle)) {
				if(totalPoints>=bombUpgradeCost){
					if(bombRange<maxBombRange){
						bombRange++;
						
						IncreasePoints(-bombUpgradeCost, _player.transform.position);
					}
				}
			}
			
			//fuel
			if (GUI.Button (new Rect (offsetx, offset+bombNormal.height/scalerY*2, FuelNormal.width/scalerX, FuelNormal.height/scalerY), " ", fuelStyle)) {
				if(totalPoints>=fuelCost){
					_player.GetComponent<PlayerController>().refuel(100);
					fuelStyle.normal.background = FuelNormal;
					IncreasePoints(-fuelCost, _player.transform.position);
					
				}
			}
			//bonus points
			if (GUI.Button (new Rect (offsetx, offset+bombNormal.height/scalerY*3, bonusNormal.width/scalerX, bonusNormal.height/scalerY), " ", bonusStyle)) {
				if(totalPoints>=bonusCost){
					if(multiplierTimer == 0){
						scoreMultiplier = 2;
						multiplierTimer = 10;
						IncreasePoints(-bonusCost, _player.transform.position);
					}
				}
			}
			
			//fuel flickering
			if (PlayerContl.getFuel() < 20) {
				fuelTimer -= 20*Time.deltaTime;
				
				if(fuelTimer >= 30){
					fuelStyle.normal.background = FuelNormal;
				}
				else
				{
					fuelStyle.normal.background = FuelRed;
				}
				if(fuelTimer<=0){
					
					fuelTimer = 60;
				}
			}
			GUI.DrawTexture (new Rect (0, 123, buttonHolder.width, buttonHolder.height), buttonHolder);
		}

	}
	
	private void loadMap()
	{
		mapGenerator.GenerateMap ();

		Tiles = mapGenerator.getTiles ();
		popMap = mapGenerator.getPopMap ();
		tileArray = mapGenerator.getTileArray ();
	}

	private List<Vector2> highlightTiles(int x, int y, int range, List<Vector2> tilePositions){
		/*
		if (endList == null) {
			endList = new List<Tile> ();
		}
		*/

		if (tilePositions == null) {
			tilePositions = new List<Vector2> ();
		}


		//Implementing a cone of lighting
		tilePositions = mapGenerator.getCone (x, y, range, direction);

		//Works for continuous lighting in a 'cirle' around the player
		/*
		if (range > 0) {
		
			List<Vector2> newPositions = mapGenerator.testNeighbours(x,y);
			foreach(Vector2 pos in newPositions){
				tilePositions.Add(pos);
				highlightTiles((int)pos.x, (int)pos.y, range - 1, tilePositions);
			}

			return tilePositions;
		
		}
		*/

		/*More like Pathfinding 
		if(range>0){
			List<Tile> newLayer = mapGenerator.getNeighbours(x,y);

			foreach(Tile tile in newLayer){
				if(tile!=null){
					endList.Add(tile);
					highlightTiles((int)tile.transform.position.x, (int)tile.transform.position.y, range - 1, endList);
				}
			}

			return endList;

		}
		*/

		return tilePositions;
	}

	public void drillTile(Vector2 tilePos, int drillPower){
		if (inArray (tilePos)) {
			Tile targetTile = tileArray[(int)tilePos.x, (int)tilePos.y];
			if(targetTile!=null){
				targetTile.decreaseHealth(drillPower);
				if(targetTile.getHealth()<=0){
					deleteTile(targetTile, false);
				}
			}
		}
	}

	public void inRange(){

		int xPos = (int)Mathf.Round(_player.transform.position.x);
		int yPos = (int)Mathf.Round(_player.transform.position.y);

		foreach (Vector2 pos in highlightTiles (xPos, yPos, 5, null)) {
			if(inArray(pos)){
				tileArray[(int)pos.x, (int)pos.y].Show();
			}
		}
	}

	private bool inArray(Vector2 pos){
		if (pos.x <= mapGenerator.mapWidth - 1 && pos.x >= 0 &&
		    pos.y <= mapGenerator.mapHeight - 1 && pos.y >= 0) {
			if(tileArray[(int)pos.x, (int)pos.y]!=null){
				return true;
			}
			else return false;
		}
		else return false;
	}

	public void collectionComplete(){
		int score = 10000;
		IncreasePoints (score, _player.transform.position);
	}

	public void deleteTile(Tile tile, bool bomb){

		int tilePoints = tile.getPoints ();
		IncreasePoints (tilePoints, tile.transform.position);
		if(tile.getID() == 21)
		{
			tutContrl.AddLava();
		}
		if(tile.getID() == 17)
		{
			tutContrl.mission = true;
		}

		if (bomb) {
			Instantiate (destructedTileByBomb, tile.transform.position, tile.transform.rotation);
		} 
		else {
			Instantiate (destructedTile, tile.transform.position, tile.transform.rotation);
			sounds.playSound (2);
		}

		if (tile.getID () > 4) {

			if (currentSubquest != null) {
				currentSubquest.AddItem(tile);
			}

			switch (tile.getID())
			{
			case 5:
				Instantiate (objSpawn[0], tile.transform.position, tile.transform.rotation);
				break;
			case 6:
				Instantiate (objSpawn[1], tile.transform.position, tile.transform.rotation);
				break;
			case 7:
				Instantiate (objSpawn[2], tile.transform.position, tile.transform.rotation);
				break;
			case 8:
				Instantiate (objSpawn[3], tile.transform.position, tile.transform.rotation);
				break;
			case 9:
				Instantiate (objSpawn[4], tile.transform.position, tile.transform.rotation);
				break;
			case 10:
				Instantiate (objSpawn[5], tile.transform.position, tile.transform.rotation);
				break;
			case 11:
				Instantiate (objSpawn[6], tile.transform.position, tile.transform.rotation);
				break;
			case 12:
				Instantiate (objSpawn[7], tile.transform.position, tile.transform.rotation);
				break;
			case 13:
				Instantiate (objSpawn[8], tile.transform.position, tile.transform.rotation);
				break;
			case 14:
				Instantiate (objSpawn[9], tile.transform.position, tile.transform.rotation);
				break;
			case 15:
				Instantiate (objSpawn[10], tile.transform.position, tile.transform.rotation);
				break;
			case 16:
				Instantiate (objSpawn[11], tile.transform.position, tile.transform.rotation);
				break;
			case 17:
				Instantiate (objSpawn[12], tile.transform.position, tile.transform.rotation);
				break;
			case 18:
				Instantiate (objSpawn[13], tile.transform.position, tile.transform.rotation);
				break;
			case 19:
				Instantiate (objSpawn[14], tile.transform.position, tile.transform.rotation);
				break;
			case 20:
				Instantiate (objSpawn[15], tile.transform.position, tile.transform.rotation);
				break;
			default:
				Instantiate (objSpawn[0], tile.transform.position, tile.transform.rotation);
				break;

			}

		}
		Tiles.Remove (tile);
		Destroy (tile.gameObject);
	}

	public void setDirection(Vector2 _direction){
		direction = _direction;
	}

	public float getTimer()
	{
		return Timer;
	}
	
	public void SetTimer()
	{
		Timer = 0.0f;
		TimerCoolDown = 0.0f;
	}

	public void submitScore(int score){
		StartCoroutine(GetComponent<DBconnection> ().UploadScore (GetComponent<Arguments> ().getUserID (), GetComponent<Arguments> ().getGameID (), score));

	}
	
	
	public void IncreasePoints(int amount, Vector3 pos)
	{
		int totalAmount;

		if (amount > 0) {
			totalAmount = amount*scoreMultiplier;
		}
		else{
			totalAmount = amount;
		}

		totalPoints += totalAmount;

		TileScore tScore = Instantiate (tileScore, new Vector3(pos.x, pos.y, 0), Quaternion.identity) as TileScore;
		tScore.setScore(totalAmount);


		if(totalPoints<0)totalPoints = 0;
		_hud.setPoints (totalPoints);

		buttonStateUpdate ();
	}

	private List<Vector2> explosion(int x, int y, int range, List<Vector2> tilePositions){
		
		if (tilePositions == null) {
			tilePositions = new List<Vector2> ();
		}
		if (range > 0) {
			List<Vector2> newPositions = mapGenerator.testNeighbours (x, y);
			foreach (Vector2 pos in newPositions) {

				if(!tilePositions.Contains(pos)){
					tilePositions.Add (pos);
				}
				explosion ((int)pos.x, (int)pos.y, range - 1, tilePositions);

			}
			return tilePositions;
		}
		return tilePositions;
	}

	public void removeController(){
		_controller.DestroyThis ();
	}
	 
	public void Bomb(Vector2 pos)
	{
		IncreasePoints(-bombCost, _player.transform.position);

		Instantiate(Splosion, _player.transform.position, Quaternion.identity);
		Shake();
		foreach (Vector2 position in explosion ((int)pos.x, (int)pos.y, bombRange, null)) {
			if(inArray(position)){
				deleteTile(tileArray[(int)position.x, (int)position.y], true);
			}
		}

		tutContrl.bomb = true;

	}
}
