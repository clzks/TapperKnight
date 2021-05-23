using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BaseCharacter : MonoBehaviour
{
    [SerializeField] private CharacterStatus status;
    //public SkinnedMeshRenderer meshRenderer;
    [SerializeField]private float _currSpeed;
    [SerializeField] private float _currHp;
    [SerializeField] private float _runningRecord;
    private List<SpriteRenderer> _renderList;
    [SerializeField] private Animator _animator;
    [Tooltip("CurrSpeed / Divider = 애니메이션 속도")]
    [Range(1f, 10f)]
    [SerializeField] private float _animSpeedDivider = 3f;
    [SerializeField] private float _maxAnimSpeed = 2.5f;
    public void SetSampleCharacter()
    {
        status.Name = "샘플";
        status.MaxHp = 60;
        status.HpDecreasePerSecond = 1;
        status.NormalSpeed = 3;
        status.MaxSpeed = 10;
        status.MaxSpeed = 1;
        _currSpeed = status.NormalSpeed;
        _currHp = status.MaxHp;
        _runningRecord = 0;
    }
    private async UniTask Awake()
    {
        _renderList = new List<SpriteRenderer>();
        var list = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var item in list)
        {
            _renderList.Add(item);
        }
        await UniTask.Yield();
    }

    private async UniTask Start()
    {
        SetSortingLayer("Character").Forget();
        
        while (_currHp >= 0)
        {
            await UniTask.Delay(1000);
            TakeDamage(status.HpDecreasePerSecond, false).Forget();
        }
    }

    public async UniTask<float> AddRecord()
    {
        float runningValue = _currSpeed * Time.deltaTime;
        _runningRecord += runningValue;
        await UniTask.Yield();
        return runningValue;
    }

    public async UniTaskVoid TakeDamage(float damage, bool changeAnim)
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

        await UniTask.Yield();
    }

    public async UniTaskVoid AddSpeed(float speed)
    {
        _currSpeed += speed;
        if(_currSpeed < status.NormalSpeed)
        {
            _currSpeed = status.NormalSpeed;
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

        _animator.speed = animSpeed;

        await UniTask.Yield();
    }
    
    public async UniTaskVoid Attack()
    {
        _animator.Play("Attack");
        await UniTask.Yield();
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
    public float NormalSpeed;
    public float MaxSpeed;
}