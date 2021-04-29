using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private string _path = "Assets/Data/";
    private string _texturePath = "Textures/Background/Stage";
    private Dictionary<ScoreType, Sprite> _scoreSpriteList;
    private Dictionary<string, Sprite> _noteSpriteList;
    private Dictionary<int, StageModel> _stageList;
    private Dictionary<int, EnemyModel> _enemyList;
    private Dictionary<int, List<BackgroundModel>> _backgroundModelList;
    private Dictionary<int, Dictionary<int, Texture>> _textureList;
    private Dictionary<ScoreType, ScoreModel> _scoreList; 
    public void Awake()
    {
        _noteSpriteList = new Dictionary<string, Sprite>();
        _stageList = new Dictionary<int, StageModel>();
        _enemyList = new Dictionary<int, EnemyModel>();
        _backgroundModelList = new Dictionary<int, List<BackgroundModel>>();
        _textureList = new Dictionary<int, Dictionary<int, Texture>>();
        _scoreList = new Dictionary<ScoreType, ScoreModel>();

        LoadScoreSprite().Forget();
        LoadNoteSprite().Forget();
        LoadStageData().Forget();
        LoadEnemyData().Forget();
        LoadBackground().Forget();
        LoadScoreData().Forget();
    }

    public async UniTaskVoid LoadScoreSprite()
    {
        _scoreSpriteList = new Dictionary<ScoreType, Sprite>();
        _scoreSpriteList.Add(ScoreType.Perfect, Resources.Load<Sprite>("Sprites/Score/Perfect"));
        _scoreSpriteList.Add(ScoreType.Great, Resources.Load<Sprite>("Sprites/Score/Great"));
        _scoreSpriteList.Add(ScoreType.Good, Resources.Load<Sprite>("Sprites/Score/Good"));
        _scoreSpriteList.Add(ScoreType.Bad, Resources.Load<Sprite>("Sprites/Score/Bad"));
        _scoreSpriteList.Add(ScoreType.Miss, Resources.Load<Sprite>("Sprites/Score/Miss"));
        await UniTask.Yield();
    }

    public async UniTaskVoid LoadNoteSprite()
    {
        _noteSpriteList.Add("Left", Resources.Load<Sprite>("Sprites/Left"));
        _noteSpriteList.Add("Right", Resources.Load<Sprite>("Sprites/Right"));
        _noteSpriteList.Add("BothSide", Resources.Load<Sprite>("Sprites/BothSide"));
        await UniTask.Yield();
    }

    public TextAsset LoadTextAsset(string name)
    {
        TextAsset textAsset;
        name = _path + name + "Data.json";

        textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(name);
        return textAsset;
    }

    private async UniTaskVoid LoadStageData()
    {
        TextAsset stageData = LoadTextAsset("Stage");
        JArray stages = JObject.Parse(stageData.text)["Stage"] as JArray;
        for (int i = 0; i < stages.Count; ++i)
        {
            var stage = stages[i].ToObject<StageModel>();
            _stageList.Add(stage.StageNumber, stage);
        }
        await UniTask.Yield();
    }

    private async UniTaskVoid LoadEnemyData()
    {
        TextAsset stageData = LoadTextAsset("Enemy");
        JArray enemys = JObject.Parse(stageData.text)["Enemy"] as JArray;
        for (int i = 0; i < enemys.Count; ++i)
        {
            var enemy = enemys[i].ToObject<EnemyModel>();
            _enemyList.Add(enemy.Id, enemy);
        }
        await UniTask.Yield();
    }

    private async UniTaskVoid LoadBackground()
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
        await UniTask.Yield();
    }

    private async UniTaskVoid LoadScoreData()
    {
        TextAsset scoreData = LoadTextAsset("Score");
        JArray scores = JObject.Parse(scoreData.text)["Score"] as JArray;

        for(int i = 0; i < scores.Count; ++i)
        {
            var score = scores[i].ToObject<ScoreModel>();
            _scoreList.Add(score.Type, score);
        }
        await UniTask.Yield();
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

    public Dictionary<ScoreType, ScoreModel> GetScoreList()
    {
        return _scoreList;
    }

    public Dictionary<string, Sprite> GetSpriteList()
    {
        return _noteSpriteList;
    }

    public Dictionary<ScoreType, Sprite> GetScoreSpriteList()
    {
        return _scoreSpriteList;
    }
}
