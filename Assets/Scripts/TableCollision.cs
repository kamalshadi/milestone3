using UnityEngine;
using System.Collections;

public class TableCollision : MonoBehaviour {

	public float strength;
	Rigidbody tableRb;
	AudioSource tableHit;


	void Awake(){
	
		tableRb = GetComponent<Rigidbody> ();
		tableHit = GetComponent<AudioSource> ();

	}

	void OnCollisionEnter(Collision other){

		//check to see if collision is with the player object
		if (other.gameObject.CompareTag ("Player")) {

			//Debug.Log ("player bumped into table");
			//add force in z direction where to collision takes place
			//collision takes place at contact point in Collision structure
			ContactPoint contact = other.contacts[0];
			//Debug.Log ("player hit point on table:" + contact.point);
			//Vector3 throwForce = new Vector3(0,strength,0);
			tableRb.AddForceAtPosition (Vector3.up*strength , contact.point);
			tableHit.Play ();
		}

	}
}
