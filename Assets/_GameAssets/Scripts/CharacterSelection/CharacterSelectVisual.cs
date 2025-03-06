using UnityEngine;

public class CharacterSelectVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer _baseMeshRenderer;

    private Material _material;

    private void Awake()
    {
        _material = new Material(_baseMeshRenderer.material);
        _baseMeshRenderer.material = _material;
    }

    public void SetPlayerColor(Color color)
    {
        _material.color = color;
    }

}
