using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TapperKinghtModel : MonoBehaviour
{
    // 스테이지 별 몬스터 등장 데이터를 들고있어야 하며
    // 프레젠터에게 요청받을 시 해당 몬스터의 정보를 뷰에게 넘겨준다.
    // 뷰는 몬스터의 정보를 받아서 생성 및 세팅한다.
    private GameManager _gameManager;
    private DataManager _dataManager;
    private Dictionary<int, StageModel> _stageModelList;
    private Dictionary<int, EnemyModel> _enemyModelList;
    private Dictionary<ScoreType, ScoreModel> _scoreList;
    private Dictionary<string, Sprite> _noteSpriteList;
    private Dictionary<ScoreType, Sprite> _scoreSpriteList;
    private PlayerModel _playerModel;
    
    private void Awake()
    {
        _gameManager = GameManager.Get();
        _dataManager = DataManager.Get();
        SetStageList();
        SetEnemyList();
        SetScoreModel();
        SetNoteSpriteList();
        SetScoreSpriteList();
        _playerModel = _dataManager.GetPlayerModel();
    }
    
    public void SetStageList()
    {
        _stageModelList = DataManager.Get().GetStageList();
    }

    public void SetEnemyList()
    {
        _enemyModelList = DataManager.Get().GetEnemyList();
    }
    public void SetScoreModel()
    {
        _scoreList = _dataManager.GetScoreList();
    }
    public void SetNoteSpriteList()
    {
        _noteSpriteList = _dataManager.GetSpriteList();
    }

    public void SetScoreSpriteList()
    {
        _scoreSpriteList = _dataManager.GetScoreSpriteList();
    }

    public StageModel GetStageModel(int index)
    {
        return _stageModelList[index];
    }

    public StageModel GetRandomStageModel()
    {
        int count = _stageModelList.Count;

        return _stageModelList[Random.Range(1, count + 1)];
    }

    public StageModel GetStageModel(int index, ref bool isLast)
    {
        if(_stageModelList.Count == index)
        {
            isLast = true;
        }

        return _stageModelList[index];
    }

    public EnemyModel GetEnemyModel(int id)
    {
        return _enemyModelList[id];
    }
    
    public float GetAccelerate(ScoreType score)
    {
        return _scoreList[score].Accelerate;
    }

    public float GetRecoveryHp(ScoreType score)
    {
        return _scoreList[score].Recovery;
    }

    public EnemyModel GetRandomEnemy(int stageNumber)
    {
        var enemyList = _stageModelList[stageNumber].EnemyList;
        int n = Random.Range(0, 1000) % enemyList.Count;
        var id = enemyList[n];

        if (true == _enemyModelList.ContainsKey(id))
        {
            return _enemyModelList[id];
        }
        else
        {
            Debug.LogError("이네미 모델 리스트에 포함되지 않은 Id 발견");
            return _enemyModelList[10000];
        }
    }

    public void AddTotalRunningRecord(int record)
    {
        //int value = _scoreList[type].ScoreValue;

        _playerModel.TotalRunningRecord += record;
    }

    public async UniTask<bool> AddGold(int gold)
    {
        if(_playerModel.OwnGold + gold >= 0)
        {
            _playerModel.OwnGold += gold;
            await UniTask.Yield();
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetTotalRunningRecord()
    {
        return _playerModel.TotalRunningRecord;
    }

    public void SetTotalRunningRecord(int value)
    {
        _playerModel.TotalRunningRecord = value;
    }

    public void CalculateExp(int exp)
    {
        int Id = _gameManager.GetSelectModel().Id;
        var characterData = _playerModel.OwnCharacterList.Find(x => x.Id == Id);
        int totalExp = characterData.CurrExp + exp;

        //int nextLevelUpExp = GetNextLevelUpExp(characterData);

        while(totalExp >= GetNextLevelUpExp(characterData))
        {
            LevelUp(characterData);
            totalExp -= GetNextLevelUpExp(characterData);
        }
    }

    private int GetNextLevelUpExp(CharacterDataModel characterData)
    {
        return (characterData.Level - 1) * _playerModel.IncreaseRequiredExperience - characterData.CurrExp;
    }

    private void LevelUp(CharacterDataModel characterData)
    {
        characterData.Level += 1;
        Debug.Log("레벨 업!!!!!!!");
    }

    public Sprite GetNoteSprite(string noteType)
    {
        return _noteSpriteList[noteType];
    }

    public Sprite GetScoreSprite(ScoreType type)
    {
        return _scoreSpriteList[type];
    }
}
