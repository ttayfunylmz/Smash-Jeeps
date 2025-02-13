using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUI : MonoBehaviour
{
    public static SkillsUI Instance { get; private set;}

    [Header("References")]
    [SerializeField] private Image _skillImage;
    [SerializeField] private Transform _timerCounterBackgroundTransform;
    [SerializeField] private TMP_Text _skillNameText;
    [SerializeField] private TMP_Text _timerCounterText;

    [Header("Settings")]
    [SerializeField] private float _scaleDuration;

    private void Awake() 
    {
        Instance = this;
    }

    private void Start()
    {
        SetSkillToNone();

        _timerCounterBackgroundTransform.localScale = Vector3.zero;
        _timerCounterBackgroundTransform.gameObject.SetActive(false);
    }

    public void SetSkill(Sprite skillSprite, string skillName, SkillUsageType skillUsageType, int timerCounter)
    {
        _skillImage.gameObject.SetActive(true);
        _skillImage.sprite = skillSprite;
        _skillNameText.text = skillName;

        if(skillUsageType == SkillUsageType.Timer || skillUsageType == SkillUsageType.Amount)
        {
            SetTimerCounterAnimation(timerCounter);
        }
    }

    private void SetTimerCounterAnimation(int timerCounter)
    {
        if(_timerCounterBackgroundTransform.gameObject.activeInHierarchy) { return; }
        _timerCounterBackgroundTransform.gameObject.SetActive(true);
        _timerCounterBackgroundTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        _timerCounterText.text = timerCounter.ToString();
    }

    public void SetSkillToNone()
    {
        _skillNameText.text = string.Empty;
        _skillImage.gameObject.SetActive(false);
        
        if(_timerCounterBackgroundTransform.gameObject.activeInHierarchy)
        {
            _timerCounterBackgroundTransform.DOScale(0f, _scaleDuration).SetEase(Ease.InBack).OnComplete(() =>
            {
                _timerCounterBackgroundTransform.gameObject.SetActive(false);
            });
        }
    }

    public void SetTimerCounterText(int timerCounter)
    {
        _timerCounterText.text = timerCounter.ToString();
    }
}
