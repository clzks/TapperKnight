using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    private ObjectPoolManager _objectPool;
    public Queue<BaseNote> enemyNotes;
    public EnemyStatus status;
    private InGamePresenter _inGamePresenter;
    [SerializeField]private GameObject noteParent;

    public void SetEnemyStatus(EnemyModel model)
    {
        status.name = model.Name;
        status.damage = model.Damage;
        status.speed = model.MoveSpeed;
        SetEnemyNote(model);
    }
    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
        _inGamePresenter = GameManager.Get().GetInGamePresenter();
    }

    private void Update()
    {
        transform.position -= new Vector3(status.speed, 0f, 0f) * Time.deltaTime;
    }

    public void SetSampleEnemy()
    {
        transform.position = new Vector3(10.5f ,transform.position.y, transform.position.z);
        enemyNotes = new Queue<BaseNote>();
        status.name = "임시적";
        status.damage = 2;
        status.speed = 3;
        //BaseEnemy enemy = Instantiate(ObjectPoolManager.Get().prefabList["BaseEnemy"]).GetComponent<BaseEnemy>();
        if (null == _inGamePresenter)
        {
            _inGamePresenter = GameManager.Get().GetInGamePresenter();
        }

        if(null == _inGamePresenter.GetTarget())
        {
            _inGamePresenter.SetTarget(this);
        }

        float y = _inGamePresenter.GetNoteBoxPosY();
        for (int i = 0; i < 3; ++i)
        {
            BaseNote note = _objectPool.MakeNote();
            //SetRandomNote(note);
            SetBothSideNote(note);
            note.transform.SetParent(noteParent.transform);
            // Interval 값들로 하여금 노트간의 간격 계산해서 생성해주는것 필요 (노트 오브젝트는 몬스터의 자식으로 들어갈것)
            note.SetPosition(new Vector3(transform.position.x + i * 0.4f - 0.4f, y, transform.position.z));
            note.SetParent(this);
            note.SetBoxPosition(-4.5f);
            note.SetBoxSize(1f);
            enemyNotes.Enqueue(note);
        }
    }

    private void SetEnemyNote(EnemyModel model)
    {
        float notePosY = _inGamePresenter.GetNoteBoxPosY();
        // MaxHp, MinHp, HpIntense를 통한 Enemy Hp 계산식필요
        status.hp = Formula.EnemyHpFormula(model);
        // 계산된 Hp 개수만큼 Queue에 Note생성 해주는것 필요
        for(int i = 0; i < status.hp; ++i)
        {
            BaseNote note = Instantiate(ObjectPoolManager.Get().prefabList["Note"]).GetComponent<BaseNote>();
            SetRandomNote(note);
            note.transform.SetParent(noteParent.transform);
            // Interval 값들로 하여금 노트간의 간격 계산해서 생성해주는것 필요 (노트 오브젝트는 몬스터의 자식으로 들어갈것)
            // 짝수일때는 몬스터를 가운데두고, 홀수일때는 가운데 노트가 몬스터와 동일한 위치다
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
    //public float MaxHp;
    //public float MinHp;
    //public float HpIntense;
    public float speed;
    //public Queue<NoteType> Notes;
    //public float MaxNoteInterval;
    //public float MinNoteInterval;
    //public float NoteIntervalIntense;
    //public float TotalTime;
}