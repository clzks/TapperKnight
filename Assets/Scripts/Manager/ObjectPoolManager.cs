using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<int, List<GameObject>> enemyPool;
    public Dictionary<int, List<GameObject>> notePool;

    public void InitEnemyPool()
    {

    }
}
