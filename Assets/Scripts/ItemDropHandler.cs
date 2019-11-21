using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;




public class ItemDropHandler : MonoBehaviour, IDropHandler,IPointerDownHandler
{
    public InventoryObject inventory;
    public GameObject inventoryObj = null;

    //Assign Slot GO's to array to obtain their transforms
    public void Awake()
    {
        inventory = gameObject.GetComponentInParent<ItemDropHandler>().inventory;

        //Grab Inventory GameObject & Debug.
        inventoryObj = GameObject.Find("Inventory");
        if(inventoryObj == null){
               Debug.Log("Couldn't locate Inventory UI");
        }

        /*
        //Assign Slot0 - Slot9 to Slot Array
        for(int i = 0 ; i < 10 ; i++)
        {
            //Grab Inventory UI & work down each Slot0 - Slot9
            go = inventoryObj.transform.GetChild(i).gameObject;
            //Working down a child of each Slot
            go = go.transform.GetChild(0).gameObject;
            //If we got the Slot i child
            if(go.name == "ItemImage"){
                slotArray[i] = go;
            }else{
            Debug.Log("Couldn't locate slot: " + i);
            } 
        }
        */
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /*
        Debug.Log("InsidePointerDown");

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
 
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        //If we have something we hit
        if(raycastResults.Count > 0)
        {
            //Determine if its a slot0 - Slot9 on FIRST[0] Obj
            for(int i = 0; i < 10 ; i++){
            if(raycastResults[0].gameObject.tag == slotTagArray[i]){
                slotNumberOne = i;
                break;
                }
            }
        }else{
            return;
        }
        */
    }

    public void OnDrop(PointerEventData eventData)
    {
        /*
        //Get this GameObjects transform as RectTransform invPanel
        RectTransform invPanel = transform as RectTransform;
        if(!RectTransformUtility.RectangleContainsScreenPoint(invPanel,Input.mousePosition)){
            inventory.DropInventoryUI(1);
        }
        */
    }
    
}
