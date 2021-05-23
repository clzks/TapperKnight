using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class BaseEnemy : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    public Queue<BaseNote> enemyNotes;
    public EnemyStatus status;
    private InGamePresenter _inGamePresenter;
    [SerializeField]private GameObject _noteParent;
    public SkinnedMeshRenderer meshRenderer;
    private Transform _inGamePool;
    private float _playerSpeedFactor;
    private List<Vector3> _popDestination;

    private CancellationTokenSource _disableCancellation = new CancellationTokenSource();
    //private CancellationTokenSource _destroyCancellation = new CancellationTokenSource();
    private async UniTask OnEnable()
    {
        if (_disableCancellation != null)
        {
            _disableCancellation.Dispose();
        }
        _disableCancellation = new CancellationTokenSource();

        meshRenderer.sortingLayerName = "Background";
        meshRenderer.sortingOrder = 4;
        if (null == _objectPool)
        {
            _objectPool = ObjectPoolManager.Get();
        }
        if(null == _inGamePresenter)
        {
            _inGamePresenter = GameManager.Get().GetInGamePresenter();
        }
        if(null == _popDestination)
        {
            _popDestination = _inGamePresenter.GetNotePopDestination();
        }

        await UniTask.Yield();
    }

    private async UniTask Update()
    {
        var playerSpd = _inGamePresenter.GetPlayerSpeed();
        transform.position -= new Vector3((status.speed + playerSpd * _playerSpeedFactor) * Time.deltaTime, 0f, 0f);
        await UniTask.Yield();
    }

    private void OnDisable()
    {
        _disableCancellation.Cancel();
    }

    public async UniTask SetEnemy(EnemyModel em, Vector3 SpawnObjectPos)
    {
        SetStatus(em).Forget();
        transform.position = new Vector3(SpawnObjectPos.x, SpawnObjectPos.y, transform.position.z);
        enemyNotes = new Queue<BaseNote>();
        if (null == _inGamePresenter)
        {
            _inGamePresenter = GameManager.Get().GetInGamePresenter();
        }

        if (null == _inGamePresenter.GetTarget())
        {
            await _inGamePresenter.SetTarget(this);
        }

        var interval = em.MinNoteInterval == em.MaxNoteInterval ? em.MinNoteInterval : Random.Range(em.MinNoteInterval, em.MaxNoteInterval); 
        SetNote(interval).Forget();
    }
    public async UniTaskVoid SetStatus(EnemyModel model)
    {
        status.name = model.Name;
        status.damage = model.Damage;
        status.speed = model.MoveSpeed;
        status.hp = model.MinNoteCount == model.MaxNoteCount ? model.MinNoteCount : Random.Range(model.MinNoteCount, model.MaxNoteCount + 1);
        await UniTask.Yield();
    }
    public async UniTaskVoid SetPlayerSpeedFactor(float playerSpeedFactor)
    {
        _playerSpeedFactor = playerSpeedFactor;
        await UniTask.Yield();
    }

    private async UniTaskVoid SetNote(float interval)
    {
        var notePos = _inGamePresenter.GetNoteBoxPos();

        for (int i = 0; i < status.hp; ++i)
        {
            BaseNote note = (BaseNote)_objectPool.MakeObject(ObjectType.Note);
            note.SetInGamePool(_inGamePool).Forget();
            SetRandomNoteType(note).Forget();
            note.SetSortingLayer(status.hp - i - 1).Forget();
            note.transform.SetParent(_inGamePool.transform);
            note.SetPosition(new Vector3(transform.position.x + i * interval, notePos.y, transform.position.z)).Forget();
            note.SetParentEnemy(this).Forget();
            note.SetBoxPosition(notePos.x).Forget();
            //note.SetBoxSize(1f).Forget();
            note.SetNoteSpeed(status.speed, _playerSpeedFactor).Forget();
            note.SetNotePopDestination(_inGamePresenter.GetNotePopDestination()).Forget();
            enemyNotes.Enqueue(note);
        }
        
        await UniTask.Yield();
    }

    private async UniTaskVoid SetRandomNoteType(BaseNote bn)
    {
        int r = Random.Range(1, 10000) % 3;

        if(0 == r)
        {
            bn.noteType = NoteType.Left;
            bn.SetNoteSprite(_inGamePresenter.GetNoteSprite("Left")).Forget();
        }
        else if(1 == r)
        {
            bn.noteType = NoteType.Right;
            bn.SetNoteSprite(_inGamePresenter.GetNoteSprite("Right")).Forget();
        }
        else
        {
            bn.noteType = NoteType.BothSide;
            bn.SetNoteSprite(_inGamePresenter.GetNoteSprite("BothSide")).Forget();
        }
        await UniTask.Yield();
    }

    public async UniTask OnNoteCall(ScoreType score)
    {
        await DropNote();
        await _inGamePresenter.OnNoteCall(score, status.damage);
        
        if(ScoreType.Miss != score)
        {
            status.hp -= 1;
        }

        await UniTask.Yield(_disableCancellation.Token);
        if(0 == enemyNotes.Count)
        {
            await ExecuteDead();
        }
        else
        {
            transform.position = new Vector3(enemyNotes.Peek().transform.position.x, transform.position.y, transform.position.z);
        }
    }

    public BaseNote GetNote()
    {
        if (enemyNotes.Count != 0)
        {
            return enemyNotes.Peek();
        }
        else
        {
            return null;
        }
    }

    public async UniTask SetInGamePool(Transform tr)
    {
        _inGamePool = tr;
        transform.SetParent(_inGamePool);
        await UniTask.Yield();
    }
    
    public async UniTask ExecuteDead()
    {
        if(status.hp == 0)
        { 
            await EnemyPop();
        }
        else
        {
            // 공격모션
        }
        ReturnObject();
        await _inGamePresenter.OnTargetDestroy();
    }

    private async UniTask EnemyPop()
    {
        float time = 0f;

        Vector3 startPos = transform.position;

        while (time <= 1f)
        {
            transform.position = Formula.BezierMove(startPos, _popDestination[0], _popDestination[1], time);
            transform.Rotate(0, 0, -720f * Time.deltaTime, Space.Self);
            await UniTask.Yield(_disableCancellation.Token);
            time += Time.deltaTime * 2f;
        }
    }

    private async UniTask DropNote()
    {
        BaseNote note = enemyNotes.Dequeue();
        note.SetParentEnemy(null).Forget();
        note.transform.SetParent(_inGamePool);
        await UniTask.Yield();
    }
    public void Init()
    {
        transform.position = new Vector3(1000, 1000, 0);
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public int GetNoteCount()
    {
        return enemyNotes.Count;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Enemy;
    }
}

public struct EnemyStatus
{
    public string name;
    public float damage;
    public int hp;
    public float speed;
}