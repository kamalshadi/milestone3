using UnityEngine;
using System.Collections;

public class collisionSound : MonoBehaviour {
	AudioSource audio;

	void Start() {
		audio = GetComponent<AudioSource>();
	}

	void OnCollisionEnter(Collision collision) {
		foreach (ContactPoint contact in collision.contacts) {
			Debug.DrawRay(contact.point, contact.normal, Color.white);
		}
		if (collision.relativeVelocity.magnitude > 2)
			audio.Play();

	}
}