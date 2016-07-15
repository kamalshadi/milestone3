using UnityEngine;
using System.Collections;

public class DoorOpening : MonoBehaviour {

	AudioSource doorAudio;

	void Awake(){

		doorAudio = GetComponent<AudioSource> ();

	}
	void OnCollisionEnter(Collision other){

		//check to see if collision is with the player object
		if (other.gameObject.CompareTag ("Player")) {
			doorAudio.Play ();
		}

	}
}
