using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float hitPoints = 100f;
    float currentHitPoints;

    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = hitPoints;
    }

    // Update is called once per frame
    [PunRPC]
    public void TakeDamage(float amount)
    {
        currentHitPoints -= amount;

        if(currentHitPoints <= 0){
            Die();
        }
    }

    void Die(){
        //if(gameObject.tag == "Item")
        //Destroy(gameObject);
        //if(gameObject.tag == "Player")

        //If GO was part of original level, delete.
        if(GetComponent<PhotonView>().InstantiationId == 0)
        {
            //This deletes items on the players own level.
            Destroy(gameObject);
        }
        //If GO was instantiated over the network
        else
        {   
            if(PhotonNetwork.IsMasterClient)
            {
            //This deletes INSTANTIATED items on the Network
            PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
