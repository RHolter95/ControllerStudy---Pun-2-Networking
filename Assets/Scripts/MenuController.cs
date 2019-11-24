using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public static MenuController MC;
    public GameObject shopCanvas = null;
    public GameObject[] buttonLocks;
    public Button[] unlockedButtons;

    private void OnEnable()
    {
        if(MenuController.MC == null)
        {
            MenuController.MC = this;
        }
        else
        {
            if(MenuController.MC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Awake()
    {
        shopCanvas = GameObject.Find("ShopCanvas");
        if(shopCanvas == null){
            Debug.Log("Couldn't Find Shop Canvas");}

    }

    private void Start()
    {
        SetUpStore();
    }

    // Start is called before the first frame update
    public void SetUpStore()
    {
        for(int i = 0; i < PersistantData.PD.allSkins.Length; i++)
        {
         buttonLocks[i].SetActive(!PersistantData.PD.allSkins[i]);
         unlockedButtons[i].interactable = PersistantData.PD.allSkins[i];   
        }
    }

    public void UnlockSkin(int index)
    {
        PersistantData.PD.allSkins[index] = true;
        PlayFabsController.PFC.SetUserData(PersistantData.PD.SkinsDataToString());
        SetUpStore();
    }

    public void OpenShop()
    {
        shopCanvas.SetActive(true);
    }

    public void SetMySkin(int whichSkin)
    {
        PersistantData.PD.mySkin = whichSkin;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
