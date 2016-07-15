using UnityEngine;
using System.Collections;

public class chase : MonoBehaviour {
    public Transform player;
    public Transform head;
    Animator anim;
    bool pursuing = false;
    public float viewAngle = 30f;
    public double stoppingDistance = 1.5f;
    public float viewDistance = 10f;
    private NavMeshAgent nav;                               // Reference to the nav mesh agent.
    private GameObject playerObject;                      // Reference to the player.
    private float chaseTimer = 0f;
    public float chaseWaitTime = 20f;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update () {
        Vector3 direction = player.position - this.transform.position;
        direction.y = 0;
        float angle = Vector3.Angle(direction, head.up);
        RaycastHit hit;
        if (Vector3.Distance(player.position, this.transform.position) < viewDistance && (angle < viewAngle || pursuing) && chaseTimer < chaseWaitTime)
        {
            

            // ... and if a raycast towards the player hits something...
            if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, viewDistance))
            {
                // ... and if the raycast hits the player...
                if (hit.collider.gameObject == playerObject)
                {
                    pursuing = true;

                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                Quaternion.LookRotation(direction), 0.1f);

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
						chaseTimer = 0f;
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
                anim.SetBool("isIdle", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("isAttacking", false);
                pursuing = false;
                chaseTimer = 0f;
            }
        }

    }
}
