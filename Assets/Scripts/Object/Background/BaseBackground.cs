using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MeshRenderer))]
public class BaseBackground : MonoBehaviour
{
    private float OwnSpeedFactor;
    private MeshRenderer meshRenderer;

    public void SetSpeed(float spd)
    {
        OwnSpeedFactor = spd;
    }
    
    public void SetLayer(int sortOrder)
    {
        meshRenderer.sortingLayerName = "Background";
        meshRenderer.sortingOrder = sortOrder;
    }

    public async UniTask Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        await UniTask.Yield();
    }

    public async UniTask Update()
    {
        Vector2 textureOffset = new Vector2(Time.time * OwnSpeedFactor * 0.1f, 0);
        meshRenderer.material.mainTextureOffset = textureOffset;
        await UniTask.Yield();
    }
}
