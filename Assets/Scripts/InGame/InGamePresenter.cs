using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePresenter : MonoBehaviour
{
    [SerializeField]private InGameView _inGameView;
    [SerializeField]private TapperKinghtModel _model;
    

    private void Awake()
    {
        
    }

    public void SetView(InGameView view)
    {
        _inGameView = view;
    }   
    
    public void SetModel(TapperKinghtModel model)
    {
        _model = model;
    }

    public void SavePlayerModel()
    {
        _model.SavePlayerModel();
    }

    public void ResetPlayerModel()
    {
        _model.ResetPlayerModel();
    }

    public Vector2 GetNoteBoxPos()
    {
        return _inGameView.GetNoteBoxPos();
    }
    
    public StageModel GetStageModel(int index)
    {
        return _model.GetStageModel(index);
    }

    public StageModel GetStageModel(int index, ref bool isLast)
    {
        return _model.GetStageModel(index, ref isLast);
    }

    public StageModel GetRandomStageModel()
    {
        return _model.GetRandomStageModel();
    }

    public EnemyModel GetRandomEnemy(int stageNumber)
    {
        return _model.GetRandomEnemy(stageNumber);
    }

    public async UniTask SetTarget(BaseEnemy enemy)
    {
        await _inGameView.SetTarget(enemy);
    }

    public BaseEnemy GetTarget()
    {
        return _inGameView.GetTarget();
    }

    public void AddTotalRunningRecord(int record)
    {
        _model.AddTotalRunningRecord(record);
    }

    public int GetTotalRunningRecord()
    {
        return _model.GetTotalRunningRecord();
    }

    public void SetTotalRunningRecord(int record)
    {
        _model.SetTotalRunningRecord(record);
    }
    public float GetPlayerSpeed()
    {
        return _inGameView.GetPlayerSpeed();
    }

    public List<Vector3> GetNotePopDestination()
    {
        return _inGameView.GetNotePopDestination();
    }

    public Sprite GetNoteSprite(string noteType)
    {
        return _model.GetNoteSprite(noteType);
    }

    public Sprite GetScoreSprite(ScoreType type)
    {
        return _model.GetScoreSprite(type);
    }

    public Dictionary<int, QuestInfo> GetQuestInfoList()
    {
        return _model.GetQuestInfoList();
    }

    public PlayerModel GetPlayerModel()
    {
        return _model.GetPlayerModel();
    }

    public void CalculateExp(int exp)
    {
        _model.CalculateExp(exp);
    }

    public void OnNoteCall(ScoreType score, float damage)
    {
        var accel = _model.GetAccelerate(score);
        var recovery = _model.GetRecoveryHp(score);

        if (score == ScoreType.Miss)
        {
            _inGameView.TakeDamage(damage);
        }
        else
        {
            _inGameView.Attack();
            _inGameView.Recovery(recovery);
        }

        _inGameView.AddSpeed(accel);
    }

    public void SendScroeType(ScoreType score)
    {
        _inGameView.SendScoreType(score);
    }

    public async UniTask OnTargetDestroy()
    {
        await _inGameView.OnTargetDestroy();
    }

}
