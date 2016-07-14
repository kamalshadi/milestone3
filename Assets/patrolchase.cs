using UnityEngine;
using System.Collections;

public class patrolchase : MonoBehaviour
{

    public Transform player;
    public Transform head;
    Animator anim;

    string state = "patrol";
    public GameObject[] waypoints;
    private int currentWP = 0;
    public float rotSpeed = 0.2f;
    public float speed = 1.5f;
    public float accuracyWP = 5.0f;

    public float viewAngle = 30f;
    public double stoppingDistance = 1.5f;
    public float viewDistance = 10f;

    private GameObject playerObject;                      // Reference to the player.
    private float chaseTimer = 0f;
    public float chaseWaitTime = 20f;
    private NavMeshAgent nav;                               // Reference to the nav mesh agent.
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - this.transform.position;
        direction.y = 0;
        float angle = Vector3.Angle(direction, head.forward);
        print (state);
        if (state == "patrol" && waypoints.Length > 0)
        {
            print("entering patrol");
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", true);
            if (Vector3.Distance(waypoints[currentWP].transform.position, transform.position) < accuracyWP)
            {

                //currentWP = Random.Range(0, waypoints.Length);
                currentWP++;
                if(currentWP >= waypoints.Length)
                {
                	currentWP = 0;
                }	
            }

            //rotate towards waypoint
            direction = waypoints[currentWP].transform.position - transform.position;
            this.transform.rotation = Quaternion.Slerp(transform.rotation,
                                 Quaternion.LookRotation(direction), 0.1f);
            //this.transform.Translate(0, 0, Time.deltaTime * speed);
            
            nav.SetDestination(waypoints[currentWP].transform.position);
            nav.Resume();
        }


        /*
        if (Vector3.Distance(player.position, this.transform.position) < 10 && (angle < 30 || state == "pursuing"))
        {

            state = "pursuing";
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                        Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

            if (direction.magnitude > 1.5)
            {
                this.transform.Translate(0, 0, Time.deltaTime * speed);
                anim.SetBool("isWalking", true);
                anim.SetBool("isAttacking", false);
            }
            else
            {
                anim.SetBool("isAttacking", true);
                anim.SetBool("isWalking", false);
            }

        }
        else
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);
            state = "patrol";
        }*/
        RaycastHit hit;
        if (Vector3.Distance(player.position, this.transform.position) < viewDistance && (angle < viewAngle || state == "pursuing") && chaseTimer < chaseWaitTime)
        {


            // ... and if a raycast towards the player hits something...
            if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, viewDistance))
            {
                // ... and if the raycast hits the player...
                if (hit.collider.gameObject == playerObject)
                {
                    state = "pursuing";

                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                Quaternion.LookRotation(direction), 0.1f);
                    print("changed to pursuing");
                    anim.SetBool("isIdle", false);
                    if (direction.magnitude > stoppingDistance)
                    {
                        nav.Resume();
                        nav.SetDestination(player.position);
                        //this.transform.Translate(0, 0, 0.05f);
                        anim.SetBool("isWalking", true);
                        anim.SetBool("isAttacking", false);
                    }
                    else
                    {
                        nav.Stop();
                        anim.SetBool("isAttacking", true);
                        anim.SetBool("isWalking", false);
                    }
                }
                else
                {
                    chaseTimer += Time.deltaTime;
                    nav.SetDestination(player.position);
                }
            }
            else
            {
                chaseTimer += Time.deltaTime;
                nav.SetDestination(player.position);
            }


        }
        else
        {
            chaseTimer += Time.deltaTime;

            // If the timer exceeds the wait time...
            if (chaseTimer >= chaseWaitTime)
            {
                nav.Stop();
                //anim.SetBool("isIdle", true);
                anim.SetBool("isWalking", true);
                anim.SetBool("isAttacking", false);
                state = "patrol";
                chaseTimer = 0f;
            }
        }

    }
}
