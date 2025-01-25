
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using TMPro;

public class ThirdPersonShooter : MonoBehaviour
{

    //Components
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private PlayerStats stats;
    private PlayerHUD HUD;

    //Weapon
    [SerializeField] private Transform weapon;
    private Vector3 normalWeaponRotation = new Vector3(0, 90, 270);
    private Vector3 reloadWeaponRotation = new Vector3(-15, 60, 270);

    //Sensitivity and speed settings
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private float normalSpeedPercentage;
    [SerializeField] private float aimSpeedPercentage;

    //Aim camera & controls
    [SerializeField] private CinemachineVirtualCamera playerAimCamera;
    private bool controls = true;

    //Shooting
    [SerializeField] private Transform bullet;
    [SerializeField] private Transform shootFromPoint;
    [SerializeField] private Transform shootToPoint;

    //Aiming
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Rig aimRig;
    private float aimRigWeight;

    //Firing speed
    [SerializeField] private float BULLETS_PER_SECOND = 5;
    private float lastBulletTime;

    //Audio
    [SerializeField] private AudioClip fireSFX;
    [SerializeField] private AudioClip reloadSFX;
    [SerializeField] private AudioClip emptyMagazineSFX;
    private AudioSource audioSource;

    //Ammo usage
    [SerializeField] private int magazineSize;
    [SerializeField] private int currentAmmo;
    [SerializeField] int currentAmmoStored;
    [SerializeField] bool magazineIsEmpty;
    [SerializeField] bool canShoot;

    //Reloading
    private float timeOfLastReload = 0;
    private bool isReloading = false;
    private bool hasReloaded = false;
    [SerializeField] private Transform magazine;
    [SerializeField] private Transform leftHand;
    GameObject magazineHand;
    private float timeOfLastShotAttempt = 0;


    private void Awake()
    {
        GetReferences();
        lastBulletTime = Time.time;
    }

