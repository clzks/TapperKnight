using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class BaseEnemy : MonoBehaviour
{
    private ObjectPoolManager _objectPool;
    public Queue<BaseNote> enemyNotes;
    public EnemyStatus status;
    private InGamePresenter _inGamePresenter;
    [SerializeField]private GameObject noteParent;
    public SkinnedMeshRenderer meshRenderer;
    private Transform _inGamePool;
   
    private void Awake()
    {
        meshRenderer.sortingLayerName = "Background";
        meshRenderer.sortingOrder = 4;
        _objectPool = ObjectPoolManager.Get();
        _inGamePresenter = GameManager.Get().GetInGamePresenter();
    }

    private async UniTask Update()
    {
        transform.position -= new Vector3(status.speed, 0f, 0f) * Time.deltaTime;
        await UniTask.Yield();
    }

    public async UniTask SetEnemy(EnemyModel em, float playerPosY)
    {
        SetStatus(em).Forget();
        transform.position = new Vector3(10.5f, playerPosY, transform.position.z);
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
    private async UniTaskVoid SetNote(float interval)
    {
        float notePosY = _inGamePresenter.GetNoteBoxPosY();
        // 계산된 Hp 개수만큼 Queue에 Note생성 해주는것 필요
        if(status.hp % 2 == 0)
        {
            //짝수
            for (int i = 0; i < status.hp; ++i)
            {
                BaseNote note = _objectPool.MakeNote();
                note.SetInGamePool(_inGamePool).Forget();
                SetRandomNote(note).Forget();
                note.transform.SetParent(noteParent.transform);
                note.SetPosition(new Vector3(transform.position.x + i * interval - interval * (status.hp / 2) + 0.5f * interval, notePosY, transform.position.z)).Forget();
                note.SetParentEnemy(this).Forget();
                note.SetBoxPosition(-4.5f).Forget();
                note.SetBoxSize(1f).Forget();
                note.SetNotePopDestination(_inGamePresenter.GetNotePopDestination()).Forget();
                enemyNotes.Enqueue(note);
            }
        }
        else
        {
            //홀수
            for (int i = 0; i < status.hp; ++i)
            {
                BaseNote note = _objectPool.MakeNote();
                note.SetInGamePool(_inGamePool).Forget();
                SetRandomNote(note).Forget();
                note.transform.SetParent(noteParent.transform);
                note.SetParentEnemy(this).Forget();
                note.SetPosition(new Vector3(transform.position.x + i * interval - interval * (status.hp / 2), notePosY, transform.position.z)).Forget();
                note.SetBoxPosition(-4.5f).Forget();
                note.SetBoxSize(1f).Forget();
                note.SetNotePopDestination(_inGamePresenter.GetNotePopDestination()).Forget();
                enemyNotes.Enqueue(note);
            }
        }

        await UniTask.Yield();
    }

    private async UniTaskVoid SetRandomNote(BaseNote bn)
    {
        int r = Random.Range(1, 10000) % 3;

        if(0 == r)
        {
            bn.noteType = NoteType.Left;
            bn.SetNoteSprite(ObjectPoolManager.Get().spriteList["Left"]).Forget();
        }
        else if(1 == r)
        {
            bn.noteType = NoteType.Right;
            bn.SetNoteSprite(ObjectPoolManager.Get().spriteList["Right"]).Forget();
        }
        else
        {
            bn.noteType = NoteType.BothSide;
            bn.SetNoteSprite(ObjectPoolManager.Get().spriteList["BothSide"]).Forget();
        }
        await UniTask.Yield();
    }

    private async UniTaskVoid SetBothSideNote(BaseNote bn)
    {
        bn.noteType = NoteType.BothSide;
        bn.SetNoteSprite(ObjectPoolManager.Get().spriteList["BothSide"]).Forget();
        await UniTask.Yield();
    }

    public async UniTask OnNoteCall(ScoreType score)
    {
        await ResetNote();
        await _inGamePresenter.OnNoteCall(score, status.damage);

        if(0 == enemyNotes.Count)
        {
            await DestroyEnemy();
            await _inGamePresenter.OnTargetDestroy();
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

    public async UniTask DestroyEnemy()
    {
        await ObjectPoolManager.Get().DestroyEnemy(this);
        await UniTask.Yield();
    }
}

public struct EnemyStatus
{
    public string name;
    public float damage;
    public int hp;
    public float speed;
}