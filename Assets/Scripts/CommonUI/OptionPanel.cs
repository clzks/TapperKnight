using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    private GameManager _gameManager;

    private void OnEnable()
    {
        if (null == _gameManager)
        {
            _gameManager = GameManager.Get();
        }

        if(SceneType.InGame == _gameManager.GetSceneType())
        {
            Time.timeScale = 0f;
        }
    }

    private void OnDisable()
    {
        if (SceneType.InGame == _gameManager.GetSceneType())
        {
            Time.timeScale = 1f;
        }
    }

    public void ReturnToLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CloseOptionPanel()
    {
        gameObject.SetActive(false);
    }

    public void OpenOptionPanel()
    {
        gameObject.SetActive(true);
    }
}
