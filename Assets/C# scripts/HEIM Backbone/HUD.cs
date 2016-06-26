using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	private Texture2D[] hudAnimationArray = new Texture2D[40];
	private Texture2D HUDtop;
	private string temp;

	private int arrayPosition = 0;


	private PlayerController player;
	public int totalPoints;
	private float depth;
	private float maxDepth;
	private float screenDepth = 620;

	private float fuelTimer = 60;

	//depthmeter
	public Texture portrait;
	public Texture depthMeter;
	public Texture arrow;
	public Texture fuelWarning;

	public Texture2D score;

	private float fps;
	private float fpsTimer = 1;
	private float playTime = 0;

	public bool count = true;

	public GUIStyle style;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (player != null) {
			depth = maxDepth - player.transform.position.y;
			if(depth>maxDepth)depth = maxDepth;
		}

		if (fpsTimer <= 0) {
			fps = Mathf.Floor (1 / Time.deltaTime);
			fpsTimer = 1;
		} else
			fpsTimer -= Time.deltaTime;

		if(totalPoints<0)totalPoints=0;

		if (count)playTime += Time.deltaTime;

	}

	public void setInfo(PlayerController _player, float _maxDepth){
		player = _player;
		maxDepth = _maxDepth;
	}
	public void setPoints(int points){
		totalPoints = points;
	}


	void OnGUI() {		
		//AnimateHudTop("HUD/Hudcrop_");

		GUI.DrawTexture (new Rect (0,0,score.width/2,score.height/2), score, ScaleMode.StretchToFill, true, 10.0f);

		style.normal.textColor = Color.white;
		GUI.TextArea (new Rect (Screen.width*0.09f, 75, 150, 50), Mathf.Floor(playTime).ToString(), style);
		GUI.TextArea (new Rect (Screen.width*0.21f, 85, 50, 25),  Mathf.Floor((player.getFuel()/100)*100) + "%", style);
		GUI.TextArea (new Rect (Screen.width*0.01f, 75, 150, 50), totalPoints.ToString(), style);

		if (Camera.main.transform.position.y - player.transform.position.y < -4) {
			GUI.DrawTexture (new Rect (Screen.width/2-50, 20, 100, 100), portrait, ScaleMode.StretchToFill, true, 10.0f);
		}

		if (player.getFuel() < 10) {
			fuelTimer -= 20*Time.deltaTime;
			
			if(fuelTimer >= 30){
				GUI.DrawTexture(new Rect(Screen.width/2-200, Screen.height/2, 300,375), fuelWarning, ScaleMode.StretchToFill, true, 10.0f);
			}
			if(fuelTimer<=0){
				fuelTimer = 60;
			}
		}


		GUI.DrawTexture (new Rect (Screen.width-60, 0, 60, screenDepth), depthMeter, ScaleMode.StretchToFill, true, 10.0f);
		float arrowDepth = (depth/maxDepth)*screenDepth;
		if(arrowDepth>582)arrowDepth = 582;
		GUI.DrawTexture (new Rect (Screen.width-60 + 17, 14 + arrowDepth, 27,12), arrow, ScaleMode.StretchToFill, true, 10.0f);
	}
	
	public void AnimateHudTop(string imageName) {
		AssignHUDTextures(imageName);
		
		if(arrayPosition < hudAnimationArray.Length) {
			arrayPosition++;
		}

		// Make sure the arrayposition does not exceed the array.length
		if(arrayPosition > hudAnimationArray.Length - 2) {
			arrayPosition = hudAnimationArray.Length - 1;
		}
		
		// Assign the texture of the selected arrayposition to the visionAnimation
		HUDtop = hudAnimationArray[arrayPosition];
		
		GUI.DrawTexture(new Rect(((Screen.width / 2)-(1150 / 2) - 20),-10,1150,150),HUDtop);
	}
	
	private void AssignHUDTextures(string imageName) {
		for(int i = 0; i < hudAnimationArray.Length; i++) {
			if(i < 10) {
				temp = "0000" + i.ToString();
			}
			else if(i >= 10 && i <= 99) {
				temp = "000" + i.ToString();
			}
			else if(i >= 100) {
				temp = "00" + i.ToString();
			}
			hudAnimationArray[i] = (Texture2D)Resources.Load(imageName + temp, typeof(Texture2D));	
		}
	}
}
