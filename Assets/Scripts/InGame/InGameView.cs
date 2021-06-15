using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class InGameView : MonoBehaviour
{
    private GameManager _gameManager;
    private ObjectPoolManager _objectPool;

    [Header("UI")]
    [SerializeField] private InGamePresenter _inGamePresenter;
    [SerializeField] private NoteButton _leftButton;
    [SerializeField] private NoteButton _rightButton;
    [SerializeField] private Button _singleRespawnButton;
    [SerializeField] private Button _autoRespawnButton;
    [SerializeField] private Button _returnToCharacterSelectButton;
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
    [Header("TitleAutoMode")]
    [SerializeField] private bool _isTitleMode;
    [SerializeField] private float _autoPlayDelay;
    [SerializeField] private float _autoPlayTimer = 0f;

    private async UniTask Awake()
    {
        _gameManager = GameManager.Get();
        _isTitleMode = _gameManager.isTitle;
        _objectPool = ObjectPoolManager.Get();
        _objectPool.InitPool();

        _state = InGameState.Ready;

        if (false == _isTitleMode)
        {
            if (null == _leftButton)
            {
                _leftButton = GameObject.Find("LeftButton").GetComponent<NoteButton>();
                _leftButton.SetBtnAction(() => OnClickButton(NoteType.Left).Forget());
            }

            if (null == _rightButton)
            {
                _rightButton = GameObject.Find("RightButton").GetComponent<NoteButton>();
                _rightButton.SetBtnAction(() => OnClickButton(NoteType.Right).Forget());
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

            if (null == _returnToCharacterSelectButton)
            {
                _returnToCharacterSelectButton = GameObject.Find("ReturnToSelectButton").GetComponent<Button>();
                _returnToCharacterSelectButton.onClick.AddListener(() => OnClickReturnToCharacterSelectButton());
            }

            _hpBar = GameObject.Find("Canvas/Status/HpGauge").GetComponent<Image>();
            _runnigRecord = GameObject.Find("Canvas/Status/RunningRecordValue").GetComponent<Text>();
            _score = GameObject.Find("Canvas/Status/ScoreValue").GetComponent<Text>();
        }

        _playerCharacter = GameObject.Find("Field/Player").GetComponent<BaseCharacter>();
        _noteBox = GameObject.Find("Field/NoteBox");
        _noteBoxPos = _noteBox.transform.position;
        _bgController = GameObject.Find("Field/Backgrounds").GetComponent<BackgroundController>();
        _notePopDestination = new List<Vector3>();
        _notePopDestination.Add(GameObject.Find("Field/NotePopDestination/Bezier").transform.position);
        _notePopDestination.Add(GameObject.Find("Field/NotePopDestination/Destination").transform.position);
        _inGamePool = GameObject.Find("ObjectPool").transform;
        _spawnObject = GameObject.Find("Field/SpawnSpot").transform;
        await UniTask.Yield();
    }

    private async UniTask Start()
    {
        SpriteRenderer spawnObjectImage = _spawnObject.GetComponentInChildren<SpriteRenderer>();
        spawnObjectImage.enabled = false;

        while (InGameState.Ready == _state)
        {
            Debug.Log("스타트");
            await Ready();
            await UniTask.Delay(1000);
        }
    }

    private async UniTask Update()
    {
        if (_state == InGameState.Play)
        {
            await Play();
            await UpdateCharacterRecord();
            await UpdateCharacterHp();
        }
        else if(_state == InGameState.AutoPlay)
        {
            await AutoPlay();
        }
    }

    public async UniTask GetStageModelAsync()
    {
        await UniTask.Yield();
        _inGameStageModel = _inGamePresenter.GetStageModel(_currentStageNumber, ref _isLastStage);
        _currStageTrackLength = _inGameStageModel.TrackLength;
        _genTime = _inGameStageModel.MaximumGenCycle;
    }

    public async UniTask GetRandomStageModelAsync()
    {
        await UniTask.Yield();
        _inGameStageModel = _inGamePresenter.GetRandomStageModel();
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
        if(true == _isTitleMode)
        {
            await GetRandomStageModelAsync();
            _state = InGameState.AutoPlay;
            return;
        }
        
        _currClickButton = NoteType.Null;
        await GetStageModelAsync();
        
        if (null != _inGameStageModel)
        {
            _playerCharacter.CastLifeTimer().Forget();
            _state = InGameState.Play;
        }
        else
        {
            Debug.Log("게임데이터 로드 실패");
            await UniTask.Delay(1000);
        }
    }

    private async UniTask AutoPlay()
    {
        _currGenTimer += Time.deltaTime;
        _autoPlayTimer += Time.deltaTime;

        if (_currGenTimer >= _genTime)
        {
            var enemyModel = _inGamePresenter.GetRandomEnemy(_currentStageNumber);
            var delayTime = CheckEnemyInterval(enemyModel);
            if (delayTime > 0f)
            {
                _currGenTimer = _genTime - delayTime;
            }
            else
            {
                _currGenTimer = 0f;
                await MakeEnemy(enemyModel);
            }
        }

        if(_autoPlayTimer >= _autoPlayDelay)
        {
            _autoPlayTimer = 0f;

            var Note = _objectPool.GetNote();

            if(Note.GetPosition() - _noteBoxPos.x <= 1f)
            {
                int r = Random.Range(0, 101);
                
                if(r % 5 >= 1)
                {
                    OnClickButton(Note.GetNoteType()).Forget();
                }
                else
                {

                }
            }    
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
            if (_currStageRunningDistance >= _currStageTrackLength && !_isLastStage)
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
                    var enemyModel = _inGamePresenter.GetRandomEnemy(_currentStageNumber);
                    var delayTime = CheckEnemyInterval(enemyModel);
                    if (delayTime > 0f)
                    {
                        _currGenTimer = _genTime - delayTime;
                    }
                    else
                    {
                        _currGenTimer = 0f;
                        await MakeEnemy(enemyModel);
                    }
                }
            }
        }

    }

    private async UniTask Change()
    {
        _playerCharacter.StopLifeTimer();
        _currentStageNumber++;
        _playerCharacter.SetSortingLayer("StageChangeBlock").Forget();
        await _bgController.ExecuteStageChange(_currentStageNumber, _blackOutTime);
        _playerCharacter.SetSortingLayer("Character").Forget();
        await InitStageFactor();
        await GetStageModelAsync();
        SetGenTime();
        _state = InGameState.Play;
        _playerCharacter.CastLifeTimer().Forget();
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
            //Debug.Log(_currClickButton.ToString() + "클릭");
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
        var enemyModel = _inGamePresenter.GetRandomEnemy(_currentStageNumber);
        await MakeEnemy(enemyModel);
    }

    private async UniTask OnClickAutoRespawnButton()
    {
        _isAutoMode = !_isAutoMode;
        await UniTask.Yield();
    }

    private void OnClickReturnToCharacterSelectButton()
    {
        SceneManager.LoadScene("CharacterSelectScene");
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

    private void SetGenTime()
    {
        if(_inGameStageModel != null)
        {
            _genTime = Random.Range(_inGameStageModel.MinimumGenCycle, _inGameStageModel.MaximumGenCycle);
        }
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

    public void TakeDamage(float damage)
    {
        _playerCharacter.TakeDamage(damage, true);
    }

    public void AddSpeed(float accel)
    {
        _playerCharacter.AddSpeed(accel);
    }

    public void Attack()
    {
        _playerCharacter.Attack();
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

    public async UniTask MakeEnemy(EnemyModel enemyModel)
    {
        BaseEnemy enemy = (BaseEnemy)_objectPool.MakeObject(ObjectType.Enemy);
        enemy.SetPlayerSpeedFactor(_enemySpeedFactorByPlayer).Forget();
        await enemy.SetInGamePool(_inGamePool);
        await enemy.SetEnemy(enemyModel, _spawnObject.position);
        SetGenTime();
    }

    public float CheckEnemyInterval(EnemyModel WaitingEnemy)
    {
        var lastEnemy = _objectPool.GetLastEnemy();

        if (null != lastEnemy && lastEnemy.GetLastNote() != null)
        {
            var playerSpd = _inGamePresenter.GetPlayerSpeed();
            var LastNote = lastEnemy.GetLastNote();
            float firstArriveTime = LastNote.GetEstimatedArrivalTime();
            float secondArriveTime = (_spawnObject.position.x - _noteBoxPos.x) / (WaitingEnemy.MoveSpeed + playerSpd * _enemySpeedFactorByPlayer);

            if(secondArriveTime < firstArriveTime)
            {
                Debug.Log("후속노트가 선행노트를 앞지르므로 " + (firstArriveTime - secondArriveTime + 0.3f) + "초 만큼 뒤로 미룹니다");
                return firstArriveTime - secondArriveTime + 0.3f;
            }
            else if(secondArriveTime - 0.1f < firstArriveTime)
            {
                Debug.Log("후속노트가 선행노트를 앞지르므로 0.2초 만큼 뒤로 미룹니다");
                return 0.2f;
            }
            else if (secondArriveTime - 0.2f < firstArriveTime)
            {
                Debug.Log("후속노트가 선행노트를 앞지르므로 0.1초 만큼 뒤로 미룹니다");
                return 0.1f;
            }
            else
            {
                return 0f;
            }
        }
        else
        {
            return 0f;
        }
    }

    public async UniTask OnTargetDestroy()
    {
        var nextTarget = _objectPool.GetNextEnemy();
        _targetEnemy = nextTarget ? nextTarget : null;
        await UniTask.Yield();
    }
}
