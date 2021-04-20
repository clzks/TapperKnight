using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePresenter : MonoBehaviour, IPresenter
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

    public float GetNoteBoxPosY()
    {
        return _inGameView.GetNoteBoxPosY();
    }
    
    public StageModel GetStageModel(int index)
    {
        return _model.GetStageModel(index);
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
    public float GetPlayerSpeed()
    {
        return _inGameView.GetPlayerSpeed();
    }

    public List<Vector3> GetNotePopDestination()
    {
        return _inGameView.GetNotePopDestination();
    }

    public async UniTaskVoid SetNoteSprite(Sprite sp)
    {

    }

    public async UniTask OnNoteCall(ScoreType score, float damage)
    {
        if (score == ScoreType.Miss)
        {
            await _inGameView.GetDamage(damage);
        }

        await _model.AddScore(score);
    }

    public async UniTask OnTargetDestroy()
    {
        await _inGameView.OnTargetDestroy();
    }

}
