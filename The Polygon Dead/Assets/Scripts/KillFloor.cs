using StarterAssets;
using UnityEngine;

public class KillFloor : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void OnTriggerEnter(Collider other) {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (other.transform.GetComponent<ThirdPersonController>() != null) { playerStats.TakeDamage(100);}
    }
}
