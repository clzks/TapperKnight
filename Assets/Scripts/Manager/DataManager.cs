using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private string _path = "Assets/Data/";
    private string _texturePath = "Textures/Background/Stage";

    private Dictionary<int, StageModel> _stageList;
    private Dictionary<int, EnemyModel> _enemyList;
    private Dictionary<int, List<BackgroundModel>> _backgroundModelList;
    private Dictionary<int, Dictionary<int, Texture>> _textureList;
    private Dictionary<ScoreType, int> _scoreList; 
    public void Awake()
    {
        _stageList = new Dictionary<int, StageModel>();
        _enemyList = new Dictionary<int, EnemyModel>();
        _backgroundModelList = new Dictionary<int, List<BackgroundModel>>();
        _textureList = new Dictionary<int, Dictionary<int, Texture>>();
        _scoreList = new Dictionary<ScoreType, int>();

        LoadStageData();
        LoadEnemyData();
        LoadBackground();
        LoadScoreData();
    }

    public TextAsset LoadTextAsset(string name)
    {
        TextAsset textAsset;
        name = _path + name + "Data.json";

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
            _stageList.Add(stage.StageNumber, stage);
        }
    }

    private void LoadEnemyData()
    {
        TextAsset stageData = LoadTextAsset("Enemy");
        JArray enemys = JObject.Parse(stageData.text)["Enemy"] as JArray;
        for (int i = 0; i < enemys.Count; ++i)
        {
            var enemy = enemys[i].ToObject<EnemyModel>();
            _enemyList.Add(enemy.Id, enemy);
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
            _textureList.Add(listModel.StageNumber, singleStageTexList);
            _backgroundModelList.Add(listModel.StageNumber, backgroundList);
        }
    }

    private void LoadScoreData()
    {
        TextAsset scoreData = LoadTextAsset("Score");
        JArray scores = JObject.Parse(scoreData.text)["Score"] as JArray;

        for(int i = 0; i < scores.Count; ++i)
        {
            var score = scores[i].ToObject<ScoreModel>();
            _scoreList.Add(score.Type, score.ScoreValue);
        }
    }

    private Texture GetTextureFromResources(int stageNumber, int layerNumber)
    {
        Texture texture = Resources.Load<Texture>(_texturePath + stageNumber.ToString() + "/Layer" + layerNumber);
        return texture;
    }

    public Dictionary<int, StageModel> GetStageList()
    {
        return _stageList;
    }

    public Dictionary<int, EnemyModel> GetEnemyList()
    {
        return _enemyList;
    }

    public Texture GetTexture(int stageNumber, int layerNumber)
    {
        if(true == _textureList.ContainsKey(stageNumber))
        {
            return _textureList[stageNumber][layerNumber];
        }
        else
        {
            Debug.LogWarning("배경 텍스쳐리스트에 해당 스테이지 정보가 없습니다. 스테이지 1의 배경텍스쳐를 가져옵니다");
            return _textureList[1][layerNumber];
        }
    }

    public List<BackgroundModel> GetBackgroundList(int stageNumber)
    {
        if (true == _backgroundModelList.ContainsKey(stageNumber))
        {
            return _backgroundModelList[stageNumber];
        }
        else
        {
            Debug.LogWarning("배경리스트에 해당 스테이지 정보가 없습니다. 스테이지 1의 배경을 가져옵니다");
            return _backgroundModelList[1];
        }
    }

    public Dictionary<ScoreType, int> GetScoreList()
    {
        return _scoreList;
    }
}
