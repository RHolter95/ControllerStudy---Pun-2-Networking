using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayOnStage : MonoBehaviour
{
    public Vector3 stageCenterVector;

    // Start is called before the first frame update
    void Awake()
    {
        stageCenterVector = GameObject.Find("StageCenter").transform.position;
    }

    void Update()
    {
        this.transform.position =  stageCenterVector;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position =  stageCenterVector;
    }
}
