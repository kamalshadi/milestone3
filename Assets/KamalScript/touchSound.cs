using UnityEngine;
using System.Collections;

public class touchSound : MonoBehaviour {

	// Use this for initialization
	private float tt;
	AudioSource myaudio;
	private bool touchme;
	Animator animator;

	public GameObject hero;

	void Start() {
		myaudio = GetComponent<AudioSource> ();
		animator = hero.GetComponent<Animator>();
	}

	void OnCollisionEnter (Collision collision) 
	{
		touchme = true;
	}

	void OnCollisionExit (Collision collision) 
	{
		touchme = false;
	}
	// Update is called once per frame
	void Update () {
		tt = animator.GetFloat("Speed");
		if (touchme && !myaudio.isPlaying && tt > 0) {
			myaudio.Play ();
		} else {
			if (tt < 0.001) {
				myaudio.Stop ();
			}
		}
	}
}