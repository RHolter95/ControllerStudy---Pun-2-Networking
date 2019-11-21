using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Weapon Object", menuName = "Inventory System/Item/Weapon")]

public class WeaponObject : ItemObject
{
    public int weaponDamage;
    public void Awake()
    {
        type = ItemType.Weapon;
    }
}
