using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabsController : MonoBehaviour
{

    public static PlayFabsController PFC;

    public NetworkController networkController = null;
    public string userEmailStr = null;
	public string userNameStr = null;
	public string userPassStr = null;

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

        networkController.userEmailGO.SetActive(false);
        networkController.userNameGO.SetActive(false);
        networkController.userPassGO.SetActive(false);
        networkController.submitButton.SetActive(false);
        networkController.onlineButton.SetActive(true);
        networkController.onlineAuthButton.SetActive(false);
        GetStats();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Logged In: " + userEmailStr);
        PlayerPrefs.SetString("EMAIL", userEmailStr);
        PlayerPrefs.SetString("PASSWORD", userPassStr);

        networkController.loginButton.SetActive(false);
        networkController.createNewButton.SetActive(false);
        networkController.userEmailGO.SetActive(false);
        networkController.userNameGO.SetActive(false);
        networkController.userPassGO.SetActive(false);
        networkController.submitButton.SetActive(false);
        networkController.onlineButton.SetActive(true);
        networkController.recoverButton.SetActive(false);
        GetStats();
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

        //Doesnt need UserName to login
        userNameStr = "";

        networkController.userNameGO.SetActive(false);
        networkController.loginButton.SetActive(true);
        networkController.submitButton.SetActive(false);
        networkController.recoverButton.SetActive(false);
        WipeTextFields();
    }

    public void Submit()
	{
		Debug.Log("Sending Registration to playfab\nEMAIL: " + userEmailStr+" , USERNAME: "+userNameStr);
        var registerRequest  = new RegisterPlayFabUserRequest  { Email = userEmailStr, Password = userPassStr ,Username = userNameStr};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
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
        WipeTextFields();
    }
#endregion Login

public int playerLevel;
public int gameLevel;
public int playerHighScore;
public int playerCash;

//Player Stats function DOES NOT WORK BECAUSE OF CHEATING 
// How to Use PlayFab in Unity 3D: Player Statistics (Lesson 4) GOOGLE IT

#region PlayerStats

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

#endregion PlayerStats
}