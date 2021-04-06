using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNote : MonoBehaviour
{
    private BaseEnemy parentEnemy;
    private float _boxSizeX;
    private float _boxPosX;
    public float Position { get { return transform.position.x; } }
    public NoteType noteType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public void SetNoteSprite(Sprite sp)
    {
        spriteRenderer.sprite = sp;
    }

    public void SetPosition(Vector2 v)
    {
        transform.position = v;
    }

    public void SetParent(BaseEnemy enemy)
    {
        parentEnemy = enemy;
    }

    public void SetBoxPosition(float xPos)
    {
        _boxPosX = xPos;
    }

    public void SetBoxSize(float halfSizeX)
    {
        _boxSizeX = halfSizeX;
    }

    public void SetParentEnemy(BaseEnemy enemy)
    {
        parentEnemy = enemy;
    }

    public void Update()
    {
        if(transform.position.x <= _boxPosX - _boxSizeX)
        {
            NoteCall(ScoreType.Miss);
        }
    }

    public void OnNoteCall(NoteType type)
    {
        if(Position >= _boxPosX + 1f)
        {
            return;
        }

        if (type != noteType)
        {
            return;
        }

        if (Mathf.Abs(Position - _boxPosX) <= 0.2f)
        {
            NoteCall(ScoreType.Perfect);
        }
        else if (Mathf.Abs(Position - _boxPosX) <= 0.4f)
        {
            NoteCall(ScoreType.Great);
        }
        else if (Mathf.Abs(Position - _boxPosX) <= 0.6f)
        {
            NoteCall(ScoreType.Good);
        }
        else if (Mathf.Abs(Position - _boxPosX) < 0.8f)
        {
            NoteCall(ScoreType.Bad);
        }
        else
        {
            NoteCall(ScoreType.Miss);
        }
    }

    private void NoteCall(ScoreType score)
    {
        parentEnemy.OnNoteCall(score);
        DestroyNote();
        Debug.Log("Á¡¼ö : " + score.ToString());
    }

    private void DestroyNote()
    {
        gameObject.SetActive(false);
    }
}
