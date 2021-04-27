using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<string, Sprite> spriteList;
    public Dictionary<ScoreType, Sprite> scoreList;
    public Dictionary<ObjectType, GameObject> prefabList;
    public Dictionary<ObjectType, List<GameObject>> _objectPoolList;
    public Dictionary<ObjectType, List<GameObject>> _activePoolList;

    private async UniTask Awake()
    {
        spriteList = new Dictionary<string, Sprite>();
        spriteList.Add("Left", Resources.Load<Sprite>("Sprites/Left"));
        spriteList.Add("Right", Resources.Load<Sprite>("Sprites/Right"));
        spriteList.Add("BothSide", Resources.Load<Sprite>("Sprites/BothSide"));

        scoreList = new Dictionary<ScoreType, Sprite>();
        scoreList.Add(ScoreType.Perfect, Resources.Load<Sprite>("Sprites/Score/Perfect"));
        scoreList.Add(ScoreType.Great, Resources.Load<Sprite>("Sprites/Score/Great"));
        scoreList.Add(ScoreType.Good, Resources.Load<Sprite>("Sprites/Score/Good"));
        scoreList.Add(ScoreType.Bad, Resources.Load<Sprite>("Sprites/Score/Bad"));
        scoreList.Add(ScoreType.Miss, Resources.Load<Sprite>("Sprites/Score/Miss"));

        prefabList = new Dictionary<ObjectType, GameObject>();

        prefabList.Add(ObjectType.Note, Resources.Load<GameObject>("Prefabs/Note"));
        prefabList.Add(ObjectType.Enemy, Resources.Load<GameObject>("Prefabs/BaseEnemy"));
        prefabList.Add(ObjectType.Score, Resources.Load<GameObject>("Prefabs/Score"));
        await UniTask.Yield();
    }

    public IPoolObject MakeObject(ObjectType type)
    {
        IPoolObject obj;
       
        List<GameObject> pool;
        List<GameObject> activePool;

        if(_objectPoolList.ContainsKey(type) == true)
        {
            pool = _objectPoolList[type];
        }
        else
        {
            pool = new List<GameObject>();
            _objectPoolList.Add(type, pool);
        }

        if(pool.Count != 0)
        {
            obj = pool[0].GetComponent<IPoolObject>();
            obj.GetObject().SetActive(true);
            pool.Remove(obj.GetObject());
        }
        else
        {
            obj = Instantiate(prefabList[type]).GetComponent<IPoolObject>();
        }

        if(_activePoolList.ContainsKey(type) == true)
        {
            activePool = _activePoolList[type];
        }
        else
        {
            activePool = new List<GameObject>();
            _activePoolList.Add(type, activePool);
        }

        obj.Init();
        activePool.Add(obj.GetObject());

        return obj;
    }

    public async UniTask ReturnObject(IPoolObject poolObject)
    {
        ObjectType type = poolObject.GetObjectType();
        var list = _activePoolList[type];
        var pool = _objectPoolList[type];
        GameObject obj = poolObject.GetObject();

        if (true == list.Contains(obj))
        {
            obj.SetActive(false);
            list.Remove(obj);
            pool.Add(obj);
            await UniTask.Yield();
        }
    }

    public void InitPool()
    {
        _objectPoolList = new Dictionary<ObjectType, List<GameObject>>();
        _activePoolList = new Dictionary<ObjectType, List<GameObject>>();
    }

    public Sprite GetScoreSprite(ScoreType type)
    {
        return scoreList[type];
    }
    
    public BaseEnemy GetEnemy()
    {
        var list = _activePoolList[ObjectType.Enemy];

        if (list.Count != 0)
        {
            return list.OrderBy(x => x.transform.position.x).First().GetComponent<BaseEnemy>();
        }
        else
        {
            return null;
        }
    }

    public int GetEnemyCount()
    {
        return _activePoolList[ObjectType.Enemy].Count;
    }
}
