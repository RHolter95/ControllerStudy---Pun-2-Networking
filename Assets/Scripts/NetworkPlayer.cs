using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using System.Globalization;
using UnityEngine.UI;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable
{



    public Animator animator;
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

    //I cant figure out how to make the player ID stay per char
    //NetworkPlayer runs this @Awake which is ran immediatly at start since prefabs have "NetworkPlayer" which may be an issue? IDK yet.
    //Lets just try to get it off their friendslist panel since its there for refrence and the panel should NEVER go away since its Start funct.
    [SerializeField]
    private string playerID = "";

    private void Awake()
    {

        PFC = GameObject.Find("NetworkController").GetComponent<PlayFabsController>();

        if (PFC != null)
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
            //Were getting the ID of the player based off whats set in friendlist panel [TRASH I KNOW]
            //playerID = GameObject.Find("FriendPanel").transform.GetChild(0).GetChild(6).transform.GetComponent<Text>().text;
        }
        else
        {
            Debug.Log("Network Player Couldn't find PlayFabsController ");
        }

        if (SceneManager.GetActiveScene().name == "Network")
        {
            GSC = GameObject.Find("GameSetupController").GetComponent<GameSetupController>();
            if (GSC == null)
            {
                Debug.Log("Network Player Couldn't find GameSetupController  ");
            }
        }

        animator = GetComponentInChildren<Animator>();
        playerCustomizeChildOBJ = animator.gameObject;
        target = transform.Find("CameraPivot/Target");
        if (animator == null || playerCustomizeChildOBJ == null || target == null) { Debug.Log("Couldn't find: PlayerOBJ || Target || Animator"); }
    }

    void Start()
    {
        if (target == null)
        {
            Debug.Log("Network Player Couldn't find Target in child ");
        }

        if (animator == null)
        {
            Debug.Log("Animator Empty in child");
        }

        //HumanBodyBones gets Spine 2, not chest, grab immediate child
        local_chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        local_chest = local_chest.transform.GetChild(0);

        local_head = animator.GetBoneTransform(HumanBodyBones.Head);
        local_neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        local_spine = animator.GetBoneTransform(HumanBodyBones.Spine);

        if (local_chest == null)
        {
            Debug.Log("Network Player Couldn't find Chest");
        }
        if (local_head == null)
        {
            Debug.Log("Network Player Couldn't find Head");
        }
        if (local_neck == null)
        {
            Debug.Log("Network Player Couldn't find Neck");
        }
        if (local_spine == null)
        {
            Debug.Log("Network Player Couldn't find Spine");
        }
    }


    public void Update()
    {
        if (this.photonView.IsMine)
        {
            //Do Nothing -- Everything is already enabled
        }
        else
        {

            SetupPlayerHat(playerHat, playerCustomizeChildOBJ);
            SetupPlayerAccessory(Playeraccessory, playerCustomizeChildOBJ);
            SetupPlayerShirt(playerTop, playerCustomizeChildOBJ);
            SetupPlayerPants(playerBottom, playerCustomizeChildOBJ);
            SetupPlayerShoes(playerShoes, playerCustomizeChildOBJ);

            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
        }
    }

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(target.transform.position);
            stream.SendNext(local_chest.transform.rotation);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));
            stream.SendNext(animator.GetBool("IsGrounded"));
            stream.SendNext(animator.GetFloat("JoyStickX"));
            stream.SendNext(animator.GetFloat("JoyStickY"));
            stream.SendNext(playerHat);
            stream.SendNext(Playeraccessory);
            stream.SendNext(playerShoes);
            stream.SendNext(playerTop);
            stream.SendNext(playerBottom);

        }
        else
        {
            //This is someone else' player. we need to get their positions
            //as of a few milliseconds ago and update or version of that player
            targetPosition = (Vector3)stream.ReceiveNext();
            chestRotation = (Quaternion)stream.ReceiveNext();
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
            animator.SetBool("IsGrounded", (bool)stream.ReceiveNext());
            animator.SetFloat("JoyStickX", (float)stream.ReceiveNext());
            animator.SetFloat("JoyStickY", (float)stream.ReceiveNext());
            playerHat = (int)stream.ReceiveNext();
            Playeraccessory = (int)stream.ReceiveNext();
            playerShoes = (int)stream.ReceiveNext();
            playerTop = (int)stream.ReceiveNext();
            playerBottom = (int)stream.ReceiveNext();

            if (gotFirstUpdate == false)
            {
                targetPosition = target.transform.position;
                chestRotation = local_chest.transform.rotation;
                transform.position = realPosition;
                transform.rotation = realRotation;
                gotFirstUpdate = true;
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
    /*
    #region GetClothesStats
    public void GetStatistics()
        {
            PlayFabClientAPI.GetPlayerStatistics(
                new GetPlayerStatisticsRequest(),
                OnGetStatistics,
                error => Debug.LogError(error.GenerateErrorReport())
            );
        }

        void OnGetStatistics(GetPlayerStatisticsResult result)
        {
            //Debug.Log("Received the following Statistics:");
            foreach (var eachStat in result.Statistics)
            {
                //Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
                switch (eachStat.StatisticName)
                {
                    case "Hat":
                        playerHat = eachStat.Value;
                        //CharacterCustomizeClothesSlotSetup(eachStat.Value, 0);
                        //tempClothesStr = eachStat.Value.ToString();
                        break;
                    case "Accessories":
                        Playeraccessory = eachStat.Value;
                        //CharacterCustomizeClothesSlotSetup(eachStat.Value,1);
                        break;
                    case "Top":
                        playerTop = eachStat.Value;
                        //CharacterCustomizeClothesSlotSetup(eachStat.Value,2);
                        break;
                    case "Jacket":
                        playerJacket = eachStat.Value;
                        //CharacterCustomizeClothesSlotSetup(eachStat.Value,5);
                        break;
                    case "Underware":
                        playerUnderware = eachStat.Value;
                        //CharacterCustomizeClothesSlotSetup(eachStat.Value,6);
                        break;
                    case "Bottom":
                        playerBottom = eachStat.Value;
                        //CharacterCustomizeClothesSlotSetup(eachStat.Value,3);
                        break;
                    case "Shoes":
                        playerShoes = eachStat.Value;
                        //CharacterCustomizeClothesSlotSetup(eachStat.Value, 4);
                        break;
                    case "Skin":
                        playerSkin = eachStat.Value;
                        //CharacterCustomizeBodySlotSetup(eachStat.Value,0);
                        break;
                    case "Sex":
                        playerSex = eachStat.Value;
                        //CharacterCustomizeBodySlotSetup(eachStat.Value,1);
                        break;
                    case "Head":
                        playerHead = eachStat.Value;
                        //CharacterCustomizeBodySlotSetup(eachStat.Value,2);
                        break;
                   case "PlayFabID":
                        playerID = eachStat.Value;
                    //CharacterCustomizeBodySlotSetup(eachStat.Value,2);
                    break;
            }
            }
            //Might not need the bool below
            run = false;
        }
    
        private static void OnErrorShared(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
        }

        #endregion GetClothesStats
        */
}
