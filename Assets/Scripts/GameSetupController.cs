using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;




public class GameSetupController : MonoBehaviourPunCallbacks
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
    public int playerSex = 0;
    public string playerID = "";

    //Add the index of all shirts with no arms
    public int[] femaleShirtsWithNoArms = new int[] { 3, 4, 6, 7, 8 };
    public int[] femaleShirtsWithNoSpine = new int[] { 6, 7 };
    public int[] femalePantsWithNoLegs = new int[] { 6 };

    //Chest is usually [OFF] when shirt [On] except here
    public int[] maleShirtsWithChest = new int[] { 2 };
    public int[] femaleShirtsWithChest = new int[] { };

    public int[] maleShirtsWithNoArms = new int[] { 3, 4, 5, 6, 7 };
    public int[] maleShirtsWithNoSpine = new int[] { };
    public int[] malePantsWithNoLegs = new int[] { 1, 3, 4, 5, 6, 7, 8, 9, 10 };

    public Material skinMaterial = null;


    [SerializeField]
    public List<NetworkObjectsClass> networkObjects = new List<NetworkObjectsClass>();


    void Update()
    {
        for (int i = 0; i < networkObjects.Count; i++)
        {
            /*
            if (networkObjects[i].Ping )
            {

            }
            */
        }
        //Get Player count constantly, If it changes : search your networkObjects list and find missing PhotonView
        //Once Photonview index is found get index.Player for PlayFabsID
        //Call Quit RPC for that ID so other players can also remove them from list
        //Possible complications: Does PhotonNetwork.room.PlayerCount work on every client?
        //If it does then then no RPC is needed but if Only Master can use then we must send RPC from master to Others
        //Because without PhotonNetwork.room.PlayerCount we wont be able to determine a change in value
    }




    void Awake()
    {
        PFC = GameObject.Find("NetworkController").GetComponentInChildren<PlayFabsController>();
        if (PFC == null)
        {
            Debug.Log("There is no PlayFabsController");
        }

        NWC = GameObject.Find("NetworkController").GetComponentInChildren<NetworkController>();
        if (NWC == null)
        {
            Debug.Log("There is no NetworkController");
        }

        //Get stats from PFC
        accessoryIndex = PFC.Playeraccessory;
        hatIndex = PFC.playerHat;
        shirtIndex = PFC.playerTop;
        pantIndex = PFC.playerBottom;
        shoesIndex = PFC.playerShoes;
        headIndex = PFC.playerHead;
        skinIndex = PFC.playerSkin;
        playerSex = PFC.playerSex;
        playerID = PFC.myID;
    }

    #region BuildPlayer

    public void SetupPlayerAccessory(int accessoryIndex, GameObject playerCustomizeChildOBJ)
    {
        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().accessoryPresets)
        {
            if (item.name == "Accessory" + accessoryIndex)
            {
                meshRenderer.sharedMesh = item.mesh;
                break;
            }
        }
    }

    public void SetupPlayerHat(int hatIndex, GameObject playerCustomizeChildOBJ)
    {
        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(5).GetComponent<SkinnedMeshRenderer>();

        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().hatsPresets)
        {
            if (item.name == "Hat" + hatIndex)
            {
                meshRenderer.sharedMesh = item.mesh;
                break;
            }
        }
    }

    public void SetupPlayerHead(int headIndex, GameObject playerCustomizeChildOBJ)
    {
        //Bump because list is not propperly setup
        headIndex++;
        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(6).GetComponent<SkinnedMeshRenderer>();

        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().headsPresets)
        {
            if (item.name == "Head" + headIndex)
            {
                meshRenderer.sharedMesh = item.mesh;
                break;
            }
        }
    }

    public void SetupPlayerSkin(int skinIndex, GameObject playerCustomizeChildOBJ)
    {
        //Increment Because List Is Not Setup Propperly
        skinIndex++;
        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().skinMaterialPresets)
        {
            if (item.name == "Skin" + skinIndex)
            {
                skinMaterial = item;
                break;
            }
        }

        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer.sharedMesh != null) { meshRenderer.sharedMaterial = skinMaterial; }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer.sharedMesh != null) { meshRenderer.sharedMaterial = skinMaterial; }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer.sharedMesh != null) { meshRenderer.sharedMaterial = skinMaterial; }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(4).GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer.sharedMesh != null) { meshRenderer.sharedMaterial = skinMaterial; }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(6).GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer.sharedMesh != null) { meshRenderer.sharedMaterial = skinMaterial; }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(7).GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer.sharedMesh != null) { meshRenderer.sharedMaterial = skinMaterial; }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(9).GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer.sharedMesh != null) { meshRenderer.sharedMaterial = skinMaterial; }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(12).GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer.sharedMesh != null) { meshRenderer.sharedMaterial = skinMaterial; }
    }


    public void SetupPlayerPants(int pantIndex, GameObject playerCustomizeChildOBJ)
    {
        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

        //If we have pants on then there is no pelivs BUT sometimes there is no LEGS
        if (pantIndex > 0)
        {
            meshRenderer = playerCustomizeChildOBJ.transform.GetChild(9).GetComponent<SkinnedMeshRenderer>();
            meshRenderer.sharedMesh = null;

            //If female
            if (playerSex == 0)
            {
                for (int i = 0; i < femalePantsWithNoLegs.Length; i++)
                {
                    if (pantIndex == femalePantsWithNoLegs[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(7).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < malePantsWithNoLegs.Length; i++)
                {
                    if (pantIndex == malePantsWithNoLegs[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(7).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
            }
        }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(8).GetComponent<SkinnedMeshRenderer>();

        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().pantsPresets)
        {
            if (item.name == "Pant" + pantIndex)
            {
                meshRenderer.sharedMesh = item.mesh;
                break;
            }
        }
    }

    public void SetupPlayerShirt(int shirtIndex, GameObject playerCustomizeChildOBJ)
    {
        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        bool removeChest = true;

        //If our current shirt has no arms or spine, remove the mesh
        if (shirtIndex > 0)
        {
            //If female
            if (playerSex == 0)
            {
                for (int i = 0; i < femaleShirtsWithChest.Length; i++)
                {
                    //If we have a shirt on that has no chest, delete chest
                    if (shirtIndex == femaleShirtsWithChest[i])
                    {
                        removeChest = false;
                        break;
                    }
                }
                for (int i = 0; i < femaleShirtsWithNoArms.Length; i++)
                {
                    if (shirtIndex == femaleShirtsWithNoArms[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
                for (int i = 0; i < femaleShirtsWithNoSpine.Length; i++)
                {
                    if (shirtIndex == femaleShirtsWithNoSpine[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(12).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < maleShirtsWithChest.Length; i++)
                {
                    //If we have a shirt on that has no chest, delete chest
                    if (shirtIndex == maleShirtsWithChest[i])
                    {
                        removeChest = false;
                        break;
                    }
                }
                for (int i = 0; i < maleShirtsWithNoArms.Length; i++)
                {
                    if (shirtIndex == maleShirtsWithNoArms[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
                for (int i = 0; i < maleShirtsWithNoSpine.Length; i++)
                {
                    if (shirtIndex == maleShirtsWithNoSpine[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(12).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
            }
        }

        if (removeChest == true)
        {
            meshRenderer = playerCustomizeChildOBJ.transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();
            meshRenderer.sharedMesh = null;
        }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(10).GetComponent<SkinnedMeshRenderer>();

        //Actually apply the correct "top" they are wearing
        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().shirtsPresets)
        {
            if (item.name == "Shirt" + shirtIndex)
            {
                meshRenderer.sharedMesh = item.mesh;
                break;
            }
        }
    }

    public void SetupPlayerShoes(int shoesIndex, GameObject playerCustomizeChildOBJ)
    {
        //Set to Shoe
        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

        //If we have shoes on then there is no feet
        if (shoesIndex > 0)
        {
            meshRenderer = playerCustomizeChildOBJ.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>();
            meshRenderer.sharedMesh = null;
        }

        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(11).GetComponent<SkinnedMeshRenderer>();

        foreach (var item in playerCustomizeChildOBJ.GetComponent<CharacterCustomization>().shoesPresets)
        {
            if (item.name == "Shoes" + shoesIndex)
            {
                meshRenderer.sharedMesh = item.mesh;
                break;
            }
        }
    }

    #endregion BuildPlayer

    void Start()
    {
        CreatePlayer(); //Create a networked player object for each player that loads into the multiplayer scenes.
    }

    private void CreatePlayer()
    {
        //THIS IS POTENTIALLY HOW TO SPAWN AT ANY VECTOR MEANING WE CAN SETUP "SPAWN POINTS"

        //Spawn Base Model 
        if (playerSex == 0)
        {
            //Female
            Myplayer = (GameObject)PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayerFEMALE"), new Vector3(-2.8f, 0.0f, 0.736f), Quaternion.identity);
        }
        else
        {
            //Male
            Myplayer = (GameObject)PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayerMALE"), new Vector3(-2.8f, 0.0f, 0.736f), Quaternion.identity);
            
            
        }
        if (Myplayer == null)
        {
            Debug.Log("Couldn't grab instantiated player!");
        }

        //Sets player name to ID for social interaction
        Myplayer.name = playerID;

        //Enables/Disables components that are/arn't required
        ((MonoBehaviour)Myplayer.GetComponent("PlayerMove")).enabled = true;
        ((MonoBehaviour)Myplayer.GetComponent("NetworkPlayer")).enabled = true;
        ((MonoBehaviour)Myplayer.GetComponent("PlayerShooting")).enabled = true;
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
        SetupPlayerAccessory(accessoryIndex, playerCustomizeChildOBJ);
        SetupPlayerHat(hatIndex, playerCustomizeChildOBJ);
        SetupPlayerShirt(shirtIndex, playerCustomizeChildOBJ);
        SetupPlayerPants(pantIndex, playerCustomizeChildOBJ);
        SetupPlayerShoes(shoesIndex, playerCustomizeChildOBJ);
        SetupPlayerHead(headIndex, playerCustomizeChildOBJ);
        SetupPlayerSkin(skinIndex, playerCustomizeChildOBJ);

        

        PhotonView photonView = Myplayer.GetComponent<PhotonView>();
        if (photonView != null)
        {
            //Add player to list for server refrence
            networkObjects.Add(new NetworkObjectsClass(Myplayer.name.ToString(), Myplayer.GetPhotonView(), PhotonNetwork.LocalPlayer.UserId));

            //Send our data to futrue players
            photonView.RPC("SetupRemotePlayer", RpcTarget.OthersBuffered, photonView.ViewID, PhotonNetwork.LocalPlayer.UserId, Myplayer.name, PFC.playerShoes, PFC.Playeraccessory, PFC.playerHat, PFC.playerTop, PFC.playerBottom, PFC.playerHead, PFC.playerSkin, PFC.playerSex);
        }
        else
        {
            Debug.Log("No PhotonView Found On New Player");
        }

            //Enables Statistic gathering for room by enabling component on GameSetupController GameObject
            //Eanabling it here allows us to utilize information in the NetworkObjects<List>
            ((MonoBehaviour)transform.GetComponent("StatisticsForLobby")).enabled = true;
    }
}