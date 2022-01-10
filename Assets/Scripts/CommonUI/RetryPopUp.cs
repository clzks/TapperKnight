using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RetryPopUp : MonoBehaviour
{
    public Text descriptionText;

    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }

    //public void OnClickRetryBotton(UnityAction action)
    //{
    //    action.Invoke();
    //}
    //
    //public void OnClickStartGuestModeButton(UnityAction action)
    //{
    //    action.Invoke();
    //}
}
