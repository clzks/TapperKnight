using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadingScene : MonoBehaviour
{
    private DataManager _dataManager;
    private async UniTask Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
        _dataManager = DataManager.Get();

        await _dataManager.GetDataAsync();
        await UniTask.Yield();
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
