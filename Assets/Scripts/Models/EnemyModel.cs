public class EnemyModel : IData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Damage { get; set; }
    public float MoveSpeed { get; set; }
    public int MinNoteCount { get; set; }
    public int MaxNoteCount { get; set; }
    public float MinNoteInterval { get; set; }
    public float MaxNoteInterval { get; set; }


    internal static EnemyModel MakeSampleEnemyModel()
    {
        return new EnemyModel()
        {
            Id = 0,
            Name = "SampleEnemy",
            Damage = 1,
            MoveSpeed = 1f,
            MinNoteCount = 3,
            MaxNoteCount = 3,
            MinNoteInterval = 0.5f,
            MaxNoteInterval = 0.5f
        };
    }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
