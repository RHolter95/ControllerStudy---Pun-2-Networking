using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class NetworkObjectsClass 
{
    [SerializeField]
    private string _player;
    [SerializeField]
    private PhotonView _photonView;
    [SerializeField]
    private string _photonUserID = PhotonNetwork.LocalPlayer.UserId;

    public string PhotonUserID { get { return _photonUserID; } set { _photonUserID = value; } }
    public string Player { get { return _player; } set { _player = value; } }
    public PhotonView viewID { get { return _photonView; } set { _photonView = value; } }

    public NetworkObjectsClass(string player, PhotonView photonView, string photonUserID)
    {
        _photonUserID = photonUserID;
        _player = player;
        _photonView = photonView;
    }
}