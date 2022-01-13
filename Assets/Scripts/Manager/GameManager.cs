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
            Debug.LogWarning("InGamePresenter �Ҵ� ����");
        }

        return _inGamePresenter;
    }

    public void SetSelectCharacter(CharacterModel model)
    {
        _currSelectCharacter = model;
    }

    // ���� ĳ���� �� �޾ƿ���
    public CharacterModel GetSelectModel()
    {
        return _currSelectCharacter;
    }

    private void OnApplicationQuit()
    {
        ObjectPoolManager.Get().ResetPool();
    }

    // �� Ÿ�� �����ϱ�
    public void SetSceneType(SceneType type)
    {
        _currSceneType = type;
    }

    // �� Ÿ�� �޾ƿ���
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
