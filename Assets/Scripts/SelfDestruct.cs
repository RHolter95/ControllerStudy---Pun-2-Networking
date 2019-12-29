using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Commented out because bullet FXs should never delete since were pooling OBJs and NEVER instantiating beyond start of game
public class SelfDestruct : MonoBehaviour
{
    /*
    private float selfDestructTime = 3f;
    bool hasRan = false;

    public PhotonView pv;
    public GameObject thisObj = null;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        thisObj = transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        selfDestructTime = selfDestructTime - Time.deltaTime;

        if(selfDestructTime <= 0 && hasRan == false){

            hasRan = true;

            if(pv.InstantiationId != 0 && pv != null && thisObj != null){
                //This deletes INSTANTIATED items on the Network
                PhotonNetwork.Destroy(thisObj);
            }
            if(pv.InstantiationId == 0 && pv != null && thisObj != null) 
            {
                Destroy(thisObj);
            }
        }
    }
    */
}
