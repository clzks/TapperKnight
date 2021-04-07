using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
public class AsyncTest : MonoBehaviour
{
    private async void Awake()
    {
        var asset = await Resources.LoadAsync<TextAsset>("foo");
        //  UniTask
        //  start
        //  async
        await GetDataAsync();
        var result = true;
        while (result)
        {
            result = await IsAvailableAsync();
            await UniTask.Yield();
            //await Task.Yield();
        }
    
        //  end
    }
    
    private void Start()
    {
        //  select card
    
        StartCoroutine(ShowEffectCo((v) =>
        {
    
        }));
    }
    
    private void DestroyCard(int value)
    {
    
    }
    
    private async UniTask GetDataAsync()
    {
    
    }
    
    private async UniTask<bool> IsAvailableAsync()
    {
        await UniTask.Delay(1000);
    
        return true;
    }
    
    private async UniTask ShowEffectAsync()
    {
        //  show effect
    
        while (true)
        {
            //  check break
            await UniTask.Yield();  //  hanging
        }
            
        await UniTask.Delay(100);
    }
    
    private IEnumerator ShowEffectCo(UnityAction<int> onDone)
    {
        yield return null;
    
        onDone?.Invoke(0);
    }
}
