using UnityEngine;
using System.Collections;

public class TrashCollision : MonoBehaviour {

	public Vector3 endCapsuleCenter;
	public float endRad;
	public float endHeight;
	Rigidbody trashCan;
	CapsuleCollider trashCollider;
	//AudioSource trashAudio;

	void Awake(){

		//trashAudio = GetComponent<AudioSource> ();
		trashCan = GetComponent<Rigidbody> ();
		trashCollider = GetComponent<CapsuleCollider> ();


	}

	void Start(){

		trashCan.isKinematic = true;
	}
	void OnCollisionEnter(Collision other){

		//check to see if collision is with the player object
		if (other.gameObject.CompareTag ("Player")) {
			//Debug.Log ("Playing box audio");
			trashCan.isKinematic=false;
			trashCollider.center = endCapsuleCenter;
			trashCollider.radius = endRad;
			trashCollider.height = endHeight;
			//trashAudio.Play ();
		}

	}
}
