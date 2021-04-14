using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private DataManager _dataManager;
    private int _maxCount = 5;
    [Tooltip("Layer숫자가 높을수록 먼 이미지")]
    public List<BaseBackground> backgroundList;
    [Range(-10, 10)]
    public List<float> backgroundSpeedList;
    
    public void Awake()
    {

    }
    
    public async UniTaskVoid SetBackgroundList(int stageNumber)
    {
        List<BackgroundModel> models = _dataManager.GetBackgroundList(stageNumber);

        int count = models.Count;

        for (int i = 0; i < count; ++i)
        {
            BackgroundModel model = models[i];
            BaseBackground bg = backgroundList[i];
            bg.gameObject.SetActive(true);
            bg.SetBackground(model);
            bg.SetTexture(_dataManager.GetTexture(stageNumber, i + 1));
        }

        if(count != _maxCount)
        {
            for (int i = count; i < _maxCount; ++i)
            {
                backgroundList[i].gameObject.SetActive(false);
            }
        }
    }

    public async UniTask Start()
    {
        await UniTask.Yield();
        _dataManager = DataManager.Get();
        int count = _maxCount;

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

#if UNITY_EDITOR
    // 에디터상에서 배경 스크롤 스피드를 조절해볼수 있게 하기위한 코드
    public async UniTask Update()
    {
        await UniTask.Yield();
        for(int i = 0; i < backgroundList.Count; ++i)
        {
            var spd = backgroundSpeedList[i];
            backgroundList[i].SetSpeed(spd);
        }
    }
#endif
}
