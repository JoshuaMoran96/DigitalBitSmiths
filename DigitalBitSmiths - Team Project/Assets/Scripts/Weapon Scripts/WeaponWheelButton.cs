using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelButton : MonoBehaviour
{
    [SerializeField] WeaponData weapon;
    [SerializeField] Button button;
    [SerializeField] Image icon;

    void Update()
    {
        if (WeaponInventory.instance == null)
        {
            return;
        }

        bool owned = WeaponInventory.instance.HasWeapon(weapon);
        button.interactable = owned;

        if (icon != null)
        {
            icon.color = owned ? Color.white : Color.gray;
        }
    }
}