    /*
     * Gets references
     */
    private void GetReferences()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        audioSource = GetComponent<AudioSource>();
        HUD = GetComponent<PlayerHUD>();
    }

    private void Update()
    {
        if (controls) {
            HandleMouseWorldPosition();
            HandleAiming();
            HandleShooting();
            HandleReloading();

            aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);
        }
        HandleNormal();
    }

    /*
     * Updates position of ShootToPoint transform with position of mouse
     * Note that the ShootToPoint transform is used for rigging
     * While the mouse is used for aiming
     */
    private void HandleMouseWorldPosition()
    {
        shootToPoint.position = GetMouseWorldPosition();
    }

    /*
     * Gets position of mouse
     */
    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenCentrePoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCentrePoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, aimColliderLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /*
     * Handles aiming
     */
    private void HandleAiming()
    {
        //Checks if player is aiming
        if (starterAssetsInputs.aim && !isReloading)
        {
            //Turns on zoom camera, decreases sensitivity, decreases speed, turns off player rotation in ThirdPersonController script, turns on aim rig
            //Note that we turn off player rotation in ThirdPersonController when aiming, as it interferes with the aim-based player rotation below
            playerAimCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetSpeedMultiplier(aimSpeedPercentage);
            thirdPersonController.SetRotateOnMove(false);
            aimRigWeight = 1f;

            //Turns aim animator layer on and reload animator layer off
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));

            //Gets aim direction
            Vector3 aimPosition = GetMouseWorldPosition();
            aimPosition.y = transform.position.y;
            Vector3 aimDirection = (aimPosition - transform.position).normalized;

            //Rotates player to aim direction
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
    }

    /*
    * Handles shooting
    */
    private void HandleShooting()
    {
        if (starterAssetsInputs.aim && starterAssetsInputs.shoot && !canShoot && (timeOfLastShotAttempt + 0.3f <= Time.time)) {
            audioSource.PlayOneShot(emptyMagazineSFX, 1f);
            timeOfLastShotAttempt = Time.time;
        }

        //Checks if player is aiming and shooting
        if (starterAssetsInputs.aim && starterAssetsInputs.shoot && canShoot && !isReloading && (lastBulletTime + (1f / BULLETS_PER_SECOND) <= Time.time))
        {
            //Stores time of shot
            lastBulletTime = Time.time;

            //Plays fire SFX
            audioSource.PlayOneShot(fireSFX, 0.3f);

            //Spawns a bullet
            Vector3 aimPosition = GetMouseWorldPosition();
            Vector3 aimDirection = (aimPosition - shootFromPoint.position).normalized;
            Transform bulletTransform = Instantiate(bullet, shootFromPoint.position, Quaternion.LookRotation(aimDirection, Vector3.up));

            //Removes 1 bullet from player's current ammo
            UseAmmo(1, 0);
        }
    }

    /*
     * Updates current ammo and current ammo stored of player
     */
    private void UseAmmo(int AmmoUsed, int AmmoStoredUsed)
    {
        //Checks if player has no ammo
        if (currentAmmo <= 0)
        {
            //Sets magazine to empty and disables player's ability to shoot
            magazineIsEmpty = true;
            UpdateCanShoot();
        }
        else
        {
            //Removes the ammo used and ammo stored used from the current ammo and the current ammo stored
            currentAmmo -= AmmoUsed;
            currentAmmoStored -= AmmoStoredUsed;
        }
        HUD.UpdateAmmo(currentAmmo, magazineSize);
    }

    /*
     * Updates player's ability to shoot 
     */
    private void UpdateCanShoot()
    {
        //Checks if the magazine is empty
        if (magazineIsEmpty) { canShoot = false; }
        else { canShoot = true; }
    }

    /*
     * Handles reloading
     */
    private void HandleReloading() {
        int ammoToReload = magazineSize - currentAmmo;

        //Checks if player wants to reload and is not already reloading
        if (starterAssetsInputs.reload && ammoToReload != 0 && !isReloading)
        {
            //Stops aiming, if player is aiming
            if (starterAssetsInputs.aim) { StopAiming(); }

            //Starts reloading
            isReloading = true;
            weapon.localEulerAngles = reloadWeaponRotation;

            //Sets time of reload and starts reload animation
            timeOfLastReload = Time.time;
            audioSource.PlayOneShot(reloadSFX, 1f);
            animator.SetTrigger("Reload");
            starterAssetsInputs.reload = false;
        }

        //Checks if player is currently reloading
        if (isReloading)
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1f, Time.deltaTime * 10f));
        }

        //Checks if the reload animation has ended
        if (isReloading && timeOfLastReload + stats.reloadDuration <= Time.time)
        {
            isReloading = false;
            weapon.localEulerAngles = normalWeaponRotation;

            hasReloaded = true;
        }

        //Updates player's current ammo and current ammo stored
        if (hasReloaded)
        {
            if (ammoToReload <= currentAmmoStored)
            {
                currentAmmo += ammoToReload;
                currentAmmoStored -= ammoToReload;

                HUD.UpdateAmmo(currentAmmo, magazineSize);

                magazineIsEmpty = false;
                UpdateCanShoot();

                weapon.localEulerAngles = normalWeaponRotation;
                hasReloaded = false;
            }
        }
    }

    /*
     * Stops reloading
     */
    private void StopReloading()
    {
        isReloading = false;
        weapon.localEulerAngles = normalWeaponRotation;
    }

    /*
     * Stops aiming
     */
    private void StopAiming()
    {
        starterAssetsInputs.aim = false;
        playerAimCamera.gameObject.SetActive(false);
        thirdPersonController.SetSensitivity(normalSensitivity);
        thirdPersonController.SetSpeedMultiplier(normalSpeedPercentage);
        thirdPersonController.SetRotateOnMove(true);
        aimRigWeight = 0f;
    }
    private void HandleNormal()
    {
        if (!starterAssetsInputs.aim && !isReloading)
        {
            //Sets settings to normal
            StopAiming();
            StopReloading();

            //Turns off aim and reload animator layers 
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));
        }
    }

    private void HandleDetachMagazine()
    {
        magazineHand = Instantiate(magazine.gameObject, leftHand, true);
        magazine.gameObject.SetActive(false);
    }

    private void HandleDropMagazine()
    {
        GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);

    }

    private void HandleRefillMagazine()
    {
        magazineHand.SetActive(true);
    }

    private void HandleAttachMagazine()
    {
        magazine.gameObject.SetActive(true);
        Destroy(magazineHand);
    }

    public void SetControls(bool newControls)
    {
        controls = newControls;
    }
}
