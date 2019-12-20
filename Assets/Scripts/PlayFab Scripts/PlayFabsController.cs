using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.Internal;

public class PlayFabsController : MonoBehaviour
{
    private int DEFAULT = 0;
    public bool newRegister = false;

    public UnderdogCity.PlayerMove PM = null;

    public static PlayFabsController PFC;
    public NetworkController networkController = null;
    public UIControllerDEMO CharCust = null;
    public string userEmailStr = null;
    public string userNameStr = null;
    public string userPassStr = null;
    public string myID = "";
    public bool isCustomizing = false;
    public bool updateFriendPanelID = false;
    public GameObject maleCust = null;
    public GameObject femaleCust = null;
    public GameObject finalChar = null;
    public int playerHat = 0;
    public int Playeraccessory = 0;
    public int playerTop = 0;
    public int playerJacket = 0;
    public int playerUnderware = 0;
    public int playerHead = 0;
    public int playerSkin = 0;
    public int playerBottom = 0;
    public int playerShoes = 0;
    public int playerSex = 0;
    public string playerID = "";
    public string temp = "";
    public UnityEngine.Object prefab;
    public GameObject maleStage = null;
    public GameObject femaleStage = null;
    public GameObject tempOBJ = null;

    public GameObject ListingPrefab;
    public GameObject leaderBoard;
    public GameObject leaderBoardPanel;
    public Transform listingContainer;
    public Transform friendScrollView;
    public GameObject friendPanel;
    public GameObject friendListPanel;
    public GameObject pauseMenu = null;
    public GameObject pausePanel = null;
    string friendSearch;




    private void Enable()
    {
        if (PlayFabsController.PFC == null)
        {
            PlayFabsController.PFC = this;
        }
        else
        {
            if (PlayFabsController.PFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Awake()
    {
        pauseMenu = GameObject.Find("PauseMenu").gameObject;
        
        if (pauseMenu == null)
        {
            Debug.Log("Couldn't locate PauseMenu GameObj, Pause broken");
        }
        else
        {
            friendPanel = pauseMenu.transform.GetChild(0).gameObject;
            friendListPanel = pauseMenu.transform.GetChild(0).GetChild(0).gameObject;
            friendScrollView = friendListPanel.transform.GetChild(1).transform;

            leaderBoard = pauseMenu.transform.GetChild(1).gameObject;
            leaderBoardPanel = pauseMenu.transform.GetChild(1).GetChild(0).gameObject;
            listingContainer = leaderBoardPanel.transform.GetChild(1).transform;

            pausePanel = pauseMenu.transform.GetChild(2).gameObject;

            pauseMenu.SetActive(false);
        }

        leaderBoardPanel.SetActive(false);
        friendPanel.SetActive(false);

        maleCust = GameObject.Find("Male_Customize");
        femaleCust = GameObject.Find("Female_Customize");

        maleStage = GameObject.Find("Male_Stand");
        femaleStage = GameObject.Find("Female_Stand");

        maleStage.SetActive(false);
        femaleStage.SetActive(false);

        if (femaleCust == null || maleCust == null)
        {
            Debug.Log("Couldn't locate Male/Female GameObj");
        }

        if (maleStage == null || femaleStage == null)
        {
            Debug.Log("Couldn't locate Male/Female Stage");
        }


        networkController = transform.gameObject.GetComponent<NetworkController>();
        if (networkController == null)
        {
            Debug.Log("Couldn't locate Network Controller");
        }
    }

    void Update()
    {
        if (updateFriendPanelID)
        {
            friendListPanel.transform.GetChild(5).GetComponent<Text>().text = myID.ToString();
            updateFriendPanelID = false;
        }

        if (networkController.charSex == 0 && isCustomizing)
        {
            CharCust = femaleStage.GetComponentInChildren<UIControllerDEMO>();
        }

        if (networkController.charSex == 1 && isCustomizing)
        {
            CharCust = maleStage.GetComponentInChildren<UIControllerDEMO>();
        }

        //If were customizing and we are Male lets grab correct component
        if (isCustomizing && networkController.charSex == 0)
        {
            finalChar = femaleCust;
        }

        //If were customizing and we are Female lets grab correct component
        if (isCustomizing && networkController.charSex == 1)
        {
            finalChar = maleCust;
        }

        if (networkController.userEmailGO != null)
        {
            userEmailStr = networkController.userEmailGO.GetComponent<InputField>().text;
        }
        if (networkController.userNameGO != null)
        {
            userNameStr = networkController.userNameGO.GetComponent<InputField>().text;
        }
        if (networkController.userPassGO != null)
        {
            userPassStr = networkController.userPassGO.GetComponent<InputField>().text;
        }
    }

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "EF1CC"; // Please change this value to your own titleId from PlayFab Game Manager
        }

        PlayerPrefs.DeleteAll();

        //Automatically try to sign them in
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmailStr = PlayerPrefs.GetString("EMAIL");
            userPassStr = PlayerPrefs.GetString("PASSWORD");
            Login();
        }
        else
        {
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest{AndroidDeviceId = ReturnMobileID(), CreateAccount = true};
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
            networkController.recoverButton.SetActive(true);
#endif

#if UNITY_IOS
            var requestIOS = new loginWithIOSDeviceIDRequest{DeviceID = ReturnMobileID(), CreateAccount = true};
            PlayFabClientAPI.loginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
            networkController.recoverButton.SetActive(true);
#endif
        }
    }

