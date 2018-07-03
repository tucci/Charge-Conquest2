using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoliderAnimScript : MonoBehaviour
{
    
    private bool isNpc;

    private Animator anim;
    private NavMeshAgent agent;
    


    private int isRunningHash = Animator.StringToHash("isRunning");
    private int forwardHash = Animator.StringToHash("forward");

    private int isStrafingHash = Animator.StringToHash("isStrafing");
    private int strafeHash = Animator.StringToHash("strafe");

    private int shootHash = Animator.StringToHash("shoot_trigger");

    public void setNpc(NavMeshAgent navAgent)
    {
        isNpc = true;
        agent = navAgent;
    }

    // Use this for initialization
    void Start ()
	{
	    anim = GetComponent<Animator>();
	    
	}
	
	// Update is called once per frame
	void Update ()
	{

	    

	    float moveV;
	    float moveH;

        if (isNpc)
        {
            moveV = agent.velocity.x * 0.1f;
            moveH = agent.velocity.z * 0.1f;
        }
	    else
	    {
            moveV = Input.GetAxis("Vertical");
            moveH = Input.GetAxis("Horizontal");
        }
	    

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
            anim.SetTrigger(shootHash);
	    }


	    
        anim.SetBool(isRunningHash, Mathf.Abs(moveH) > 0.1);
        anim.SetFloat(forwardHash, moveH);

        anim.SetBool(isStrafingHash, Mathf.Abs(moveV) > 0.1);
        anim.SetFloat(strafeHash, moveV);

	    

    }
}
