using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPopUp : MonoBehaviour, IPointerClickHandler
{
    public Text nameText;
    public Text conditionText;
    public Image characterImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }

    public void SetUI(string name, string condition, Sprite image)
    {
        nameText.text = name;
        conditionText.text = condition;
        characterImage.sprite = image;
    }
}
