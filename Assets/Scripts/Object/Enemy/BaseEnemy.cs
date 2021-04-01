using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public Queue<BaseNote> enemyNotes;
    public EnemyStatus status;
    private InGamePresenter inGamePresenter;

    public void SetEnemyStatus(EnemyModel model)
    {
        status.name = model.Name;
        status.damage = model.Damage;
        status.speed = model.MoveSpeed;
        SetEnemyNote(model);
    }

    private void SetEnemyNote(EnemyModel model)
    {
        float notePosY = inGamePresenter.GetNoteBoxPosY();
        // MaxHp, MinHp, HpIntense를 통한 Enemy Hp 계산식필요
        status.hp = Formula.EnemyHpFormula(model);
        // 계산된 Hp 개수만큼 Queue에 Note생성 해주는것 필요
        // Interval 값들로 하여금 노트간의 간격 계산해서 생성해주는것 필요 (노트 오브젝트는 몬스터의 자식으로 들어갈것)
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