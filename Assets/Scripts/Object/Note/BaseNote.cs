using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNote : MonoBehaviour
{
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
}
