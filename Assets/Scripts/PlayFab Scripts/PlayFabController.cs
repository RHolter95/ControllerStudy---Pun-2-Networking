using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabLogin : MonoBehaviour
{

    public NetworkController networkController = null;
    public string userEmailStr = null;
	public string userNameStr = null;
	public string userPassStr = null;

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
        userEmailStr = networkController.userEmailGO.GetComponent<InputField>().text;
        userNameStr = networkController.userNameGO.GetComponent<InputField>().text;
        userPassStr = networkController.userPassGO.GetComponent<InputField>().text;
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

        networkController.loginButton.SetActive(true);
        networkController.submitButton.SetActive(false);
        networkController.recoverButton.SetActive(false);
        WipeTextFields();

        // networkController.userNameInput.text = "";
        // networkController.userEmailInput.text = "";
        // networkController.userPassInput.text = "";
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

}