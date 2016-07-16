using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class KayaIsoLocomotion : MonoBehaviour {
    
	private Vector3 mainCamForward;
	private Vector3 mainCamRight;
	private Vector3 moveInput;

    //variables for mecanim AC
    private Animator anim;
	private float turn;
	private float forward;
	private float animSpeed;
	private float animDirection;
    public float timeToRagdoll;
    private AnimatorStateInfo currentBaseState;
    public float jumpPower;

    static int moveAnimState = Animator.StringToHash("Base Layer.WalkRun");
    static int jumpAnimState = Animator.StringToHash("Base Layer.Jumping");

    //info about ground
    public float groundHeight;				//compensation in y-direction when checking for ground
    public float maxFallDistance;           //max distance to ground; player rag dolls at large heights  
    public LayerMask mask;                  //layers for capsule check, raycast, etc. to ignore
    static float origDistToGround;          //distance from player (collider) center to the ground
    private float distToGround;             //current distance to ground; defined as origDistToGround on Start()
    private Vector3 groundNormal;           //current normal of walking plane
    private Vector3 landingPoint;           //point where player will land if not grounded
    
    struct groundInfo {public bool onGround; public float distance; public Vector3 landing; public Vector3 normal; };

	private IsometricCamera isoCam;
	private Transform cam;

	private Rigidbody kayaRB;
	private CapsuleCollider kayaCollider;

	public Component[] avatarBones;			//array of bone rigid bodies in player game object
    
	private bool ragDoll = false;			//condition to enable ragdoll simulation when colliding with game object tagged as obstacle
	private bool jump = false;				//condition indicating user has requested jump
    private bool isGrounded = true;

	/*on screen debug
	private Text directionText;
	private Text speedText;
	private Text moveText;
	private Text groundedText;
    private Text distanceText;*/

    void Start(){
        
        //debug UI components
		/*speedText = GameObject.Find("/Canvas/Speed").GetComponent<Text>();
		directionText = GameObject.Find ("/Canvas/Direction").GetComponent<Text> ();
		moveText = GameObject.Find ("/Canvas/MoveVector").GetComponent<Text> ();
		groundedText = GameObject.Find ("/Canvas/Grounded").GetComponent<Text> ();
        distanceText = GameObject.Find("/Canvas/Distance").GetComponent<Text>();*/

        kayaRB = GetComponent<Rigidbody> ();					//init player rigid body component\
		kayaCollider = GetComponent<CapsuleCollider>();			//init player collider
		anim = GetComponent<Animator>();						//init player animation component
		cam = Camera.main.transform;							//init main camera transform
		isoCam = Camera.main.GetComponent<IsometricCamera>();	//init isoCam component of main camera
        
        //define avatarBones with rigid bodies in player game object

        avatarBones = gameObject.GetComponentsInChildren<Rigidbody>();

		//set avatarBones to kinematic

		foreach(Rigidbody bone in avatarBones){

			bone.isKinematic = true;
		}

		//reset player parent rigid body component to non kinematic and freez rotations

		kayaRB.isKinematic = false;
		kayaRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
        //define layer mask to be complement of those selected for ignore

        mask = ~mask;

        //determine and define starting ground distance and define normal as up vector

        origDistToGround = kayaCollider.bounds.extents.y;
        distToGround = origDistToGround;
        groundNormal = Vector3.up;
        jump = false;
    }
		
	void OnCollisionEnter(Collision other){

		//check to see if collision is with an obstacle in the scene
		if (other.gameObject.CompareTag ("Obstacle")) {
			ragDoll = true;
		}

	}

	void Update () {
			
		if (!jump)			
			jump = Input.GetButtonDown ("Jump");

		if (ragDoll)
            Invoke("goForRagdoll", timeToRagdoll);

	}

	void FixedUpdate(){

		//get user input
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		//set forward and right move vectors for the player relative to the camera 
		mainCamForward = Vector3.Scale (cam.forward, new Vector3 (1.0f, 0.0f, 1.0f)).normalized;
		mainCamRight = cam.right;

		// calculate move direction to pass to character
		moveInput = v * mainCamForward + h * mainCamRight;

        //move the player game object
        Move(moveInput);
        
        //copied (from milestone 2) input controls for iso cam control
		if (Input.GetKey (KeyCode.Alpha6))
			isoCam.RotateCamera (0);
		if (Input.GetKey (KeyCode.Alpha7))
			isoCam.RotateCamera (1);
		if (Input.GetKey (KeyCode.Alpha8))
			isoCam.RotateCamera (2);
		if (Input.GetKey (KeyCode.Alpha9))
			isoCam.RotateCamera (3);

	}

	public void Move(Vector3 move){
	
		//normalize the move vector
		if (move.magnitude > 1.0f)
			move = Vector3.Normalize (move);

		//Debug.Log ("updated move vector: " + move);
		//on screen debug 
		//moveText.text = "Move: x = "+move.x+" y = "+move.y+" z = "+move.z;

		//convert world relative input move vector to local relative
		move = transform.InverseTransformDirection(move);
        
		//check for player ground condition
        groundInfo currentGround = playerGrounded();
		isGrounded = currentGround.onGround;
        distToGround = currentGround.distance;
        groundNormal = currentGround.normal;
        landingPoint = currentGround.landing;

        //ragdoll death if fall distance is large
        if (distToGround > maxFallDistance)
            ragDoll = true;

        //determine local relative player movement if ground conditions are okay
        move = Vector3.ProjectOnPlane (move, groundNormal);
        forward = move.z;
		turn = Mathf.Atan2 (move.x, move.z);

		//update mecanim AC parameters
		//move allows for updating the AC using polar coordinates

		animSpeed = Vector3.SqrMagnitude(move); //transition between walk (0.0) and run (1.0)-magnitude not used to improve runtime
		animDirection = turn;			//radians to rotate: +ve - turn right; -ve - turn left
        updateAC(animSpeed,animDirection,landingPoint);
		
        /*on screen debug for what is passed to mecanim
		speedText.text = "Speed: " + forward;
		directionText.text = "Direction: " + turn;
        groundedText.text = "Grounded: " + isGrounded;
        distanceText.text = "Distance to Ground: " + distToGround;*/
        	
	}
    
    // update parameters for mecanim AC
	public void updateAC(float s, float d, Vector3 land){

        currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // set  currentState variable to the current state of the Base Layer of animation

        if (Mathf.Abs (d) > 1.8) {
			anim.SetBool ("Pivot", true);
		}
		else {
			anim.SetBool ("Pivot", false);
		}

		anim.SetFloat ("Speed", s, 0.1f, Time.deltaTime);
		anim.SetFloat("Direction", d, 0.05f, Time.deltaTime);
        /*
        //jump snitch
        //standard jump
        if(currentBaseState.fullPathHash == moveAnimState)
        {
            if (jump)
            {
                anim.SetBool("Jump", true);
             }
                
        }
        // currently in jump state
        else if(currentBaseState.fullPathHash == jumpAnimState)
        {
            //not transitioning
            if (anim.IsInTransition(0))
            {
                //collider animation curve here
                Debug.Log("Jump Distance: " + anim.GetFloat("JumpDistance"));
                transform.Translate(transform.forward * anim.GetFloat("JumpDistance"));
                
                //reset jump bool
                anim.SetBool("Jump", false);
                jump = false;
            }
            //jump start and stop are not the same
           // if(distToGround > 2.0f)
           // {
                //match target with animation 
           //     anim.MatchTarget(land, Quaternion.identity,AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.35f, 0.5f);
           // }
                
        }*/
	}

	public void goForJump (bool j){

		anim.SetBool ("Jump", j);
		Invoke ("stopJump", 0.1f);
	}

    void stopJump(){

		//anim.SetBool ("Jump", false);
		jump = false;
	}

	//transition from anim to ragdoll physics
	public void goForRagdoll(){

		//set avatar Bones to non kinematic objects
		foreach (Rigidbody bone in avatarBones) {
			bone.isKinematic = false;
		}

		//disable animation controller
		anim.enabled = false;

		ragDoll = false;

		StartCoroutine (reloadScene ());
	}

	groundInfo playerGrounded(){

        groundInfo currentGroundInfo;
		bool gnd;
        float dist = 0.0f;
        Vector3 norm = Vector3.up;
        Vector3 land = transform.position;

        Vector3 start = kayaCollider.bounds.center;
		Vector3 end = new Vector3(kayaCollider.bounds.center.x, kayaCollider.bounds.min.y - groundHeight, kayaCollider.bounds.center.z);
		float rad = kayaCollider.radius;

		gnd = Physics.CheckCapsule (start, end, rad, mask);

		if(!gnd){
			
			//Debug.Log("Airborn snitch!");

            RaycastHit hit;

            if(Physics.Raycast(kayaCollider.bounds.center, -Vector3.up, out hit, 100f, mask))
            {
                dist = hit.distance;
                norm = hit.normal;
                land = hit.point;
            }
        }
        else {
			
            dist = origDistToGround;
            land = transform.position;

		}

        currentGroundInfo.onGround = gnd;
        currentGroundInfo.distance = dist;
        currentGroundInfo.normal = norm;    //returns up vector by default
        currentGroundInfo.landing = land;   //returns player position by default

        return currentGroundInfo;
	}

	IEnumerator reloadScene(){
		yield return new WaitForSeconds (3);
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}
}
