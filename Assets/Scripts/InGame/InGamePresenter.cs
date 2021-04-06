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

    public void SetView(InGameView view)
    {
        _inGameView = view;
    }   
    
    public void SetModel(TapperKinghtModel model)
    {
        _model = model;
    }

    public float GetNoteBoxPosY()
    {
        return _inGameView.GetNoteBoxPosY();
    }
    
    public StageModel GetStageModel(int index)
    {
        return _model.GetStageModel(index);
    }

    public Dictionary<ScoreType, float> GetScoreModel()
    {
        return _model.GetScoreModel();
    }

    public void SetTarget(BaseEnemy enemy)
    {
        _inGameView.SetTarget(enemy);
    }

    public BaseEnemy GetTarget()
    {
        return _inGameView.GetTarget();
    }

    public void SetNoteSprite(Sprite sp)
    {

    }

    public void OnNoteCall(ScoreType score)
    {
        
    }

    public void OnTargetDestroy()
    {
        _inGameView.OnTargetDestroy();
    }
}
