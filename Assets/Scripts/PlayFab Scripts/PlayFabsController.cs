using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabsController : MonoBehaviour
{
    private int DEFAULT = 0;

    public static PlayFabsController PFC;
    public NetworkController networkController = null;
    public string userEmailStr = null;
	public string userNameStr = null;
	public string userPassStr = null;
    private string myID;

    private void Enable()
    {
        if(PlayFabsController.PFC == null)
        {
            PlayFabsController.PFC = this;
        }
        else
        {
            if(PlayFabsController.PFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Awake()
    {
        
        networkController = transform.gameObject.GetComponent<NetworkController>();
        if(networkController == null)
        {
            Debug.Log("Couldn't locate Network Controller");
        }
    }

    void Update()
    {
        if(networkController.userEmailGO != null){
            userEmailStr = networkController.userEmailGO.GetComponent<InputField>().text;
        }
        if(networkController.userNameGO != null){
            userNameStr = networkController.userNameGO.GetComponent<InputField>().text;
        }
        if(networkController.userPassGO != null){
            userPassStr = networkController.userPassGO.GetComponent<InputField>().text;
        }
    }

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)){
            PlayFabSettings.TitleId = "EF1CC"; // Please change this value to your own titleId from PlayFab Game Manager
        }

        PlayerPrefs.DeleteAll();

        //Automatically try to sign them in
        if(PlayerPrefs.HasKey("EMAIL"))
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
        var request = new LoginWithEmailAddressRequest { Email = userEmailStr, Password = userPassStr};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
	}

       private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("Logged In: ");

        myID = result.PlayFabId;
        GetStatistics();
        //GetPlayerData();

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

        networkController.loginButton.SetActive(false);
        networkController.createNewButton.SetActive(false);
        networkController.userEmailGO.SetActive(false);
        networkController.userNameGO.SetActive(false);
        networkController.userPassGO.SetActive(false);
        networkController.submitButton.SetActive(false);
        networkController.recoverButton.SetActive(false);

        networkController.onlineButton.SetActive(true);
        networkController.shopButton.SetActive(true);
        GetStatistics();
        //GetPlayerData();
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());;
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
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest{DisplayName = userNameStr},OnDisplayName,OnLoginMobileFailure);

        //Evertime we register we initialize stats to 0 and push to server
        playerLevel = 1;
        playerHighScore = 0;
        playerCash = 0;
        StartCloudUpdatePlayerStats();

        //start With default clothes & push to server
        playerHat = playerTop = playerJacket = playerUnderware = playerBottom = playerShoes = DEFAULT;
        StartCloudUpdatePlayerClothes();

        myID = result.PlayFabId;
        //GetPlayerData();
        GetStatistics();

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
        if(userEmailStr.Length > 6 && userNameStr.Length > 3 && userPassStr.Length >= 6)
        {
		Debug.Log("Sending Registration to playfab\nEMAIL: " + userEmailStr+" , USERNAME: "+userNameStr);
        var registerRequest  = new RegisterPlayFabUserRequest  { Email = userEmailStr, Password = userPassStr ,Username = userNameStr};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
        }else{
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
        var addLoginRequest = new AddUsernamePasswordRequest{Email = userEmailStr, Password = userPassStr, Username = userNameStr};
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest,OnAddLoginSuccess,OnRegisterFailure); 
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

//Set all clothing bellow to 0 which is default
public int playerHat = 0;
public int playerTop = 0;
public int playerJacket = 0;
public int playerUnderware = 0;
public int playerBottom = 0;
public int playerShoes = 0;


#region PlayerStats  

#region Level_HighScore_Cash_StatPush
// Build the request object and access the API
public void StartCloudUpdatePlayerStats()
{
    
    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
    {
        FunctionName = "UpdatePlayerStats",
        FunctionParameter = new { Level = playerLevel , HighScore = playerHighScore , Cash = playerCash },
        GeneratePlayStreamEvent = true, 
    }, OnCloudUpdateStats, OnErrorShared);
}

private static void OnCloudUpdateStats(ExecuteCloudScriptResult result) {
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
        FunctionName = "UpdatePlayerClothes", // Arbitrary function name (must exist in your uploaded cloud.js file)
        FunctionParameter = new { Hat = playerHat, Top = playerTop, Jacket = playerJacket, Underware = playerUnderware, Bottom = playerBottom, Shoes = playerShoes}, // The parameter provided to your function
        GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
    }, OnCloudUpdateClothes, OnErrorShared);
}
// OnCloudHelloWorld defined in the next code block

private static void OnCloudUpdateClothes(ExecuteCloudScriptResult result) {
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
var requestLeaderboard = new GetLeaderboardRequest{StartPosition = 0,StatisticName = "ObjectsDestroyed", MaxResultsCount = 20};
PlayFabClientAPI.GetLeaderboard(requestLeaderboard,OnGetLeaderboard,OnErrorLeaderboard);
}

void OnGetLeaderboard(GetLeaderboardResult result)
{
    //Debug.Log(result.Leaderboard[0].StatValue);
    foreach(PlayerLeaderboardEntry player in result.Leaderboard){
        Debug.Log(player.DisplayName+ ": " + player.StatValue);
    }
}

void OnErrorLeaderboard(PlayFabError error)
{
    Debug.LogError(error.GenerateErrorReport());
}

#endregion Leaderboard


#region PlayerData

void GetStatistics()
{
    PlayFabClientAPI.GetPlayerStatistics(
        new GetPlayerStatisticsRequest(),
        OnGetStatistics,
        error => Debug.LogError(error.GenerateErrorReport())
    );
}


void OnGetStatistics(GetPlayerStatisticsResult result)
{
    Debug.Log("Received the following Statistics:");
    foreach (var eachStat in result.Statistics){
        Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
        switch(eachStat.StatisticName){
            case "Hat" :
            playerHat = eachStat.Value;
            break;
             case "Top" :
            playerTop = eachStat.Value;
            break;
             case "Jacket" :
            playerJacket = eachStat.Value;
            break;
             case "Underware" :
            playerUnderware = eachStat.Value;
            break;
             case "Shoes" :
            playerShoes = eachStat.Value;
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
    if(result.Data == null || !result.Data.ContainsKey("Skins"))
    {
        Debug.Log("Skins not set");
    }
    else
    {
        PersistantData.PD.SkinsStringToData(result.Data["Skins"].Value);
    }
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