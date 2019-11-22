using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

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
        userEmailStr = networkController.userEmailStr;
        userNameStr = networkController.userNameStr;
        userPassStr = networkController.userPassStr;
    }

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)){
            PlayFabSettings.TitleId = "EF1CC"; // Please change this value to your own titleId from PlayFab Game Manager
        }

        //PlayerPrefs.DeleteAll();

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
            #endif
        }
    }

    public void Login()
	{
        var request = new LoginWithEmailAddressRequest { Email = userEmailStr, Password = userPassStr};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
	}

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Logged In: " + userEmailStr);
        PlayerPrefs.SetString("EMAIL", userEmailStr);
        PlayerPrefs.SetString("PASSWORD", userPassStr);

        networkController.userEmailGO.SetActive(false);
        networkController.userNameGO.SetActive(false);
        networkController.userPassGO.SetActive(false);
        networkController.submitButton.SetActive(false);
        networkController.onlineButton.SetActive(true);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());;
    }


    private void OnLoginFailure(PlayFabError error)
    {
        var registerRequest = new RegisterPlayFabUserRequest{Email = userEmailStr, Password = userPassStr, Username = userNameStr};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest,OnRegisterSuccess,OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        PlayerPrefs.SetString("EMAIL", userEmailStr);
        PlayerPrefs.SetString("PASSWORD", userPassStr);

        networkController.loginButton.SetActive(true);
        networkController.submitButton.SetActive(false);

        networkController.userNameInput.text = "";
        networkController.userEmailInput.text = "";
        networkController.userPassInput.text = "";


    }

    public void Submit()
	{
		Debug.Log("Sending Registration to playfab\nEMAIL: " + userEmailStr+" , USERNAME: "+userNameStr);
        var request = new LoginWithEmailAddressRequest { Email = userEmailStr, Password = userPassStr};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
	}

}