using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Text References")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHealthUI(float currentHP, float maxHP)
    {
        healthText.text = $"HP: {Mathf.CeilToInt(currentHP)} / {maxHP}";
    }

    public void UpdateAmmoUI(int currentAmmo, int totalAmmo)
    {
        if (totalAmmo <= 0 && currentAmmo <= 0)
            ammoText.text = "Ammo: 0 / 0"; // viskas baigta
        else
            ammoText.text = $"Ammo: {currentAmmo} / {totalAmmo}";
    }
    void Start()
    {
        // This clears the "New Text" placeholder immediately
        UpdateHealthUI(100, 100);
        UpdateAmmoUI(0, 0);
    }
}