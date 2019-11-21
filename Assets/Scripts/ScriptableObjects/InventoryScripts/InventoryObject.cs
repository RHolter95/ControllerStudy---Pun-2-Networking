using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]

public class InventoryObject : ScriptableObject
{
    public InventoryObject inventory;
    public GameObject slot = null;
    public Sprite image;
    public Image img;

    public InventorySlot[] Container = new InventorySlot[11];
    //public List<InventorySlot> Container = new List<InventorySlot>(11);

    public void Awake(){

        for (int i = 0; i < Container.Length; i++)
        {
            Container[i] = new InventorySlot(null,0,null);
        }
    }

    public void AddItemToInventoryUI(int i, Sprite image)
    {
        //Find parent "Slot" and get child "Image" container
        slot = GameObject.FindGameObjectWithTag("Slot" + i);
        //slot = slot.transform.GetChild(0).gameObject;

        //Get Image component and replace its Sprite W/ item.image
        img = slot.GetComponent<Image>();
        img.sprite = image;     
    }

     public void SwapInventoryUI(int slotOne , int slotTwo)
    {
        //Debug.Log("Swapping: " + slotOne + " With " + slotTwo);

        //Grab all from slot1
        var itemToSwap =  inventory.Container[slotOne].item;
        Sprite imageToSwap = inventory.Container[slotOne].image;
        int amountToSwap = inventory.Container[slotOne].amount;

        slot = GameObject.FindGameObjectWithTag("Slot" + slotOne);
        if(!slot)
        Debug.Log("Couldn't find slot"+slotOne);

        var img = slot.GetComponent<Image>();
        img.sprite = inventory.Container[slotTwo].image; 

        slot = GameObject.FindGameObjectWithTag("Slot" + slotTwo);
        img = slot.GetComponent<Image>();
        img.sprite = inventory.Container[slotOne].image;       

        //Set slotOne to = slotTwo content
        inventory.Container[slotOne].item = inventory.Container[slotTwo].item;
        inventory.Container[slotOne].amount = inventory.Container[slotTwo].amount;
        inventory.Container[slotOne].image = inventory.Container[slotTwo].image;

        //Set SlotTwo = SlotOne content
        inventory.Container[slotTwo].item = itemToSwap;
        inventory.Container[slotTwo].amount = amountToSwap;
        inventory.Container[slotTwo].image = imageToSwap;
    }

    public void DropInventoryUI(int slotNumber)
    {
        inventory.Container[slotNumber].amount = 0;
        inventory.Container[slotNumber].image = null;
        inventory.Container[slotNumber].item = null;
        
        slot = GameObject.FindGameObjectWithTag("Slot" + slotNumber);
        if(!slot)
        Debug.Log("Couldn't find slot" + slotNumber);

        var img = slot.GetComponent<Image>();
        img.sprite = null; 
    }

    public void AddItem(ItemObject _item, int _amount, Sprite _image)
    {
        bool hasItem = false;
        //If item exists in inventory then increment amount
        for(int i = 0 ; i < Container.Length; i++)
        {
            if(Container[i].item == _item)
            {
                Container[i].AddAmount(_amount);
                hasItem = true;
                break;
            }
        }

        //If it doesnt exist add it & update UI in other func
        if(hasItem == false)
        {
             for (int i = 0; i < Container.Length; i++)
                {
                    if (Container[i].item == null)
                    {
                        Container[i].item = _item;
                        Container[i].amount = _amount;
                        Container[i].image = _image;
                        AddItemToInventoryUI(i,_image);
                        break;
                    }
                }
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    public Sprite image;

    public InventorySlot(ItemObject _item, int _amount, Sprite _image)
    {
        image = _image;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}
