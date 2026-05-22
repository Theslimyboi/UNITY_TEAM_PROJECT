using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int maxAmmo = 100;
    public int currentAmmo = 30;

    public void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            // čia kulkos šaudymo logika
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
    }
}
