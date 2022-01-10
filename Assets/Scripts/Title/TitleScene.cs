using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TitleScene : MonoBehaviour
{
    [SerializeField] private InGameView _inGameView;
    [SerializeField] private InGamePresenter _inGamePresenter;
    [SerializeField] private TapperKinghtModel _model;
    private GPGSManager _gpgsManager;
    private GameManager _gameManager;
    private DataManager _dataManager;
    public Text titleText;
    public Text pressButtonText;
    public TitleTouchPanel touchPanel;
    public LogInPanel logInPanel;
    public Button optionButton;
    public RetryPopUp retryPopUp;
    private bool _isStart;
    private void Awake()
    {
        _isStart = false;
        MakeMvpPattern();
        SetMvpPattern();
        _gpgsManager = GPGSManager.Get();
        _dataManager = DataManager.Get();
        _gameManager = GameManager.Get();
        _gameManager.SetInGamePresenter(_inGamePresenter);
        _gameManager.SetSceneType(SceneType.Title);
    }

    private async UniTask Start()
    {
        _gpgsManager.SignIn();
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

        touchPanel.SetAction(() => OnClickTouchPanel());
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

    public void OnClickTouchPanel()
    {
        if (true == _gpgsManager.IsAuthenticated())
        {
            LoadLobbyScene().Forget();
        }
        else
        {
            logInPanel.gameObject.SetActive(true);
        }
    }

    public void OnClickPlayWithGoogleLogin()
    {
        _gpgsManager.SignIn();

        if(true == _gpgsManager.IsAuthenticated())
        {
            LoadLobbyScene().Forget();
        }
        else
        {
            logInPanel.gameObject.SetActive(false);
            retryPopUp.gameObject.SetActive(true);
        }
    }

    public void OnClickRetryGoogleLogin()
    {
        retryPopUp.gameObject.SetActive(false);

        _gpgsManager.SignIn();

        if (true == _gpgsManager.IsAuthenticated())
        {
            LoadLobbyScene().Forget();
        }
        else
        {
            retryPopUp.gameObject.SetActive(true);
        }
    }

    public void OnClickPlayGuestMode()
    {
        _gpgsManager.SetLogin(false);
        LoadLobbyScene().Forget();
    }

    public async UniTask LoadLobbyScene()
    {
        await LoadPlayerData(_gpgsManager.IsAuthenticated());
        _isStart = true;
        SceneManager.LoadScene("LobbyScene");
    }

    public async UniTask LoadPlayerData(bool isLogin)
    {
        await _dataManager.LoadPlayerData(isLogin);
    }
}
