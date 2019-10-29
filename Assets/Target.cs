using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{


    public Transform player;
    public Transform target;
    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void LateUpdate()
    {
    camera.transform.LookAt(target);
    camera.transform.Rotate(0,0,0);
    }
}
