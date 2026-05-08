using UnityEngine;
using System.Collections;

public class AmmoSystem : MonoBehaviour
{
    private WeaponData data;
    private int currentAmmo;
    private bool isReloading = false;

    void Awake()
    {
        data = GetComponent<WeaponBase>()?.data;
        if (data != null) currentAmmo = data.maxAmmo;
    }

    public bool HasAmmo()
    {
        if (data == null || data.maxAmmo == 0) return true;
        return currentAmmo > 0 && !isReloading;
    }

    public void UseAmmo()
    {
        if (data == null || data.maxAmmo == 0) return;
        currentAmmo = Mathf.Max(0, currentAmmo - 1);
        UIManager.Instance?.UpdateAmmoUI(currentAmmo, data.maxAmmo); // null-safe
        if (currentAmmo <= 0) StartReload();
    }

    public void StartReload()
    {
        if (!isReloading && data != null && currentAmmo < data.maxAmmo)
            StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        // Tell the weapon base we are reloading so CanAttack() blocks firing
        WeaponBase wb = GetComponent<WeaponBase>();
        if (wb != null) wb.SendMessage("OnReloadStart", SendMessageOptions.DontRequireReceiver);

        yield return new WaitForSeconds(data.reloadTime);

        currentAmmo = data.maxAmmo;
        isReloading = false;
        UIManager.Instance?.UpdateAmmoUI(currentAmmo, data.maxAmmo); // null-safe
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => data != null ? data.maxAmmo : 0;
    public bool IsReloading() => isReloading;
}