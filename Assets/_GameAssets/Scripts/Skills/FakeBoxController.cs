using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

public class FakeBoxController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _fakeBoxCollider;
    [SerializeField] private Canvas _fakeBoxCanvas;
    [SerializeField] private RectTransform _arrowTransform;

    [Header("Settings")]
    [SerializeField] private float _animationDuration;

    private Tween _arrowTween;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetOwnerVisualsRpc();
        }
    }

    [Rpc(SendTo.Owner)]
    public void SetOwnerVisualsRpc()
    {
        _fakeBoxCanvas.gameObject.SetActive(true);
        _fakeBoxCanvas.worldCamera = Camera.main;
        _arrowTween = _arrowTransform.DOAnchorPosY(-1, _animationDuration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        _fakeBoxCollider.enabled = false;
    }

    public override void OnNetworkDespawn()
    {
        _arrowTween.Kill();
    }
}
