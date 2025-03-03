using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsText;
    private float _deltaTime;

    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
    }
}
