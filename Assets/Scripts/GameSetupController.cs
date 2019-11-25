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
        
        GameObject Myplayer = (GameObject)PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayerTEST"), new Vector3 (-2.8f,0.0f,0.736f), Quaternion.identity);
        ((MonoBehaviour)Myplayer.GetComponent("PlayerMove")).enabled = true;
        ((MonoBehaviour)Myplayer.GetComponent("NetworkPlayer")).enabled = true;
        ((MonoBehaviour)Myplayer.GetComponent("PlayerShooting")).enabled = true;
        Myplayer.transform.GetChild(2).Find("Main Camera").gameObject.SetActive(true);
        Myplayer.transform.Find("Canvas").gameObject.SetActive(true);
    }
}

//We need to setup prefab in Assets/Scenes/Female_Customizable to not have anything
//that that char when instantiated will need. We need this prefab to look
//EXACTLY like the COP prefab but with a female swapped prefab

//Remove stand, maincamera, ambient light, yellow light,canvas,eventsystem,
//Add MobileCanvas and CameraPivot to
//Female_Customizable should = Female_Customizable,MobileCanvas,CameraPivot
//Character customizer stand w/ lights and camera need to be seperate prefab