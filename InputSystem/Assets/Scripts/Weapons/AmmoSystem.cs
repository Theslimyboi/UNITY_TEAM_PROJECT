using UnityEngine;
using System.Collections;

public class AmmoSystem : MonoBehaviour
{
    private WeaponData data;
    private int currentAmmo;
    private int totalAmmo;
    private bool isReloading = false;

    void Awake()
    {
        data = GetComponent<WeaponBase>()?.data;
        if (data != null)
        {
            currentAmmo = data.maxAmmo;
            totalAmmo = data.totalAmmo;
        }
    }

    public bool HasAmmo()
    {
        if (data == null) return false;
        if (data.maxAmmo == 0 && data.totalAmmo == 0) return false; // nėra ammo
        return currentAmmo > 0 && !isReloading;
    }

    public void UseAmmo(int pelletCount = 4)
    {
        if (data == null) return;

        if (data.weaponType == WeaponType.Melee)
            return;

        bool isShotgun = GetComponent<ShotgunWeapon>() != null;

        int amount = isShotgun
            ? pelletCount
            : 1;

        currentAmmo = Mathf.Max(0, currentAmmo - amount);

        UIManager.Instance?.UpdateAmmoUI(currentAmmo, totalAmmo);

        if (currentAmmo <= 0)
            StartReload();
    }



    public void StartReload()
    {
        if (!isReloading && data != null && totalAmmo > 0 && currentAmmo < data.maxAmmo)
            StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(data.reloadTime);

        int neededAmmo = data.maxAmmo - currentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, totalAmmo);
        currentAmmo += ammoToLoad;
        totalAmmo -= ammoToLoad;

        isReloading = false;
        UIManager.Instance?.UpdateAmmoUI(currentAmmo, totalAmmo);
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => totalAmmo;
    public bool IsReloading() => isReloading;
}