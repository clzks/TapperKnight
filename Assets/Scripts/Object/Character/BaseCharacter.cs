using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class BaseCharacter : MonoBehaviour
{
    public CharacterStatus status;
    public SkinnedMeshRenderer meshRenderer;
    public float currSpeed;
    public void SetSampleCharacter()
    {
        status.Name = "»ùÇÃ";
        status.Hp = 100;
        status.HpDecreasePerSecond = 1;
        status.NormalSpeed = 3;
        status.Accel = 1;
        status.MaxSpeed = 10;
        status.MaxSpeed = 1;
    }

    private async UniTask Start()
    {
        SetSortingLayer("Background", 4);

        while (status.Hp >= 0)
        {
            await UniTask.Delay(1000);
            GetDamage(status.HpDecreasePerSecond).Forget();
        }
    }

    private async UniTask Update()
    {
        await UniTask.Yield();
    }

    public async UniTaskVoid GetDamage(float damage)
    {
        await UniTask.Yield();

        status.Hp -= damage;
        if (status.Hp <= 0f)
        {
            // Á×À½Ã³¸®
        }
    }

    public void SetSortingLayer(string layerName, int sortingOrder)
    {
        meshRenderer.sortingLayerName = layerName;
        meshRenderer.sortingOrder = sortingOrder;
    }
}

[System.Serializable]
public struct CharacterStatus
{
    public string Name;
    public float Hp;
    public float HpDecreasePerSecond;
    public float NormalSpeed;
    public float Accel;
    public float MaxSpeed;
    public float MinSpeed;
}