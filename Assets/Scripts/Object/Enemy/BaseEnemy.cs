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
        // MaxHp, MinHp, HpIntense�� ���� Enemy Hp �����ʿ�
        status.hp = Formula.EnemyHpFormula(model);
        // ���� Hp ������ŭ Queue�� Note���� ���ִ°� �ʿ�
        // Interval ����� �Ͽ��� ��Ʈ���� ���� ����ؼ� �������ִ°� �ʿ� (��Ʈ ������Ʈ�� ������ �ڽ����� ����)
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