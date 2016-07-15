using UnityEngine;
using System.Collections;

public class BoxCollision : MonoBehaviour {

	AudioSource boxAudio;

	void Awake(){

		boxAudio = GetComponent<AudioSource> ();
	
	}
	void OnCollisionEnter(Collision other){

		//check to see if collision is with the player object
		if (other.gameObject.CompareTag ("Player")) {
			Debug.Log ("player hit box");
			boxAudio.Play ();
		}

	}

}
