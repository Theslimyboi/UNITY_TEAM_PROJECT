using UnityEngine;
using UnityEngine.InputSystem;

// Attach to the Player GameObject
// Handles weapon switching via keyboard (1/2/3), scroll wheel and firing input
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon List (assign in Inspector)")]
    public WeaponBase[] weapons;

    [Header("Settings")]
    public int startingWeapon = 0;

    private WeaponBase currentWeapon;
    private int currentIndex = 0;

    void Start()
    {
        // Disable all weapons, then activate the starting one
        foreach (var weapon in weapons)
            weapon.gameObject.SetActive(false);

        SwitchToWeapon(startingWeapon);
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Left mouse button — attack
        if (Mouse.current.leftButton.isPressed)
            currentWeapon?.Attack();

        // Release — stop attack
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            currentWeapon?.StopAttack();

        // Number keys 1-3 to switch weapons
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchToWeapon(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchToWeapon(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchToWeapon(2);

        // Scroll wheel to cycle weapons
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) SwitchToWeapon((currentIndex + 1) % weapons.Length);
        if (scroll < 0f) SwitchToWeapon((currentIndex - 1 + weapons.Length) % weapons.Length);

        // R key to reload
        if (Keyboard.current.rKey.wasPressedThisFrame)
            currentWeapon?.GetComponent<AmmoSystem>()?.StartReload();
    }

    // Switches to the weapon at the given index
    public void SwitchToWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        // Deactivate current weapon
        if (currentWeapon != null)
        {
            currentWeapon.StopAttack();
            currentWeapon.gameObject.SetActive(false);
        }

        // Activate new weapon
        currentIndex = index;
        currentWeapon = weapons[currentIndex];
        currentWeapon.gameObject.SetActive(true);

        Debug.Log($"Switched to: {currentWeapon.data.weaponName}");
        OnWeaponChanged(currentWeapon);
    }

    public void SwitchToNextWeapon() => SwitchToWeapon((currentIndex + 1) % weapons.Length);
    public void SwitchToPreviousWeapon() => SwitchToWeapon((currentIndex - 1 + weapons.Length) % weapons.Length);

    // Called when weapon changes — hook up UI updates here
    private void OnWeaponChanged(WeaponBase newWeapon)
    {
        if (newWeapon.ammo != null)
        {
            //UIManager.Instance.UpdateAmmoUI(newWeapon.ammo.GetCurrentAmmo(), newWeapon.data.maxAmmo);
        }
        else
        {
            //UIManager.Instance.UpdateAmmoUI(0, 0); // For melee or infinite
        }
        // Example: UIManager.instance?.UpdateWeaponDisplay(newWeapon.data);
    }

    public WeaponBase GetCurrentWeapon() => currentWeapon;
}