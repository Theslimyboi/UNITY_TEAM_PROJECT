using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public Weapon weapon; // nuoroda į ginklą

    void Update()
    {
        ammoText.text = weapon.currentAmmo + " / " + weapon.maxAmmo;
    }
}

