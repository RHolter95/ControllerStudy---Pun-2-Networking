using UnityEngine;
using System.Collections;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

public class FXManager : MonoBehaviour 
{

    public GameSetupController GSC = null;
    public float soundCoolDown = 3f;
	public int audioSourceHash;
	public GameObject SniperBulletFXPrefab;
	public AudioClip SniperBulletFXAudio;
	public WeaponData wd;
	float distance;
	float counter;
	LineRenderer lr;
	public GameObject lineRendererOBJ;

	public Vector3 p0,p1;
	//float lineDrawSpeed = 6f;
	Vector3 endPosition ;
	GameObject bullet;
	//float bulletSpeed = 100f;
	WeaponData weaponData;
	Vector3 weaponDataTransform;
	

	void Awake(){
		//This will find weapon in hands @ start
		//Wont work if has no weapon and give one later
		//weaponData = gameObject.GetComponentInChildren<WeaponData>();
		//weaponDataTransform = weaponData.transform.position;
	}

	void Start() 
    {
        //Looks for GameSetupController as long as were in a game and NOT in Menu
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            GSC = GameObject.Find("GameSetupController").GetComponent<GameSetupController>();
            if (GSC == null)
            {
                Debug.Log("Network Player Couldn't find GameSetupController  ");
            }
        }
    }

	void Update()
	{
	

	}

	[PunRPC]
	public void SniperBulletFX(Vector3 startPos, Vector3 endPos) {
		Debug.Log("SniperBulletFX");
        foreach (var item in GSC.WeaponFXPooling)
        {
            //Search for a sniper FX OBJ in list that is NOT playing currently and places @ shot location and plays audio
            if (item && item.GetComponent<AudioSource>().isPlaying == false)
            {
                item.transform.root.position = startPos;
                item.transform.root.rotation = Quaternion.LookRotation(startPos - endPos);
                item.GetComponent<AudioSource>().enabled = true;
                item.GetComponent<AudioSource>().Play();
                return;
            }
        }
       		
	}
}
