using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class KillScreenUI : MonoBehaviour
{
    public event Action OnRespawnTimerFinished;

    public static KillScreenUI Instance { get; private set; }

    [Header("Smash UI")]
    [SerializeField] private RectTransform _smashUITransform;
    [SerializeField] private TMP_Text _smashedPlayerText;

    [Header("Smashed UI")]
    [SerializeField] private RectTransform _smashedUITransform;
    [SerializeField] private TMP_Text _respawnTimerText;
    [SerializeField] private TMP_Text _smashedByPlayerText;

    [Header("Settings")]
    [SerializeField] private float _scaleDuration;

    [SerializeField] private float _timer;
    private bool _isTimerActive;

    private void Awake()
    {
        Instance = this;        
    }

    private void Start()
    {
        _smashUITransform.localScale = Vector3.zero;
        _smashedUITransform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (_isTimerActive)
        {
            _timer -= Time.deltaTime;

            int timer = (int)_timer;
            _respawnTimerText.text = timer.ToString();

            if(_timer <= 0f)
            {
                _smashedUITransform.localScale = Vector3.zero;
                OnRespawnTimerFinished?.Invoke();
                _isTimerActive = false;
            }
        }
    }

    public void SetSmashUI(string playerName)
    {
        _smashUITransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        
        _smashedPlayerText.text = playerName;
    }

    public void SetSmashedUI(string playerName, int respawnTimeCounter)
    {
        _smashedUITransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);

        _smashedByPlayerText.text = playerName;
        _respawnTimerText.text = respawnTimeCounter.ToString();
        
        _isTimerActive = true;
        _timer = respawnTimeCounter;
    }
}
