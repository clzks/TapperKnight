using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class InGameView : MonoBehaviour, IView
{
    private GameManager _gameManager;
    private ObjectPoolManager _objectPool;

    [SerializeField] private InGamePresenter _inGamePresenter;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private GameObject noteBox;
    [SerializeField] private float noteBoxPosY;
    [SerializeField] private Vector3 playerPos;
    private InGameState state = InGameState.Count;
    private StageModel inGameStageModel;
    Dictionary<ScoreType, float> scoreDistanceList;
    private int currentStageIndex = 0;
    [SerializeField] private float currStageTimer = 0f;
    [SerializeField] private float maxStageTime;
    [SerializeField] private float currGenTimer = 3f;
    [SerializeField] private float maxGenTime;
    [Tooltip("버튼 동시 입력을 위한 대기시간(1ms = 0.001s)")]
    [SerializeField] private float bothSideDelayTime = 0.03f;
    private float delayTimer = 0f;
    private BaseEnemy targetEnemy;
    private NoteType currClickButton = NoteType.Null;
    private void Awake()
    {
        _gameManager = GameManager.Get();
        _objectPool = ObjectPoolManager.Get();

        state = InGameState.Ready;

        if (null == leftButton)
        {
            leftButton = GameObject.Find("LeftButton").GetComponent<Button>();
            leftButton.onClick.AddListener(() => OnClickButton(NoteType.Left));
        }

        if (null == rightButton)
        {
            rightButton = GameObject.Find("RightButton").GetComponent<Button>();
            rightButton.onClick.AddListener(() => OnClickButton(NoteType.Right));
        }

        noteBox = GameObject.Find("Field/NoteBox");
        noteBoxPosY = noteBox.transform.position.y;
        playerPos = GameObject.Find("Field/PlayerPos").transform.position;
    }

    private async void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            OnClickButton(NoteType.Left);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            OnClickButton(NoteType.Right);
        }

        switch (state)
        {
            case InGameState.Ready:
                currClickButton = NoteType.Null;
                await GetStageModelAsync();
                await GetScoreModelAsync();

                if (null != inGameStageModel && null != scoreDistanceList)
                {
                    state = InGameState.Play;
                }
                else
                {
                    Debug.Log("게임데이터 로드 실패");
                    await UniTask.Delay(100);
                }
                break;

            case InGameState.Play:
                currGenTimer += Time.deltaTime;
                currStageTimer += Time.deltaTime;

                if (currGenTimer >= maxGenTime)
                {
                    BaseEnemy enemy = _objectPool.MakeEnemy();
                    enemy.SetSampleEnemy();
                    currGenTimer = 0f;
                }

                if (currStageTimer >= maxStageTime)
                {
                    state = InGameState.Change;
                }
                break;

            case InGameState.Change:

                currGenTimer = 0f;
                currStageTimer = 0f;
                currentStageIndex++;
                await GetStageModelAsync();
                state = InGameState.Play;
                break;
        }
    }

    public async UniTask GetStageModelAsync()
    {
        await UniTask.Yield();
        inGameStageModel = _inGamePresenter.GetStageModel(currentStageIndex);
        maxStageTime = inGameStageModel.TotalTime;
        maxGenTime = inGameStageModel.MaximumGenCycle;
    }

    public async UniTask GetScoreModelAsync()
    {
        await UniTask.Yield();
        scoreDistanceList = _inGamePresenter.GetScoreModel();
    }

    public void SetPresenter(InGamePresenter presenter)
    {
        _inGamePresenter = presenter;
    }

    public async void OnClickButton(NoteType type)
    {
        if(NoteType.Null == currClickButton)
        {
            currClickButton = type;
        }
        else
        {
            if(1 == ((int)currClickButton + (int)type))
            {
                currClickButton = NoteType.BothSide;
            }
            else
            {
                delayTimer += bothSideDelayTime;
            }

            return;
        }

        while(delayTimer <= bothSideDelayTime)
        {
            delayTimer += Time.deltaTime;
            await UniTask.Yield();
        }

        var note = targetEnemy?.GetNote();

        note?.OnNoteCall(currClickButton);
        currClickButton = NoteType.Null;
        delayTimer = 0f;
    }

    public float GetNoteBoxPosY()
    {
        return noteBoxPosY;
    }

    public void SetTarget(BaseEnemy enemy)
    {
        targetEnemy = enemy;
    }

    public BaseEnemy GetTarget()
    {
        return targetEnemy;
    }

    public void OnTargetDestroy()
    {
        targetEnemy = null;
    }
}
