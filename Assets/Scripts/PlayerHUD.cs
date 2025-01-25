using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text ammoCount;

    /*
     * Updates player's health stats on HUD
     */
    public void UpdateHealth(int currentHealth, int maxHealth) {
        float ratio = (float) currentHealth / maxHealth;
        healthBar.fillAmount = math.clamp(ratio, 0, 1);
        
        byte r = Convert.ToByte(176 - math.floor(176 * ratio));
        byte g = Convert.ToByte(33 + math.floor(87 * ratio));
        byte b = Convert.ToByte(math.floor(54 * ratio));
        healthBar.color = new Color32(r, g, b, 255);
        
        new Color32(0, 120, 54, 255);
        new Color32(176, 33, 0, 255);
    }

    /*
     * Updates player's stamina stats on HUD
     */
    public void UpdateStamina(int currentStamina, int maxStamina) {
        float ratio = (float) currentStamina / maxStamina;
        staminaBar.fillAmount = math.clamp(ratio, 0, 1);
    }

    /*
     * Updates player's ammo count
     */
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        ammoCount.text = currentAmmo + "/" + maxAmmo;
    }
}
