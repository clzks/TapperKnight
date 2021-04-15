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

   
    private void Awake()
    {
        meshRenderer.sortingLayerName = "Background";
        meshRenderer.sortingOrder = 4;
        _objectPool = ObjectPoolManager.Get();
        _inGamePresenter = GameManager.Get().GetInGamePresenter();
    }

    private void Update()
    {
        transform.position -= new Vector3(status.speed, 0f, 0f) * Time.deltaTime;
    }

    public void SetEnemy(EnemyModel em)
    {
        SetStatus(em);
        transform.position = new Vector3(10.5f, transform.position.y, transform.position.z);
        enemyNotes = new Queue<BaseNote>();
        float interval = em.NoteInterval;
        if (null == _inGamePresenter)
        {
            _inGamePresenter = GameManager.Get().GetInGamePresenter();
        }

        if (null == _inGamePresenter.GetTarget())
        {
            _inGamePresenter.SetTarget(this);
        }

        SetNote(interval);
    }
    public void SetStatus(EnemyModel model)
    {
        status.name = model.Name;
        status.damage = model.Damage;
        status.speed = model.MoveSpeed;
        status.hp = model.NoteCount;
        //SetEnemyNote(model);
    }
    private void SetNote(float interval)
    {
        float notePosY = _inGamePresenter.GetNoteBoxPosY();
        // 계산된 Hp 개수만큼 Queue에 Note생성 해주는것 필요
        if(status.hp % 2 == 0)
        {
            //짝수
            for (int i = 0; i < status.hp; ++i)
            {
                BaseNote note = _objectPool.MakeNote();
                SetRandomNote(note);
                note.transform.SetParent(noteParent.transform);
                note.SetPosition(new Vector3(transform.position.x + i * interval - interval * (status.hp / 2) + 0.5f * interval, notePosY, transform.position.z));
                note.SetParentEnemy(this);
                note.SetBoxPosition(-4.5f);
                note.SetBoxSize(1f);
                enemyNotes.Enqueue(note);
            }
        }
        else
        {
            //홀수
            for (int i = 0; i < status.hp; ++i)
            {
                BaseNote note = _objectPool.MakeNote();
                SetRandomNote(note);
                note.transform.SetParent(noteParent.transform);
                note.SetPosition(new Vector3(transform.position.x + i * interval - interval * (status.hp / 2), notePosY, transform.position.z));
                note.SetParentEnemy(this);
                note.SetBoxPosition(-4.5f);
                note.SetBoxSize(1f);
                enemyNotes.Enqueue(note);
            }
        }
    }

    private void SetRandomNote(BaseNote bn)
    {
        int r = Random.Range(1, 10000) % 3;

        if(0 == r)
        {
            bn.noteType = NoteType.Left;
            bn.SetNoteSprite(ObjectPoolManager.Get().spriteList["Left"]);
        }
        else if(1 == r)
        {
            bn.noteType = NoteType.Right;
            bn.SetNoteSprite(ObjectPoolManager.Get().spriteList["Right"]);
        }
        else
        {
            bn.noteType = NoteType.BothSide;
            bn.SetNoteSprite(ObjectPoolManager.Get().spriteList["BothSide"]);
        }
    }

    private void SetBothSideNote(BaseNote bn)
    {
        bn.noteType = NoteType.BothSide;
        bn.SetNoteSprite(ObjectPoolManager.Get().spriteList["BothSide"]);
    }

    public void OnNoteCall(ScoreType score)
    {
        _inGamePresenter.OnNoteCall(score);
        enemyNotes.Dequeue();

        if(0 == enemyNotes.Count)
        {
            DestroyEnemy();
            _inGamePresenter.OnTargetDestroy();
        }
    }

    public BaseNote GetNote()
    {
        return enemyNotes.Peek();
    }

    public void DestroyEnemy()
    {
        //gameObject.SetActive(false);
        ObjectPoolManager.Get().DestroyEnemy(this);
    }
}

public struct EnemyStatus
{
    public string name;
    public float damage;
    public int hp;
    public float speed;
}