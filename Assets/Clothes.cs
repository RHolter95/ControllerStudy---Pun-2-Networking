using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine.SceneManagement;

public class Clothes : MonoBehaviour
{

    public bool needClothesStats = true;

    public int playerHat = 0;
    public int Playeraccessory = 0;
    public int playerTop = 0;
    public int playerJacket = 0;
    public int playerUnderware = 0;
    public int playerHead = 0;
    public int playerSkin = 0;
    public int playerBottom = 0;
    public int playerShoes = 0;
    public int playerSex = 0;
    public bool run = true;

    void Awake()
    {
        /*
        if (SceneManager.GetActiveScene().name == "Network" && run)
        {
            GetStatistics();
            run = false;
        }
        */

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "Hat":
                    playerHat = eachStat.Value;
                    break;
                case "Accessories":
                    Playeraccessory = eachStat.Value;
                    break;
                case "Top":
                    playerTop = eachStat.Value;
                    break;
                case "Jacket":
                    playerJacket = eachStat.Value;
                    break;
                case "Underware":
                    playerUnderware = eachStat.Value;
                    break;
                case "Bottom":
                    playerBottom = eachStat.Value;
                    break;
                case "Shoes":
                    playerShoes = eachStat.Value;
                    break;
                case "Skin":
                    playerSkin = eachStat.Value;
                    break;
                case "Sex":
                    playerSex = eachStat.Value;
                    break;
                case "Head":
                    playerHead = eachStat.Value;
                    break;
            }
        }
        run = false;
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }


}
