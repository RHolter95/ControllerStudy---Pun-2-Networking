using UnityEngine;
using System.Collections;
using Photon.Pun;
using System.IO;


public class FXManager : MonoBehaviour {

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

	void Start() {
	}

	void Update()
	{
	

	}

	[PunRPC]
	public void SniperBulletFX(Vector3 startPos, Vector3 endPos) {
		Debug.Log("SniperBulletFX");

		

		if(SniperBulletFXPrefab != null){
			GameObject sniperFX = (GameObject)Instantiate(SniperBulletFXPrefab, startPos ,Quaternion.LookRotation(startPos - endPos));
			
		}else{
			Debug.LogError("SniperBulletFXPrefab Is Missing");
		}
		
		
		}
}
