using System.Collections;
using System.Collections.Generic;

public class EnemyModel : IModel
{
    public int Id;
    public string Name;
    public float Damage;
    public int MaxHp;
    public int MinHp;
    public float HpIntense;
    public float MoveSpeed;
    public Queue<NoteType> Notes;
    public float MaxNoteInterval;
    public float MinNoteInterval;
    public float NoteIntervalIntense;
    public float TotalTime;
    
    internal static EnemyModel MakeSampleEnemyModel()
    {
        return new EnemyModel()
        {
            Id = 0,
            Name = "SampleEnemy",
            Damage = 1,
            MaxHp = 5,
            MinHp = 2,
            HpIntense = 0.5f,
            MoveSpeed = 1f,
            Notes = new Queue<NoteType>(),
            MaxNoteInterval = 1f,
            MinNoteInterval = 0.2f,
            NoteIntervalIntense = 0.5f,
            TotalTime = 0f
        };
    }


}
