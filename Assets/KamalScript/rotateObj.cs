using UnityEngine;
using System.Collections;

public class rotateObj : MonoBehaviour {
	public Transform player;
	public GameObject refme;
	public float smooth = 20F;
	public float rotme;
	float X;
	float Z;
	public bool pur;
	private Vector3 fixing;
	// Use this for initialization
	void Start () {
		pur = false;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 p2p = player.position - refme.transform.position;
		Vector3 aim = -this.transform.up;
		float cone = Vector3.Angle (aim, p2p);
		if (p2p.magnitude < 10 && cone < 30) {
			Rigidbody T = player.GetComponent<Rigidbody>();
			Vector3 new_point;
			if (T.velocity.magnitude < .1F) {
				new_point = player.position;
			} else {
				new_point = player.position + T.velocity * Time.deltaTime;
			}
			Vector3 predicted = (new_point - refme.transform.position);
			float maxr = smooth * Time.deltaTime * Mathf.PI / 180.0F;
			Vector3 newDir = Vector3.RotateTowards(aim, predicted, maxr, 0.0F);
			newDir.y = 0.0F;
			float tohere = Vector3.Angle (aim, newDir);

			rotme = Mathf.Min (smooth * Time.deltaTime, tohere);
			float sig = Vector3.Dot (newDir, aim);
			if (rotme < 0.01F) {
				pur = true;
			}

			if (sig > 0 && pur) {
					rotme = -rotme;
			}

		} else {
			pur = false;
			rotme = smooth * Time.deltaTime;
		}
		transform.RotateAround(transform.position, Vector3.up, rotme);
	}
}
