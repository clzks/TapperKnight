using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Vector2 _noteBoxPos;
    [SerializeField] private List<Vector3> _notePopDestination;
    [SerializeField] private Image _hpBar;
    [SerializeField] private Text _runnigRecord;
    [SerializeField] private Text _score;
    [Header("Stage")]
    private InGameState _state = InGameState.Count;
    private StageModel _inGameStageModel;
    [SerializeField] private int _currentStageNumber = 1;
    [SerializeField] private float _currGenTimer = 3f;
    [SerializeField] private float _genTime;
    [SerializeField] private float _enemySpeedFactorByPlayer = 0.1f;
    [SerializeField] private float _currStageRunningDistance;
    [SerializeField] private float _currStageTrackLength;
    private bool _isLastStage = false;
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
    [SerializeField] private Transform _inGamePool;
    [SerializeField] private Transform _spawnObject;
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
            _leftButton.onClick.AddListener(() => OnClickButton(NoteType.Left).Forget());
        }

        if (null == _rightButton)
        {
            _rightButton = GameObject.Find("RightButton").GetComponent<Button>();
            _rightButton.onClick.AddListener(() => OnClickButton(NoteType.Right).Forget());
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
        _noteBoxPos = _noteBox.transform.position;
        _playerCharacter = GameObject.Find("Field/Player").GetComponent<BaseCharacter>();
        _bgController = GameObject.Find("Field/Backgrounds").GetComponent<BackgroundController>();
        _notePopDestination = new List<Vector3>();
        _notePopDestination.Add(GameObject.Find("Field/NotePopDestination/Bezier").transform.position);
        _notePopDestination.Add(GameObject.Find("Field/NotePopDestination/Destination").transform.position);
        _hpBar = GameObject.Find("Canvas/Status/HpGauge").GetComponent<Image>();
        _runnigRecord = GameObject.Find("Canvas/Status/RunningRecordValue").GetComponent<Text>();
        _score = GameObject.Find("Canvas/Status/ScoreValue").GetComponent<Text>();
        _inGamePool = GameObject.Find("ObjectPool").transform;
        _spawnObject = GameObject.Find("Field/SpawnSpot").transform;
    }

    private async UniTask Start()
    {
        SpriteRenderer spawnObjectImage = _spawnObject.GetComponentInChildren<SpriteRenderer>();
        spawnObjectImage.enabled = false;

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
        await UpdateCharacterRecord();
        await UpdateCharacterHp();
    }

    public async UniTask GetStageModelAsync()
    {
        await UniTask.Yield();
        _inGameStageModel = _inGamePresenter.GetStageModel(_currentStageNumber, ref _isLastStage);
        _currStageTrackLength = _inGameStageModel.TrackLength;
        _genTime = _inGameStageModel.MaximumGenCycle;
    }

    

    public void SetPresenter(InGamePresenter presenter)
    {
        _inGamePresenter = presenter;
    }

    #region FSM
    private async UniTask Ready()
    {
        //_playerCharacter.SetSampleCharacter();
        _currClickButton = NoteType.Null;
        await GetStageModelAsync();

        if (null != _inGameStageModel)
        {
            _state = InGameState.Play;
        }
        else
        {
            Debug.Log("게임데이터 로드 실패");
            await UniTask.Delay(1000);
        }
    }

    private async UniTask Play()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnClickButton(NoteType.Left).Forget();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            OnClickButton(NoteType.Right).Forget();
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

            var runDistance = await _playerCharacter.AddRecord();
            _currStageRunningDistance += runDistance;
            if (_currStageRunningDistance >= _currStageTrackLength)
            {
                if (0 == _objectPool.GetEnemyCount())
                {
                    _state = InGameState.Change;
                    await Change();
                }
            }
            else
            {
                if (_currGenTimer >= _genTime)
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
        _playerCharacter.SetSortingLayer("StageChangeBlock", 4).Forget();
        await _bgController.ExecuteStageChange(_currentStageNumber, _blackOutTime);
        _playerCharacter.SetSortingLayer("Background", 4).Forget();
        await InitStageFactor();
        await GetStageModelAsync();
        SetGenTime().Forget();
        _state = InGameState.Play;
    }
    #endregion
    private async UniTask InitStageFactor()
    {
        _currGenTimer = 0f;
        _currStageRunningDistance = 0f;
        await UniTask.Yield();
    }

    private async UniTaskVoid OnClickButton(NoteType type)
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
            //else
            //{
            //    delayTimer += _bothSideDelayTime;
            //}

            return;
        }

        while(delayTimer <= _bothSideDelayTime)
        {
            delayTimer += Time.deltaTime;
            await UniTask.Yield();
        }

        var note = _targetEnemy?.GetNote();

        note?.OnNoteCall(_currClickButton).Forget();
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
        await UniTask.Yield();
    }


    private async UniTask UpdateCharacterHp()
    {
        _hpBar.fillAmount = _playerCharacter.GetHpPercent();
        await UniTask.Yield();
    }

    private async UniTask UpdateCharacterRecord()
    {
        _runnigRecord.text = _playerCharacter.GetRunningRecord().ToString();
        _score.text = _inGamePresenter.GetScore().ToString();
        await UniTask.Yield();
    }

    private async UniTaskVoid SetGenTime()
    {
        if(_inGameStageModel != null)
        {
            _genTime = Random.Range(_inGameStageModel.MinimumGenCycle, _inGameStageModel.MaximumGenCycle);
        }
        await UniTask.Yield();
    }

    public float GetPlayerSpeed()
    {
        return _playerCharacter.GetSpeed();
    }

    public Vector2 GetNoteBoxPos()
    {
        return _noteBoxPos;
    }

    public List<Vector3> GetNotePopDestination()
    {
        return _notePopDestination;
    }

    public async UniTask TakeDamage(float damage)
    {
        await _playerCharacter.TakeDamage(damage);
    }

    public async UniTask AddSpeed(float accel)
    {
        await _playerCharacter.AddSpeed(accel);
    }

    public async UniTask SetTarget(BaseEnemy enemy)
    {
        _targetEnemy = enemy;
        await UniTask.Yield();
    }

    public BaseEnemy GetTarget()
    {
        return _targetEnemy;
    }

    public async UniTask MakeEnemy()
    {
        await UniTask.Yield();
        BaseEnemy enemy = (BaseEnemy)_objectPool.MakeObject(ObjectType.Enemy);
        enemy.SetPlayerSpeedFactor(_enemySpeedFactorByPlayer).Forget();
        await enemy.SetInGamePool(_inGamePool);
        var enemyModel = _inGamePresenter.GetRandomEnemy(_currentStageNumber);
        await enemy.SetEnemy(enemyModel, _spawnObject.position);
        SetGenTime().Forget();
    }

    public async UniTask OnTargetDestroy()
    {
        var nextTarget = _objectPool.GetEnemy();
        _targetEnemy = nextTarget ? nextTarget : null;
        await UniTask.Yield();
    }
}
