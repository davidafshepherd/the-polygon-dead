using UnityEngine;

public class CharacterStats : MonoBehaviour {
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;

    [SerializeField] protected bool isDead;

    private void Start() {
        InitVariables();
    }

    /*
     * Initialises character's stats
     */
    public virtual void InitVariables()
    {
        maxHealth = 100;
        SetHealthTo(maxHealth);
        isDead = false;
    }

    /*
     * Updates character's health
     */
    public virtual void CheckHealth() {
        //If health is 0 or below, sets character as dead
        if (health <= 0) {
            health = 0;
            Die();
        }

        //If health is max or above, sets character's health to max
        if (health >= maxHealth) {
            health = maxHealth;
        }
    }

    /*
     * Sets character as dead
     */
    public virtual void Die() {
        isDead = true;
    }

    /*
     * Checks if character is dead or not
     */
    public bool IsDead() {
        return isDead;
    }

    /*
     * Changes character's health
     */
    public void SetHealthTo(int healthToSetTo) {
        health = healthToSetTo;
        CheckHealth();
    }

    /*
     * Damages character 
     */
    public virtual void TakeDamage(int damage) {
        int healthAfterDamage = health - damage;
        SetHealthTo(healthAfterDamage);
    }

    /*
     * Heals character
     */
    public void Heal(int heal) {
        int healthAfterHeal = health + heal;
        SetHealthTo(healthAfterHeal);
    }
}
