using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [SerializeField]public List<GameObject> enemyPool;
    public List<GameObject> notePool;

    public Dictionary<string, Sprite> spriteList;
    public Dictionary<string, GameObject> prefabList;
    public List<BaseEnemy> activeEnemyList;
    public List<BaseNote> activeNoteList;
    private async UniTask Awake()
    {
        activeEnemyList = new List<BaseEnemy>();
        activeNoteList = new List<BaseNote>();
        spriteList = new Dictionary<string, Sprite>();
        spriteList.Add("Left", Resources.Load<Sprite>("Sprites/Left"));
        spriteList.Add("Right", Resources.Load<Sprite>("Sprites/Right"));
        spriteList.Add("BothSide", Resources.Load<Sprite>("Sprites/BothSide"));

        prefabList = new Dictionary<string, GameObject>();

        prefabList.Add("Note", Resources.Load<GameObject>("Prefabs/Note"));
        prefabList.Add("BaseEnemy", Resources.Load<GameObject>("Prefabs/BaseEnemy"));
        await UniTask.Yield();
    }

    public BaseEnemy MakeEnemy()
    {
        BaseEnemy enemy;

        if (enemyPool.Count != 0)
        {
            enemy = enemyPool[0].GetComponent<BaseEnemy>();
            enemy.gameObject.SetActive(true);
            enemyPool.Remove(enemy.gameObject);
        }
        else
        {
            enemy = Instantiate(prefabList["BaseEnemy"]).GetComponent<BaseEnemy>();
        }
        activeEnemyList.Add(enemy);
        return enemy;
    }

    public BaseNote MakeNote()
    {
        BaseNote note;

        if (notePool.Count != 0)
        {
            note = notePool[0].GetComponent<BaseNote>();
            note.gameObject.SetActive(true);
            notePool.Remove(note.gameObject);
        }
        else
        {
            note = Instantiate(prefabList["Note"]).GetComponent<BaseNote>();
        }
        activeNoteList.Add(note);
        return note;
    }

    public void InitPool()
    {
        enemyPool = new List<GameObject>();
        notePool = new List<GameObject>();
    }

    public async UniTask DestroyEnemy(BaseEnemy enemy)
    {
        if(true == activeEnemyList.Contains(enemy))
        {
            //enemy.transform.SetParent(transform);
            enemy.gameObject.SetActive(false);
            activeEnemyList.Remove(enemy);
            enemyPool.Add(enemy.gameObject);
            await UniTask.Yield();
        }
        //else
        //{
        //    Debug.LogError("이네미 풀 에러");
        //}
    }

    public async UniTask DestroyNote(BaseNote note)
    {
        if (true == activeNoteList.Contains(note))
        {
            //note.transform.SetParent(transform);
            note.gameObject.SetActive(false);
            activeNoteList.Remove(note);
            notePool.Add(note.gameObject);
            await UniTask.Yield();
        }
        //else
        //{
        //    Debug.LogError("노트풀 에러");
        //}
    }

    public BaseEnemy GetEnemy()
    {
        if (activeEnemyList.Count != 0)
        {
            return activeEnemyList.OrderBy(x => x.transform.position.x).First();
        }
        else
        {
            return null;
        }
    }

    public int GetEnemyCount()
    {
        return activeEnemyList.Count;
    }
}
