using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _gameOverBackgroundImage;
    [SerializeField] private RectTransform _gameOverTransform;
    [SerializeField] private RectTransform _scoreTableTransform;
    [SerializeField] private TMP_Text _winnerText;
    [SerializeField] private GameObject _confettiParticles;
    
    [Header("Buttons")]
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _oneMoreButton;

    [Header("Score Table References")]
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private ScoreTablePlayer _scoreTablePlayerPrefab;
    [SerializeField] private LeaderboardUI _leaderboardUI;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private float _scaleDuration = 0.5f;

    private RectTransform _mainMenuButtonTransform;
    private RectTransform _oneMoreButtonTransform;
    private RectTransform _winnerTransform;

    private void Awake()
    {
        _mainMenuButtonTransform = _mainMenuButton.GetComponent<RectTransform>();
        _oneMoreButtonTransform = _oneMoreButton.GetComponent<RectTransform>();
        _winnerTransform = _winnerText.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _scoreTableTransform.gameObject.SetActive(false);
        _scoreTableTransform.localScale = Vector3.zero;

        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.GameOver)
        {
            AnimateGameOver();
        }
    }

    private void AnimateGameOver()
    {
        _gameOverBackgroundImage.DOFade(.8f, _animationDuration / 2);
        _gameOverTransform.DOAnchorPosY(0f, _animationDuration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _gameOverTransform.GetComponent<TMP_Text>().DOFade(0f, _animationDuration / 2).SetDelay(1f).OnComplete(() =>
            {
                AnimateLeaderboardAndButtons();
            });
        });
    }

    private void AnimateLeaderboardAndButtons()
    {
        _scoreTableTransform.gameObject.SetActive(true);
        _scoreTableTransform.DOScale(0.8f, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _mainMenuButtonTransform.DOScale(1f,_scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                _oneMoreButtonTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _winnerTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        _confettiParticles.SetActive(true);
                    });
                });
            });
        });

        PopulateGameOverLeaderboard();
    }

    private void PopulateGameOverLeaderboard()
    {
        foreach (Transform child in _contentTransform)
        {
            Destroy(child.gameObject);
        }

        var leaderboardData = _leaderboardUI.GetLeaderboardData()
            .OrderByDescending(x => x.Score)
            .ToList();

        HashSet<ulong> existingClientIds = new HashSet<ulong>();

        for (int i = 0; i < leaderboardData.Count; i++)
        {
            var entry = leaderboardData[i];

            if (existingClientIds.Contains(entry.ClientId))
            {
                continue;
            }

            ScoreTablePlayer scoreTableInstance = Instantiate(_scoreTablePlayerPrefab, _contentTransform);
            bool isOwner = entry.ClientId == NetworkManager.Singleton.LocalClientId;

            int rank = i + 1;
            scoreTableInstance.SetScoreTableData(rank.ToString(), entry.PlayerName, entry.Score.ToString(), isOwner);

            existingClientIds.Add(entry.ClientId);
        }

        SetWinnersName();
    }

    private void SetWinnersName()
    {
        string winnersName = _leaderboardUI.GetWinnersName();
        _winnerText.text = winnersName + " SMASHED Y'ALL!";
    }
}
