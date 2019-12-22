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
        //currentHitPoints = hitPoints;
    }

    private void Update()
    {
        //If youre alive you have permission to die!
        if (GetComponentInParent<Animator>().GetBool("IsDead") == false)
        {
            currentHitPoints = hitPoints;
        }
    }



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

                //Set respawn timer
                GameObject.FindObjectOfType<GameSetupController>().respawnTimer = RESPAWNTIME;
                //Set Animation to "IsDead"
                GameObject.FindObjectOfType<Animator>().SetBool("IsDead", true);
                //Sends message for server stream
                GetComponent<PhotonView>().RPC("BroadcastDeath", RpcTarget.All, GetComponent<PhotonView>().ViewID);
                break;

            case "Object":
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
