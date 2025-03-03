using System;
using DG.Tweening;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] _canvasGroups;
    [SerializeField] private float _fadeDuration;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.GameOver:
                CloseOtherUI();
                break;
        }
    }

    private void CloseOtherUI()
    {
        foreach (CanvasGroup canvasGroup in _canvasGroups)
        {
            canvasGroup.DOFade(0f, _fadeDuration);
        }
    }
}
