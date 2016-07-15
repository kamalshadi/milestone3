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
	public Vector3 lr1;
	public Vector3 lr2;
	// Use this for initialization

	/// <summary>
	/// line renderer
	public Color c1 = Color.blue;
	public Color c2 = Color.black;
	public int lengthOfLineRenderer = 2;
	/// </summary>
	void Start () {
		///////////
		//// line renderer
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Unlit/Texture"));
		lineRenderer.SetColors(c1, c1);
		lineRenderer.SetWidth(0.1F, 0.1F);
		//////////
		pur = false;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 p2p = player.position - transform.position;
		Vector3 aim = -this.transform.up;
		/////
		/// line renderer
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		Vector3 pos1 = transform.position;
		Vector3 pos2 = pos1 - 10 * transform.up;
		lineRenderer.SetPosition(0, pos1);
		lineRenderer.SetPosition(1, pos2);

		float cone = Vector3.Angle (aim, p2p);
		if (p2p.magnitude < 10 && cone < 30) {
			/// pass element for line renderer

			Rigidbody T = player.GetComponent<Rigidbody>();
			Vector3 new_point;
			if (T.velocity.magnitude < .05F) {
				new_point = player.position;
			} else {
				new_point = player.position + T.velocity * Time.deltaTime;
			}
			Vector3 predicted = (new_point - transform.position);
			float maxr = smooth * Time.deltaTime * Mathf.PI / 180.0F;
			Vector3 newDir = Vector3.RotateTowards(aim, predicted, maxr, 0.0F);
			//////
			lr1 = pos1;
			predicted.Normalize ();
			lr2 = pos1 + 10 * predicted;
			///
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
			lr2 = lr1;
		}
		transform.RotateAround(transform.position, Vector3.up, rotme);
	}
}
