using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNote : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    private BaseEnemy parentEnemy;
    private float _boxSizeX;
    private float _boxPosX;
    private float _speed;
    private float _playerSpeedFactor;
    private List<Vector3> _notePopDestination;
    private SpriteRenderer _renderer;
    public float Position { get { return transform.position.x; } }
    public NoteType noteType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private InGamePresenter _inGamePresenter;
    private async UniTask OnEnable()
    {
        _objectPool = _objectPool ?? ObjectPoolManager.Get();
        _renderer = _renderer ?? GetComponentInChildren<SpriteRenderer>();
        await UniTask.Yield();
        if (null == _inGamePresenter)
        {
            _inGamePresenter = GameManager.Get().GetInGamePresenter();
        }
    }

    public async UniTask Update()
    {
        var playerSpeed = _inGamePresenter.GetPlayerSpeed();

        transform.position -= new Vector3((_speed + playerSpeed *_playerSpeedFactor) * Time.deltaTime, 0f, 0f);

        if (Position <= _boxPosX - _boxSizeX)
        {
            await NoteCall(ScoreType.Miss);
        }
    }

    public async UniTaskVoid SetNoteSprite(Sprite sp)
    {
        spriteRenderer.sprite = sp;
        await UniTask.Yield();
    }

    public async UniTaskVoid SetPosition(Vector2 v)
    {
        transform.position = v;
        await UniTask.Yield();
    }

    public async UniTaskVoid SetParentEnemy(BaseEnemy enemy)
    {
        parentEnemy = enemy;
        await UniTask.Yield();
    }

    public async UniTaskVoid SetBoxPosition(float xPos)
    {
        _boxPosX = xPos;
        await UniTask.Yield();
    }

    public async UniTaskVoid SetBoxSize(float halfSizeX)
    {
        _boxSizeX = halfSizeX;
        await UniTask.Yield();
    }

    public async UniTaskVoid SetSortingLayer(int sortingOrder)
    {
        _renderer.sortingOrder = sortingOrder;
    }

    public async UniTaskVoid SetNoteSpeed(float speed, float playerSpeedFactor)
    {
        _speed = speed;
        _playerSpeedFactor = playerSpeedFactor;
    }
    public async UniTaskVoid SetNotePopDestination(List<Vector3> dest)
    {
        _notePopDestination = dest;
        await UniTask.Yield();
    }

    public async UniTaskVoid SetInGamePool(Transform tr)
    {
        transform.SetParent(tr);
        await UniTask.Yield();
    }

    public async UniTask OnNoteCall(NoteType type)
    {
        ScoreType score = ScoreType.Miss;

        if(Position >= _boxPosX + 2f)
        {
            Debug.Log("거리가 너무멀어 무효처리");
            return;
        }

        if (type != noteType)
        {
            Debug.Log("노트와 버튼정보 불일치");
            await NoteCall(score);
            return;
        }

        if (Mathf.Abs(Position - _boxPosX) <= 0.4f)
        {
            score = ScoreType.Perfect;
        }
        else if (Mathf.Abs(Position - _boxPosX) <= 0.8f)
        {
            score = ScoreType.Great;
        }
        else if (Mathf.Abs(Position - _boxPosX) <= 1.2f)
        {
            score = ScoreType.Good;
        }
        else if (Mathf.Abs(Position - _boxPosX) < 1.6f)
        {
            score = ScoreType.Bad;
        }
        else
        {
            score = ScoreType.Miss;
        }
        
        await NoteCall(score);
    }

    private async UniTask NoteCall(ScoreType score)
    {
        await parentEnemy.OnNoteCall(score);
        NotePop(score).Forget();
        ScorePop(score).Forget();
        Debug.Log("점수 : " + score.ToString());
    }

    private async UniTask ScorePop(ScoreType score)
    {
        var scoreObj = (BaseScore)_objectPool.MakeObject(ObjectType.Score);
        scoreObj.SetScore(score, transform.position).Forget();
        await scoreObj.ScorePop();
    }

    private async UniTask NotePop(ScoreType score)
    {
        float time = 0f;

        if (score != ScoreType.Miss)
        {
            Vector3 startPos = transform.position;
            while (time <= 1f)
            {
                transform.position = Formula.BezierMove(startPos, _notePopDestination[0], _notePopDestination[1], time);
                await UniTask.Yield();
                time += Time.deltaTime * 2f;
            }
        }
        else
        {
            while (time <= 2f)
            {
                await UniTask.Yield();
                time += Time.deltaTime;
            }
        }
        await ReturnObject();
    }

    public async UniTaskVoid Init()
    {
      
    }

    public async UniTask ReturnObject()
    {
        await _objectPool.ReturnObject(this);
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.Note;
    }
}
