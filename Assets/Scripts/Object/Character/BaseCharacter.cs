using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BaseCharacter : MonoBehaviour
{
    public CharacterStatus status;

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
        while (status.Hp >= 0)
        {
            await UniTask.Delay(1000);
            GetDamage(status.HpDecreasePerSecond).Forget();
        }
    }

    private async UniTask Update()
    {
        
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