using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InGameView : MonoBehaviour, IView
{
    [SerializeField] private InGamePresenter _inGamePresenter;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private float noteBoxPosY;
    private void Awake()
    {
        if (null == leftButton)
        {
            leftButton = GameObject.Find("LeftButton").GetComponent<Button>();
            leftButton.onClick.AddListener(OnClickLeftButton);
        }

        if (null == rightButton)
        {
            rightButton = GameObject.Find("RightButton").GetComponent<Button>();
            rightButton.onClick.AddListener(OnClickRightButton);
        }
    }

    public void SetPresenter(InGamePresenter presenter)
    {
        _inGamePresenter = presenter;
    }

    public void SetNoteTouch()
    {
        
    }

    public void OnClickLeftButton()
    {
        
    }

    public void OnClickRightButton()
    {
        
    }

    public float GetNoteBoxPosY()
    {
        return noteBoxPosY;
    }
}
