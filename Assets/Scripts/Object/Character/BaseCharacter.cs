using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class BaseCharacter : MonoBehaviour
{
    public CharacterStatus status;
    public SkinnedMeshRenderer meshRenderer;
    private float _currSpeed;
    private float _currHp;
    private float _runningRecord;
    public void SetSampleCharacter()
    {
        status.Name = "샘플";
        status.MaxHp = 60;
        status.HpDecreasePerSecond = 1;
        status.NormalSpeed = 3;
        status.Accel = 1;
        status.MaxSpeed = 10;
        status.MaxSpeed = 1;
        _currSpeed = status.NormalSpeed;
        _currHp = status.MaxHp;
        _runningRecord = 0;
    }

    private async UniTask Start()
    {
        SetSortingLayer("Background", 4).Forget();
        
        while (_currHp >= 0)
        {
            await UniTask.Delay(1000);
            await GetDamage(status.HpDecreasePerSecond);
        }
    }

    public async UniTask AddRecord()
    {
        _runningRecord += _currSpeed * Time.deltaTime;
        await UniTask.Yield();
    }

    public async UniTask GetDamage(float damage)
    {
        await UniTask.Yield();

        _currHp -= damage;
        //Debug.Log("데미지 " + damage + "입음. 현재 체력 : " + _currHp);
        if (_currHp <= 0f)
        {
            _currHp = 0f;
            // 죽음처리
        }
    }

    public float GetPositionY()
    {
        return transform.position.y;
    }

    public async UniTaskVoid SetSortingLayer(string layerName, int sortingOrder)
    {
        meshRenderer.sortingLayerName = layerName;
        meshRenderer.sortingOrder = sortingOrder;
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
    public float Accel;
    public float MaxSpeed;
    public float MinSpeed;
}