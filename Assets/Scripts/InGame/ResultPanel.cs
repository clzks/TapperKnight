using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    public Text recordText;
    public Text scoreText;
    public Button RetryButton;

    public void SetRecord(int record)
    {
        recordText.text = record.ToString();
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetActive(bool enabled)
    {
        gameObject.SetActive(enabled);
    }

    
}
