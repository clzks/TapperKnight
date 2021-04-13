using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private InGamePresenter _inGamePresenter;
    [Tooltip("Layer숫자가 높을수록 먼 이미지")]
    public List<BaseBackground> backgroundList;
    [Range(-10, 10)]
    public List<float> backgroundSpeedList;

    public void Awake()
    {
        if (backgroundList.Count == 0)
        {
            Debug.LogError("배경리스트 숫자가 잘못 설정되었습니다");
        }

        if (backgroundSpeedList.Count == 0)
        {
            Debug.LogError("배경리스트에 입력할 속도가 잘못 설정되었습니다");
        }

        if (backgroundList.Count != backgroundSpeedList.Count)
        {
            Debug.LogError("");
        }

    
    }
    
    public async UniTask Start()
    {
        await UniTask.Yield();
        _inGamePresenter = GameManager.Get().GetInGamePresenter();
        int count = backgroundList.Count;

        for (int i = 0; i < count; ++i)
        {
            if (i == 0)
            {
                backgroundList[i].SetLayer(count);
            }
            else
            {
                backgroundList[i].SetLayer(count - (1 + i));
            }
        }
    }

    public async UniTask Update()
    {
        //float playerSpeed = _inGamePresenter.GetPlayerSpeed();

        await UniTask.Yield();
        for(int i = 0; i < backgroundList.Count; ++i)
        {
            var spd = backgroundSpeedList[i];
            backgroundList[i].SetSpeed(spd);
        }
    }
}
