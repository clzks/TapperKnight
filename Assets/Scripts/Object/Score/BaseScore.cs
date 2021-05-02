using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BaseScore : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    private SpriteRenderer _renderer;
    [SerializeField] private float _life;
    [SerializeField] private float _speed;
    private CancellationTokenSource _disableCancellation = new CancellationTokenSource();
    private async UniTask OnEnable()
    {
        if (_disableCancellation != null)
        {
            _disableCancellation.Dispose();
        }
        _disableCancellation = new CancellationTokenSource();
        _objectPool = _objectPool ?? ObjectPoolManager.Get();
        _renderer = _renderer ?? GetComponentInChildren<SpriteRenderer>();
        await UniTask.Yield();
    }

    private void OnDisable()
    {
        _disableCancellation.Cancel();
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
            await UniTask.Yield(_disableCancellation.Token);
        }

        ReturnObject();
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Score;
    }

    public void Init()
    {
        transform.position = new Vector3(1000, 1000, 0);
        _renderer.color = new Color(1f, 1f, 1f, 1f);
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }
}
