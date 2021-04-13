using System.Collections.Generic;
using UnityEngine;

public class StageModel : IModel
{
    public int Id;
    public int StageNumber;
    public float MinimumGenCycle;
    public float MaximumGenCycle;
    public float StageTime;
    public List<int> EnemyList;
}

public struct SpawnEnemyInfo
{
    public int Id;
    public int Intense;
}