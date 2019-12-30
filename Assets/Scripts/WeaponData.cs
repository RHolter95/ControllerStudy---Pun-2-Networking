using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public float fireRate = 0.1f;
    public float damage = 50f;
    public string weaponType = "";

    // Update is called once per frame
    void Update()
    {
        if (weaponType == "")
        {
            weaponType = gameObject.transform.tag;
        }
    }
}
