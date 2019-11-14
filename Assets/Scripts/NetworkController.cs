using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private GameObject offlineButton; 

    private GameObject onlineButton; 

    bool offlineButtonPressed = false;
    bool onlineButtonPressed = false;

    void Start()
    {
        PhotonNetwork.OfflineMode = true;
        offlineButton = GameObject.Find("Offline");
        onlineButton = GameObject.Find("Online");
    }

    public void Offline()
    {
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