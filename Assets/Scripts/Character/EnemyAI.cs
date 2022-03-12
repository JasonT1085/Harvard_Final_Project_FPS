using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();       
        anim = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if(distance > 1.5)
        {
            agent.updatePosition = true;

            agent.SetDestination(target.position);
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);
        }
        else
        {
            agent.updatePosition = false;
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", true);  
        }
    }
}
