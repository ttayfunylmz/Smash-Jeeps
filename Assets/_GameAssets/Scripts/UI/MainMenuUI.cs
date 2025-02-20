using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        if(_nameInputField.text == string.Empty)
        {
            Debug.Log("CAN NOT BE EMPTY");
            return;
        }

        string playerName = _nameInputField.text;
        PlayerPrefs.SetString(Consts.PlayerData.PLAYER_NAME, playerName);

        SceneManager.LoadScene(Consts.SceneNames.GAME_SCENE);
    }
}
