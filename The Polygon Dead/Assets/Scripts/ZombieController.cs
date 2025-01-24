using System.Linq;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour {

    [SerializeField] private float stoppingDistance;
    private float timeOfLastAttack = 0;
    private bool hasDealtDamage = false;

    private NavMeshAgent agent = null;
    private Animator animator = null;
    private ZombieStats stats = null;
    private Transform player;

    // audio
    [SerializeField] private AudioClip[] idleSounds;
    [SerializeField] private AudioClip[] attackSounds;
    private AudioSource audioSource;
    private float lastIdle;
    private int idleLoop;

    private bool movement = true;

    private void Start() {
        GetReferences();

        // initialise idle sounds
        idleLoop = Random.Range(5, 15);
        lastIdle = Time.time - idleLoop + Random.Range(0, 5);
    }

    /*
     * Gets references
     */
    private void GetReferences() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<ZombieStats>();
        audioSource = GetComponent<AudioSource>();
        player = ThirdPersonController.instance;
    }

    private void Update() {
        if (movement) { HandleBehaviour();}
    }

    /*
     * Handles zombie movement and attack
     */
    private void HandleBehaviour() {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        //Checks if zombie is far from player and is not in middle of attack
        if (agent.stoppingDistance < distanceToPlayer && (Time.time < stats.attackDuration || timeOfLastAttack + stats.attackDuration <= Time.time)) {
            //Animates zombie moving
            float normalizedVelocity = Mathf.Clamp(agent.velocity.magnitude / 4f, 0, 1);
            animator.SetFloat("Speed", normalizedVelocity);

            //Moves zombie towards player
            agent.SetDestination(player.position);
        } 
        //Checks if zombie is close to player
        else if (distanceToPlayer <= agent.stoppingDistance) {
            //Animates zombie idle
            float normalizedVelocity = Mathf.Clamp(agent.velocity.magnitude / 4f, 0, 1);
            animator.SetFloat("Speed", normalizedVelocity);

            //Checks if zombie is not in middle of attack
            if (timeOfLastAttack + stats.attackDuration <= Time.time) {
                //Starts attack
                audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length - 1)], 0.15f);
                hasDealtDamage = false;
                timeOfLastAttack = Time.time;
                animator.SetTrigger("Attack");
            }
            //Checks if zombie is in middle of attack and hasn't dealt damage
            else if ((timeOfLastAttack + stats.attackTime <= Time.time) && (Time.time < timeOfLastAttack + stats.attackTime + 0.1f) && !hasDealtDamage) {
                //Deals damage to player
                CharacterStats playerStats = player.GetComponent<CharacterStats>();
                AttackPlayer(playerStats);
                hasDealtDamage = true;
            }
            RotateToPlayer();
        }

        // Handle idle sounds
        if ( lastIdle + idleLoop <= Time.time ) {
            lastIdle = Time.time;
            audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Length - 1)], 0.15f);
        }
    }

    /*
     * Rotates zombie towards player 
     */
    private void RotateToPlayer() {
        Vector3 direction = player.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rotation; 
    }

    /*
     * Deals damage to player's health
     */ 
    private void AttackPlayer(CharacterStats statsToDamage) {
        stats.DealDamage(statsToDamage);
    }

    public void SetMovement(bool newMovement) {
        movement = newMovement;
    }

    public void CancelMovement() {
        agent.ResetPath();
    }

}
