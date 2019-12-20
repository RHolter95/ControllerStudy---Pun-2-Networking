using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float hitPoints = 100f;
    float currentHitPoints;
    float RESPAWNTIME = 5f;

    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = hitPoints;
    }

    // Update is called once per frame
    [PunRPC]
    public void TakeDamage(float amount, string tag)
    {
        currentHitPoints -= amount;

        if(currentHitPoints <= 0){
            Die(tag);
        }
    }

    void Die(string tag)
    {

        switch (tag)
        {

            case "Player":

                GameObject.FindObjectOfType<GameSetupController>().respawnTimer = RESPAWNTIME;

                //If GO was instantiated over the network
                if (GetComponent<PhotonView>().IsMine)
                {
                    //use this to ensure the destroy message is only sent once over the network y photonview owner

                    //This deletes INSTANTIATED items on the Network
                    PhotonNetwork.Destroy(gameObject);
                    
                }
                break;

            case "Item":
                //If GO was part of original level, delete.
                if (GetComponent<PhotonView>().InstantiationId == 0)
                {
                    //This deletes items on the players own level.
                    Destroy(gameObject);
                }
                //If GO was instantiated over the network
                else
                {
                    //use this to ensure the destroy message is only sent once over the network
                    if (PhotonNetwork.IsMasterClient)
                    {
                        //This deletes INSTANTIATED items on the Network
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
                break;
        }

    }
}
