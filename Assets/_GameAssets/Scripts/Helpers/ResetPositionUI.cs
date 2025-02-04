using UnityEngine;

public class ResetPositionUI : MonoBehaviour
{
    private void Awake() 
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        Destroy(this);    
    }
}
