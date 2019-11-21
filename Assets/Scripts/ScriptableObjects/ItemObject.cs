using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType{
    Food,
    Equipment,
    Default,
    Weapon
}
public abstract class ItemObject : ScriptableObject
{
public GameObject prefeb;
public Sprite image;
public ItemType type;
[TextArea(15,20)]
public string description;
}
