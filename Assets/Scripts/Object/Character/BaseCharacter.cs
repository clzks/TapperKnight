using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
public class BaseCharacter : MonoBehaviour
{
    private GameManager _gameManager;
    private ObjectPoolManager _objectPool;
    [SerializeField] private Transform _prefabParent;
    [SerializeField] private CharacterStatus status;
    //public SkinnedMeshRenderer meshRenderer;
    [SerializeField]private float _currSpeed;
    [SerializeField] private float _currHp;
    [SerializeField] private float _runningRecord;
    private List<SpriteRenderer> _renderList;
    private Animator _animator;
    [Tooltip("CurrSpeed / Divider = 애니메이션 속도")]
    [Range(1f, 10f)]
    [SerializeField] private float _animSpeedDivider = 5f;
    [SerializeField] private float _maxAnimSpeed = 2f;
    [SerializeField] private float _minAnimSpeed = 1f;
    private CancellationTokenSource _lifeTimerCancelToken = new CancellationTokenSource();

    
    private void OnDisable()
    {
        _lifeTimerCancelToken.Cancel();
    }

    private async UniTask Awake()
    {
        _gameManager = GameManager.Get();
        _objectPool = ObjectPoolManager.Get();

        CharacterModel model = _gameManager.GetSelectModel();
        string prefabName = model.PrefabName;
        GameObject prefab = Instantiate(_objectPool.GetCharacterPrefab(prefabName));
        prefab.transform.SetParent(_prefabParent);
        prefab.transform.localPosition = new Vector3(0, 0, 0);
        SetCharacterStatus(model);
        await UniTask.Yield();
    }

    private async UniTask Start()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();

        _renderList = new List<SpriteRenderer>();
        var list = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var item in list)
        {
            _renderList.Add(item);
        }

        SetSortingLayer("Character").Forget();
        await UniTask.Yield();
    }

    public void SetCharacterStatus(CharacterModel model)
    {
        status.Name = model.NameKR;
        status.MaxHp = model.Hp;
        status.HpDecreasePerSecond = model.HpDecreasePerSecond;
        status.MaxSpeed = model.MaxSpeed;
        status.MinSpeed = model.MinSpeed;
        _currSpeed = status.MinSpeed;
        _currHp = status.MaxHp;
        _runningRecord = 0;
    }

    public async UniTaskVoid CastLifeTimer()
    {
        _lifeTimerCancelToken = new CancellationTokenSource();

        while (_currHp >= 0)
        {
            await UniTask.Delay(1000, false, 0f, _lifeTimerCancelToken.Token);
            TakeDamage(status.HpDecreasePerSecond, false);
        }
    }

    public void StopLifeTimer()
    {
        _lifeTimerCancelToken.Cancel();
    }

    public async UniTask<float> AddRecord()
    {
        float runningValue = _currSpeed * Time.deltaTime;
        _runningRecord += runningValue;
        await UniTask.Yield();
        return runningValue;
    }

    public void TakeDamage(float damage, bool changeAnim)
    {
        if(true == changeAnim)
        {
            _animator.Play("Damage");
        }

        _currHp -= damage;

        //Debug.Log("데미지 " + damage + "입음. 현재 체력 : " + _currHp);
        if (_currHp <= 0f)
        {
            _currHp = 0f;
            // 죽음처리
        }
    }

    public void AddSpeed(float speed)
    {
        _currSpeed += speed;
        if(_currSpeed < status.MinSpeed)
        {
            _currSpeed = status.MinSpeed;
        }
        else if(_currSpeed > status.MaxSpeed)
        {
            _currSpeed = status.MaxSpeed;
        }
        var animSpeed = 1f + (_currSpeed - 3f) / _animSpeedDivider;
        
        if(animSpeed >= _maxAnimSpeed)
        {
            animSpeed = _maxAnimSpeed;
        }
        
        if(animSpeed <= _minAnimSpeed)
        {
            animSpeed = _minAnimSpeed;
        }

        _animator.speed = animSpeed;
    }
    
    public void Attack()
    {
        _animator.Play("Attack");
    }

    public float GetPositionY()
    {
        return transform.position.y;
    }

    public async UniTaskVoid SetSortingLayer(string layerName)
    {
        foreach (var item in _renderList)
        {
            item.sortingLayerName = layerName;
        }

        await UniTask.Yield();
    }

    public float GetHpPercent()
    {
        return _currHp / status.MaxHp;
    }

    public int GetRunningRecord()
    {
        return (int)_runningRecord;
    }

    public float GetSpeed()
    {
        return _currSpeed;
    }
}

[System.Serializable]
public struct CharacterStatus
{
    public string Name;
    public float MaxHp;
    public float HpDecreasePerSecond;
    public float MaxSpeed;
    public float MinSpeed;
}