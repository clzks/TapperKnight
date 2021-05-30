using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private DataManager _dataManager;
    private int _maxCount = 4;
    [Tooltip("Layer숫자가 높을수록 먼 이미지")]
    public List<BaseBackground> backgroundList;
    [Range(-10, 10)]
    public List<float> backgroundSpeedList;
    [SerializeField] private SpriteRenderer _stageChangeBlock;
    [SerializeField] private float _playerSpeedFactor;
    public void Awake()
    {

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
                backgroundList[i].SetLayer(count).Forget();
            }

            backgroundList[i].SetPlayerSpeedFactor(_playerSpeedFactor).Forget();
        }
    }

    public async UniTask Update()
    {
        await UniTask.Yield();
        for (int i = 0; i < backgroundList.Count; ++i)
        {
            var spd = backgroundSpeedList[i];
            backgroundList[i].SetSpeed(spd).Forget();
            backgroundList[i].SetPlayerSpeedFactor(_playerSpeedFactor).Forget();
        }
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

        await UniTask.Yield();
    }


    public async UniTask ExecuteStageChange(int stageNumber, float screenBlackOutTime)
    {
        screenBlackOutTime = 1 / screenBlackOutTime;

        while (_stageChangeBlock.color.a < 1f)
        {
            _stageChangeBlock.color += new Color(0, 0, 0, Time.deltaTime * screenBlackOutTime);
            await UniTask.Yield();
        }
        
        SetBackgroundList(stageNumber).Forget();

        while(_stageChangeBlock.color.a > 0f)
        {
            _stageChangeBlock.color -= new Color(0, 0, 0, Time.deltaTime * screenBlackOutTime);
            await UniTask.Yield();
        }
    }

}
