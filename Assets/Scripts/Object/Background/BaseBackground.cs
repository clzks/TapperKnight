using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MeshRenderer))]
public class BaseBackground : MonoBehaviour
{
    private float _ownSpeedFactor;
    private float _playerSpeedFactor;
    private MeshRenderer _meshRenderer;
    private InGamePresenter _inGamePresenter;
    public void SetBackground(BackgroundModel model)
    {
        _ownSpeedFactor = model.ScrollSpeed;
        transform.position = new Vector3(transform.position.x, model.YPosition, transform.position.z);
        transform.localScale = new Vector3(model.XScale, model.YScale, 1);
    }
    public async UniTaskVoid SetPlayerSpeedFactor(float playerSpeedFactor)
    {
        _playerSpeedFactor = playerSpeedFactor;
    }

    public async UniTaskVoid SetSpeed(float spd)
    {
        _ownSpeedFactor = spd;
    }
    
    public async UniTaskVoid SetLayer(int sortOrder)
    {
        _meshRenderer.sortingOrder = sortOrder;
    }

    public void SetTexture(Texture texture)
    {
        _meshRenderer.material.SetTexture("_MainTex", texture);
    }

    public async UniTask Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.sortingLayerName = "Background";
        await UniTask.Yield();
        _inGamePresenter = GameManager.Get().GetInGamePresenter();
    }

    public async UniTask Update()
    {
        var playerSpeed = _inGamePresenter.GetPlayerSpeed();

        Vector2 textureOffset = new Vector2(Time.time * (_ownSpeedFactor * 0.1f + playerSpeed * _playerSpeedFactor ), 0);
        _meshRenderer.material.mainTextureOffset = textureOffset;
        await UniTask.Yield();
    }
}
