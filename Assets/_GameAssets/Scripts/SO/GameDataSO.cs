using UnityEngine;

[CreateAssetMenu(fileName = "Game Data", menuName = "Scriptable Objects/Game Data")]
public class GameDataSO : ScriptableObject
{
    [SerializeField] private int _gameTimer;

    public int GameTimer => _gameTimer;
}
