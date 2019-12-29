using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class WeaponFXPooling : MonoBehaviour
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private GameObject _sniperFX;

    public GameObject SniperFX { get { return _sniperFX; } set { _sniperFX = value; } }
    public string Name { get { return _name; } set { _name = value; } }

    public WeaponFXPooling(GameObject SniperFX, string Name)
    {
        _sniperFX = SniperFX;
        _name = Name;
    }
}
