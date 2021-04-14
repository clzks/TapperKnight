using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private string path = "Assets/Data/";
    private string texturePath = "Textures/Background/Stage";

    private Dictionary<int, StageModel> stageList;
    private Dictionary<int, EnemyModel> enemyList;
    private Dictionary<int, List<BackgroundModel>> backgroundModelList;
    private Dictionary<int, Dictionary<int, Texture>> textureList;

    public void Awake()
    {
        stageList = new Dictionary<int, StageModel>();
        enemyList = new Dictionary<int, EnemyModel>();
        backgroundModelList = new Dictionary<int, List<BackgroundModel>>();
        textureList = new Dictionary<int, Dictionary<int, Texture>>();

        LoadStageData();
        LoadEnemyData();
        LoadBackground();
    }

    public TextAsset LoadTextAsset(string name)
    {
        TextAsset textAsset;
        name = path + name + "Data.json";

        textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(name);
        return textAsset;
    }

    private void LoadStageData()
    {
        TextAsset stageData = LoadTextAsset("Stage");
        JArray stages = JObject.Parse(stageData.text)["Stage"] as JArray;
        for (int i = 0; i < stages.Count; ++i)
        {
            var stage = stages[i].ToObject<StageModel>();
            stageList.Add(stage.StageNumber, stage);
        }
    }

    private void LoadEnemyData()
    {
        TextAsset stageData = LoadTextAsset("Enemy");
        JArray enemys = JObject.Parse(stageData.text)["Enemy"] as JArray;
        for (int i = 0; i < enemys.Count; ++i)
        {
            var enemy = enemys[i].ToObject<EnemyModel>();
            enemyList.Add(enemy.Id, enemy);
        }
    }

    private void LoadBackground()
    {
        TextAsset bgData = LoadTextAsset("Background");
        JArray backgroundListData = JObject.Parse(bgData.text)["Background"] as JArray;
        for(int i = 0; i < backgroundListData.Count; ++i)
        {
            var listModel = backgroundListData[i].ToObject<BackgroundListModel>();
            var backgroundList = listModel.List;
            Dictionary<int, Texture> singleStageTexList = new Dictionary<int, Texture>();
            foreach (var item in backgroundList)
            {
                Texture tex = GetTextureFromResources(listModel.StageNumber, item.LayerNumber);
                singleStageTexList.Add(item.LayerNumber, tex);
            }
            textureList.Add(listModel.StageNumber, singleStageTexList);
            backgroundModelList.Add(listModel.StageNumber, backgroundList);
        }
    }

    private Texture GetTextureFromResources(int stageNumber, int layerNumber)
    {
        Texture texture = Resources.Load<Texture>(texturePath + stageNumber.ToString() + "/Layer" + layerNumber);
        return texture;
    }

    public Dictionary<int, StageModel> GetStageList()
    {
        return stageList;
    }

    public Dictionary<int, EnemyModel> GetEnemyList()
    {
        return enemyList;
    }

    public Texture GetTexture(int stageNumber, int layerNumber)
    {
        return textureList[stageNumber][layerNumber];
    }

    public List<BackgroundModel> GetBackgroundList(int stageNumber)
    {
        return backgroundModelList[stageNumber];
    }
}
