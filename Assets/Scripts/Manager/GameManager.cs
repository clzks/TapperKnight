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
            Debug.LogWarning("InGamePresenter 할당 실패");
        }

        return _inGamePresenter;
    }
}
