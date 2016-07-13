
using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour 
{
	public Transform target;
	public float distanceX;
	public float distanceZ;
	public float distanceY;
	public float distanceDamping = 1.0f;
	public float heightDamping = 1.0f;


    private float[] cameraXPos = { 0, 0, 0, 0 };
	private float[] cameraZPos = { 0, 0, 0, 0 };

	// Use this for initialization
	void Start () 
	{
        cameraXPos[0] = distanceX;
        cameraXPos[1] = distanceX;
        cameraXPos[2] = -distanceX;
        cameraXPos[3] = -distanceX;

        cameraZPos[0] = distanceZ;
        cameraZPos[1] = -distanceZ;
        cameraZPos[2] = -distanceZ;
        cameraZPos[3] = distanceZ;

    }
	
	// Update is called once per frame
	void Update () 
	{

	}

	void LateUpdate ()
	{
		float wantedDistanceX = target.position.x + distanceX;
		float wantedDistanceY = target.position.y + distanceY;
		float wantedDistanceZ = target.position.z - distanceZ;
		float currentDistanceX = transform.position.x;
		float currentDistanceY = transform.position.y;
		float currentDistanceZ = transform.position.z;

		currentDistanceX = Mathf.Lerp (currentDistanceX, wantedDistanceX, distanceDamping * Time.deltaTime);
		currentDistanceY = Mathf.Lerp (currentDistanceY, wantedDistanceY, heightDamping * Time.deltaTime);
		currentDistanceZ = Mathf.Lerp (currentDistanceZ, wantedDistanceZ, distanceDamping * Time.deltaTime);

		transform.position = new Vector3(currentDistanceX, currentDistanceY, currentDistanceZ);

		transform.LookAt(target);
	}

	public void RotateCamera(int cameraPos)
	{
		distanceX = cameraXPos[cameraPos];
		distanceZ = cameraZPos[cameraPos];
	}
}
