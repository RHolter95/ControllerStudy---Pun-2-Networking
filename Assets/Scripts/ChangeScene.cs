using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    NetworkController NWC = null;
    public void ChangeSceneController(int gameMode)
    {
        //Finding our NetworkController to manage Game Mode and passing it game mode int from scene
        if (NWC == null) { NWC = GameObject.Find("NetworkController").GetComponent<NetworkController>(); }
        NWC.gameMode = gameMode;
        Debug.Log("Launching Scene Change");
        StartCoroutine(DisconnetcAndLoad());
    }

    private void Awake()
    {
        NWC = GameObject.Find("NetworkController").GetComponent<NetworkController>();
    }

    IEnumerator DisconnetcAndLoad()
    {
        //Leaving current room
        PhotonNetwork.LeaveRoom();

        while (PhotonNetwork.InRoom)
        {
            Debug.Log("Disconnecting!");
            yield return null;
        }

        //Load original scene(BECAUSE WE HAVE TO!)
        PhotonNetwork.LoadLevel(0);
    }
}
