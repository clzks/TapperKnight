using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : Singleton<DataManager>
{
    //private string _path = "Assets/Data/";
    private string _texturePath = "Textures/Background/Stage";
    private Dictionary<ScoreType, Sprite> _scoreSpriteList;
    private Dictionary<string, Sprite> _noteSpriteList;
    private Dictionary<int, StageModel> _stageList;
    private Dictionary<int, EnemyModel> _enemyList;
    private Dictionary<int, List<BackgroundModel>> _backgroundModelList;
    private Dictionary<int, Dictionary<int, Texture>> _textureList;
    private Dictionary<int, CharacterModel> _characterList;
    private Dictionary<ScoreType, ScoreModel> _scoreList;
    private PlayerModel _playerModel;
    public async UniTask GetDataAsync()
    {
        _noteSpriteList = new Dictionary<string, Sprite>();
        _stageList = new Dictionary<int, StageModel>();
        _enemyList = new Dictionary<int, EnemyModel>();
        _backgroundModelList = new Dictionary<int, List<BackgroundModel>>();
        _textureList = new Dictionary<int, Dictionary<int, Texture>>();
        _scoreList = new Dictionary<ScoreType, ScoreModel>();
        _characterList = new Dictionary<int, CharacterModel>();

        LoadScoreSprite().Forget();
        LoadNoteSprite().Forget();
        await LoadStageData();
        await LoadEnemyData();
        await LoadBackground();
        await LoadScoreData();
        await LoadCharacterData();
        await LoadPlayerData();
        await UniTask.Yield();
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
    
    public async UniTask<string> LoadTextAsset(string name)
    {
#if UNITY_EDITOR
        //TextAsset textAsset;
        //textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(name);
        //textAsset = Resources.Load("Data/" + name + "Data") as TextAsset;
        //return textAsset.text;
        var filePath = Path.Combine(Application.streamingAssetsPath, name + "Data.json");
#elif UNITY_ANDROID
        var filePath = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", name + "Data.json");
#endif 
        var text = await LoadJsonString(filePath);
        return text;
    }

    private async UniTask<string> LoadJsonString(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get($"{url}");
        while(!www.isDone)
        {
            await www.SendWebRequest();
            Debug.Log("SendWebRequest 중");
        }
        return www.downloadHandler.text;
    }

    private async UniTask LoadStageData()
    {
        var stageData = await LoadTextAsset("Stage");
        Debug.Log(stageData);
        JArray stages = JObject.Parse(stageData)["Stage"] as JArray;
        for (int i = 0; i < stages.Count; ++i)
        {
            var stage = stages[i].ToObject<StageModel>();
            _stageList.Add(stage.StageNumber, stage);
        }

        if(_stageList.Count == 0)
        {
            Debug.LogWarning("스테이지 불러오기 실패");
        }
        else
        {
            Debug.Log("스테이지 불러오기 성공");
        }
        await UniTask.Yield();
    }

    private async UniTask LoadEnemyData()
    {
        var enemyList = await LoadTextAsset("Enemy");

        JArray enemys = JObject.Parse(enemyList)["Enemy"] as JArray;
        for (int i = 0; i < enemys.Count; ++i)
        {
            var enemy = enemys[i].ToObject<EnemyModel>();
            _enemyList.Add(enemy.Id, enemy);
        }

        if (_enemyList.Count == 0)
        {
            Debug.LogWarning("이네미 불러오기 실패");
        }
        else
        {
            Debug.Log("이네미 불러오기 성공");
        }
        await UniTask.Yield();
    }

    private async UniTask LoadBackground()
    {
        var bgData = await LoadTextAsset("Background");

        JArray backgroundListData = JObject.Parse(bgData)["Background"] as JArray;
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

        if (_backgroundModelList.Count == 0)
        {
            Debug.LogWarning("배경정보 불러오기 실패");
        }
        else
        {
            Debug.Log("배경정보 불러오기 성공");
        }
        await UniTask.Yield();
    }

    private async UniTask LoadScoreData()
    {
        var scoreData = await LoadTextAsset("Score");

        JArray scores = JObject.Parse(scoreData)["Score"] as JArray;

        for(int i = 0; i < scores.Count; ++i)
        {
            var score = scores[i].ToObject<ScoreModel>();
            _scoreList.Add(score.Type, score);
        }

        if (_scoreList.Count == 0)
        {
            Debug.LogWarning("스코어 정보 불러오기 실패");
        }
        else
        {
            Debug.Log("스코어 불러오기 성공");
        }
        await UniTask.Yield();
    }

    private async UniTask LoadCharacterData()
    {
        var characterData = await LoadTextAsset("Character");

        JArray characters = JObject.Parse(characterData)["Character"] as JArray;

        for (int i = 0; i < characters.Count; ++i)
        {
            var character = characters[i].ToObject<CharacterModel>();
            _characterList.Add(character.Id, character);
        }

        if (_characterList.Count == 0)
        {
            Debug.LogWarning("캐릭터 정보 불러오기 실패");
        }
        else
        {
            Debug.Log("캐릭터 불러오기 성공");
        }
        await UniTask.Yield();
    }

    private async UniTask LoadPlayerData()
    {
        var playerData = await LoadTextAsset("Player");

        JObject player = JObject.Parse(playerData)["Player"] as JObject;

        _playerModel = player.ToObject<PlayerModel>();
    }

    private Texture GetTextureFromResources(int stageNumber, int layerNumber)
    {
        Texture texture = Resources.Load<Texture>(_texturePath + stageNumber.ToString() + "/Layer" + layerNumber);
        return texture;
    }

    public Dictionary<int, StageModel> GetStageList()
    {
        Debug.Log("DataManager GetStageList Start");
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

    public Dictionary<int, CharacterModel> GetCharacterList()
    {
        return _characterList;
    }

    public PlayerModel GetPlayerModel()
    {
        return _playerModel;
    }
}
