using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private InGamePresenter _inGamePresenter;
    private CharacterModel _currSelectCharacter;
    public bool isTitle;
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

    public CharacterModel GetSelectModel()
    {
        return _currSelectCharacter;
    }

    private async UniTask OnApplicationQuit()
    {
        ObjectPoolManager.Get().ResetPool();
        await UniTask.Yield();
    }
}