    //Login finction
    #region Login

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmailStr, Password = userPassStr };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("Logged In: ");

        myID = result.PlayFabId;

        //Throw playerID onto friendlist Panel for friend request refrence
        updateFriendPanelID = true;

        networkController.userEmailGO.SetActive(false);
        networkController.userNameGO.SetActive(false);
        networkController.userPassGO.SetActive(false);
        networkController.submitButton.SetActive(false);
        networkController.onlineAuthButton.SetActive(false);

        networkController.onlineButton.SetActive(true);
        networkController.shopButton.SetActive(true);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Logged In: " + userEmailStr);
        PlayerPrefs.SetString("EMAIL", userEmailStr);
        PlayerPrefs.SetString("PASSWORD", userPassStr);

        myID = result.PlayFabId;

        //Throw playerID onto friendlist Panel
        updateFriendPanelID = true;

        networkController.loginButton.SetActive(false);
        networkController.createNewButton.SetActive(false);
        networkController.userEmailGO.SetActive(false);
        networkController.userNameGO.SetActive(false);
        networkController.userPassGO.SetActive(false);
        networkController.submitButton.SetActive(false);
        networkController.recoverButton.SetActive(false);
        networkController.onlineButton.SetActive(false);
        networkController.shopButton.SetActive(false);

        if (newRegister)
        {
            Debug.Log("Customize Now");
            networkController.maleSex.SetActive(true);
            networkController.femaleSex.SetActive(true);
        }
        else
        {
            GetStatistics();
            networkController.onlineButton.SetActive(true);
            networkController.shopButton.SetActive(true);
        }
    }

    int GetIndex(String inputString)
    {
        int index = int.Parse(inputString.ToString());
        if (index >= 0 && index <= 100)
        {
        }
        else
        {
            //We dont have a valid index 0 of Head or Skin ( not sure yet) so set it to default 1
            index = 1;
        }
        return index;
    }

    public void FinalizeCharacter()
    {
        isCustomizing = false;

        //THIS WILL NEVER WORK OUTSIDE OF EDITOR
        //Lets push clothing index's and generate the character based off those index's
        //inside gameManager. Load stats and generate a base character
        //Then change the skin asset "Mesh" component to the index of correct "Mesh"


        //Determine sex and spawn base model
        //prefab = EditorUtility.CreateEmptyPrefab("Assets/Photon/Resources/PhotonPrefabs/PhotonPlayerMALE.prefab");
        //EditorUtility.ReplacePrefab(finalChar, prefab, ReplacePrefabOptions.ConnectToPrefab);

        //Grabs all list index's and pushes to cloud
        playerSex = networkController.charSex;
        playerHat = GetIndex(CharCust.hat_text.text);
        Playeraccessory = GetIndex(CharCust.accessory_text.text);
        playerTop = GetIndex(CharCust.shirt_text.text);
        playerBottom = GetIndex(CharCust.pant_text.text);
        playerShoes = GetIndex(CharCust.shoes_text.text);
        playerSkin = GetIndex(CharCust.skin_text.text);
        playerHead = GetIndex(CharCust.head_text.text);
        StartCloudUpdatePlayerClothes();

        //Get Stats we just posted
        GetStatistics();

        networkController.maleCharCustomizer.SetActive(false);
        networkController.femaleCharCustomizer.SetActive(false);
        networkController.onlineButton.SetActive(true);
        networkController.shopButton.SetActive(true);

    }

    public void FemaleSex()
    {
        networkController.maleCharCustomizer.SetActive(false);
        networkController.femaleCharCustomizer.SetActive(true);

        networkController.maleSex.SetActive(false);
        networkController.femaleSex.SetActive(false);

        femaleStage.SetActive(true);

        isCustomizing = true;
        networkController.charSex = 0;
    }

    public void SwapSex()
    {
        switch (networkController.charSex)
        {
            case 0:
                Debug.Log("Female -> Male");
                femaleCust.SetActive(false);
                femaleStage.SetActive(false);
                maleCust.SetActive(true);
                maleStage.SetActive(true);
                networkController.charSex = 1;
                break;
            case 1:
                Debug.Log("Male -> Female");
                maleCust.SetActive(false);
                maleStage.SetActive(false);
                femaleCust.SetActive(true);
                femaleStage.SetActive(true);
                networkController.charSex = 0;
                break;
        }
    }

    public void MaleSex()
    {
        networkController.maleCharCustomizer.SetActive(true);
        networkController.femaleCharCustomizer.SetActive(false);

        networkController.maleSex.SetActive(false);
        networkController.femaleSex.SetActive(false);

        maleStage.SetActive(true);

        isCustomizing = true;
        networkController.charSex = 1;
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport()); ;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private void OnLoginMobileFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void WipeTextFields()
    {
        networkController.userNameInput.text = "";
        networkController.userEmailInput.text = "";
        networkController.userPassInput.text = "";
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registered: " + userEmailStr);
        PlayerPrefs.SetString("EMAIL", userEmailStr);
        PlayerPrefs.SetString("PASSWORD", userPassStr);
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = userNameStr }, OnDisplayName, OnLoginMobileFailure);
        newRegister = true;

        //Evertime we register we initialize stats to 0 and push to server
        playerLevel = 1;
        playerHighScore = 0;
        playerCash = 0;
        StartCloudUpdatePlayerStats();

        //start With default clothes & push to server
        playerHat = playerTop = playerJacket = playerUnderware = playerBottom = playerShoes = DEFAULT;

        myID = result.PlayFabId;


        //Doesnt need UserName to login
        userNameStr = "";
        networkController.userNameGO.SetActive(false);
        networkController.loginButton.SetActive(true);
        networkController.submitButton.SetActive(false);
        networkController.recoverButton.SetActive(false);
        WipeTextFields();
    }

    void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        //Debug.Log(result.DisplayName + " is your new display name");
    }

    public void Submit()
    {
        if (userEmailStr.Length > 6 && userNameStr.Length > 3 && userPassStr.Length > 6)
        {
            Debug.Log("Sending Registration to playfab\nEMAIL: " + userEmailStr + " , USERNAME: " + userNameStr);
            var registerRequest = new RegisterPlayFabUserRequest { Email = userEmailStr, Password = userPassStr, Username = userNameStr };
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
        }
        else
        {
            Debug.Log("Fix Username Field");
        }
    }

    public static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    public void OnClickAddLogin()
    {
        var addLoginRequest = new AddUsernamePasswordRequest { Email = userEmailStr, Password = userPassStr, Username = userNameStr };
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSuccess, OnRegisterFailure);
    }

    private void OnAddLoginSuccess(AddUsernamePasswordResult result)
    {

        Debug.Log("Successfully Added Info To Anonymous Account");
        PlayerPrefs.SetString("EMAIL", userEmailStr);
        PlayerPrefs.SetString("PASSWORD", userPassStr);
        networkController.userEmailGO.SetActive(false);
        networkController.userNameGO.SetActive(false);
        networkController.userPassGO.SetActive(false);
        networkController.submitRecovery.SetActive(false);
        networkController.loginButton.SetActive(false);
        networkController.submitButton.SetActive(false);
        networkController.onlineButton.SetActive(true);
        networkController.shopButton.SetActive(true);
        WipeTextFields();
    }
    #endregion Login

    public int playerLevel;
    public int gameLevel;
    public int playerHighScore;
    public int playerCash;

    #region PlayerStats  

    #region Level_HighScore_Cash_StatPush
    // Build the request object and access the API
    public void StartCloudUpdatePlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats",
            FunctionParameter = new { Level = playerLevel, HighScore = playerHighScore, Cash = playerCash },
            GeneratePlayStreamEvent = true,
        }, OnCloudUpdateStats, OnErrorShared);
    }

    private static void OnCloudUpdateStats(ExecuteCloudScriptResult result)
    {
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue);
        Debug.Log((string)messageValue);
    }

    #endregion Level_HighScore_Cash_StatPush

    #region Clothing_StatPush
    public void StartCloudUpdatePlayerClothes()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerClothes",
            FunctionParameter = new { Hat = playerHat, Accessories = Playeraccessory, Top = playerTop, Jacket = playerJacket, Underware = playerUnderware, Bottom = playerBottom, Shoes = playerShoes, Skin = playerSkin, Sex = playerSex, Head = playerHead },
            GeneratePlayStreamEvent = true,
        }, OnCloudUpdateClothes, OnErrorShared);
    }

    private static void OnCloudUpdateClothes(ExecuteCloudScriptResult result)
    {
        // Cloud Script returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        //Debug.Log(JsonWrapper.SerializeObject(result.FunctionResult));
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
        Debug.Log((string)messageValue);
    }
    #endregion Clothing_StatPush

    //Error reporting for StatPush
    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }


    #endregion PlayerStats

    #region Leaderboard

    public void GetLeaderboarder()
    {
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "ObjectsDestroyed", MaxResultsCount = 20 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderboard, OnErrorLeaderboard);
    }
    public void GetLeaderBoardPauseMenu()
    {
        pausePanel.SetActive(false);
        leaderBoardPanel.SetActive(true);
        leaderBoard.SetActive(true);
        GetLeaderboarder();
    }

    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        //leaderBoardPanel.SetActive(true);
        //Debug.Log(result.Leaderboard[0].StatValue);
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            GameObject tempListing = Instantiate(ListingPrefab, listingContainer);
            ListingPrefabb LL = tempListing.GetComponent<ListingPrefabb>();
            LL.playerNameText.text = player.DisplayName;
            LL.playerScoreText.text = player.StatValue.ToString();
            Debug.Log(player.DisplayName + ": " + player.StatValue);
        }
    }

    public void CloseLeaderboardPanel()
    {
        leaderBoardPanel.SetActive(false);
        for (int i = listingContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(listingContainer.GetChild(i).gameObject);
        }
        pauseMenu.SetActive(true);
        pausePanel.SetActive(true);
    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    #endregion Leaderboard

    #region PlayerData

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
                    break;
                case "Accessories":
                    Playeraccessory = eachStat.Value;
                    break;
                case "Top":
                    playerTop = eachStat.Value;
                    break;
                case "Jacket":
                    playerJacket = eachStat.Value;
                    break;
                case "Underware":
                    playerUnderware = eachStat.Value;
                    break;
                case "Bottom":
                    playerBottom = eachStat.Value;
                    break;
                case "Shoes":
                    playerShoes = eachStat.Value;
                    break;
                case "Skin":
                    playerSkin = eachStat.Value;
                    break;
                case "Sex":
                    playerSex = eachStat.Value;
                    break;
                case "Head":
                    playerHead = eachStat.Value;
                    break;
            }
        }
    }

    public void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myID,
            Keys = null
        }, UserDataSuccess, OnErrorLeaderboard);
    }

    void UserDataSuccess(GetUserDataResult result)
    {
        if (result.Data == null || !result.Data.ContainsKey("Skins"))
        {
            Debug.Log("Skins not set");
        }
        else
        {
            PersistantData.PD.SkinsStringToData(result.Data["Skins"].Value);
        }
    }



    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myID,
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("playerID")) Debug.Log("No playerID");
            else playerID = result.Data["playerID"].Value; //Debug.Log("playerID: " + result.Data["playerID"].Value);
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void SetUserData(string SkinsData)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
        {
            {"Skins", SkinsData}
        }
        }, SetDataSuccess, OnErrorLeaderboard);
    }

    void SetDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log(result.DataVersion);
    }


    #endregion PlayerData

    #region Friends
    [SerializeField]
    List<FriendInfo> myFriends;

    public void GetFriendPanelPauseMenu()
    {
        pausePanel.SetActive(false);
        friendListPanel.SetActive(true);
        friendPanel.SetActive(true);
        GetFriends();
    }

    void DisplayFriends(List<FriendInfo> friendsCache)
    {
        foreach (FriendInfo f in friendsCache)
        {
            bool isFound = false;
            if (myFriends != null)
            {
                foreach (FriendInfo g in myFriends)
                {
                    if (f.FriendPlayFabId == g.FriendPlayFabId)
                    {
                        isFound = true;
                    }
                }
            }
            if (isFound == false)
            {
                GameObject listing = Instantiate(ListingPrefab, friendScrollView);
                ListingPrefabb tempListing = listing.GetComponent<ListingPrefabb>();
                //Debug.Log(tempListing.playerNameText);
                //Debug.Log(f.TitleDisplayName);
                tempListing.playerNameText.text = f.TitleDisplayName;
            }
        }
        myFriends = friendsCache;
        //HERE
    }

    IEnumerator WaitForFriend()
    {
        yield return new WaitForSeconds(2);
        GetFriends();
    }

    public void RunWaitForFriend()
    {
        StartCoroutine(WaitForFriend());
    }

    enum FriendIdType { PlayFabId, Username, Email, DisplayName };

    void AddFriend(FriendIdType idType, string friendId)
    {
        var request = new AddFriendRequest();
        switch (idType)
        {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        // Execute request and update friends when we are done
        PlayFabClientAPI.AddFriend(request, result => {
            Debug.Log("Friend added successfully!");
        }, OnErrorShared);
    }

    List<FriendInfo> _friends = null;

    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            IncludeSteamFriends = false,
            IncludeFacebookFriends = false
        }, result =>
        {
            _friends = result.Friends;
            DisplayFriends(_friends);//triggers your UI
        }, OnErrorShared);
    }
    

    public void InputFriendID(string idIn)
    {
        friendSearch = idIn;
    }

    public void SubmitFriendRequest()
    {
        AddFriend(FriendIdType.PlayFabId, friendSearch);
    }

    public void OpenCloseFriends()
    {
        
        friendPanel.SetActive(!friendPanel.activeInHierarchy);
        pauseMenu.SetActive(true);
        pausePanel.SetActive(true);
    }

    #endregion Friends



    //Use for Offline in a while
    #region OfflineSetStats
    /*
    public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest
        {
        // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate>
            {
            new StatisticUpdate { StatisticName = "PlayerLevel", Value = playerLevel},
            new StatisticUpdate { StatisticName = "GameLevel", Value = gameLevel},
            new StatisticUpdate { StatisticName = "PlayerHighScore", Value = playerHighScore},
            new StatisticUpdate { StatisticName = "PlayerCash", Value = playerCash},
            }
        },
        result => {Debug.Log("User statistics updated");},
        error => {Debug.LogError(error.GenerateErrorReport());});
    }

    void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStats,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }


    void OnGetStats(GetPlayerStatisticsResult result)
    {
            Debug.Log("Received the following Statistics:");
            foreach (var eachStat in result.Statistics)
            {
                Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
                switch(eachStat.StatisticName)
                {
                    case "PlayerLevel":
                    playerLevel = eachStat.Value;
                        break;
                    case "GameLevel" :
                    gameLevel = eachStat.Value;
                        break;
                    case "PlayerHighScore":
                    playerHighScore = eachStat.Value;
                        break;
                    case "PlayerCash" :
                    playerCash = eachStat.Value;
                        break;
                }
            }
    }
    */
    #endregion OfflineSetStats
}