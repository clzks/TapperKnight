using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<string, Sprite> spriteList;
    public Dictionary<ObjectType, GameObject> prefabList;
    public Dictionary<ObjectType, List<GameObject>> _objectPoolList;
    public Dictionary<ObjectType, List<GameObject>> _activePoolList;

    private async UniTask Awake()
    {
        spriteList = new Dictionary<string, Sprite>();
        spriteList.Add("Left", Resources.Load<Sprite>("Sprites/Left"));
        spriteList.Add("Right", Resources.Load<Sprite>("Sprites/Right"));
        spriteList.Add("BothSide", Resources.Load<Sprite>("Sprites/BothSide"));

        prefabList = new Dictionary<ObjectType, GameObject>();

        prefabList.Add(ObjectType.Note, Resources.Load<GameObject>("Prefabs/Note"));
        prefabList.Add(ObjectType.Enemy, Resources.Load<GameObject>("Prefabs/BaseEnemy"));
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

    //public BaseEnemy MakeEnemy()
    //{
    //    BaseEnemy enemy;
    //
    //    if (enemyPool.Count != 0)
    //    {
    //        enemy = enemyPool[0].GetComponent<BaseEnemy>();
    //        enemy.gameObject.SetActive(true);
    //        enemyPool.Remove(enemy.gameObject);
    //    }
    //    else
    //    {
    //        enemy = Instantiate(prefabList["BaseEnemy"]).GetComponent<BaseEnemy>();
    //    }
    //    activeEnemyList.Add(enemy);
    //    return enemy;
    //}

    //public BaseNote MakeNote()
    //{
    //    BaseNote note;
    //
    //    if (notePool.Count != 0)
    //    {
    //        note = notePool[0].GetComponent<BaseNote>();
    //        note.gameObject.SetActive(true);
    //        notePool.Remove(note.gameObject);
    //    }
    //    else
    //    {
    //        note = Instantiate(prefabList["Note"]).GetComponent<BaseNote>();
    //    }
    //    activeNoteList.Add(note);
    //    return note;
    //}

    public void InitPool()
    {
        _objectPoolList = new Dictionary<ObjectType, List<GameObject>>();
        _activePoolList = new Dictionary<ObjectType, List<GameObject>>();
    }

   

    //public async UniTask DestroyNote(BaseNote note)
    //{
    //    if (true == activeNoteList.Contains(note))
    //    {
    //        //note.transform.SetParent(transform);
    //        note.gameObject.SetActive(false);
    //        activeNoteList.Remove(note);
    //        notePool.Add(note.gameObject);
    //        await UniTask.Yield();
    //    }
    //    //else
    //    //{
    //    //    Debug.LogError("노트풀 에러");
    //    //}
    //}

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
