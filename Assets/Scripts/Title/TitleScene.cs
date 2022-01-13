using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class TitleScene : MonoBehaviour
{
    [SerializeField] private InGameView _inGameView;
    [SerializeField] private InGamePresenter _inGamePresenter;
    [SerializeField] private TapperKinghtModel _model;
    //private GPGSManager _gpgsManager;
    private GameManager _gameManager;
    private DataManager _dataManager;
    public Text titleText;
    public Text pressButtonText;
    public TitleTouchPanel touchPanel;
    public LogInPanel logInPanel;
    public Button optionButton;
    public RetryPopUp retryPopUp;
    private bool _isStart;
    private bool _isSuccess; // GPGS 관련 bool값
    private void Awake()
    {
        _isStart = false;
        MakeMvpPattern();
        SetMvpPattern();
        //_gpgsManager = GPGSManager.Get();
        _dataManager = DataManager.Get();
        _gameManager = GameManager.Get();
        _gameManager.SetInGamePresenter(_inGamePresenter);
        _gameManager.SetSceneType(SceneType.Title);
    }

    private async UniTask Start()
    {
        _dataManager.SignIn();
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
        if (true == _dataManager.IsAuthenticated())
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
        _dataManager.SignIn();
        StartCoroutine(ExecuteLogIn());

        if(true == _isSuccess)
        {
            _gameManager.SetGameNetworkType(GameNetworkType.Online);
            LoadLobbyScene().Forget();
        }
        else
        {
            logInPanel.gameObject.SetActive(false);
            retryPopUp.SetButtonAction(OnClickRetryGoogleLogin());
            retryPopUp.SetDescription("구글 로그인 실패");
            retryPopUp.gameObject.SetActive(true);
        }
    }

  

    public async UniTask OnClickRetryGoogleLogin()
    {
        retryPopUp.gameObject.SetActive(false);

        _dataManager.SignIn();
        StartCoroutine(ExecuteLogIn());

        if (true == _isSuccess)
        {
            _gameManager.SetGameNetworkType(GameNetworkType.Online);
            await LoadLobbyScene();
        }
        else
        {
            //logInPanel.gameObject.SetActive(false);
            retryPopUp.SetButtonAction(OnClickRetryGoogleLogin());
            retryPopUp.SetDescription("구글 로그인 실패");
            retryPopUp.gameObject.SetActive(true);
        }
    }

    public void OnClickPlayGuestMode()
    {
        _gameManager.SetGameNetworkType(GameNetworkType.Offline);
        LoadLobbyScene().Forget();
    }

    public async UniTask LoadLobbyScene()
    {
        await LoadPlayerData(_gameManager.GetGameNetworkType());
    }

    public async UniTask LoadPlayerData(GameNetworkType type)
    {
        retryPopUp.gameObject.SetActive(false);
        await _dataManager.LoadPlayerData(type);
        
        if(NetworkRequestStatus.Done == _dataManager.GetRequestStatus())
        {
            // 성공
            _isStart = true;
            SceneManager.LoadScene("LobbyScene");
        }
        else if (NetworkRequestStatus.Fail == _dataManager.GetRequestStatus())
        {
            // 실패 UI 띄우기
            retryPopUp.SetButtonAction(LoadLobbyScene());
            retryPopUp.SetDescription("데이터 불러오기 실패");
            retryPopUp.gameObject.SetActive(true);
        }
    }

    private IEnumerator ExecuteLogIn()
    {
        _isSuccess = false;
        float timer = 0f;

        while (timer <= 5f)
        {
            if(true == _dataManager.IsAuthenticated())
            {
                _isSuccess = true;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
