using UnityEngine;
using System.Collections;

public class forceOnCollision : MonoBehaviour
{

	public float forceApplied = 50;

	void OnCollisionEnter(Collision col)
	{
		Debug.Log ("Collision!");
		GetComponent<Rigidbody>().AddForce (0, forceApplied, 0);
	}
}