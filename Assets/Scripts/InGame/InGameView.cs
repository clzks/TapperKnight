using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class InGameView : MonoBehaviour, IView
{
    private GameManager _gameManager;
    private ObjectPoolManager _objectPool;

    [Header("UI")]
    [SerializeField] private InGamePresenter _inGamePresenter;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Button _singleRespawnButton;
    [SerializeField] private Button _autoRespawnButton;
    [SerializeField] private GameObject _noteBox;
    [SerializeField] private float _noteBoxPosY;

    [Header("Stage")]
    private InGameState _state = InGameState.Count;
    private StageModel _inGameStageModel;
    private Dictionary<ScoreType, float> _scoreDistanceList;
    private int _currentStageNumber = 1;
    [SerializeField] private float _currStageTimer = 0f;
    [SerializeField] private float _maxStageTime;
    [SerializeField] private float _currGenTimer = 3f;
    [SerializeField] private float _maxGenTime;
    private float _blackOutTime = 1.3f;

    [Header("Control")]
    [Tooltip("버튼 동시 입력을 위한 대기시간(초)")]
    [SerializeField] private float _bothSideDelayTime = 0.05f;
    private float delayTimer = 0f;

    [Header("Object")]
    [SerializeField] private BackgroundController _bgController;
    [SerializeField] private BaseCharacter _playerCharacter;
    [SerializeField] private BaseEnemy _targetEnemy;
    [SerializeField] private NoteType _currClickButton = NoteType.Null;

    [Header("Test")]
    public int testMaxStageNumber;
    [SerializeField] private bool _isAutoMode = false;
    private void Awake()
    {
        _gameManager = GameManager.Get();
        _objectPool = ObjectPoolManager.Get();
        _objectPool.InitPool();

        _state = InGameState.Ready;

        if (null == _leftButton)
        {
            _leftButton = GameObject.Find("LeftButton").GetComponent<Button>();
            _leftButton.onClick.AddListener(async () => await OnClickButton(NoteType.Left));
        }

        if (null == _rightButton)
        {
            _rightButton = GameObject.Find("RightButton").GetComponent<Button>();
            _rightButton.onClick.AddListener(async () => await OnClickButton(NoteType.Right));
        }

        if (null == _singleRespawnButton)
        {
            _singleRespawnButton = GameObject.Find("SingleRespawn").GetComponent<Button>();
            _singleRespawnButton.onClick.AddListener(async () => await OnClickSingleRespawnButton());
        }

        if (null == _autoRespawnButton)
        {
            _autoRespawnButton = GameObject.Find("AutoRespawn").GetComponent<Button>();
            _autoRespawnButton.onClick.AddListener(async () => await OnClickAutoRespawnButton());
        }

        _noteBox = GameObject.Find("Field/NoteBox");
        _noteBoxPosY = _noteBox.transform.position.y;
        _playerCharacter = GameObject.Find("Field/Player").GetComponent<BaseCharacter>();
        _bgController = GameObject.Find("Field/Backgrounds").GetComponent<BackgroundController>();
    }

    private async UniTask Start()
    {
        while (InGameState.Ready == _state)
        {
            await Ready();
        }
    }

    private async UniTask Update()
    {
        if (InGameState.Play != _state)
        {
            return;
        }

        await Play();
    }

    public async UniTask GetStageModelAsync()
    {
        await UniTask.Yield();
        _inGameStageModel = _inGameStageModel ?? _inGamePresenter.GetStageModel(_currentStageNumber);
        _maxStageTime = _inGameStageModel.StageTime;
        _maxGenTime = _inGameStageModel.MaximumGenCycle;
    }

    public async UniTask GetScoreModelAsync()
    {
        await UniTask.Yield();
        _scoreDistanceList = _inGamePresenter.GetScoreModel();
    }

    public void SetPresenter(InGamePresenter presenter)
    {
        _inGamePresenter = presenter;
    }

    #region FSM
    private async UniTask Ready()
    {
        _playerCharacter.SetSampleCharacter();
        _currClickButton = NoteType.Null;
        await GetStageModelAsync();
        await GetScoreModelAsync();

        if (null != _inGameStageModel && null != _scoreDistanceList)
        {
            _state = InGameState.Play;
        }
        else
        {
            Debug.Log("게임데이터 로드 실패");
            await UniTask.Delay(100);
        }
    }

    private async UniTask Play()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            await OnClickButton(NoteType.Left);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            await OnClickButton(NoteType.Right);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            await OnClickAutoRespawnButton();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            await OnClickSingleRespawnButton();
        }

        if (true == _isAutoMode)
        {
            _currGenTimer += Time.deltaTime;
            _currStageTimer += Time.deltaTime;

            if (_currStageTimer >= _maxStageTime)
            {
                if (0 == _objectPool.GetEnemyCount())
                {
                    _state = InGameState.Change;
                    await Change();
                }
            }
            else
            {
                if (_currGenTimer >= 1f)
                {
                    _currGenTimer = 0f;
                    await MakeEnemy();
                }
            }
        }
    }

    private async UniTask Change()
    {
        _currentStageNumber++;
        _playerCharacter.SetSortingLayer("StageChangeBlock", 4);
        await _bgController.ExecuteStageChange(_currentStageNumber, _blackOutTime);
        _playerCharacter.SetSortingLayer("Background", 4);
        await InitStageFactor();
        await GetStageModelAsync();
        _state = InGameState.Play;
    }
    #endregion
    private async UniTask InitStageFactor()
    {
        _currGenTimer = 0f;
        _currStageTimer = 0f;
        await UniTask.Yield();
    }

    private async UniTask OnClickButton(NoteType type)
    {
        if(NoteType.Null == _currClickButton)
        {
            _currClickButton = type;
        }
        else
        {
            if(1 == ((int)_currClickButton + (int)type))
            {
                _currClickButton = NoteType.BothSide;
            }
            else
            {
                delayTimer += _bothSideDelayTime;
            }

            return;
        }

        while(delayTimer <= _bothSideDelayTime)
        {
            delayTimer += Time.deltaTime;
            await UniTask.Yield();
        }

        var note = _targetEnemy?.GetNote();

        note?.OnNoteCall(_currClickButton);
        _currClickButton = NoteType.Null;
        delayTimer = 0f;
    }

    private async UniTask OnClickSingleRespawnButton()
    {
        await MakeEnemy();
    }

    private async UniTask OnClickAutoRespawnButton()
    {
        _isAutoMode = !_isAutoMode;
    }

    public float GetNoteBoxPosY()
    {
        return _noteBoxPosY;
    }

    public void SetTarget(BaseEnemy enemy)
    {
        _targetEnemy = enemy;
    }

    public BaseEnemy GetTarget()
    {
        return _targetEnemy;
    }

    public async UniTask MakeEnemy()
    {
        await UniTask.Yield();
        BaseEnemy enemy = _objectPool.MakeEnemy();
        var enemyModel = _inGamePresenter.GetRandomEnemy(_currentStageNumber);
        enemy.SetEnemy(enemyModel, _playerCharacter.GetPositionY());
    }

    public void OnTargetDestroy()
    {
        var nextTarget = _objectPool.GetEnemy();

        _targetEnemy = nextTarget ? nextTarget : null;
    }
}
