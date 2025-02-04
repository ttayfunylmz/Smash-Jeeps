using UnityEngine;

public class MysteryBoxCollectible : MonoBehaviour, ICollectible
{
    public void Collect()
    {
        Debug.Log("Collected!");
        Destroy(gameObject);
    }
}
