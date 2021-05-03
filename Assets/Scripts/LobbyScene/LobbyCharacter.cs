using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacter : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;
    [SerializeField] private Animator _animator;
    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    public List<CharacterAnimType> actionStateList;
    private float _speed;
    private float _leftEnd = -8.0f;
    private float _rightEnd = 8.0f;
    private CancellationTokenSource _disableCancellation = new CancellationTokenSource();
    
    private void Awake()
    {
        _meshRenderer.sortingLayerName = "Character";
        _meshRenderer.sortingOrder = 0;

        if (_disableCancellation != null)
        {
            _disableCancellation.Dispose();
        }
        _disableCancellation = new CancellationTokenSource();
    }

    private async UniTask Start()
    {;
        await TakeRandomAction(CharacterAnimType.Idle);
    }

    private async UniTask Update()
    {
        Position += new Vector3(_speed * Time.deltaTime, 0f, 0f);

        if (Position.x <= _leftEnd && _speed < 0f)
        {
            Position = new Vector3(_leftEnd, Position.y, Position.z);
        }
        else if (Position.x >= _rightEnd && _speed > 0f)
        {
            Position = new Vector3(_rightEnd, Position.y, Position.z);
        }

        await UniTask.Yield();
    }

    private void OnDisable()
    {
        _disableCancellation.Cancel();
    }

    private void SetAnimationClip(CharacterAnimType type)
    {
        _animator.CrossFade(type.ToString(), 0.3f);
    }

    private async UniTask TakeRandomAction(CharacterAnimType state)
    {
        SetAnimationClip(state);

        switch (state)
        {
            case CharacterAnimType.Idle:
                await Idle();
                break;

            case CharacterAnimType.Walk:
                await Walk();
                break;

            case CharacterAnimType.Run:
                await Run();
                break;

            case CharacterAnimType.Attack:
                await Attack();
                break;

            case CharacterAnimType.Attack2:
                await SecondAttack();
                break;

            case CharacterAnimType.Skill:
                await Skill();
                break;

            case CharacterAnimType.Dead:
                await Dead();
                break;
        }

        var nextState = actionStateList[UnityEngine.Random.Range(0, actionStateList.Count)];

        await TakeRandomAction(nextState);
    }

    private void TakeLeft()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void TakeRight()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    private async UniTask Idle()
    {
        SetAnimationClip(CharacterAnimType.Idle);
        await UniTask.Delay(1000);
        _speed = 0f;

        int r = Random.Range(0, 100) % 2;

        if(0 == r)
        {
            TakeLeft();
        }
        else
        {
            TakeRight();
        }

        await UniTask.Delay(1000);
    }

    private async UniTask Walk()
    {
        int r = Random.Range(0, 100) % 2;
        float time = Random.Range(2f, 4f);
        float timer = 0f;
        if (0 == r)
        {
            if(Position.x - _leftEnd >= 4f)
            {
                TakeLeft();
                _speed = -0.8f;
            }
            else
            {
                TakeRight();
                _speed = 0.8f;
            }
        }
        else
        {
            if(_rightEnd - Position.x >= 4f)
            {
                TakeRight();
                _speed = 0.8f;
            }    
            else
            {
                TakeLeft();
                _speed = -0.8f;
            }
        }

        while(timer <= time)
        {
            timer += Time.deltaTime;
            await UniTask.Yield(_disableCancellation.Token);
        }
        _speed = 0f;
        await Idle();
    }
    private async UniTask Run()
    {
        float time = Random.Range(2f, 4f);
        float timer = 0f;

        if (Position.x - _leftEnd >= _rightEnd - Position.x)
        {
            TakeLeft();
            _speed = -2f;
        }
        else
        {
            TakeRight();
            _speed = 2f;
        }

        while (timer < time)
        {
            timer += Time.deltaTime;

            if(Position.x <= _leftEnd)
            {
                timer = time;
            }
            else if(Position.x >= _rightEnd)
            {
                timer = time;
            }
            await UniTask.Yield(_disableCancellation.Token);
        }
        _speed = 0f;
        await Idle();
    }
    private async UniTask Attack()
    {
        int r = Random.Range(0, 100) % 2;

        if (0 == r)
        {
            TakeLeft();
        }
        else
        {
            TakeRight();
        }

        await UniTask.Delay(1500, false, 0f, _disableCancellation.Token);
    }
    private async UniTask SecondAttack()
    {
        int r = Random.Range(0, 100) % 2;

        if (0 == r)
        {
            TakeLeft();
        }
        else
        {
            TakeRight();
        }

        await UniTask.Delay(1500, false, 0f, _disableCancellation.Token);
    }
    private async UniTask Dead()
    {
        int r = Random.Range(0, 100) % 2;

        if (0 == r)
        {
            TakeLeft();
        }
        else
        {
            TakeRight();
        }
        await UniTask.Delay(2500, false, 0f, _disableCancellation.Token);
        await Idle();
    }
    private async UniTask Skill()
    {
        int r = Random.Range(0, 100) % 2;

        if (0 == r)
        {
            TakeLeft();
        }
        else
        {
            TakeRight();
        }

        await UniTask.Delay(2500, false, 0f, _disableCancellation.Token);
    }
}
