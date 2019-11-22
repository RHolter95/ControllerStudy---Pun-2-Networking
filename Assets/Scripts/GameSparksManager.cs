using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// Created for GameSparks tutorial, October 2015, Sean Durkan
/// This class sets up the GameSparksUnity script with a persistant gameobjec
/// </summary>
public class GameSparksManager : MonoBehaviour 
{
	/// <summary>The GameSparks Manager singleton</summary>
	private static GameSparksManager instance = null;
	public NetworkController networkController;
	public Component registerPlayer;
	public InputField userName = null;
	public InputField userEmail = null;
	public InputField userPass = null;
	public string userEmailStr = null;
	public string userNameStr = null;
	public string userPassStr = null;

	private void SubmitEmail(string arg0){
		userEmailStr = arg0;
    }

	private void SubmitUserName(string arg0){
		userNameStr = arg0;
    }

	private void SubmitUserPass(string arg0){
		userPassStr = arg0;
    }

	public void Login()
	{
	
	}

	public void NewUserCanvas(){
		Debug.Log("Creating new user canvas");
		networkController.createNewButton.SetActive(false);
		networkController.userNameGO.SetActive(true);
		networkController.loginButton.SetActive(false);
		networkController.submitButton.SetActive(true);
	}

	public void Submit()
	{
		
	}

	//MORE FUNCTIONS HERE

}
