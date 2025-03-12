using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;

public class StartingGameUI : NetworkBehaviour
{
    public static StartingGameUI Instance { get; private set; }

    public event Action OnAllPlayersConnected;

    [Header("References")]
    [SerializeField] private TMP_Text _countdownText;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    [SerializeField] private NetworkVariable<int> _playersLoaded = new NetworkVariable<int>
            (0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
    private WaitForSeconds _waitingSeconds = new WaitForSeconds(1f);

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            SetPlayersLoadedRpc();
        }

        if (IsServer)
        {
            _playersLoaded.OnValueChanged += OnPlayersLoadedChanged;
        }
    }

    private void OnPlayersLoadedChanged(int oldPlayerCount, int newPlayerCount)
    {
        if (IsServer && newPlayerCount == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            StartCountdownRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void SetPlayersLoadedRpc()
    {
        _playersLoaded.Value++;
        Debug.Log("Client Scene Loaded. Total Loaded: " + _playersLoaded.Value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartCountdownRpc()
    {
        OnAllPlayersConnected?.Invoke();
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        _countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; --i)
        {
            _countdownText.text = i.ToString();
            AnimateText();
            yield return _waitingSeconds;
        }

        GameManager.Instance.ChangeGameState(GameState.Playing);

        _countdownText.text = "GO!";
        AnimateText();
        yield return _waitingSeconds;

        _countdownText.transform.DOScale(0f, _animationDuration / 2).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            _countdownText.gameObject.SetActive(false);
        });
    }

    private void AnimateText()
    {
        _countdownText.transform.localScale = Vector3.zero;
        _countdownText.transform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-30f, 30f));

        _countdownText.transform.DOScale(1f, _animationDuration).SetEase(Ease.OutBack);
        _countdownText.transform.DORotate(Vector3.zero, _animationDuration).SetEase(Ease.OutBack);
    }
}
