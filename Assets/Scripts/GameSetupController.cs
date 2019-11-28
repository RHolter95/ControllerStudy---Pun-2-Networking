using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{

    public GameObject playerCustomizeChildOBJ = null;
    GameObject Myplayer = null;
    public NetworkController NWC = null;
    public PlayFabsController PFC = null;
    public CharacterCustomization CC;
    public int accessoryIndex = 0;
    public int hatIndex = 0;
    public int pantIndex = 0;
    public int shirtIndex = 0;
    public int shoesIndex = 0;
    public int headIndex = 0;
    public int skinIndex = 0;

    //Add the index of all shirts with no arms
    public int[] shirtsWithNoArms = new int[] {3, 4, 6, 7, 8};
    public int[] shirtsWithNoSpine = new int[] {6,7};
    public int[] pantsWithNoLegs = new int[] {6};





    public SkinnedMeshRenderer SMR;
    void Update()
    {

    }
    void Awake()
    {
        PFC = GameObject.Find("NetworkController").GetComponentInChildren<PlayFabsController>();
        if(PFC == null){
            Debug.Log("There is no PlayFabsController");
        }

        NWC = GameObject.Find("NetworkController").GetComponentInChildren<NetworkController>();
        if(NWC == null){
            Debug.Log("There is no NetworkController");
        }

        PFC.GetStatistics();

        //Get stats from cloud
        accessoryIndex = PFC.Playeraccessory;
        hatIndex = PFC.playerHat;
        shirtIndex = PFC.playerTop;
        pantIndex = PFC.playerBottom;
        shoesIndex = PFC.playerShoes;
        headIndex = PFC.playerHead;
        skinIndex = PFC.playerSkin;

        /*
        Debug.Log("Accesories "+accessoryIndex);
        Debug.Log("Hat "+hatIndex);
        Debug.Log("Shirt "+shirtIndex);
        Debug.Log("Pant "+pantIndex);
        Debug.Log("Shoes "+shoesIndex);
        Debug.Log("Head "+headIndex);
        Debug.Log("Skin "+skinIndex);
        */
    }

    // This script will be added to any multiplayer scene
    void Start()
    {  
        CreatePlayer(); //Create a networked player object for each player that loads into the multiplayer scenes.
    }

    private void CreatePlayer()
    {
        //THIS IS POTENTIALLY HOW TO SPAWN AT ANY VECTOR MEANING WE CAN SETUP "SPAWN POINTS"
        //Debug.Log("Creating Player");
        
        //Spawn Base Model 
        if(NWC.charSex == 0){
            //Female
            Myplayer = (GameObject)PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayerFEMALE"), new Vector3 (-2.8f,0.0f,0.736f), Quaternion.identity);
        }else{
            //Male
            Myplayer = (GameObject)PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayerMALE"), new Vector3 (-2.8f,0.0f,0.736f), Quaternion.identity);
        }
        if(Myplayer == null){
            Debug.Log("Couldn't grab instantiated player!");
        }

        ((MonoBehaviour)Myplayer.GetComponent("PlayerMove")).enabled = true;
        ((MonoBehaviour)Myplayer.GetComponent("NetworkPlayer")).enabled = true;
        ((MonoBehaviour)Myplayer.GetComponent("PlayerShooting")).enabled = false;//true
        ((MonoBehaviour)Myplayer.GetComponent("StayOnStage")).enabled = false;
        Myplayer.transform.GetChild(2).Find("Main Camera").gameObject.SetActive(true);
        Myplayer.transform.Find("Canvas").gameObject.SetActive(true);

        //Leave customization script active until we customize, then get the CharCustomize script
        ((MonoBehaviour)Myplayer.transform.GetChild(0).GetComponent("CharacterCustomization")).enabled = true;
        
        CC = Myplayer.transform.GetChild(0).GetComponent<CharacterCustomization>();

        //Grabbing CharCustomize GO (Character@Idle)
        playerCustomizeChildOBJ = Myplayer.transform.GetChild(0).gameObject;

        //Grab Accessory meshRenderer & = null;
        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
            
        //Slow AF but hopefully lists are small, Iterate over Entire list w/o order (Lists == unordered)
        //Accessories applied to char.
        //Debug.Log("Accessory: "+ meshRenderer.gameObject.name);
        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().accessoryPresets)
        {
            if(item.name == "Accessory"+accessoryIndex){
                meshRenderer.sharedMesh = item.mesh;
                //Move renderer down to next element (OBJ) to customize
                meshRenderer = playerCustomizeChildOBJ.transform.GetChild(5).GetComponent<SkinnedMeshRenderer>();
                break;
            }
        }
        //Hats applied to char.
        //Debug.Log("Hat: "+ meshRenderer.gameObject.name);
        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().hatsPresets)
        {
            if(item.name == "Hat"+hatIndex){
                meshRenderer.sharedMesh = item.mesh;
                meshRenderer = playerCustomizeChildOBJ.transform.GetChild(8).GetComponent<SkinnedMeshRenderer>();
                break;
            }
        }
        //Pants applied to char.
        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().pantsPresets)
        {
            if(item.name == "Pant"+pantIndex){
                meshRenderer.sharedMesh = item.mesh;
                meshRenderer = playerCustomizeChildOBJ.transform.GetChild(10).GetComponent<SkinnedMeshRenderer>();
                break;
            }
        }
        //Shirts applied to char.
        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().shirtsPresets)
        {
            if(item.name == "Shirt"+shirtIndex){
                meshRenderer.sharedMesh = item.mesh;
                meshRenderer = playerCustomizeChildOBJ.transform.GetChild(11).GetComponent<SkinnedMeshRenderer>();
                break;
            }
        }
        //Shoes applied to char.
        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().shoesPresets)
        {
            if(item.name == "Shoes"+shoesIndex){
                meshRenderer.sharedMesh = item.mesh;
                break;
            }
        }

        //If we have a SHIRT[ON] then there is not CHEST[OFF]
        if(shirtIndex > 0){
            meshRenderer = playerCustomizeChildOBJ.transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();
            meshRenderer.sharedMesh = null;
        }

        //If our current shirt has no arms or spine, remove the mesh
        if(shirtIndex > 0 ){
            for (int i = 0; i < shirtsWithNoArms.Length ; i++)
            {
                if(shirtIndex == shirtsWithNoArms[i]){
                    meshRenderer = playerCustomizeChildOBJ.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
                    meshRenderer.sharedMesh = null;
                    break;
                }
            }
            for (int i = 0; i < shirtsWithNoSpine.Length; i++)
            {
                if(shirtIndex == shirtsWithNoSpine[i]){
                    meshRenderer = playerCustomizeChildOBJ.transform.GetChild(12).GetComponent<SkinnedMeshRenderer>();
                    meshRenderer.sharedMesh = null;
                    break;
                }
            }
        }

        //If we have pants on then there is no pelivs BUT sometimes there is no LEGS
        if(pantIndex > 0){
            meshRenderer = playerCustomizeChildOBJ.transform.GetChild(9).GetComponent<SkinnedMeshRenderer>();
            meshRenderer.sharedMesh = null;
            for (int i = 0; i < pantsWithNoLegs.Length; i++)
            {
                if(pantIndex == pantsWithNoLegs[i]){
                    meshRenderer = playerCustomizeChildOBJ.transform.GetChild(7).GetComponent<SkinnedMeshRenderer>();
                    meshRenderer.sharedMesh = null;
                    break;
                }
            }
        }

        //If we have shoes on then there is no feet
        if(shoesIndex > 0){
            meshRenderer = playerCustomizeChildOBJ.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>();
            meshRenderer.sharedMesh = null;
        }

        //HEAD always exists
        //Set Head
        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(6).GetComponent<SkinnedMeshRenderer>();
        meshRenderer.sharedMesh = CC.headsPresets[headIndex].mesh;


        //FOREARMS always exists
        
    }
}
