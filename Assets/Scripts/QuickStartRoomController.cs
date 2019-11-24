using Photon.Pun;
using UnityEngine;

public class QuickStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex; //Number for the build index to the multiplay scene.
    public static QuickStartRoomController QSRC;

    public override void OnEnable()
    {
        if(QuickStartRoomController.QSRC == null)
        {
            QuickStartRoomController.QSRC = this;
        }
        else
        {
            if(QuickStartRoomController.QSRC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    
        PhotonNetwork.AddCallbackTarget(this); 
        
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() //Callback function for when we successfully create or join a room.
    {
        Debug.Log("Joined Room");
        StartGame();
    }

    private void StartGame() //Function for loading into the multiplayer scene.
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex); //because of AutoSyncScene all players who join the room will also be loaded into the multiplayer scene.
        }
    }
}