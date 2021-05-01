using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
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

    private async UniTask OnEnable()
    {
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
        await UniTask.Yield();
    }

    private async UniTask Update()
    {
        var playerSpd = _inGamePresenter.GetPlayerSpeed();
        transform.position -= new Vector3((status.speed + playerSpd * _playerSpeedFactor) * Time.deltaTime, 0f, 0f);
        await UniTask.Yield();
    }

    public async UniTask SetEnemy(EnemyModel em, Vector3 SpawnObjectPos)
    {
        SetStatus(em).Forget();
        transform.position = new Vector3(SpawnObjectPos.x, SpawnObjectPos.y, transform.position.z);
        enemyNotes = new Queue<BaseNote>();
        float interval = em.NoteInterval;
        if (null == _inGamePresenter)
        {
            _inGamePresenter = GameManager.Get().GetInGamePresenter();
        }

        if (null == _inGamePresenter.GetTarget())
        {
            await _inGamePresenter.SetTarget(this);
        }

        SetNote(interval).Forget();
    }
    public async UniTaskVoid SetStatus(EnemyModel model)
    {
        status.name = model.Name;
        status.damage = model.Damage;
        status.speed = model.MoveSpeed;
        status.hp = model.NoteCount;
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
            SetRandomNote(note).Forget();
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

    private async UniTaskVoid SetRandomNote(BaseNote bn)
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

    private async UniTaskVoid SetBothSideNote(BaseNote bn)
    {
        bn.noteType = NoteType.BothSide;
        bn.SetNoteSprite(_inGamePresenter.GetNoteSprite("BothSide")).Forget();
        await UniTask.Yield();
    }

    public async UniTask OnNoteCall(ScoreType score)
    {
        await ResetNote();
        await _inGamePresenter.OnNoteCall(score, status.damage);

        if(0 == enemyNotes.Count)
        {
            await ReturnObject();
            await _inGamePresenter.OnTargetDestroy();
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

    private async UniTask ResetNote()
    {
        BaseNote note = enemyNotes.Dequeue();
        note.SetParentEnemy(null).Forget();
        note.transform.SetParent(_inGamePool);
        await UniTask.Yield();
    }
    public async UniTaskVoid Init()
    {
        transform.position = new Vector3(1000, 1000, 0);
        await UniTask.Yield();
    }

    public async UniTask ReturnObject()
    {
        await _objectPool.ReturnObject(this);
    }

    public GameObject GetObject()
    {
        return gameObject;
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