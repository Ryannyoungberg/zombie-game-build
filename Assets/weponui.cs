using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI currentWeaponText;
    public WeaponSystem weaponSystem;

    private float fpsUpdateInterval = 0.5f;
    private float fpsAccumulator = 0f;
    private int fpsFrameCount = 0;
    private float fpsTimeLeft;

    void Start()
    {

        UpdateInstructions();
        fpsTimeLeft = fpsUpdateInterval;
    }

    void Update()
    {
        UpdateAmmoCount();
        UpdateFPSCounter();
        UpdateCurrentWeapon();
    }

    void UpdateAmmoCount()
    {
        if (weaponSystem != null && ammoText != null)
        {
            ammoText.text = $"Ammo: {weaponSystem.GetCurrentAmmo()} / {weaponSystem.GetMaxAmmo()}";
        }
    }

    void UpdateInstructions()
    {
        if (instructionsText != null)
        {
            instructionsText.text = "Controls:\n" +
                                    "WASD - Move\n" +
                                    "Space - Jump\n" +
                                    "Left Click - Shoot\n" +
                                    "Right Click - Zoom\n" +
                                    "R - Reload\n" +
                                    "Q - Switch Weapon\n" +
                                    "1-9 - Select Weapon";
        }
    }

    void UpdateFPSCounter()
    {
        fpsTimeLeft -= Time.deltaTime;
        fpsAccumulator += Time.timeScale / Time.deltaTime;
        fpsFrameCount++;

        if (fpsTimeLeft <= 0f)
        {
            if (fpsText != null)
            {
                float fps = fpsAccumulator / fpsFrameCount;
                fpsText.text = $"FPS: {Mathf.Round(fps)}";
            }

            fpsTimeLeft = fpsUpdateInterval;
            fpsAccumulator = 0f;
            fpsFrameCount = 0;
        }
    }

    void UpdateCurrentWeapon()
    {
        if (weaponSystem != null && currentWeaponText != null)
        {
            currentWeaponText.text = $"Current Weapon: {weaponSystem.GetCurrentWeaponName()}";
        }
    }
}