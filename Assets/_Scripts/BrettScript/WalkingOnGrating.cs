using UnityEngine;
using System.Collections;

public class WalkingOnGrating : MonoBehaviour {

	public GameObject player;

	Animator anim;
	AudioSource walkOnGrating;
	bool step = true;

	// Use this for initialization
	void Awake()
	{
		// Get access to player animator and audio sources
		anim = player.GetComponent<Animator>();
		walkOnGrating = GetComponent<AudioSource> ();
	}

	void OnTriggerStay(Collider other)
	{

		// Check if player has entered the grating trigger
		if (other.gameObject.tag == "Player")
		{
			//check to make sure player is moving
			if (anim.GetFloat("Speed") > 0.5 && step){
				StartCoroutine(StepClip());
			}
		}
	}

	IEnumerator StepClip()
	{
		step = false;
		// Plays a walk step and waits ample time before the next on
		walkOnGrating.Play();
		yield return new WaitForSeconds(0.3f);
		step = true;
	}
}
