using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RetryPopUp : MonoBehaviour
{
    public Text descriptionText;
    public Button retryButton;
    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }

    public void SetButtonAction(UniTask action)
    {
        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(async () => await action);
    }
}
