using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : CharacterStats
{

    //Components
    private PlayerHUD HUD;

    private ThirdPersonController thirdPersonController;
    private ThirdPersonShooter thirdPersonShooterController;
    private Animator animator;

    // ui
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject uiHUD;
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject death;

    //Reload length
    [SerializeField] public float reloadDuration;

    // audio
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] damageGrunts;

    // animation
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 0.5f;

    [SerializeField] private GameObject bloodParticleSystem = null;

    private void Start()
    {
        GetReferences();
        InitVariables();
    }

    /*
     * Gets references
     */
    private void GetReferences()
    {
        HUD = GetComponent<PlayerHUD>();
        audioSource = GetComponent<AudioSource>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        thirdPersonShooterController = GetComponent<ThirdPersonShooter>();
        animator = GetComponent<Animator>();
    }

    /*
    * Initialises player's stats
    */
    public override void InitVariables()
    {
        maxHealth = 100;
        SetHealthTo(maxHealth);
        isDead = false;

        reloadDuration = 3.4f;
    }

    /*
     * Checks player's health
     */
    public override void CheckHealth()
    {
        base.CheckHealth();
        HUD.UpdateHealth(health, maxHealth);
    }

    /*
     * Damages character 
     */
    public override void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(damageGrunts[Random.Range(0, damageGrunts.Length - 1)], 0.5f);
        StartCoroutine(SpawnBloodParticles(gameObject.transform.position + new Vector3(0, 0.8f, 0)));
        base.TakeDamage(damage);
    }

    /*
    * Sets player as dead
    */
    public override void Die()
    {
        base.Die();
        StartCoroutine(HandleDie());
    }

    private IEnumerator HandleDie() {
        thirdPersonController.SetMovement(false);
        thirdPersonShooterController.SetControls(false);

        // death animation
        crosshair.SetActive(false);
        uiHUD.SetActive(false);
        tutorial.SetActive(false);
        death.SetActive(true);
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(2f);

        // main menu
        transition.SetTrigger("start");
        yield return new WaitForSeconds(transitionTime);
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }

    private IEnumerator SpawnBloodParticles(Vector3 position) {
        GameObject bloodParticles = Instantiate(bloodParticleSystem, position, new Quaternion(0, 0, 0, 0));
        yield return new WaitForSeconds(2f);
        Destroy(bloodParticles);
    }
}

