using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantData : MonoBehaviour
{
    public static PersistantData PD;
    public bool[] allSkins;
    public int mySkin;

   
    private void OnEnable()
    {
        if(PersistantData.PD == null)
        {
            PersistantData.PD = this;
        }
        else
        {
            if(PersistantData.PD != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void SkinsStringToData(string skinsIn)
    {
        for (int i = 0; i < skinsIn.Length; i++)
        {
         if(int.Parse(skinsIn[i].ToString()) > 0)
         {
             allSkins[i] = true;
         }
         else
         {
             allSkins[i] = false;
         }   
        }

        MenuController.MC.SetUpStore();
    }

    public string SkinsDataToString()
    {
        string toString = "";
        for (int i = 0; i < allSkins.Length; i++)
        {
            if(allSkins[i] == true)
            {
                toString += "1";
            }
            else
            {
                toString += "0";
            }
        }
        return toString;
    }

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
