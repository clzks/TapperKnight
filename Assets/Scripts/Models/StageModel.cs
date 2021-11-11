using System.Collections.Generic;
using UnityEngine;

public class StageModel : IData
{
    public int Id { get; set; }
    public int StageNumber { get; set; }
    public float MinimumGenCycle { get; set; }
    public float MaximumGenCycle { get; set; }
    public float StageTime { get; set; }
    public List<int> EnemyList { get; set; }
    public float TrackLength { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return string.Empty;
    }
}

public struct SpawnEnemyInfo
{
    public int Id { get; set; }
    public int Intense { get; set; }
}