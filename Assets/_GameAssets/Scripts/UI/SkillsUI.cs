using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUI : MonoBehaviour
{
    public static SkillsUI Instance { get; private set; }

    [Header("Skill References")]
    [SerializeField] private Image _skillImage;
    [SerializeField] private GameObject _skillParent;
    [SerializeField] private Transform _timerCounterBackgroundTransform;
    [SerializeField] private TMP_Text _skillNameText;
    [SerializeField] private TMP_Text _timerCounterText;

    [Header("Dodge References")]
    [SerializeField] private Image _dodgeOutlineImage;
    [SerializeField] private Image _dodgeBackgroundImage;

    [Header("Sprites")]
    [SerializeField] private Sprite _blueBackgroundSprite;
    [SerializeField] private Sprite _yellowBackgroundSprite;

    [Header("Settings")]
    [SerializeField] private float _scaleDuration;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetSkillToNone();
        SetDodgeToReady();

        _timerCounterBackgroundTransform.localScale = Vector3.zero;
        _timerCounterBackgroundTransform.gameObject.SetActive(false);
    }

    public void SetSkill(Sprite skillSprite, string skillName, SkillUsageType skillUsageType, int timerCounter)
    {
        _skillParent.SetActive(true);
        _skillImage.sprite = skillSprite;
        _skillNameText.text = skillName;

        if (skillUsageType == SkillUsageType.Timer || skillUsageType == SkillUsageType.Amount)
        {
            SetTimerCounterAnimation(timerCounter);
        }
    }

    private void SetTimerCounterAnimation(int timerCounter)
    {
        if (_timerCounterBackgroundTransform.gameObject.activeInHierarchy) { return; }
        _timerCounterBackgroundTransform.gameObject.SetActive(true);
        _timerCounterBackgroundTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        _timerCounterText.text = timerCounter.ToString();
    }

    public void SetSkillToNone()
    {
        _skillNameText.text = string.Empty;
        _skillParent.SetActive(false);

        if (_timerCounterBackgroundTransform.gameObject.activeInHierarchy)
        {
            _timerCounterBackgroundTransform.gameObject.SetActive(false);
        }
    }

    public void SetTimerCounterText(int timerCounter)
    {
        _timerCounterText.text = timerCounter.ToString();
    }

    public void SetDodge(float timerMax)
    {
        _dodgeBackgroundImage.sprite = _blueBackgroundSprite;
        _dodgeOutlineImage.fillAmount = 0f;
        _dodgeOutlineImage.DOFillAmount(1f, timerMax).SetEase(Ease.Linear);
    }

    public void SetDodgeToReady()
    {
        _dodgeBackgroundImage.sprite = _yellowBackgroundSprite;
        _dodgeOutlineImage.fillAmount = 1f;
    }
}
