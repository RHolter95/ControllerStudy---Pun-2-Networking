using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeController : MonoBehaviour
{
    NetworkController[] NWC = null;

    public int gameMode = 0;

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
    public int i = 0;



    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Mode: " + gameMode);
        NWC = GameObject.FindObjectsOfType<NetworkController>();

        //If Were at Menu scene we have to handle duplicate NetworkControllers and transfer data
        //This will always need to be done on "In-Game Room Transfers"
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            NWC = GameObject.FindObjectsOfType<NetworkController>();
            if (NWC == null)
            {
                Debug.Log("Couldn't find NetworkController");
            }

            for (int i = 0; i < NWC.Length; i++)
            {//Find the NWC with missing vars (chosen at random, any will do) THIS WHOLE BRANCH ONLY RUNS ON 2ND ENTRY OF MENU
                if (NWC[i].shopCanvas == null)
                {
                    
                    transform.GetChild(0).gameObject.SetActive(true);

                    //Grabs PlayFabsController Comp statistics we've pulled from server within old NetworkController
                    PlayFabsController PFC = NWC[i].GetComponent<PlayFabsController>();
                    PlayFabsController lastingPFC = null;

                    //Check the next index for THE surviving NetworkController PlayFabsController Comp
                    if (NWC[i+1] != null)
                    {
                        i++;
                        Debug.Log("Next One");
                        lastingPFC = NWC[i].GetComponent<PlayFabsController>();
                    }
                    else
                    {
                        i--;
                        Debug.Log("Previous One");
                        lastingPFC = NWC[i].GetComponent<PlayFabsController>();
                    }


                    //Transferrs info we don't want to lose 
                    lastingPFC.playerHat = PFC.playerHat;
                    lastingPFC.Playeraccessory = PFC.Playeraccessory;
                    lastingPFC.playerTop = PFC.playerTop;
                    lastingPFC.playerJacket = PFC.playerJacket;
                    lastingPFC.playerUnderware = PFC.playerUnderware;
                    lastingPFC.playerHead = PFC.playerHead;
                    lastingPFC.playerSkin = PFC.playerSkin;
                    lastingPFC.playerBottom = PFC.playerBottom;
                    lastingPFC.playerShoes = PFC.playerShoes;
                    lastingPFC.playerSex = PFC.playerSex;
                    lastingPFC.myID = PFC.myID;
                    lastingPFC.userEmailStr = PFC.userEmailStr;

                    //Deleting old NetwrokController but keeping the data it called the API for.
                    Destroy(NWC[i].transform.root.gameObject);
                    Debug.Log("Destroyed Old NetworkController");

                    
                    StartCoroutine(JoinLobbyWaiter());

                    //Find LobbyController and create room
                    //GameObject.Find("QuickStartLobbyController").GetComponent<QuickStartLobbyController>().CreateRoom();
                }
            }
        }

        //Once maintainance is complete

        //Figure out what scene to load and create room
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    


    IEnumerator JoinLobbyWaiter()
    {
        //PhotonNetwork.JoinLobby();
/*
        while (!PhotonNetwork.InLobby)
        {
            Debug.Log("Not in lobby!");
            yield return null;
        }
        */
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Not Ready!");
            yield return null;
        }
        GameObject.Find("QuickStartLobbyController").GetComponent<QuickStartLobbyController>().CreateRoom();

    }

}
