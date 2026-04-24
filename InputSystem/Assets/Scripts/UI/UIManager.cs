using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Text References")]
    public Text healthText;
    public Text ammoText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHealthUI(float currentHP, float maxHP)
    {
        healthText.text = $"HP: {Mathf.CeilToInt(currentHP)} / {maxHP}";
    }

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        if (maxAmmo == 0) ammoText.text = "Ammo: ∞";
        else ammoText.text = $"Ammo: {currentAmmo} / {maxAmmo}";
    }
    void Start()
    {
        // This clears the "New Text" placeholder immediately
        UpdateHealthUI(100, 100);
        UpdateAmmoUI(0, 0);
    }
}