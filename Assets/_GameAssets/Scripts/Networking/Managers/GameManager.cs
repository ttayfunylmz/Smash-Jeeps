using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public event Action<GameState> OnGameStateChanged;

    public static GameManager Instance { get; private set; }

    [SerializeField] private GameDataSO _gameData;

    private NetworkVariable<int> _gameTimer = new NetworkVariable<int>(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );

    [SerializeField] private GameState _currentGameState;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _gameTimer.Value = _gameData.GameTimer;
            OnTimerChanged(0, _gameTimer.Value);
            InvokeRepeating(nameof(DecreaseTimer), 1f, 1f);
        }

        _gameTimer.OnValueChanged += OnTimerChanged;
    }

    private void OnTimerChanged(int previousValue, int newValue)
    {
        TimerUI.Instance.SetTimerUI(newValue);

        if (IsServer && newValue <= 0)
        {
            ChangeGameState(GameState.GameOver);
        }
    }

    private void DecreaseTimer()
    {
        if (IsServer)
        {
            _gameTimer.Value--;

            if (_gameTimer.Value <= 0)
            {
                CancelInvoke(nameof(DecreaseTimer));
            }
        }
    }

    public void ChangeGameState(GameState newGameState)
    {
        if (IsServer)
        {
            _currentGameState = newGameState;
            OnGameStateChanged?.Invoke(newGameState);
            ChangeGameStateRpc(newGameState);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeGameStateRpc(GameState newGameState)
    {
        _currentGameState = newGameState;
        OnGameStateChanged?.Invoke(newGameState);
        Debug.Log($"Game State: {newGameState}");
    }

    public GameState GetGameState()
    {
        return _currentGameState;
    }
}
