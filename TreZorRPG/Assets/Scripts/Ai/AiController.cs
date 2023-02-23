using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Control;
using UnityEngine.AI;

namespace RPG.Player
{
    public class AiController : MonoBehaviour
    {
        [SerializeField] float AggroAreaDistance = 5f;
        public GameObject Target;
        [SerializeField] int ChaseDistance;
        [SerializeField] int DistanceSpawnPointReset = 30;
        bool returningToPoint = false;
        float distanceToPoint;
        bool foundTarget = false;
        AiMover aiMover;
        Health health;
        EnemyTarget enemyTarget;

        //Patroling Behaviour
        public bool ReturnToSpawnPoint = true; // toggle between going to spawn point or going to guard point
        [SerializeField] public PatrolPattern patrolPattern;
        [SerializeField] float CloseToWayPointDistance = 1f;
        public Vector3 nextPosition;
        int currentWayPointIndex = 0;
        
        
        private void Start()
        {
            // Initialisation
            Target = null;
            aiMover = GetComponent<AiMover>();
            health = GetComponent<Health>();
            enemyTarget= GetComponent<EnemyTarget>();   
        }
        private void Update()
        {
            if(!health.IsDead())
            {
                Aggro();
            }else
            {
                
            }
           if (Target != null)
            {
               HealthPlayer targetHealth = Target.GetComponent<HealthPlayer>();
                if (targetHealth.IsDead())
                {
                    ReturnToSpawn(); // If the player is dead, the enemy will return to the spawn point or guard point
                }
            }

        }

        private void Aggro() // Taking in account all the conditions to follow the player
        {
            if(ReturnToSpawnPoint == true) // If the enemy is set to return to the spawn point
            {
                 // Setting returnToPoint = false 
                distanceToPoint = Vector3.Distance(aiMover.spawnPoint.transform.position, this.transform.position);
                if (distanceToPoint < 2)
                {
                    returningToPoint = false;
                }
            }
            else if(ReturnToSpawnPoint == false && patrolPattern == null) // If the enemy is set to return to the guard point and he is patroling   
            {
                distanceToPoint = Vector3.Distance(aiMover.My_Position_Guard, this.transform.position);
                if(distanceToPoint < 2)
                {
                    returningToPoint = false;
                }
            }
            else // If the ennemy is patroling and not returning to the spawn point
            {
                distanceToPoint = Vector3.Distance(nextPosition, this.transform.position);
                if(distanceToPoint < 2)
                {
                    returningToPoint = false;
                }
            }
           

            if (!returningToPoint) // If neither
            {
                if (Target == null)
                {
                    SearchTarget(); // Search for the player
                    if(patrolPattern!=null)
                    {
                        PatrolBehaviour(); // If the enemy is patroling, he will patrol
                    }
                }

                if (foundTarget && Target != null)
                {
                    FollowTheTarget(); // If the player is found, the enemy will follow him
                }
            }
        }


        void SearchTarget() 
        {
            Vector3 center = new Vector3(this.transform.position.x, this.transform.position.y,this.transform.position.z);
            Collider[] hitColliders = Physics.OverlapSphere(center, AggroAreaDistance); // Search for the player in a sphere
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].transform.tag == "Player")
                {
                    Target = hitColliders[i].transform.gameObject;
                    foundTarget = true;

                }
                i++;
            }
        }
        void FollowTheTarget()
        {
            // The enemy is always facing
            Vector3 targetPosition = Target.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);

           

            float distanceToPlayer = Vector3.Distance(Target.transform.position, this.transform.position); // Distance beween target and enemy 
            if (distanceToPlayer < ChaseDistance && distanceToPoint < DistanceSpawnPointReset )
            {

                if (distanceToPlayer<2 && returningToPoint == false)
                {
                    GetComponent<Animator>().SetBool("CanAttack", true);
                    aiMover.WithinRange();
                    //attackFunction
                    enemyTarget.Attack();                    
                }else
                {
                    GetComponent<Animator>().SetBool("CanAttack", false);
                    aiMover.NotWithinRange();
                    aiMover.ChaseTarget();
                    
                }

                
            }else
            {
                ReturnToSpawn();

            }
        }

        private void ReturnToSpawn()
        {
            // If the player is dead, the enemy will return to the spawn point or guard point
            GetComponent<Animator>().SetBool("CanAttack", false);
            returningToPoint = true;
            foundTarget = false;
            Target = null;
            aiMover.StopChaseTarget();
        }
        private void OnDrawGizmos() // Draw the sphere of the aggro area
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AggroAreaDistance);
        }
        
        private void PatrolBehaviour() // Patroling Behaviour
        {
            nextPosition = aiMover.My_Position_Guard;
            if(patrolPattern != null)
            {
                if (AnyWayPoint()) // If the enemy is close to a waypoint, he will go to the next one
                {
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
            }
            GetComponent<NavMeshAgent>().destination = nextPosition;
        }
        private void CycleWayPoint()
        {
            currentWayPointIndex = patrolPattern.GetNextIndex(currentWayPointIndex);
        }
        private bool AnyWayPoint()
        {
            float distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWayPoint< CloseToWayPointDistance;
        }
        private Vector3 GetCurrentWayPoint()
        {
            return patrolPattern.GetPosWayPoint(currentWayPointIndex);
        }
    }
}
