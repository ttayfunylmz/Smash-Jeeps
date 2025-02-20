using DG.Tweening;
using TMPro;
using UnityEngine;

public class KillScreenUI : MonoBehaviour
{
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
        if(Input.GetKeyDown(KeyCode.T))
        {
            SetSmashUI("Player 1");
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            SetSmashedUI("Player 2", 5);
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
    }
}
