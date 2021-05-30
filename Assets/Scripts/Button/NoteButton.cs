using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class NoteButton : MonoBehaviour, IPointerDownHandler
{
    public UnityAction buttonAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonAction.Invoke();
    }

    public void SetBtnAction(UnityAction action)
    {
        buttonAction = action;
    }
}
