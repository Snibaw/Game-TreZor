using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Player;

public class AiMover : MonoBehaviour
{
    AiController AiControl;  
    public Transform spawnPoint;
    // Update is called once per frame
    
    public Vector3 My_Position_Guard;

    private void Start()
    {
          AiControl = GetComponent<AiController>(); 
          My_Position_Guard = transform.position;
    }
    void Update()
    {
        

        UpdateAnim();
        
    }
    void UpdateAnim()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
    }
    public void ChaseTarget()
    {
        Vector3 Target = AiControl.Target.transform.position;
        GetComponent<NavMeshAgent>().destination = Target;
    }
    public void StopChaseTarget()
    {
        GetComponent<NavMeshAgent>().isStopped = true;
        GetComponent<NavMeshAgent>().isStopped = false;
        
        if(AiControl.ReturnToSpawnPoint == true)
        {
            GetComponent<NavMeshAgent>().destination = My_Position_Guard;
        }
        else if (AiControl.ReturnToSpawnPoint == false && AiControl.patrolPattern == null)
        {
            GetComponent<NavMeshAgent>().destination = spawnPoint.position;
        }
        else if (AiControl.ReturnToSpawnPoint == false && AiControl.patrolPattern != null)
        {
            GetComponent<NavMeshAgent>().destination = AiControl.nextPosition; // Later we gonna chance it to the place it got pulled
        }
        
    }
    public void WithinRange()
    {
        GetComponent<NavMeshAgent>().isStopped = true;
    }
    public void NotWithinRange()
    {
        GetComponent<NavMeshAgent>().isStopped = false;
    }

   
}
