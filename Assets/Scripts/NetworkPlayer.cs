using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using System.Globalization;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable
{



    public Animator animator;
    public PhotonView photonView = null;

    public GameSetupController GSC = null;
    public PlayFabsController PFC = null;

    protected Vector3 realPosition = Vector3.zero;
    protected Quaternion realRotation = Quaternion.identity;
    public Quaternion chestRotation = Quaternion.identity;
    public Vector3 targetPosition = Vector3.zero;
    public Vector3 offset = new Vector3(10, 47.32f, 12);
    public Transform local_head, local_neck, local_spine, local_chest = null;
    public Transform target;

    public bool sentFirstUpdate = false;
    public bool gotFirstUpdate = false;

    public GameObject playerCustomizeChildOBJ;
    [SerializeField]
    private int playerHat = 0;
    [SerializeField]
    private int Playeraccessory = 0;
    [SerializeField]
    private int playerTop = 0;
    [SerializeField]
    private int playerJacket = 0;
    [SerializeField]
    private int playerUnderware = 0;
    [SerializeField]
    private int playerHead = 0;
    [SerializeField]
    private int playerSkin = 0;
    [SerializeField]
    private int playerBottom = 0;
    [SerializeField]
    private int playerShoes = 0;
    [SerializeField]
    private int playerSex = 0;
    [SerializeField]
    private string playerID = "";

    private Material skinMaterial = null;
    [SerializeField]
    public int ping = 0;


#region Setup
    private void Awake()
    {
        //Looks for GameSetupController as long as were in a game and NOT in Menu
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            GSC = GameObject.Find("GameSetupController").GetComponent<GameSetupController>();
            if (GSC == null)
            {
                Debug.Log("Network Player Couldn't find GameSetupController  ");
            }
        }

        //Assigning various components/GameObj's to vars for use
        photonView = transform.GetComponent<PhotonView>();
        PFC = GameObject.Find("NetworkController").GetComponent<PlayFabsController>();
        animator = GetComponentInChildren<Animator>();
        playerCustomizeChildOBJ = animator.gameObject;
        //target = transform.Find("CameraPivot/Target");

        if (PFC != null && GSC != null)
        {
            Playeraccessory = PFC.Playeraccessory;
            playerHat = PFC.playerHat;
            playerTop = PFC.playerTop;
            playerJacket = PFC.playerJacket;
            playerUnderware = PFC.playerUnderware;
            playerHead = PFC.playerHead;
            playerSkin = PFC.playerSkin;
            playerBottom = PFC.playerBottom;
            playerShoes = PFC.playerShoes;
            playerSex = PFC.playerSex;
            playerID = PFC.myID;
        }
        else
        {
            Debug.Log("Network Player Couldn't find PlayFabsController ");
        }

    }

    void Start()
    {
        //Gets player Ping With 5s delay, repeat every 5s
        InvokeRepeating("WaitForPingUpdate", 5f, 5f);

        //HumanBodyBones gets body parts
        local_chest = animator.GetBoneTransform(HumanBodyBones.Chest).GetChild(0);
        local_head = animator.GetBoneTransform(HumanBodyBones.Head);
        local_neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        local_spine = animator.GetBoneTransform(HumanBodyBones.Spine);

        /*
        if (target == null){
            Debug.Log("Network Player Couldn't find Target in child ");}
            */

        if (animator == null){
            Debug.Log("Animator Empty in child");}

        if (local_chest == null){
            Debug.Log("Network Player Couldn't find Chest");}

        if (local_head == null){
            Debug.Log("Network Player Couldn't find Head");}

        if (local_neck == null){
            Debug.Log("Network Player Couldn't find Neck");}
        
        if (local_spine == null){
            Debug.Log("Network Player Couldn't find Spine");}

        if (playerCustomizeChildOBJ == null){
            Debug.Log("Couldn't find: PlayerOBJ");}

    }
#endregion Setup

    public void Update()
    {
        if (this.photonView.IsMine)
        {
            //Do Nothing -- Everything is already enabled
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
        }
    }

