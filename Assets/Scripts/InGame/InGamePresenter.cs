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

    public async UniTaskVoid SetView(InGameView view)
    {
        _inGameView = view;
        await UniTask.Yield();
    }   
    
    public async UniTaskVoid SetModel(TapperKinghtModel model)
    {
        _model = model;
        await UniTask.Yield();
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

    public int GetScore()
    {
        return _model.GetScore();
    }

    public void SetScore(int score)
    {
        _model.SetScore(score);
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

    public async UniTask OnNoteCall(ScoreType score, float damage)
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

        await _model.AddScore(score);
    }

    public async UniTask OnTargetDestroy()
    {
        await _inGameView.OnTargetDestroy();
    }

}
