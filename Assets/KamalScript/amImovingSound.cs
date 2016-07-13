using UnityEngine;
using System.Collections;

public class amImovingSound : MonoBehaviour {
	private Vector3 pos;
	private Rigidbody rb;
	AudioSource myaudio;
	// Use this for initialization
	void Start () {
		myaudio = GetComponent<AudioSource> ();
		rb = GetComponent<Rigidbody>();
		pos = rb.position;
	}
	
	// Update is called once per frame
	void Update () {

		if (!myaudio.isPlaying && rb.position != pos) {
			myaudio.Play ();
		} 
		pos = rb.position;
	}
}
