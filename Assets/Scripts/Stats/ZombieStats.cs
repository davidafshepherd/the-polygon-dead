using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class ZombieStats : CharacterStats {

    private ZombieController zombieController;
    private Animator animator;

    [SerializeField] private int damage;
    [SerializeField] private bool canAttack;

    [SerializeField] public float attackDuration;
    [SerializeField] public float attackTime;


    private void Start() {
        GetReferences();
        InitVariables();
    }

    /*
     * Gets references
     */
    private void GetReferences()
    {
        zombieController = GetComponent<ZombieController>();
        animator = GetComponentInChildren<Animator>();
    }

    /*
     * Initialises zombie's stats
     */
    public override void InitVariables() {
        maxHealth = 25;
        SetHealthTo(maxHealth);
        isDead = false;

        damage = 10;
        attackDuration = 1.8f;
        attackTime = 0.5f;
        canAttack = true;
    }

    /*
     * Deals damage to player's health
     */
    public void DealDamage(CharacterStats statsToDamage) {
        statsToDamage.TakeDamage(damage);
    }

    /*
     * Sets zombie as dead
     */
    public override void Die() {
        base.Die();
        StartCoroutine(HandleDie());
    }

    private IEnumerator HandleDie() {
        zombieController.SetMovement(false);
        zombieController.CancelMovement();
        animator.SetTrigger("Death");
        
        yield return new WaitForSeconds(2.7f);
        Destroy(gameObject);
    }
}
