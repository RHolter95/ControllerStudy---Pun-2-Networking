using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    // This script will be added to any multiplayer scene
    void Start()
    {
        CreatePlayer(); //Create a networked player object for each player that loads into the multiplayer scenes.
    }

    private void CreatePlayer()
    {
        //THIS IS POTENTIALLY HOW TO SPAWN AT ANY VECTOR MEANING WE CAN SETUP "SPAWN POINTS"
        //Debug.Log("Creating Player");
        
        GameObject Myplayer = (GameObject)PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), new Vector3 (-2.8f,0.0f,0.736f), Quaternion.identity);
        ((MonoBehaviour)Myplayer.GetComponent("PlayerMove")).enabled = true;
        ((MonoBehaviour)Myplayer.GetComponent("NetworkPlayer")).enabled = true;
        Myplayer.transform.Find("Main Camera").gameObject.SetActive(true);
        Myplayer.transform.Find("Canvas").gameObject.SetActive(true);
    }
}