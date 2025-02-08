using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUI : MonoBehaviour
{
    public static SkillsUI Instance { get; private set;}

    [SerializeField] private Image _skillImage;
    [SerializeField] private TMP_Text _skillNameText;
    [SerializeField] private TMP_Text _timerCounterText;

    private void Awake() 
    {
        Instance = this;    
    }

    public void SetSkill(Sprite skillSprite, string skillName)
    {
        _skillImage.sprite = skillSprite;
        _skillNameText.text = skillName;
    }
}
