using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class TitleTouchPanel : MonoBehaviour, IPointerClickHandler
{
    public UnityAction Action;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (null != Action)
        {
            Action.Invoke();
        }
    }

    public void SetAction(UnityAction action)
    {
        Action = action;
    }
}
