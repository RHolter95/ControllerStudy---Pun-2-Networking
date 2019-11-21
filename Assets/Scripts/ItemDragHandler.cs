using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public int slotNumberOne;
    public int slotNumberTwo;
    public InventoryObject inventory;
    public GameObject inventoryGO;
    string[] slotTagArray = new string[10]{"Slot0","Slot1","Slot2","Slot3","Slot4","Slot5","Slot6","Slot7","Slot8","Slot9"};

    public void Awake()
    {
        inventory = gameObject.GetComponentInParent<ItemDropHandler>().inventory;
        inventoryGO = gameObject.GetComponentInParent<ItemDropHandler>().gameObject;
        if(inventoryGO == null){
            Debug.Log("Couldn't Find Parent Inventory GO");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Figure out where were releasing our mouse
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero; 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RectTransform invPanel = inventoryGO.transform as RectTransform;
        //If we release the mouse outside of the panel then the FIRST slot
        //we clicked will be dropped from eventData raycast at OnPointerDown
        if(!RectTransformUtility.RectangleContainsScreenPoint(invPanel,Input.mousePosition)){
            inventory.DropInventoryUI(slotNumberOne);
            return;
        }

        //Figure out where were releasing our mouse
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
 
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        //If we have something we hit
        if(raycastResults.Count > 0)
        {
            //Determine if its a slot0 - Slot9 on SECOND[1] Obj
            for(int i = 0; i < 10 ; i++){
            if(raycastResults[1].gameObject.tag == slotTagArray[i]){
                slotNumberTwo = i;
                break;
                }
            }
        }
        if(slotNumberOne != slotNumberTwo){
            inventory.SwapInventoryUI(slotNumberOne,slotNumberTwo);
        }else{
            return;
        }
    }
}
