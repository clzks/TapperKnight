using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BaseNote : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    private BaseEnemy parentEnemy;
    //private float _boxSizeX;
    private float _boxPosX;
    private float _speed;
    private float _playerSpeedFactor;
    private List<Vector3> _notePopDestination;
    private SpriteRenderer _renderer;
    private int _sortingOrder;
    private bool _isDiscriminated;
    //private bool _isOnNoteCall = false;
    //private Transform _inGamePool;
    public float Position { get { return transform.position.x; } }
    private NoteType _noteType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private InGamePresenter _inGamePresenter;
    //private CircleCollider2D _collider;
    private CancellationTokenSource _disableCancellation = new CancellationTokenSource();

    private async UniTask OnEnable()
    {
        if (_disableCancellation != null)
        {
            _disableCancellation.Dispose();
        }
        _isDiscriminated = false;
        _disableCancellation = new CancellationTokenSource();
        _objectPool = _objectPool ?? ObjectPoolManager.Get();
        _renderer = _renderer ?? GetComponentInChildren<SpriteRenderer>();
        //_collider = _collider ?? GetComponentInChildren<CircleCollider2D>();
        await UniTask.Yield();
        if (null == _inGamePresenter)
        {
            _inGamePresenter = GameManager.Get().GetInGamePresenter();
        }
    }

    private async UniTask Update()
    {
        var playerSpeed = _inGamePresenter.GetPlayerSpeed();

        transform.position -= new Vector3((_speed + playerSpeed * _playerSpeedFactor) * Time.deltaTime, 0f, 0f);


        if (Position - _boxPosX < -0.4f)
        {
            await NoteCall(ScoreType.Miss);
        }
    }

    private void OnDisable()
    {
        _disableCancellation.Cancel();
    }

    public void SetNoteSprite(Sprite sp)
    {
        spriteRenderer.sprite = sp;
    }

    public void SetPosition(Vector2 v)
    {
        transform.position = v;
    }

    public void SetParentEnemy(BaseEnemy enemy)
    {
        parentEnemy = enemy;
    }

    public void SetBoxPosition(float xPos)
    {
        _boxPosX = xPos;
    }

    //public async UniTaskVoid SetBoxSize(float halfSizeX)
    //{
    //    _boxSizeX = halfSizeX;
    //    await UniTask.Yield();
    //}

    public void SetSortingLayer(int sortingOrder)
    {
        _renderer.sortingOrder = sortingOrder;
        _sortingOrder = sortingOrder;
    }

    public void SetNoteSpeed(float speed, float playerSpeedFactor)
    {
        _speed = speed;
        _playerSpeedFactor = playerSpeedFactor;
    }

    public void SetNoteSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetNotePopDestination(List<Vector3> dest)
    {
        _notePopDestination = dest;
    }

    public async UniTaskVoid SetInGamePool(Transform tr)
    {
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

        if (type != _noteType)
        {
            Debug.Log("노트와 버튼정보 불일치");
            await NoteCall(score);
            return;
        }

        if (Mathf.Abs(Position - _boxPosX) <= 0.4f)
        {
            score = ScoreType.Perfect;
        }
        else if (Position - _boxPosX <= 0.8f)
        {
            score = ScoreType.Great;
        }
        else if (Position - _boxPosX <= 1.2f)
        {
            score = ScoreType.Good;
        }
        else if (Position - _boxPosX < 1.6f)
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
        gameObject.layer = 0;
        _isDiscriminated = true;
        parentEnemy.OnNoteCall(score).Forget();
        NotePop(score).Forget();
        ScorePop(score).Forget();
        //Debug.Log("점수 : " + score.ToString());
        await UniTask.Yield();
    }

    private async UniTask ScorePop(ScoreType score)
    {
        var scoreObj = (BaseScore)_objectPool.MakeObject(ObjectType.Score);
        var scoreSprite = _inGamePresenter.GetScoreSprite(score);
        scoreObj.SetScore(scoreSprite, transform.position, _sortingOrder).Forget();

        await scoreObj.ScorePop();
    }

    private async UniTask NotePop(ScoreType score)
    {
        float time = 0f;
        //transform.SetParent(_inGamePool);

        if (score != ScoreType.Miss)
        {
            Vector3 startPos = transform.position;
            while (time <= 1f)
            {
                transform.position = Formula.BezierMove(startPos, _notePopDestination[0], _notePopDestination[1], time);
                await UniTask.Yield(_disableCancellation.Token);
                time += Time.deltaTime * 2f;
            }
        }
        else
        {
            while (time <= 2f)
            {
                await UniTask.Yield(_disableCancellation.Token);
                time += Time.deltaTime;
            }
        }
        ReturnObject();
    }

    public void Init()
    {
        transform.position = new Vector3(1000, 1000, 0);
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public float GetEstimatedArrivalTime()
    {
        var playerSpeed = _inGamePresenter.GetPlayerSpeed();

        return (Position - _boxPosX) / (_speed + playerSpeed * _playerSpeedFactor);
    }
    public ObjectType GetObjectType()
    {
        return ObjectType.Note;
    }
    
    public bool IsScoreDiscrimitated()
    {
        return _isDiscriminated;
    }

    public float GetPosition()
    {
        return Position;
    }

    public NoteType GetNoteType()
    {
        return _noteType;
    }

    public void SetNoteType(NoteType type)
    {
        _noteType = type;
    }

    public void SetLayer(int layerIndex)
    {
        gameObject.layer = layerIndex;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // LastNote or FirstLastNote
        if(gameObject.layer == 7 || gameObject.layer == 8)
        {
            var note = collision.gameObject.GetComponent<BaseNote>();
            
            if(note.parentEnemy == parentEnemy)
            {
                return;
            }
            else
            {
                Debug.Log("선행 Enemy의 노트와 충돌했으므로 속력을 일치하게 합니다");
                _speed = note.GetSpeed();
                parentEnemy.SetSpeed(_speed);
            }
        }
    }
}
