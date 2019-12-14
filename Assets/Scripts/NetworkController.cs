using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks

{
     /******************************************************
     * Refer to the Photon documentation and scripting API for official definitions and descriptions
     * 
     * Documentation: https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro
     * Scripting API: https://doc-api.photonengine.com/en/pun/v2/index.html
     * 
     * If your Unity editor and standalone builds do not connect with each other but the multiple standalones
     * do then try manually setting the FixedRegion in the PhotonServerSettings during the development of your project.
     * https://doc.photonengine.com/en-us/realtime/current/connection-and-authentication/regions
     *
     * ******************************************************/

	
    public static NetworkController NWC;
    public PlayFabsController playFabManager;
    public GameObject shopCanvas = null;
    public GameObject submitRecovery = null;
    public GameObject shopButton = null;
    public GameObject loginButton = null; 
    public GameObject submitButton = null; 
    public GameObject offlineButton = null; 
    public GameObject createNewButton = null; 
    public GameObject onlineButton = null; 
    public GameObject onlineAuthButton = null; 
    public GameObject recoverButton = null;
    public GameObject userNameGO = null;
    public GameObject userEmailGO = null;
    public GameObject userPassGO = null;

    public GameObject PauseMenu = null;

    public Component userInputEmail = null;
    public Component userInputName = null;
    public Component userInputPass = null;
    public InputField userNameInput = null;
	public InputField userEmailInput = null;
	public InputField userPassInput = null;
    public string userEmailStr = null;
	public string userNameStr = null;
	public string userPassStr = null;
    bool offlineButtonPressed = false;
    bool onlineButtonPressed = false;
    //bool playFab = true;


    public GameObject maleCharCustomizer = null;
    public GameObject femaleCharCustomizer = null;
    public int charSex = 2;//Neither Male 0 or Female 1
    public GameObject maleSex = null;
    public GameObject femaleSex = null;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    

    void OnDisconnected()
    {
    }

    void Awake()
    {     
        DontDestroyOnLoad(this.gameObject); 

        //playFab = true;
        createNewButton = GameObject.Find("NewUser");
        userNameGO = GameObject.Find("UserName");
        userEmailGO = GameObject.Find("Email");
        userPassGO = GameObject.Find("Password");
        loginButton = GameObject.Find("Login");
        submitButton = GameObject.Find("Submit");
        recoverButton = GameObject.Find("Recover");
        submitRecovery = GameObject.Find("SubmitRecovery");
        shopButton = GameObject.Find("Shop");
        shopCanvas = GameObject.Find("ShopCanvas");

     

        

        //Start Char Customization   
        maleCharCustomizer = GameObject.Find("Male_Customize");
        femaleCharCustomizer = GameObject.Find("Female_Customize");
        maleSex = GameObject.Find("Female");
        femaleSex = GameObject.Find("Male");
        //End Char Customization    

        playFabManager = transform.GetComponent<PlayFabsController>();
        userEmailInput = userEmailGO.gameObject.GetComponent<InputField>();
        userNameInput = userNameGO.gameObject.GetComponent<InputField>();
        userPassInput = userPassGO.gameObject.GetComponent<InputField>();

        offlineButton = GameObject.Find("Offline");
        onlineButton = GameObject.Find("Online");
        onlineAuthButton = GameObject.Find("OnlineAuth");


        
        createNewButton.SetActive(false);
        onlineButton.SetActive(false);
        userNameGO.SetActive(false);
        userPassGO.SetActive(false);
        userEmailGO.SetActive(false);
        loginButton.SetActive(false);
        submitButton.SetActive(false);
        recoverButton.SetActive(false);
        submitRecovery.SetActive(false);
        shopButton.SetActive(false);
        shopCanvas.SetActive(false);

        //Start Char Customization    
        femaleCharCustomizer.SetActive(false);
        maleCharCustomizer.SetActive(false);
        maleSex.SetActive(false);
        femaleSex.SetActive(false);

        if (PauseMenu == null){
            Debug.Log("Couldn't Find PauseMenu");}

        if (maleSex == null){
            Debug.Log("Couldn't Find Male Sex Button");}

        if(femaleSex == null){
            Debug.Log("Couldn't Find Female Sex Button");}

        if(maleCharCustomizer == null){
            Debug.Log("Couldn't Find Male Customize OBJ");}

        if(femaleCharCustomizer == null){
            Debug.Log("Couldn't Find Female Customize OBJ");}
        //End Char Customization    
        
        if(shopCanvas == null){
            Debug.Log("Couldn't Find Shop Canvas");}
        
        if(userNameGO == null){
            Debug.Log("Couldn't Find userName Field");}
        
        if(userEmailGO == null){
            Debug.Log("Couldn't Find userEmail Field");}
        
        if(userPassGO == null){
            Debug.Log("Couldn't Find userPass Field");}

        if(loginButton == null){
            Debug.Log("Couldn't Find login Button");}

        if(shopButton == null){
            Debug.Log("Couldn't Find Shop Button ");}

        if(submitRecovery == null){
             Debug.Log("Couldn't Find Submit Recovery Button");}

        if(onlineAuthButton == null){
            Debug.Log("Couldn't Find onlineAuth Button");}

        if(offlineButton == null){
            Debug.Log("Couldn't Find Offline Button");}

        if(recoverButton == null){
            Debug.Log("Couldn't Find recover Button");}

        if(onlineButton == null){
            Debug.Log("Couldn't Find Online Button");}

        if(createNewButton == null){
            Debug.Log("Couldn't Find createNew Button");}
        
         if(submitButton == null){
            Debug.Log("Couldn't Find Submit Button");}
    }

    public void playFabCanvas()
	{
        
		Debug.Log("Prepparing PlayFab Page");
		createNewButton.SetActive(true);
		offlineButton.SetActive(false);
		onlineButton.SetActive(false);
		onlineAuthButton.SetActive(false);

		userNameGO.SetActive(false);

        userPassGO.SetActive(true);
        userEmailGO.SetActive(true);
		loginButton.SetActive(true);
        recoverButton.SetActive(false);

		userEmailInput = userEmailGO.GetComponent<InputField>();
		if(userEmailInput == null){
			Debug.Log("Couldn't Find UserEmail InputField Component");
		}
		userNameInput = userNameGO.GetComponent<InputField>(); 
		if(userNameInput == null){
			Debug.Log("Couldn't Find UserName InputField Component");
		}
		userPassInput = userPassGO.GetComponent<InputField>(); 
		if(userPassInput == null){
			Debug.Log("Couldn't Find UserPass InputField Component");
		}
		//Once the field has been edited and entered we send the info
		userEmailInput.onEndEdit.AddListener(SubmitEmail);
		//userNameInput.onEndEdit.AddListener(SubmitUserName);
		userPassInput.onEndEdit.AddListener(SubmitUserPass);	
        //we just reset username to nothing because it only needs an ACTUAL email/pass
        userNameStr = "";
	}



    public void NewUserCanvas(){
        playFabManager.WipeTextFields();
		Debug.Log("Creating new user canvas");
		createNewButton.SetActive(false);
		userNameGO.SetActive(true);
		loginButton.SetActive(false);
		submitButton.SetActive(true);
	}

    public void Back()
    {
        shopCanvas.SetActive(false);
        onlineButton.SetActive(true);
        shopButton.SetActive(true);
    }

    

    public void MakeRecoveryAccountCanvas(){
        
        userNameGO.SetActive(true);
        userPassGO.SetActive(true);
        userEmailGO.SetActive(true);
        playFabManager.WipeTextFields();

        onlineButton.SetActive(false);
        submitButton.SetActive(false);
        offlineButton.SetActive(false);
		createNewButton.SetActive(false);
		loginButton.SetActive(false);
        recoverButton.SetActive(false);
        submitRecovery.SetActive(true);
	}

    public void MakeShopCanvas()
    {
        shopButton.SetActive(false);
        onlineButton.SetActive(false);
        shopCanvas.SetActive(true);
    }

    private void SubmitEmail(string arg0){
        //Debug.Log("Email: " + arg0);
		userEmailStr = arg0;
    }

	private void SubmitUserName(string arg0){
        //Debug.Log("UserName: " + arg0);
		userNameStr = arg0;
    }

	private void SubmitUserPass(string arg0){
        //Debug.Log("Password: " + arg0);
		userPassStr = arg0;
    }

    public void Offline()
    {
      // playFab = false;
       offlineButtonPressed = true;
       PhotonNetwork.Disconnect();
    }

    void Update()
    {
        // If were initially connected and press offline play
         if(offlineButtonPressed == true)
         {
             //Let user know were DC'ing
            Debug.Log("Disconecting");

            //Once DC'ed we enter Offline Mode and Create room
            if(PhotonNetwork.IsConnected == false)
            {
                Debug.Log("Disconected");
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.CreateRoom("Offline");
            }
         }
    }

    // Start is called before the first frame update
    public void Online()
    {
     onlineButtonPressed = true;
     PhotonNetwork.OfflineMode = false;
     PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if(offlineButtonPressed == true){
        Debug.Log("We are now connected to the offline server!");
        }
        if(onlineButtonPressed == true){
        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
        PhotonNetwork.QuickStart();
        }
    }
}