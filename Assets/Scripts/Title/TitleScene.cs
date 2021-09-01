using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TitleScene : MonoBehaviour
{
    [SerializeField] private InGameView _inGameView;
    [SerializeField] private InGamePresenter _inGamePresenter;
    [SerializeField] private TapperKinghtModel _model;
    private GameManager _gameManager;
    public Text titleText;
    public Text pressButtonText;
    public TitleTouchPanel touchPanel;
    public Button optionButton;
    private bool _isStart;
    private void Awake()
    {
        _isStart = false;
        MakeMvpPattern();
        SetMvpPattern();
        _gameManager = GameManager.Get();
        _gameManager.SetInGamePresenter(_inGamePresenter);
        _gameManager.SetSceneType(SceneType.Title);
    }

    private async UniTask Start()
    {
        await OpeningEvent();
    }

    private void MakeMvpPattern()
    {
        if (null == _inGameView)
        {
            GameObject viewObject = new GameObject();
            _inGameView = viewObject.AddComponent<InGameView>();
            viewObject.name = "InGameView";
            viewObject.transform.SetParent(transform);
        }

        if (null == _inGamePresenter)
        {
            GameObject viewObject = new GameObject();
            _inGamePresenter = viewObject.AddComponent<InGamePresenter>();
            viewObject.name = "InGamePresenter";
            viewObject.transform.SetParent(transform);
        }

        if (null == _model)
        {
            GameObject viewObject = new GameObject();
            _model = viewObject.AddComponent<TapperKinghtModel>();
            viewObject.name = "Model";
            viewObject.transform.SetParent(transform);
        }
    }

    private void SetMvpPattern()
    {
        _inGameView.SetPresenter(_inGamePresenter);
        _inGamePresenter.SetModel(_model);
        _inGamePresenter.SetView(_inGameView);
    }

    private async UniTask OpeningEvent()
    {
        float timer = 0f;
        var c = titleText.color;
        while(timer <= 1.5f)
        {
            timer += Time.deltaTime;
            titleText.color = new Color(c.r, c.g, c.b, timer / 1.5f);
            await UniTask.Yield();
        }

        touchPanel.SetAction(() => LoadLobbyScene());
        timer = 0f;
        c = pressButtonText.color;
        while (!_isStart)
        {
            timer += Time.deltaTime;
            if (timer <= 1f)
            {
                pressButtonText.color = new Color(c.r, c.g, c.b, timer);
            }
            else
            {
                pressButtonText.color = new Color(c.r, c.g, c.b, 2f - timer);
            }
            await UniTask.Yield();

            if (timer >= 2f)
            {
                timer = 0f;
            }
        }
    }

    public void LoadLobbyScene()
    {
        _isStart = true;
        SceneManager.LoadScene("LobbyScene");
        //SceneManager.LoadScene("CharacterSelectScene");
    }
}