#region Sync
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(target.transform.position);
            stream.SendNext(local_chest.transform.rotation);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            /*
            stream.SendNext(animator.GetFloat("Speed"));
            stream.SendNext(animator.GetBool("IsGrounded"));
            stream.SendNext(animator.GetBool("IsDead"));
            stream.SendNext(animator.GetFloat("JoyStickX"));
            stream.SendNext(animator.GetFloat("JoyStickY"));
            */
            

        }
        else
        {
            //This is someone else' player. we need to get their positions
            //as of a few milliseconds ago and update or version of that player
            //targetPosition = (Vector3)stream.ReceiveNext();
            chestRotation = (Quaternion)stream.ReceiveNext();
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            /*
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
            animator.SetBool("IsGrounded", (bool)stream.ReceiveNext());
            animator.SetBool("IsDead", (bool)stream.ReceiveNext());
            animator.SetFloat("JoyStickX", (float)stream.ReceiveNext());
            animator.SetFloat("JoyStickY", (float)stream.ReceiveNext());
     */

            if (gotFirstUpdate == false)
            {
                //targetPosition = target.transform.position;
                chestRotation = local_chest.transform.rotation;
                transform.position = realPosition;
                transform.rotation = realRotation;
                gotFirstUpdate = true;
            }
        }
    }
#endregion Sync

#region ChestLookAt()
    public void LateUpdate()
    {
        if (this.photonView.IsMine)
        {
            //Do Nothing -- Everything is already enabled
        }
        else
        {
            local_chest.LookAt(target);
            target.position = Vector3.Lerp(target.position, targetPosition, 0.1f);
            local_chest.rotation = local_chest.rotation * Quaternion.Euler(offset);
            local_chest.rotation = Quaternion.Lerp(local_chest.rotation, chestRotation, 0.1f);
        }
    }

#endregion ChestLookAt()

#region Ping
    public void WaitForPingUpdate()
{
    ping = PhotonNetwork.GetPing();
    //photonView.RPC("SendPing", RpcTarget.Others, photonView.ViewID);
}
    #endregion Ping

#region RPC

[PunRPC]//On dead client and everyone else, stream killfeed,wait 5s(while death anim plays),Hide Player,Reset Health,Teleport to spawn point,Show Player!
public void BroadcastDeath(int PhotonViewID)
{
    //Set anim to "IsDead" via health == 0 (PlayerMove)
    PhotonView.Find(PhotonViewID).GetComponent<Health>().hitPoints = 0; 

    string PlayFabID = "";

    //Use given ID to extrapolate further info on player locally
    for (int i = 0; i< GSC.networkObjects.Count; i++)
    {
        if (GSC.networkObjects[i].viewID.ViewID == PhotonViewID)
        {
            PlayFabID = GSC.networkObjects[i].Player;
            break;
        }
    }

    //Stream local info
    Debug.Log("Player: " + PlayFabID + " Has Died.");

    //Wait 5 seconds
    StartCoroutine(waiter(PhotonViewID));

}

    IEnumerator waiter(int PhotonViewID)
    {
        float waitTime = 5;
        yield return wait(waitTime);

        //After 5 seconds get player OBJ
        GameObject deadPlayer = PhotonView.Find(PhotonViewID).gameObject;

        //Random Spawn
        Vector3 spawn = new Vector3(2f, 0f, 2f);

        //Hide dead player
        deadPlayer.SetActive(false);

        //Reset their HP
        deadPlayer.GetComponent<Health>().hitPoints = 100;

        //spawn deap player @ point
        deadPlayer.gameObject.transform.position = spawn;

        //Dead player is "alive"
        deadPlayer.GetComponent<Health>().gameObject.SetActive(true);
    }

    IEnumerator wait(float waitTime)
    {
        float counter = 0;

        while (counter < waitTime)
        {
            //Increment Timer until counter >= waitTime
            counter += Time.deltaTime;
            //Debug.Log("We have waited for: " + counter + " seconds");
          
            //Wait for a frame so that Unity doesn't freeze
            yield return null;
        }
    }

