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
    [SerializeField] private float currGenTimer = 0f;
    [SerializeField] private float maxGenTime;
    private BaseEnemy targetEnemy;
    private void Awake()
    {
        _gameManager = GameManager.Get();
        _objectPool = ObjectPoolManager.Get();

        state = InGameState.Ready;

        if (null == leftButton)
        {
            leftButton = GameObject.Find("LeftButton").GetComponent<Button>();
            leftButton.onClick.AddListener(OnClickLeftButton);
        }

        if (null == rightButton)
        {
            rightButton = GameObject.Find("RightButton").GetComponent<Button>();
            rightButton.onClick.AddListener(OnClickRightButton);
        }

        noteBox = GameObject.Find("Field/NoteBox");
        noteBoxPosY = noteBox.transform.position.y;
        playerPos = GameObject.Find("Field/PlayerPos").transform.position;
    }

    private async void Update()
    {
        switch (state)
        {
            case InGameState.Ready:

                await GetStageModelAsync();
                await GetScoreModelAsync();

                if (null != inGameStageModel && null != scoreDistanceList)
                {
                    state = InGameState.Play;
                }
                else
                {
                    Debug.Log("게임데이터 로드 실패");
                    await UniTask.Delay(1000);
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

    public void OnClickLeftButton()
    {
        if (null != targetEnemy)
        {
            BaseNote note = targetEnemy.GetNote();
            note?.OnNoteCall(NoteType.Left);
        }
    }

    public void OnClickRightButton()
    {
        if (null != targetEnemy)
        {
            BaseNote note = targetEnemy.GetNote();
            note?.OnNoteCall(NoteType.Right);
        }
    }

    public void OnClickBothSideButtons()
    {
        if (null != targetEnemy)
        {
            BaseNote note = targetEnemy.GetNote();
            note?.OnNoteCall(NoteType.BothSide);
        }
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
