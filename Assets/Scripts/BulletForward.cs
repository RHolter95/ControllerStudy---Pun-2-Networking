using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletForward : MonoBehaviour
{
    public float bulletSpeed = 130f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.back * Time.deltaTime * bulletSpeed);
    }
}
