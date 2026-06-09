using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] WeaponData weapon;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (WeaponInventory.instance != null)
        {
            WeaponInventory.instance.AddWeapon(weapon);
            WeaponInventory.instance.EquipWeapon(weapon);
        }

        Destroy(gameObject);
    }
}
