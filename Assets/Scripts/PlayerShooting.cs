using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerShooting : MonoBehaviour
{

    GameObject fxManagerOBJ;
    [SerializeField]
    FXManager fxManager;
    public float fireRate = 0.5f;
    float coolDown = 0f;
    public float damage = 25f;





    void Start()
    {
        fxManagerOBJ = GameObject.Find("FXManager");
        fxManager = fxManagerOBJ.GetComponent<FXManager>();

        if(fxManager == null)
        {
            Debug.Log("Couldn't Find FXManager");
        }
    }

    // Start is called before the first frame update
    void Update()
    {
     coolDown -= Time.deltaTime;


     if(Input.GetButtonDown("Fire1")){
         Fire();
     }   
    }


    // Update is called once per frame
    void Fire()
    { 
        if(coolDown > 0){
            return;
        }


        Ray ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
        Transform hitTransform;
        Vector3 hitPoint;

       hitTransform = FindClosestHitObject(ray, out hitPoint);

        if(hitTransform.transform != null){
            Debug.Log("We Hit: " + hitTransform.name);

            //Do special effect at hit location

            Health h = hitTransform.GetComponent<Health>();

            //Search through hirarchy of OBJ parents to find if it has a Health component
            while(h == null && hitTransform.parent){
                hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<Health>();
            }

            //once we find health
            if(h != null){
                PhotonView PhotonView = h.GetComponent<PhotonView>();
                if(PhotonView == null){
                    Debug.Log("There is no PhotonView");
                }else{
                h.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                }
                //h.TakeDamage(damage); This is offline version of above code./
            }
        }
        if(fxManager != null){
            DoGunFX(hitPoint);
        }
        else
        {
            //We didnt hit anything but empty space, lets do FX anyways
            if(fxManager != null){
            hitPoint = Camera.main.transform.position + (Camera.main.transform.forward*100f);
            DoGunFX(hitPoint);
            }
        }
        coolDown = fireRate;
    }

    void DoGunFX(Vector3 hitPoint)
    {
        WeaponData wd = gameObject.GetComponentInChildren<WeaponData>();
        fxManager.GetComponent<PhotonView>().RPC("SniperBulletFX",RpcTarget.All, wd.transform.position, hitPoint);
    }

    Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint){
        RaycastHit[] hits = Physics.RaycastAll(ray);

        Transform closestHit = null;
        float distance = 0f;
        hitPoint = Vector3.zero;

        foreach(RaycastHit hit in hits)
        {
         if(hit.transform != this.transform && (closestHit == null || hit.distance < distance))  {
            closestHit = hit.transform;
            distance = hit.distance;
            hitPoint = hit.point;
         } 
        }
        return closestHit;
    }

}
