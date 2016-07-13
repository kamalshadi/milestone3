using UnityEngine;
using System.Collections;

public class canTip : MonoBehaviour {
	public GameObject refme;
	private float smooth;
	GameObject firearm;
	rotateObj clone;

	// Use this for initialization
	void Start () {
		firearm = GameObject.Find("cannon");
		clone = firearm.GetComponent<rotateObj>();
	}
	
	// Update is called once per frame
	void Update () {
		smooth = clone.rotme;
		transform.RotateAround(refme.transform.position, Vector3.up, smooth);
	}
}
