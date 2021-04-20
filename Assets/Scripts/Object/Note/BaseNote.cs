using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNote : MonoBehaviour
{
    private ObjectPoolManager _objectPool;
    private BaseEnemy parentEnemy;
    private float _boxSizeX;
    private float _boxPosX;
    private float _speed;
    private List<Vector3> _notePopDestination;
    public float Position { get { return transform.position.x; } }
    public NoteType noteType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private async UniTask Awake()
    {
        _objectPool = ObjectPoolManager.Get();
        await UniTask.Yield();
    }

    public async UniTask Update()
    {
        transform.position -= new Vector3(_speed * Time.deltaTime, 0f, 0f);

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

    public async UniTaskVoid SetNoteSpeed(float speed)
    {
        _speed = speed;
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

        if(Position >= _boxPosX + 1f)
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

        if (Mathf.Abs(Position - _boxPosX) <= 0.2f)
        {
            score = ScoreType.Perfect;
        }
        else if (Mathf.Abs(Position - _boxPosX) <= 0.4f)
        {
            score = ScoreType.Great;
        }
        else if (Mathf.Abs(Position - _boxPosX) <= 0.6f)
        {
            score = ScoreType.Good;
        }
        else if (Mathf.Abs(Position - _boxPosX) < 0.8f)
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
        await NotePop(score);
        Debug.Log("점수 : " + score.ToString());
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
        await DestroyNote();
    }

    private async UniTask DestroyNote()
    {
        await _objectPool.DestroyNote(this);
        await UniTask.Yield();
    }
}
