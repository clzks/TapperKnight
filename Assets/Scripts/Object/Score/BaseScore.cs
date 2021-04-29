using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScore : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    private SpriteRenderer _renderer;
    [SerializeField] private float _life;
    [SerializeField] private float _speed;
    private async UniTask OnEnable()
    {
        _objectPool = _objectPool ?? ObjectPoolManager.Get();
        _renderer = _renderer ?? GetComponentInChildren<SpriteRenderer>();
        await UniTask.Yield();
    }

    public async UniTaskVoid SetScore(Sprite sprite, Vector3 pos, int sortingOrder)
    {
        _renderer.sprite = sprite;
        _renderer.sortingOrder = sortingOrder;
        transform.position = pos;
        await UniTask.Yield();
    }

    public async UniTask ScorePop()
    {
        float timer = 0f;

        while(timer <= _life)
        {
            transform.position += new Vector3(0f, _speed * Time.deltaTime, 0f);
            timer += Time.deltaTime;
            _renderer.color = new Color(1, 1, 1, 1 - (timer / _life));
            await UniTask.Yield();
        }

        await ReturnObject();
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Score;
    }

    public async UniTaskVoid Init()
    {
        _renderer.color = new Color(1f, 1f, 1f, 1f);
        await UniTask.Yield();
    }

    public async UniTask ReturnObject()
    {
        await _objectPool.ReturnObject(this);
    }
}
