using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CharacterIcon : MonoBehaviour, IPointerClickHandler
{
    private CharacterModel _character;
    [SerializeField] private Image _characterImage;
    [SerializeField] private GameObject _lockImage;
    [SerializeField] private GameObject _lockText;
    [SerializeField] private GameObject _unSelectImage;
    public UnityAction clickAction;
    


    public void SetCharacter(CharacterModel character)
    {
        _character = character;
    }

    public void ActivateIcon(bool isAvailable)
    {
        if(true == isAvailable)
        {
            _lockImage.SetActive(false);
            _lockText.SetActive(false);     
        }
        else
        {
            _lockImage.SetActive(true);
            _lockText.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (false == _lockImage.activeSelf)
        {
            clickAction.Invoke();
            _unSelectImage.SetActive(false);
        }
    }

    public void SetClickAction(UnityAction action)
    {
        clickAction = action;
    }

    public void SetCharacterSprite(Sprite sprite)
    {
        _characterImage.sprite = sprite;
    }

    public void ExecuteDeselect()
    {
        _unSelectImage.SetActive(true);
    }
}
