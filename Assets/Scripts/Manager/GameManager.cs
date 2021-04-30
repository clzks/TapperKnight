using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private InGamePresenter _inGamePresenter;

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

    private async UniTask OnApplicationQuit()
    {
        ObjectPoolManager.Get().InitPool();
        await UniTask.Yield();
    }
}
