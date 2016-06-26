using UnityEngine;
using System.Collections;


public class CameraSounds : MonoBehaviour {

	public AudioClip[] sounds = new AudioClip[3];
	private AudioSource source;

	void Awake(){

		source = gameObject.GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			source.PlayOneShot(sounds[0]);
		}
	}

	public void playSound(int sound){
		source.PlayOneShot(sounds[sound]);
	}
}