[PunRPC]//Sends new spawn position of newly spawned char
public void BroadcastSpawn(int PhotonViewID, Vector3 spawnPos)
{
   PhotonView.Find(PhotonViewID).GetComponent<Health>().hitPoints = 100;
   //Find OBJ of newly spawned player and set them to spawn position.
   PhotonView.Find(PhotonViewID).gameObject.transform.position = spawnPos;
   //Find OBJ of newly spawned player and set Animator to Idle.
   //PhotonView.Find(PhotonViewID).gameObject.GetComponentInChildren<Animator>().SetBool("IsDead", false);
}



[PunRPC]//Stores encountered ID
public void SetupRemotePlayer(int PhotonViewID, string photonUserID, string PlayFabID, int shoesIndex, int accessoryIndex, int hatIndex, int shirtIndex, int pantIndex, int headIndex, int skinIndex, int sexIndex)
{
    
    var playerPhotonView = PhotonView.Find(PhotonViewID);
    var player = playerPhotonView.gameObject;
    

    player.name = PlayFabID;
    player = player.transform.GetChild(0).gameObject;

    //Store PlayerFabID as Name & ViewID & UserID for server refrence
    //Add player to list for server refrence
    GSC.networkObjects.Add(new NetworkObjectsClass(PlayFabID, PhotonView.Find(PhotonViewID), photonUserID));

    SetupPlayerAccessory(accessoryIndex, player);
    SetupPlayerShirt(shirtIndex, player, sexIndex);
    SetupPlayerPants(pantIndex, player, sexIndex);
    SetupPlayerShoes(shoesIndex, player);
    SetupPlayerHead(headIndex, player);
    SetupPlayerSkin(skinIndex, player);
    SetupPlayerHat(hatIndex, player);


}
#endregion RPC

#region BuildCharacter

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

    public void SetupPlayerPants(int pantIndex, GameObject playerCustomizeChildOBJ, int playerSex)
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
                for (int i = 0; i < GSC.femalePantsWithNoLegs.Length; i++)
                {
                    if (pantIndex == GSC.femalePantsWithNoLegs[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(7).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < GSC.malePantsWithNoLegs.Length; i++)
                {
                    if (pantIndex == GSC.malePantsWithNoLegs[i])
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

    public void SetupPlayerShirt(int shirtIndex, GameObject playerCustomizeChildOBJ, int playerSex)
    {
        var meshRenderer = playerCustomizeChildOBJ.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        bool removeChest = true;

        //If our current shirt has no arms or spine, remove the mesh
        if (shirtIndex > 0)
        {
            //If female
            if (playerSex == 0)
            {
                for (int i = 0; i < GSC.femaleShirtsWithChest.Length; i++)
                {
                    //If we have a shirt on that has no chest, delete chest
                    if (shirtIndex == GSC.femaleShirtsWithChest[i])
                    {
                        removeChest = false;
                        break;
                    }
                }
                for (int i = 0; i < GSC.femaleShirtsWithNoArms.Length; i++)
                {
                    if (shirtIndex == GSC.femaleShirtsWithNoArms[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
                for (int i = 0; i < GSC.femaleShirtsWithNoSpine.Length; i++)
                {
                    if (shirtIndex == GSC.femaleShirtsWithNoSpine[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(12).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < GSC.maleShirtsWithChest.Length; i++)
                {
                    //If we have a shirt on that has no chest, delete chest
                    if (shirtIndex == GSC.maleShirtsWithChest[i])
                    {
                        removeChest = false;
                        break;
                    }
                }
                for (int i = 0; i < GSC.maleShirtsWithNoArms.Length; i++)
                {
                    if (shirtIndex == GSC.maleShirtsWithNoArms[i])
                    {
                        meshRenderer = playerCustomizeChildOBJ.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
                        meshRenderer.sharedMesh = null;
                        break;
                    }
                }
                for (int i = 0; i < GSC.maleShirtsWithNoSpine.Length; i++)
                {
                    if (shirtIndex == GSC.maleShirtsWithNoSpine[i])
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

    #endregion BuildCharacter
}
