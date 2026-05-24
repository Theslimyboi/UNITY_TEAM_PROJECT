using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon List (assign in Inspector)")]
    public WeaponBase[] weapons;

    [Header("Settings")]
    public int startingWeapon = 0;

    private WeaponBase currentWeapon;
    private int currentIndex = 0;

    // Scroll debounce — prevents a single scroll tick switching multiple weapons
    private float lastScrollTime = -1f;
    private const float ScrollCooldown = 0.12f;

    void Start()
    {
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
        if (Mouse.current.leftButton.isPressed) currentWeapon?.Attack();
        if (Mouse.current.leftButton.wasReleasedThisFrame) currentWeapon?.StopAttack();

        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchToWeapon(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchToWeapon(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchToWeapon(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchToWeapon(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SwitchToWeapon(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SwitchToWeapon(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SwitchToWeapon(6);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SwitchToWeapon(7);
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SwitchToWeapon(8);
        if (Keyboard.current.digit0Key.wasPressedThisFrame) SwitchToWeapon(9);

        // Debounced scroll — was firing 3-5 times per scroll tick before
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0f && Time.time - lastScrollTime > ScrollCooldown)
        {
            lastScrollTime = Time.time;
            if (scroll > 0f) SwitchToWeapon((currentIndex + 1) % weapons.Length);
            else SwitchToWeapon((currentIndex - 1 + weapons.Length) % weapons.Length);
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
            currentWeapon?.ammo?.StartReload();
    }

    public void SwitchToWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        if (currentWeapon != null)
        {
            currentWeapon.StopAllCoroutines();
            currentWeapon.StopAttack();
            currentWeapon.ammo?.CancelReload(); // ← fixes stuck reload on weapon swap
            currentWeapon.gameObject.SetActive(false);
        }

        currentIndex = index;
        currentWeapon = weapons[currentIndex];
        currentWeapon.gameObject.SetActive(true);

        UIManager.Instance?.UpdateAmmoUI(
            currentWeapon.ammo?.GetCurrentAmmo() ?? 0,
            currentWeapon.ammo?.GetMaxAmmo() ?? 0);
    }

    public WeaponBase GetCurrentWeapon() => currentWeapon;
}