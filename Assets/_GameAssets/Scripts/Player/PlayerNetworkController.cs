using TMPro;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _playerCinemachineCamera;
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private TMP_Text _playerNameText;

    private NetworkVariable<FixedString32Bytes> _playerName 
        = new NetworkVariable<FixedString32Bytes>("SkinnyDev", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        _playerCinemachineCamera.gameObject.SetActive(IsOwner);

        // SetPlayerText();
    }

    // private void SetPlayerText()
    // {
    //     _playerName.Value = PlayerPrefs.GetString(Consts.PlayerData.PLAYER_NAME);

    //     _playerCanvas.worldCamera = Camera.main;
    //     _playerNameText.text = _playerName.Value.ToString();
    // }
}
