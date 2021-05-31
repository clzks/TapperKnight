using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float _standardRatio = 1.778f;
    [SerializeField] private float _wideMoveFactor = 4.15f;
    [SerializeField] private float _narrowMoveFactor = 2.2f;
#if UNITY_ANDROID
    private void Awake()
    {
        float CurrRatio = (float)Screen.width / (float)Screen.height;
        
        if(Mathf.Abs(CurrRatio - _standardRatio) < 0.1f)
        {
            return;
        }
        else if(CurrRatio > _standardRatio)
        {
            transform.position = new Vector3((CurrRatio - _standardRatio) * _wideMoveFactor, 0f, -10f);
        }
        else if(CurrRatio < _standardRatio)
        {
            transform.position = new Vector3((CurrRatio - _standardRatio) * _narrowMoveFactor, 0f, -10f);
        }
    }
#endif

#if UNITY_EDITOR
    private void Update()
    {
        float CurrRatio = (float)Screen.width / (float)Screen.height;

        if (Mathf.Abs(CurrRatio - _standardRatio) < 0.1f)
        {
            transform.position = new Vector3(0, 0, -10f);
        }
        else if (CurrRatio > _standardRatio)
        {
            transform.position = new Vector3((CurrRatio - _standardRatio) * _wideMoveFactor, 0f, -10f);
        }
        else if (CurrRatio < _standardRatio)
        {
            transform.position = new Vector3((CurrRatio - _standardRatio) * _narrowMoveFactor, 0f, -10f);
        }
    }
#endif
}
