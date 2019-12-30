using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetMuzzleFlash : MonoBehaviour
{
    //The length of the sound I want to play, We might need to change this based off weaponType with a switch or something
    public float timer = 0.0f;


    // Update is called once per frame
    void Update()
    {
        //If the sound has ran atleast "timer" time then Stop() audio
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                if (transform.parent.GetComponent<AudioSource>().isPlaying)
                {
                    transform.parent.GetComponent<AudioSource>().Stop();
                }
                transform.gameObject.SetActive(false);
            }
        }
    }
}
