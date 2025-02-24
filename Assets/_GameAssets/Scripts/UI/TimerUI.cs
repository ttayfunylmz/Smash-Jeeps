using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public static TimerUI Instance { get; private set; }

    [SerializeField] private TMP_Text _timerText;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTimerUI(int timerCounter)
    {
        _timerText.text = timerCounter.ToString();
    }
}
