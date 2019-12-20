using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StayOnStage : MonoBehaviour
{
    public Vector3 stageCenterVector;
    public GameObject target = null;

    // Start is called before the first frame update
    void Awake()
    {

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            stageCenterVector = GameObject.Find("StageCenter").transform.position;
        }

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
