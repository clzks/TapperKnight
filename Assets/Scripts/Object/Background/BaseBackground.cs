using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MeshRenderer))]
public class BaseBackground : MonoBehaviour
{
    private float OwnSpeedFactor;
    private MeshRenderer meshRenderer;

    public void SetBackground(BackgroundModel model)
    {
        OwnSpeedFactor = model.ScrollSpeed;
        transform.position = new Vector3(transform.position.x, model.YPosition, transform.position.z);
        transform.localScale = new Vector3(model.XScale, model.YScale, 1);
    }

    public void SetSpeed(float spd)
    {
        OwnSpeedFactor = spd;
    }
    
    public void SetLayer(int sortOrder)
    {
        meshRenderer.sortingOrder = sortOrder;
    }

    public void SetTexture(Texture texture)
    {
        meshRenderer.material.SetTexture("_MainTex", texture);
    }

    public async UniTask Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = "Background";
        await UniTask.Yield();
    }

    public async UniTask Update()
    {
        Vector2 textureOffset = new Vector2(Time.time * OwnSpeedFactor * 0.1f, 0);
        meshRenderer.material.mainTextureOffset = textureOffset;
        await UniTask.Yield();
    }
}
