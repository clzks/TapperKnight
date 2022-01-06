using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainCharacterUIEffect : MonoBehaviour
{
    public RectTransform rt;

    private void Update()
    {
        rt.Rotate(new Vector3(0, 0, Time.realtimeSinceStartup * 0.03f));
    }
}
