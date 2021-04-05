using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<int, List<GameObject>> enemyPool;
    public Dictionary<int, List<GameObject>> notePool;

    public Dictionary<string, Sprite> spriteList;
    public Dictionary<string, GameObject> prefabList;

    private void Awake()
    {
        spriteList = new Dictionary<string, Sprite>();
        spriteList.Add("Left", Resources.Load<Sprite>("Sprites/Left"));
        spriteList.Add("Right", Resources.Load<Sprite>("Sprites/Right"));
        spriteList.Add("BothSide", Resources.Load<Sprite>("Sprites/BothSide"));

        prefabList = new Dictionary<string, GameObject>();

        prefabList.Add("Note", Resources.Load<GameObject>("Prefabs/Note"));
        prefabList.Add("BaseEnemy", Resources.Load<GameObject>("Prefabs/BaseEnemy"));
    }

    public BaseEnemy MakeEnemy()
    {
        BaseEnemy enemy = Instantiate(prefabList["BaseEnemy"]).GetComponent<BaseEnemy>();

        return enemy;
    }

    public void InitEnemyPool()
    {

    }
}
