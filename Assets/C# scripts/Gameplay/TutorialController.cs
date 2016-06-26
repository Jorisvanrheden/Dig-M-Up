using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TutorialController : MonoBehaviour 
{
	public GUIStyle Tutstyle;
	public GUIStyle NextStyle;

	public GameObject TutDudePrefab01;
	public GameObject TutDudePrefab02;
	public GameObject TutDudePrefab03;
// 0 = explaining 1 = angry 2 = congratulatory

	private List<GameObject> TutDudePrefabs = new List<GameObject>();

	public Texture2D TutDude;
	public Texture2D TutBubble;
	public Texture2D TutArrow;
	public Texture2D TutNextBtn;

	private bool tutDudeVisible = true;


	private string tutState = "Start";
	public int tutStartState = 1;

	private float timer = 0.0f;

	public int Lava = 0;
	public float Gas = 0.0f;
	public bool bomb  = false;
	public bool mission = false;

	public GameManager gManager;

	private CameraSounds sounds;

	private string startText1 = "Hey daar!\nWil je me helpen om\nde diepte in te graven?";
	private string startText2 = "Geweldig!\nWe gebruiken een GPR om\nonder de grond te kunnen kijken.";
	private string startText3 = "GPR staat voor\n\"Grond Penetrerende Radar\".\nKlaar om te beginnen?";
	private string startText4 = "Houd je vinger in de cirkel\nen begin met graven!\nYIEHAAAAAA !!!!";

	private string helpText1 = "Zie je die 3 voorwerpen daar?\nIk zou het super vinden als\nje die verzamelt.\nIk geef je er bonuspunten voor.\nGraaf ze maar snel op!";

	private string helpText2 = "Hey daar partner!\nPas je op voor de lava?\nDat kost je geld!";

	private string helpText3 = "Let je wel goed op?\nJe benzine raakt steeds op!\nKoop maar snel wat meer!";

	private string helpText4 = "Psssst!\nZal ik je een geheimpje vertellen?\nMet deze knop\nmaak je een grote explosie.\nProbeer maar!";

	void Awake(){
		sounds = Camera.main.GetComponent<CameraSounds> ();
	}

	void Start()
	{
		gManager = GetComponent<GameManager>();
		TutDudePrefabs.Add(TutDudePrefab01);
		TutDudePrefabs.Add(TutDudePrefab02);
		TutDudePrefabs.Add(TutDudePrefab03);
	}

	void Update()
	{
		timer += Time.deltaTime;

	}

	void OnGUI()
	{
		switch(tutState)
		{
		case "Idle":
			HideGuy();
			break;
		case "Start":
			ShowGuy(0);

			GUI.DrawTexture (new Rect (Screen.width - 770, Screen.height - 510, 400, 300), TutBubble, ScaleMode.StretchToFill, true, 10.0f);
			if(tutStartState == 1)
			{
				GUI.Label(new Rect(Screen.width - 780, Screen.height - 560, 400, 300), startText1, Tutstyle);

			}
			if(tutStartState == 2)
			{
				GUI.Label(new Rect(Screen.width - 780, Screen.height - 560, 400, 300), startText2, Tutstyle);
			}
			if(tutStartState == 3)
			{
				GUI.Label(new Rect(Screen.width - 780, Screen.height - 560, 400, 300), startText3, Tutstyle);
			}
			if(tutStartState == 4)
			{
				GUI.Label(new Rect(Screen.width - 780, Screen.height - 560, 400, 300), startText4, Tutstyle);
				Rect rect = new Rect(Screen.width/2 - 250, Screen.height /2 + 100, 200, 200);
				Vector2 pivot = new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
				Matrix4x4 matrixBackup = GUI.matrix;
				GUIUtility.RotateAroundPivot(50, pivot);
				GUI.DrawTexture(rect, TutArrow);
				GUI.matrix = matrixBackup;
			}


			if (GUI.Button (new Rect (Screen.width - 450, Screen.height - 310, 100, 100), " ", NextStyle)) 
			{
				tutStartState++;
				if(tutStartState == 5)
				{
					tutState = "Idle";

					UnFreezeTime();
				}
				sounds.playSound(1);
			}

			break;
		case "help1":
			ShowGuy(0);
			gManager.cam.disable();
			GUI.DrawTexture (new Rect (Screen.width - 770, Screen.height - 510, 400, 300), TutBubble, ScaleMode.StretchToFill, true, 10.0f);
			GUI.Label(new Rect(Screen.width - 770, Screen.height - 560, 400, 300), helpText1, Tutstyle);

			Rect rect2 = new Rect(Screen.width - 450, 150, 200, 200);
			Vector2 pivot2 = new Vector2(rect2.xMin + rect2.width * 0.5f, rect2.yMin + rect2.height * 0.5f);
			Matrix4x4 matrixBackup2 = GUI.matrix;
			GUIUtility.RotateAroundPivot(-50, pivot2);
			GUI.DrawTexture(rect2, TutArrow);
			GUI.matrix = matrixBackup2;

			if (GUI.Button (new Rect (Screen.width - 450, Screen.height - 310, 100, 100), " ", NextStyle)) 
			{
				tutState = "Idle";
				gManager.cam.enable();

				UnFreezeTime();
			}
			break;
		case "help2":
			ShowGuy(1);
			gManager.cam.disable();
			GUI.DrawTexture (new Rect (Screen.width - 770, Screen.height - 510, 400, 300), TutBubble, ScaleMode.StretchToFill, true, 10.0f);
			GUI.Label(new Rect(Screen.width - 770, Screen.height - 560, 400, 300), helpText2, Tutstyle);
			if (GUI.Button (new Rect (Screen.width - 450, Screen.height - 310, 100, 100), " ", NextStyle)) 
			{
				tutState = "Idle";
				gManager.cam.enable();
				UnFreezeTime();
			}
			break;
		case "help3":
			ShowGuy(1);
			gManager.cam.disable();
			GUI.DrawTexture (new Rect (Screen.width - 770, Screen.height - 510, 400, 300), TutBubble, ScaleMode.StretchToFill, true, 10.0f);
			GUI.Label(new Rect(Screen.width - 770, Screen.height - 560, 400, 300), helpText3, Tutstyle);

			Rect rect3 = new Rect(150, Screen.height /2 - 125, 200, 200);
			Vector2 pivot3 = new Vector2(rect3.xMin + rect3.width * 0.5f, rect3.yMin + rect3.height * 0.5f);
			Matrix4x4 matrixBackup3 = GUI.matrix;
			GUIUtility.RotateAroundPivot(180, pivot3);
			GUI.DrawTexture(rect3, TutArrow);
			GUI.matrix = matrixBackup3;

			if (GUI.Button (new Rect (Screen.width - 450, Screen.height - 310, 100, 100), " ", NextStyle)) 
			{
				tutState = "Idle";
				gManager.cam.enable();
				UnFreezeTime();
			}
			break;
		case "help4":
			ShowGuy(0);
			gManager.cam.disable();
			GUI.DrawTexture (new Rect (Screen.width - 770, Screen.height - 510, 400, 300), TutBubble, ScaleMode.StretchToFill, true, 10.0f);
			GUI.Label(new Rect(Screen.width - 770, Screen.height - 560, 400, 300), helpText4, Tutstyle);

			Rect rect4 = new Rect(150, Screen.height /2 - 400, 200, 200);
			Vector2 pivot4 = new Vector2(rect4.xMin + rect4.width * 0.5f, rect4.yMin + rect4.height * 0.5f);
			Matrix4x4 matrixBackup4 = GUI.matrix;
			GUIUtility.RotateAroundPivot(180, pivot4);
			GUI.DrawTexture(rect4, TutArrow);
			GUI.matrix = matrixBackup4;

			if (GUI.Button (new Rect (Screen.width - 450, Screen.height - 310, 100, 100), " ", NextStyle)) 
			{
				tutState = "Idle";
				gManager.cam.enable();
				UnFreezeTime();
			}
			break;
		}
	}

	private void FreezeTime()
	{

	}
	private void UnFreezeTime()
	{
		
	}

	private void ShowGuy(int dudenr)
	{
		if(tutDudeVisible == false)
		{
			tutDudeVisible = true;
			TutDudePrefabs[dudenr].SetActive(true);
		}
	}
	private void HideGuy()
	{
		if(tutDudeVisible)
		{
			tutDudeVisible = false;
			for (int i = 0; i < TutDudePrefabs.Count; i ++)
			{
				TutDudePrefabs[i].SetActive(false);
			}
		}
	}

	public void AddLava()
	{
		Lava++;
		if(Lava > 15)
		{
			tutState = "help2";
			Lava = 0;
		}
	}
	public void AddFuel()
	{
		Gas += Time.deltaTime;
		if(Gas > 20.0f)
		{
			tutState = "help3";
			Gas = 0.0f;
		}
	}
	public void BombUsedCheck()
	{
		if(bomb == false)
		{
			tutState = "help4";
			bomb = true;
		}
	}
	public void MisiionUsedCheck()
	{
		if(mission == false)
		{
			tutState = "help1";
			mission = true;
		}
	}

}
