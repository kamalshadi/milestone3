using UnityEngine;
using System.Collections;

public class plotLine : MonoBehaviour {
	GameObject firearm;
	rotateObj myScript;
	public Color c1;
	public Color c2;
	public int lengthOfLineRenderer = 2;
	// Use this for initialization
	void Start () {
		c1 = Color.blue;
		c2 = Color.cyan;
		firearm = GameObject.Find("cannon");
		myScript = firearm.GetComponent<rotateObj>();
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(c1, c2);
		lineRenderer.SetWidth(0.5F, 0.5F);
	}

	// Update is called once per frame
	void Update () {
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		Vector3 pos1 = myScript.lr1;
		Vector3 pos2 = myScript.lr2;
		lineRenderer.SetPosition(0, pos1);
		lineRenderer.SetPosition(1, pos2);
	}
}

