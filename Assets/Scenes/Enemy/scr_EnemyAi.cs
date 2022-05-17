
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class scr_EnemyAi : scr_EnemyBase
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public Transform attackPoint;


    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, playerInLineOfSight;
    public bool attacksAreBullets;
    public int meeleeAttackDamage;
    public Vector3 meeleeAttackDimensions;

    private Quaternion lookRotation;
    private Vector3 direction;

    public float rotationSpeed=1;
    public int score = 0;
    public Text scoreboard;

    public scr_HordeController hordeController;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInLineOfSight = CheckLineOfSight();


        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //Check if point not above cliff
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        //find the vector pointing from our position to the target
        direction = (player.position - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        lookRotation = Quaternion.LookRotation(direction);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        if (attacksAreBullets)
        {
            if (!alreadyAttacked)
            {
                ///Attack code here
                Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
                ///End of attack code

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
        else
        {
            if (!alreadyAttacked)
            {
                ///Attack code here
                Collider[] enemies = Physics.OverlapBox(attackPoint.position, meeleeAttackDimensions, transform.rotation);
                foreach(Collider enemy in enemies)
                {
                    if (enemy.CompareTag("Player"))
                    {
                        enemy.GetComponent<scr_CharacterController>().TakeDamage(meeleeAttackDamage, scr_Models.DamageType.Electric);
                        Debug.Log(enemy);
                    }
                }
                ///End of attack code

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;
        src_ScoreScript.scoreValue += (int)damage;

        if (health <= 0)
        {
            agent.isStopped=true;
            Invoke(nameof(DestroyEnemy), 0.1f);
        }
    }
    private void DestroyEnemy()
    {
        if (hordeController != null) hordeController.EnemyDied();

        Destroy(gameObject);
    }

    private bool CheckLineOfSight()
    {
        RaycastHit hit;
        Vector3 direection = transform.position - player.position;
        if (Physics.Raycast(transform.position, direction, out hit)){
            Debug.DrawRay(transform.position, direction * hit.distance, Color.yellow);
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public void SetWalkpoint(Vector3 walk)
    {
        walkPoint = walk;
        walkPointSet = true;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackPoint.position, meeleeAttackDimensions);
    }
}
