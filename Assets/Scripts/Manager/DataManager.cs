using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private string path = "Assets/Data/";

    private Dictionary<int, StageModel> stageList;
    private Dictionary<int, EnemyModel> enemyList;
    
    public void Awake()
    {
        stageList = new Dictionary<int, StageModel>();
        enemyList = new Dictionary<int, EnemyModel>();

        LoadStageData();
        LoadEnemyData();
    }

    public TextAsset LoadTextAsset(string name)
    {
        TextAsset textAsset;
        name = path + name + "Data.json";

        textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(name);
        return textAsset;
    }

    public void LoadStageData()
    {
        TextAsset stageData = LoadTextAsset("Stage");
        JArray stages = JObject.Parse(stageData.text)["Stage"] as JArray;
        for (int i = 0; i < stages.Count; ++i)
        {
            var stage = stages[i].ToObject<StageModel>();
            stageList.Add(stage.StageNumber, stage);
        }
    }

    public void LoadEnemyData()
    {
        TextAsset stageData = LoadTextAsset("Enemy");
        JArray enemys = JObject.Parse(stageData.text)["Enemy"] as JArray;
        for (int i = 0; i < enemys.Count; ++i)
        {
            var enemy = enemys[i].ToObject<EnemyModel>();
            enemyList.Add(enemy.Id, enemy);
        }
    }

    public Dictionary<int, StageModel> GetStageList()
    {
        return stageList;
    }

    public Dictionary<int, EnemyModel> GetEnemyList()
    {
        return enemyList;
    }
}
