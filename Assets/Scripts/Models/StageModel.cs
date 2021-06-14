using System.Collections.Generic;
using UnityEngine;

public class StageModel
{
    public int Id { get; set; }
    public int StageNumber { get; set; }
    public float MinimumGenCycle { get; set; }
    public float MaximumGenCycle { get; set; }
    public float StageTime { get; set; }
    public List<int> EnemyList { get; set; }
    public float TrackLength { get; set; }
}

public struct SpawnEnemyInfo
{
    public int Id { get; set; }
    public int Intense { get; set; }
}