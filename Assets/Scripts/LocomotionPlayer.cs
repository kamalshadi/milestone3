
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class LocomotionPlayer : MonoBehaviour {

    protected Animator animator;

    private float speed = 0;
    private float direction = 0;
    private Locomotion locomotion = null;
    private bool isJump = false;
    private bool isCrouch = false;
    private bool crouching = false; 

    private Vector3 move;


    [SerializeField] float movingTurnSpeed = 360;

    [SerializeField] float stationaryTurnSpeed = 180;
    [SerializeField] float jumpPower = 12f;
    [Range(1f, 4f)][SerializeField] float gravityMultiplier = 2f;
    [SerializeField] float runCycleLegOffset = 0.2f; 
    [SerializeField] float moveSpeedMultiplier = 1f;
    [SerializeField] float animSpeedMultiplier = 1f;
    [SerializeField] float groundCheckDistance = 0.1f;
    private Transform cam;
    private Vector3 camForward;

    Rigidbody rigidBody;
    bool isGrounded;
    bool isVault = false;
    bool isSlide = false;
    float origGroundCheckDistance;
    const float k_Half = 0.5f;
    float turnAmount;
    float forwardAmount;
    Vector3 groundNormal;
    float capsuleHeight;
    Vector3 capsuleCenter;
    CapsuleCollider capsule;

    private IsometricCamera isoCam;

	/*enabling ragdoll requires: 
	 * 	disabling the animation controller,
	 * 	changing game objects rigid body to kinematic
	 * 	changing bone rigid bodies to non-kinematic
	 */

	//array of bone rigid bodies in player game object
	public Component[] avatarBones;

	//need to make a condition for ragdoll
	public bool ragDoll = false;


    // Use this for initialization
    void Start () 
	{
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }

		rigidBody = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator>();
        locomotion = new Locomotion(animator);
        isoCam = Camera.main.GetComponent<IsometricCamera>();

        capsule = GetComponent<CapsuleCollider>();
        capsuleHeight = capsule.height;
        capsuleCenter = capsule.center;

		//define avatarBones with rigid bodies in player game object

		avatarBones = gameObject.GetComponentsInChildren<Rigidbody>();

		//set avatarBones to is kinematic

		foreach(Rigidbody bone in avatarBones){

			bone.isKinematic = true;
		}

		rigidBody.isKinematic = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        origGroundCheckDistance = groundCheckDistance;
    }

	void OnCollisionEnter(Collision other){

		//check to see if collision is with an obstacle in the scene
		if (other.gameObject.CompareTag ("Obstacle")) {
			ragDoll = true;
		}

	}
    
	void Update () 
	{
		
        if (!isJump)
        {
            isJump = Input.GetButtonDown("Jump");
        }
		if (ragDoll) {

			goForRagdoll ();
		}

	}

    private void FixedUpdate()
	{
		 
        isCrouch = Input.GetKey(KeyCode.C);
/*
        if (isCrouch) { 
            if (!crouchToggle) { 
                animator.SetBool("Crouch", true);
                capsule.center = capsule.center / 2;
                capsule.height = capsule.height / 2;
                isCrouch = false;
                crouchToggle = true;
            }
           else {
                animator.SetBool("Crouch", false);
                capsule.center = capsuleCenter;
                capsule.height = capsuleHeight;
                crouchToggle = false;
            }
        }
        */
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // calculate move direction to pass to character
        if (cam != null)
        {
            // calculate camera relative direction to move:
            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            move = v * camForward + h * cam.right;
        }
        else { 
            // we use world-relative directions in the case of no main camera
            move = v * Vector3.forward + h * Vector3.right;
        }

        // walk speed multiplier
        if (Input.GetKey(KeyCode.LeftShift))
            move *= 0.5f;

        if (Input.GetKey(KeyCode.V))
            StartCoroutine(doVault());

        if (Input.GetKey(KeyCode.B))
            StartCoroutine(doSlide());

        if (Input.GetKey(KeyCode.Alpha1))
            isoCam.RotateCamera(0);
        if (Input.GetKey(KeyCode.Alpha2))
            isoCam.RotateCamera(1);
        if (Input.GetKey(KeyCode.Alpha3))
            isoCam.RotateCamera(2);
        if (Input.GetKey(KeyCode.Alpha4))
            isoCam.RotateCamera(3);


        // pass all parameters to the character control script
		Move(move, isJump, isCrouch);
        isJump = false;

    }

    public IEnumerator doVault()
    {

        animator.SetBool("Vault", true);
        capsule.center = new Vector3(0, 1.35f, 0);
        capsule.height = 1.03f;
        rigidBody.useGravity = false;
        isVault = true;
        yield return new WaitForSeconds(1);
        animator.SetBool("Vault", false);
        capsule.center = capsuleCenter;
        capsule.height = capsuleHeight;
        rigidBody.useGravity = true;
        isVault = false;
    }
    public IEnumerator doSlide()
    {
        animator.SetBool("Slide", true);
        capsule.center = new Vector3(0, 0.39f, 0);
        capsule.height = 0.70f;
        rigidBody.useGravity = false;
        isSlide = true;
        yield return new WaitForSeconds(1);
        animator.SetBool("Slide", false);
        capsule.center = capsuleCenter;
        capsule.height = capsuleHeight;
        rigidBody.useGravity = true;
        isSlide = false;
    }

	//transition from mecanim Animation Controller to ragdoll physics
	public void goForRagdoll(){

		Debug.Log ("entered goForRagdoll()");

		//set avatar Bones to non kinematic objects
		foreach (Rigidbody bone in avatarBones) {
			bone.isKinematic = false;
		}
		//set game object rigid body to kinematic
		//rigidBody.isKinematic = true;

		//disable animation controller
		animator.enabled = false;

		Debug.Log ("Condition for transitioning to next scene can be ragDoll");
		ragDoll = false;

		StartCoroutine (reloadScene ());


	}

	IEnumerator reloadScene(){
		yield return new WaitForSeconds (3);
		SceneManager.LoadScene ("TestScene");
	}

    
    public void Move(Vector3 move, bool jump, bool crouch)
    {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, groundNormal);
        turnAmount = Mathf.Atan2(move.x, move.z);
        forwardAmount = move.z;

        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (isGrounded)
        {
            HandleGroundedMovement(jump, crouch);
        }
        else
        {
            HandleAirborneMovement();
        }

        ScaleCapsuleForCrouching(crouch);
        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }

    void HandleGroundedMovement(bool jump, bool crouch)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion.WalkRun"))
        {
            // jump!
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpPower, rigidBody.velocity.z);
            isGrounded = false;
            animator.applyRootMotion = false;
            groundCheckDistance = 0.1f;
        }
    }

    void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
        rigidBody.AddForce(extraGravityForce);

        groundCheckDistance = rigidBody.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
    }
        

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
        #if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
        #endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
            isGrounded = true;
            animator.applyRootMotion = true;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
            animator.applyRootMotion = false;
        }
    }

    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        animator.SetFloat("Speed", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Direction", turnAmount, 0.1f, Time.deltaTime);
        animator.SetBool("OnGround", isGrounded);
        animator.SetBool("Crouch", crouching);
        if (!isGrounded)
        {
            animator.SetFloat("Jump", rigidBody.velocity.y);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (isGrounded && move.magnitude > 0)
        {
            animator.speed = animSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            animator.speed = 1;
        }
    }

    void ScaleCapsuleForCrouching(bool crouch)
    {
        if (isGrounded && crouch)
        {
            if (crouching)
                return;
            capsule.height = capsule.height / 2f;
            capsule.center = capsule.center / 2f;
            crouching = true;
        }
        else
        { /*
            Ray crouchRay = new Ray(rigidBody.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = capsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
            {
                crouching = true;
                return;
            } */

            
            if(!isVault && !isSlide) { 
                capsule.height = capsuleHeight;
                capsule.center = capsuleCenter;
            }
            crouching = false;
        }
    }

}
