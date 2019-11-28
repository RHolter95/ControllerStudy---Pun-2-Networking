using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    public PlayFabsController PFC = null;
    [SerializeField]
    private GameObject quickStartButton; //button used for creating and joining a game.
    [SerializeField]
    private GameObject quickCancelButton; //button used to stop searching for a game to join.
    private GameObject onlineButton;

    private GameObject offlineButton; 

    [SerializeField]
    private int RoomSize = 4; //Manual set the number of player in the room at one time.
    public static QuickStartLobbyController QSLC;

   //IF YOU MAKE THIS PERSIST SAY GOODBYE TO YOUR MATCHMAKING
   //YOU WONT CONNECT TO A SERVER
   //Enable() == DEATH
    void Awake()
    {
        PFC = GameObject.Find("NetworkController").GetComponentInChildren<PlayFabsController>();
        if(PFC == null){
            Debug.Log("There is no PlayFabsController");
        }
    }

    public override void OnConnectedToMaster() //Callback function for when the first connection is established successfully.
    {
        PhotonNetwork.AutomaticallySyncScene = true; //Makes it so whatever scene the master client has loaded is the scene all other clients will load
//      quickStartButton.SetActive(false);
        onlineButton.SetActive(true);
        offlineButton.SetActive(true);
    }

    void Start()
    {
        onlineButton = GameObject.Find("OnlineAuth");
        offlineButton = GameObject.Find("Offline");
        quickStartButton = GameObject.Find("QuickStart");
        quickCancelButton = GameObject.Find("QuickCancel");
//      quickStartButton.SetActive(false);
        onlineButton.SetActive(false);
        offlineButton.SetActive(false);
    }

    public void QuickStart() //Paired to the Quick Start button
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();

        PFC.GetStatistics();
        
        //quickStartButton.SetActive(false);
        //quickCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(); //First tries to join an existing room
        Debug.Log("Quick start");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) //Callback function for if we fail to join a rooom
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    void CreateRoom() //trying to create our own room
    {
        Debug.Log("Creating room now");
        int randomRoomNumber = Random.Range(0, 10000); //creating a random name for the room
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps); //attempting to create a new room
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //callback function for if we fail to create a room. Most likely fail because room name was taken.
    {
        Debug.Log("Failed to create room... trying again");
        CreateRoom(); //Retrying to create a new room with a different name.
    }

    public void QuickCancel() //Paired to the cancel button. Used to stop looking for a room to join.
    {
        quickCancelButton.SetActive(false);
        quickStartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}