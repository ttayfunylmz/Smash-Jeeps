using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController _playerNetworkController;
    [SerializeField] private TMP_Text _playerNameText;

    private void Start()
    {
        HandlePlayerNameChanged(string.Empty, _playerNetworkController.PlayerName.Value);

        _playerNetworkController.PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        _playerNameText.text = newName.ToString();
    }

    private void OnDestroy()
    {
        _playerNetworkController.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}
