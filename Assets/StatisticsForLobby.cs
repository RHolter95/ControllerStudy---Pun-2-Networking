using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnderdogCity {
[System.Serializable]
public class StatisticsForLobby : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    [SerializeField]
    PhotonView photonView = null;
    [SerializeField]
    GameSetupController GSC = null;

void Awake()
{
    GSC = transform.GetComponent<GameSetupController>();
    if (GSC == null)
    {
        Debug.Log("Network Player Couldn't find GameSetupController  ");
    }
}

void Update()
{
    if (photonView == null)
    {
        photonView = GSC.networkObjects[0].viewID;
    }
}

//When a player on remote client leaves their PhotonNetwork.UserID is sent here
//to be removed from GSC.networkOBJs<List> on remaining players in game
public override void OnPlayerLeftRoom(Player otherPlayer)
{
    for (int i = 0; i < GSC.networkObjects.Count; i++)
    {
        if (GSC.networkObjects[i].PhotonUserID == otherPlayer.UserId)
        {
            Debug.Log("Player: " + GSC.networkObjects[i].PhotonUserID + " Has Left The Game");

            GSC.networkObjects.RemoveAt(i);
            PhotonNetwork.RemoveRPCs(GSC.networkObjects[i].viewID);
            PhotonNetwork.Destroy(GSC.networkObjects[i].viewID);
        }
    }
}

}
}
