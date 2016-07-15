using UnityEngine;
using System.Collections;

public class patrolFlee : MonoBehaviour
{

    public Transform player;
    public Transform head;
    Animator anim;

    string state = "idle";  // different states are idle, fleeing, returning, hiding

    public GameObject startWaypoint;        // Start waypoint of the fleeing character. Character returns to this position after hiding for a while
    public GameObject fleeWaypoint;         // Hiding position of the fleeing character

    private int currentWP = 0;
    public float rotSpeed = 0.01f;
    public float speed = 1.5f;
    public float accuracyWP = 1.0f;      // Used to see if a character has reached a waypoint. This is the margin of error allowed when distance is calculated between character and waypoint

    public float viewAngle = 30f;
    public float viewDistance = 10f;

    private GameObject playerObject;                      // Reference to the player.
    private float currentWaitTime;       // Used to count the time that the AI hides
    public float waitingTime = 5f;       // Maximum time that the AI hides
    private NavMeshAgent nav;                               // Reference to the nav mesh agent.
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        currentWaitTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the direction between AI and the player
        Vector3 direction = player.position - this.transform.position;
        direction.y = 0;
        float angle = Vector3.Angle(direction, head.forward); 
 
        RaycastHit hit;

        //Initial state. When the character is idling, try raycasting to see if he can see the player.
        // If the player is in sight, change the state to "fleeing"
        if(state != "fleeing")
        {
            if (Vector3.Distance(player.position, this.transform.position) < viewDistance && (angle < viewAngle))
            {
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, viewDistance))
                {
                    if (hit.collider.gameObject == playerObject)
                    {
                        state = "fleeing";
                    }
                }
            }
        }

        // If player is fleeing, rotate their direction to hiding point and navigate to that.
        if (state == "fleeing")
        {
            direction = transform.position - fleeWaypoint.transform.position;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                        Quaternion.LookRotation(direction), rotSpeed);

            anim.SetBool("isFleeing", true);
            anim.SetBool("isReturning", false);
            anim.SetBool("isIdle", false);
            nav.Resume();
            nav.SetDestination(fleeWaypoint.transform.position);

            //Once the hiding point has been reached, change state to hiding and initiate hiding timer
            if (Vector3.Distance(fleeWaypoint.transform.position, transform.position) < accuracyWP)
            {
                state = "hiding";
                anim.SetBool("isFleeing", false);
                anim.SetBool("isReturning", false);
                anim.SetBool("isIdle", true);
                nav.Stop();
                currentWaitTime += Time.deltaTime;
            }
        }

        // Increment hiding timer until maximum allowed time is reached.
        if(state == "hiding" && currentWaitTime <= waitingTime)
        {
            currentWaitTime += Time.deltaTime;
        }

        // Once the maximum allowed time is reached, change the state to returning
        //Rotate the character towards the starting position and start navigating.
        if(state == "hiding" && currentWaitTime > waitingTime)
        {

            direction = startWaypoint.transform.position - transform.position;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                        Quaternion.LookRotation(direction), rotSpeed);
            state = "returning";
            anim.SetBool("isFleeing", false);
            anim.SetBool("isReturning", true);
            anim.SetBool("isIdle", false);


            nav.SetDestination(startWaypoint.transform.position);
            nav.Resume();
        }

        // If character has returned to original position, change state back to idle
        if(state == "returning")
        {
            if (Vector3.Distance(startWaypoint.transform.position, transform.position) < accuracyWP)
            {
                state = "idle";
                anim.SetBool("isFleeing", false);
                anim.SetBool("isReturning", false);
                anim.SetBool("isIdle", true);
                nav.Stop();
                currentWaitTime = 0;
            }
        }
  
    }
}
