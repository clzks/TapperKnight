using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
    [SerializeField]private InGameView _inGameView;
    [SerializeField] private InGamePresenter _inGamePresenter;
    [SerializeField] private TapperKinghtModel _model;
    private GameManager _gameManager;
    private void Awake()
    {
        MakeMvpPattern();
        SetMvpPattern();
        _gameManager = GameManager.Get();
        _gameManager.SetInGamePresenter(_inGamePresenter);
    }

    private void MakeMvpPattern()
    {
        if(null == _inGameView)
        {
            GameObject viewObject = new GameObject();
            _inGameView = viewObject.AddComponent<InGameView>();
            viewObject.name = "InGameView";
            viewObject.transform.SetParent(transform);
        }

        if (null == _inGamePresenter)
        {
            GameObject viewObject = new GameObject();
            _inGamePresenter = viewObject.AddComponent<InGamePresenter>();
            viewObject.name = "InGamePresenter";
            viewObject.transform.SetParent(transform);
        }

        if (null == _model)
        {
            GameObject viewObject = new GameObject();
            _model = viewObject.AddComponent<TapperKinghtModel>();
            viewObject.name = "Model";
            viewObject.transform.SetParent(transform);
        }
    }


    private void SetMvpPattern()
    {
        _inGameView.SetPresenter(_inGamePresenter);
        _inGamePresenter.SetModel(_model);
        _inGamePresenter.SetView(_inGameView);
    }
}
