using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : NetworkBehaviour
{
    private Color _currentColor;
    private Button _button;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _currentColor = _image.color;

        _button.onClick.AddListener(ChangeColorRpc);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeColorRpc()
    {
        if(!IsOwner) return;

        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out var client))
        {
            var playerObject = client.PlayerObject;

            if (playerObject != null)
            {
                playerObject.GetComponent<PlayerNetworkController>().SetBaseColor(_currentColor);
            }
        }
    }
}
