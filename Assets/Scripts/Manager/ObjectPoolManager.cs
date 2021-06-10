using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<ObjectType, GameObject> prefabList;
    public Dictionary<ObjectType, List<GameObject>> _objectPoolList;
    public Dictionary<ObjectType, List<GameObject>> _activePoolList;
    public Dictionary<string, GameObject> _characterPrefabList;
    private GameObject _characterIconPrefab;
    private Dictionary<string, Sprite> _characterIconImageList;
    private async UniTask Awake()
    {
        prefabList = new Dictionary<ObjectType, GameObject>();
        prefabList.Add(ObjectType.Note, Resources.Load<GameObject>("Prefabs/Note"));
        prefabList.Add(ObjectType.Enemy, Resources.Load<GameObject>("Prefabs/BaseEnemy"));
        prefabList.Add(ObjectType.Score, Resources.Load<GameObject>("Prefabs/Score"));
        _characterPrefabList = new Dictionary<string, GameObject>();
        LoadCharacterPrefabs().Forget();
        _characterIconPrefab = Resources.Load<GameObject>("Prefabs/Characters/Icon/CharacterIcon");
        _characterIconImageList = new Dictionary<string, Sprite>();
        LoadCharacterIconImageList().Forget();
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

    public void ReturnObject(IPoolObject poolObject)
    {
        ObjectType type = poolObject.GetObjectType();
        var list = _activePoolList[type];
        var pool = _objectPoolList[type];
        GameObject obj = poolObject.GetObject();

        if (true == list.Contains(obj))
        {
            obj.transform.rotation = new Quaternion();
            obj.SetActive(false);
            list.Remove(obj);
            pool.Add(obj);
        }
    }

    public void InitPool()
    {
        _objectPoolList = new Dictionary<ObjectType, List<GameObject>>();
        _activePoolList = new Dictionary<ObjectType, List<GameObject>>();
    }
    
    public void ResetPool()
    {
        foreach (var item in _objectPoolList)
        {
            foreach (var obj in item.Value)
            {
                Destroy(obj);
            }

            item.Value.Clear();
        }

        foreach (var item in _activePoolList)
        {
            foreach (var obj in item.Value)
            {
                Destroy(obj);
            }

            item.Value.Clear();
        }
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

    public BaseEnemy GetNextEnemy()
    {
        var list = _activePoolList[ObjectType.Enemy];

        if (list.Count >= 2)
        {
            return list.OrderBy(x => x.transform.position.x).ToList()[1].GetComponent<BaseEnemy>();
        }
        else
        {
            return null;
        }
    }

    public BaseEnemy GetLastEnemy()
    {
        if(false == _activePoolList.ContainsKey(ObjectType.Enemy))
        {
            return null;
        }

        var list = _activePoolList[ObjectType.Enemy];

        if (list.Count != 0)
        {
            return list.OrderBy(x => x.transform.position.x).Last().GetComponent<BaseEnemy>();
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

    private async UniTask LoadCharacterPrefabs()
    {
        var prefabArray = Resources.LoadAll<GameObject>("Prefabs/Characters");

        foreach (var item in prefabArray)
        {
            _characterPrefabList.Add(item.name, item);
        }

        await UniTask.Yield();
    }

    private async UniTask LoadCharacterIconImageList()
    {
        var prefabArray = Resources.LoadAll<Sprite>("Sprites/CharacterIcon");

        foreach (var item in prefabArray)
        {
            _characterIconImageList.Add(item.name, item);
        }
        
        await UniTask.Yield();
    }

    public GameObject GetCharcterIcon()
    {
        return _characterIconPrefab;
    }

    public Dictionary<string, Sprite> GetCharacterIconImageList()
    {
        return _characterIconImageList;
    }

    public GameObject GetCharacterPrefab(string prefabName)
    {
        return _characterPrefabList[prefabName];
    }
}
