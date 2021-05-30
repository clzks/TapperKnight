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
        Application.targetFrameRate = 60;

        _dataManager = DataManager.Get();

        await _dataManager.GetDataAsync();
        await UniTask.Yield();
        SceneManager.LoadScene("NoteTest");
    }
}
