using System.Collections;
using System.Collections.Generic;

public class EnemyModel : IModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Damage { get; set; }
    public float MoveSpeed { get; set; }
    public int NoteCount { get; set; }
    public float NoteInterval { get; set; }

    internal static EnemyModel MakeSampleEnemyModel()
    {
        return new EnemyModel()
        {
            Id = 0,
            Name = "SampleEnemy",
            Damage = 1,
            MoveSpeed = 1f,
            NoteCount = 3,
            NoteInterval = 0.5f
        };
    }


}
