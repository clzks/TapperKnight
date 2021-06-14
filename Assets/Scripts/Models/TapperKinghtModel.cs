using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TapperKinghtModel : MonoBehaviour
{
    // 스테이지 별 몬스터 등장 데이터를 들고있어야 하며
    // 프레젠터에게 요청받을 시 해당 몬스터의 정보를 뷰에게 넘겨준다.
    // 뷰는 몬스터의 정보를 받아서 생성 및 세팅한다.
    private DataManager _dataManager;
    private Dictionary<int, StageModel> _stageModelList;
    private Dictionary<int, EnemyModel> _enemyModelList;
    private Dictionary<ScoreType, ScoreModel> _scoreList;
    private Dictionary<string, Sprite> _noteSpriteList;
    private Dictionary<ScoreType, Sprite> _scoreSpriteList;
    private PlayerModel _playerModel;
    
    private void Awake()
    {
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

    public async UniTask AddScore(ScoreType type)
    {
        int value = _scoreList[type].ScoreValue;

        var result = await AddScore(value);

        if(false == result)
        {
            Debug.LogWarning("획득할 수 없는 점수입니다");
        }
    }

    public async UniTask<bool> AddScore(int score)
    {
        if(_playerModel.OwnScore + score >= 0)
        {
            _playerModel.OwnScore += score;
            //Debug.Log("점수" + score + "점 획득. 현재 스코어 : " + _playerModel.OwnScore);
            await UniTask.Yield();
            return true;
        }
        else
        {
            return false;
        }
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

    public int GetScore()
    {
        return _playerModel.OwnScore;
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
