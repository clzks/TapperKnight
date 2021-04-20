using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapperKinghtModel : MonoBehaviour, IModel
{
    // 스테이지 별 몬스터 등장 데이터를 들고있어야 하며
    // 프레젠터에게 요청받을 시 해당 몬스터의 정보를 뷰에게 넘겨준다.
    // 뷰는 몬스터의 정보를 받아서 생성 및 세팅한다.
    private DataManager _dataManager;
    private Dictionary<int, StageModel> _stageModelList;
    private Dictionary<int, EnemyModel> _enemyModelList;
    private Dictionary<ScoreType, int> _scoreList;
    private PlayerModel _playerModel;
    
    private async UniTask Awake()
    {
        _dataManager = DataManager.Get();
        await GetStageList();
        await GetEnemyList();
        await SetScoreModel();
        _playerModel = new PlayerModel(); // 임시
    }
    
    public async UniTask GetStageList()
    {
        _stageModelList = DataManager.Get().GetStageList();
        await UniTask.Yield();
    }

    public async UniTask GetEnemyList()
    {
        _enemyModelList = DataManager.Get().GetEnemyList();
        await UniTask.Yield();
    }
    public async UniTask SetScoreModel()
    {
        _scoreList = _dataManager.GetScoreList();
        await UniTask.Yield();
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
        int value = _scoreList[type];

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
            Debug.Log("점수" + score + "점 획득. 현재 스코어 : " + _playerModel.OwnScore);
            await UniTask.Yield();
            return true;
        }
        else
        {
            await UniTask.Yield();
            return false;
        }
    }

    public async UniTask<bool> AddGold(int gold)
    {
        if(_playerModel.OwnGold + gold >= 0)
        {
            _playerModel.OwnGold += gold;
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
}
