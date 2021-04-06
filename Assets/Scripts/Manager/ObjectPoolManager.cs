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
    public List<BaseEnemy> activeEnemyList;
    private void Awake()
    {
        activeEnemyList = new List<BaseEnemy>();
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
        activeEnemyList.Add(enemy);
        return enemy;
    }

    public void InitPool()
    {
        enemyPool = new Dictionary<int, List<GameObject>>();
        notePool = new Dictionary<int, List<GameObject>>();
    }

    public void DestroyEnemy(BaseEnemy enemy)
    {
        if(activeEnemyList.Contains(enemy))
        {
            enemy.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("이네미 풀 에러");
        }
    }

    public BaseEnemy GetEnemy()
    {
        return activeEnemyList.OrderBy(x => x.transform.position.x).First();
    }
}
