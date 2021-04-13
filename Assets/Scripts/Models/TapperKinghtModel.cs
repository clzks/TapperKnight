using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapperKinghtModel : MonoBehaviour, IModel
{
    // 스테이지 별 몬스터 등장 데이터를 들고있어야 하며
    // 프레젠터에게 요청받을 시 해당 몬스터의 정보를 뷰에게 넘겨준다.
    // 뷰는 몬스터의 정보를 받아서 생성 및 세팅한다.

    private Dictionary<int, StageModel> stageModelList;
    private Dictionary<int, EnemyModel> enemyModelList;
    private Dictionary<ScoreType, float> scoreDistanceList;
    private BaseCharacter player;

    private void Awake()
    {
        GetStageList();
        GetEnemyList();
        SetSampleScoreDistance();
    }
    
    public void GetStageList()
    {
        stageModelList = DataManager.Get().GetStageList();
    }

    public void GetEnemyList()
    {
        enemyModelList = DataManager.Get().GetEnemyList();
    }

    public void SetSampleScoreDistance()
    {
        scoreDistanceList = new Dictionary<ScoreType, float>();

        scoreDistanceList.Add(ScoreType.Miss, 1f);
        scoreDistanceList.Add(ScoreType.Bad, 1f);
        scoreDistanceList.Add(ScoreType.Good, 0.6f);
        scoreDistanceList.Add(ScoreType.Great, 0.4f);
        scoreDistanceList.Add(ScoreType.Perfect, 0.2f);
    }

    //public Dictionary<int, StageModel> GetStageModelList()
    //{
    //    return stageModelList;
    //}
    
    public StageModel GetStageModel(int index)
    {
        return stageModelList[index];
    }

    public EnemyModel GetEnemyModel(int id)
    {
        return enemyModelList[id];
    }

    public EnemyModel GetRandomEnemy(int stageNumber)
    {
        var enemyList = stageModelList[stageNumber].EnemyList;
        int n = Random.Range(0, 1000) % enemyList.Count;
        var id = enemyList[n];

        if (true == enemyModelList.ContainsKey(id))
        {
            return enemyModelList[id];
        }
        else
        {
            Debug.LogError("이네미 모델 리스트에 포함되지 않은 Id 발견");
            return enemyModelList[10000];
        }
    }

    public Dictionary<ScoreType, float> GetScoreModel()
    {
        return scoreDistanceList;
    }

    public float GetPlayerSpeed()
    {
        return player.currSpeed;
    }
}
