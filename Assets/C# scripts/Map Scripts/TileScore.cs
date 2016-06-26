using UnityEngine;
using System.Collections;

public class TileScore : MonoBehaviour {

	private float alpha = 1;
	private float yOffset = -50;
	private float score = 0;

	private int maxFontSize = 100;
	private int minFontSize = 50;
	private int fontSize;

	public GUIStyle style;

	// Use this for initialization
	void Start () {
	
	}

	public void setScore(int _score){
		score = _score;

		if (score <= 0) {
			style.normal.textColor = Color.red;
			fontSize = maxFontSize;
		}
		else {
			style.normal.textColor = Color.green;
			fontSize = (int)(minFontSize + (score/2500)*(maxFontSize/2));
			if(fontSize>maxFontSize){
				fontSize = maxFontSize;
			}
		}

		style.fontSize = fontSize;
	}
	
	// Update is called once per frame
	void Update () {
		yOffset -= 40 * Time.deltaTime;
		alpha -= 0.8f * Time.deltaTime;

		if(alpha<=0){
			Destroy(gameObject);
		}
	}

	void OnGUI(){
		Vector3 position = Camera.main.WorldToScreenPoint (transform.position);
		GUI.color = new Color (GUI.color.r,GUI.color.g,GUI.color.b, alpha);
		GUI.TextField (new Rect (position.x - 20, Screen.height-position.y + yOffset, 50, 50), score.ToString(), style);
	}
}
