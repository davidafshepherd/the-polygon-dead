using StarterAssets;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {

    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    private Rigidbody bulletRigidBody;

    private void Awake() {
        GetReferences();
    }

    /*
     * Gets references
     */
    private void GetReferences()
    {
        bulletRigidBody = GetComponent<Rigidbody>();
    }

    /*
     * Sets velocity of bullet
     */
    private void Start() {
        float speed = 40f;
        bulletRigidBody.linearVelocity = transform.forward * speed;
    }

    /*
     * Handles bullet hitting something
     */ 
    private void OnTriggerEnter(Collider other) {
        //Checks if bullet has hit a zombie
        if (other.GetComponent<BulletTarget>() != null) {
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);

            CharacterStats zombieStats = other.transform.GetComponent<CharacterStats>();
            zombieStats.TakeDamage(5);

        } else {
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
