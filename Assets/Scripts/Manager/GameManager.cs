using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private SceneType _currSceneType;
    private InGamePresenter _inGamePresenter;
    private CharacterModel _currSelectCharacter;
    public bool isTitle;
    private GameNetworkType _networkType = GameNetworkType.Count;
    public void SetInGamePresenter(InGamePresenter presenter)
    {
        _inGamePresenter = presenter;
    }

    public InGamePresenter GetInGamePresenter()
    {
        if (null == _inGamePresenter)
        {
            Debug.LogWarning("InGamePresenter 할당 실패");
        }

        return _inGamePresenter;
    }

    public void SetSelectCharacter(CharacterModel model)
    {
        _currSelectCharacter = model;
    }

    // 현재 캐릭터 모델 받아오기
    public CharacterModel GetSelectModel()
    {
        return _currSelectCharacter;
    }

    private void OnApplicationQuit()
    {
        ObjectPoolManager.Get().ResetPool();
    }

    // 씬 타입 설정하기
    public void SetSceneType(SceneType type)
    {
        _currSceneType = type;
    }

    // 씬 타입 받아오기
    public SceneType GetSceneType()
    {
        return _currSceneType;
    }

    public GameNetworkType GetGameNetworkType()
    {
        return _networkType;
    }

    public void SetGameNetworkType(GameNetworkType type)
    {
        _networkType = type;
    }
}
