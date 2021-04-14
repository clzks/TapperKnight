using System.Collections;
using System.Collections.Generic;

public class EnemyModel : IModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Damage { get; set; }
    public int MaxHp { get; set; }
    public int MinHp { get; set; }
    public float HpIntense { get; set; }
    public float MoveSpeed { get; set; }
    public Queue<NoteType> Notes { get; set; }
    public float MaxNoteInterval { get; set; }
    public float MinNoteInterval { get; set; }
    public float NoteIntervalIntense { get; set; }
    public float TotalTime { get; set; }

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
