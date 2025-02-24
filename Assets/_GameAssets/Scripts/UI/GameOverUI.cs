using System;
using DG.Tweening;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.5f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.GameOver:
                SetGameOverUI();
                break;
        }
    }

    public void SetGameOverUI()
    {
        _canvasGroup.DOFade(1f, _fadeDuration);
    }
}
