using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothes : MonoBehaviour
{

    public GameObject clone = null;
    public GameObject characterIdle = null;

    public CharacterCustomization CharacterCustomization = null;

    void Awake()
    {
        /*
        characterIdle = clone.transform.GetChild(0).gameObject;
        CharacterCustomization = clone.GetComponent<CharacterCustomization>();
        if(CharacterCustomization == null)
        {
            Debug.Log("Couldn't find CharacterCustomize script on clone");
        }
            int i = 0;
            foreach (var item in CharacterCustomization.pantsPresets)
            {
                i++;
                Debug.Log("Item "+i+": "+item.name);
            }

        //clone.transform.GetChild()
*/
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if(clone == null){
        //  clone = GameObject.Find("PhotonPlayerTEST(Clone)");
        // }


    }
}
