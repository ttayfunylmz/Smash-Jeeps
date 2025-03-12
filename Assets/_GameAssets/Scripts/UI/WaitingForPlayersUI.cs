using System;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour
{
    private void Start()
    {
        Show();

        StartingGameUI.Instance.OnAllPlayersConnected += OnAllPlayersConnected;
    }

    private void OnAllPlayersConnected()
    {
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
