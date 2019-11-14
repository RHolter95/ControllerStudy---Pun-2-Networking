using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SelfDestruct : MonoBehaviour
{
    public float selfDestructTime = 1f;

    public PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        selfDestructTime -= Time.deltaTime;
        if(selfDestructTime <= 0){

            //PhotonView pv = GetComponent<PhotonView>();
            if(this.pv.IsMine == true && pv.InstantiationId != 0){
                //This deletes INSTANTIATED items on the Network
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            
        }
    }
}
