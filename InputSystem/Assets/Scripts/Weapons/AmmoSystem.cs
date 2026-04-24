using UnityEngine;
using System.Collections;

// Attach to each weapon GameObject
// Handles ammo count, consumption and reloading
public class AmmoSystem : MonoBehaviour
{
    private WeaponData data;
    private int currentAmmo;
    private bool isReloading = false;

    void Awake()
    {
        data = GetComponent<WeaponBase>()?.data;
        if (data != null)
            currentAmmo = data.maxAmmo;
    }

    // Returns true if the weapon has ammo available
    public bool HasAmmo()
    {
        if (data == null || data.maxAmmo == 0) return true; // 0 = infinite ammo
        return currentAmmo > 0 && !isReloading;
    }

    // Consumes one ammo and triggers auto-reload if empty
    public void UseAmmo()
    {
        if (data == null || data.maxAmmo == 0) return;
        currentAmmo = Mathf.Max(0, currentAmmo - 1);

        // Updating UI
        UIManager.Instance.UpdateAmmoUI(currentAmmo, data.maxAmmo);

        if (currentAmmo <= 0)
            StartReload();
    }

    // Starts the reload coroutine if conditions are met
    public void StartReload()
    {
        if (!isReloading && data != null && currentAmmo < data.maxAmmo)
            StartCoroutine(ReloadCoroutine());
    }

    // Waits for reload duration, then restores full ammo
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log($"Reloading... ({data.reloadTime}s)");
        yield return new WaitForSeconds(data.reloadTime);
        currentAmmo = data.maxAmmo;
        isReloading = false;

        // Updating UI
        UIManager.Instance.UpdateAmmoUI(currentAmmo, data.maxAmmo);

        Debug.Log("Reload complete!");
    }

    // Getters for UI use
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => data != null ? data.maxAmmo : 0;
    public bool IsReloading() => isReloading;
}