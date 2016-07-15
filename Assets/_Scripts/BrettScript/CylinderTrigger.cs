using UnityEngine;
using System.Collections;

public class CylinderTrigger : MonoBehaviour {

	public Vector3 cylinderThrust;	// amount of force to add to cylinder

	GameObject cylinder;
	Rigidbody [] cylinderRb;
	AudioSource [] cylinderAudio;

	void Awake () {

		cylinder = GameObject.FindWithTag ("Cylinder");
		cylinderRb = cylinder.GetComponentsInChildren<Rigidbody> ();

		foreach (Rigidbody cyl in cylinderRb) {
			cyl.isKinematic = true;
		}

		cylinderAudio = cylinder.GetComponentsInChildren<AudioSource> ();
	}

	void Start(){


	}

	void OnTriggerEnter(Collider other){
		
		if(other.gameObject.CompareTag("Player")){
		
			//Debug.Log("apply a force to roll the cylinder");
			foreach (Rigidbody cyl in cylinderRb) {
				cyl.isKinematic = false;
				cyl.AddForce (cylinderThrust);
				foreach (AudioSource aud in cylinderAudio)
					aud.Play ();
				cyl.tag = "Obstacle";
				Destroy (cylinder, 6.0f);
			}
				
			//cylinderAudio.Play ();
			//cylinder.tag = "Obstacle";

			//disable collider
			gameObject.SetActive(false);
			//destroy cylinder(s)


		}
	}
}
