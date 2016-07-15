using UnityEngine;
using System.Collections;

public class ballSpawn : MonoBehaviour {
	// C#

	// Require the rocket to be a rigidbody.
	// This way we the user can not assign a prefab without rigidbody
	public Rigidbody cannonBall;
	public float speed = 10f;
	rotateObj clone;
	GameObject firearm;
	float last_shoot;
	AudioSource audio;
//	void FireRocket () {
//		Rigidbody rocketClone = (Rigidbody) Instantiate(cannonBall, transform.position, transform.rotation);
//		rocketClone.velocity = transform.forward * speed;
//
//		// You can also acccess other components / scripts of the clone
//		//rocketClone.GetComponent<MyRocketScript>().DoSomething();
//	}

	// Calls the fire method when holding down ctrl or mouse
	// Use this for initialization
	void Start () {
		firearm = GameObject.Find("cannon");
		clone = firearm.GetComponent<rotateObj>();
		last_shoot = 0.0F;
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		bool shootornot = clone.pur;
		//if (Input.GetKeyDown("l")){ // This must be viewing cone
		if (shootornot && Time.time - last_shoot > 5){
			//transform.position and Quanterion must be updated
			Rigidbody A = (Rigidbody) Instantiate(cannonBall, transform.position, Quaternion.identity);
			//Instantiate(cannonBall, transform.position, Quaternion.identity);
			//Rigidbody A_rigid = A.GetComponent<Rigidbody> ();
			audio.Play();
			//Rigidbody clone = A.GetComponent<Rigidbody>();
			// Velocity of regidbody must be predicted
			Vector3 elevation = transform.forward;
			A.velocity = elevation * speed;
			//Ins
			// play sound when hit the object
			// disappear somehow
			last_shoot = Time.time;
			Destroy (A.gameObject, 5.0F);
		}
	}
	}
