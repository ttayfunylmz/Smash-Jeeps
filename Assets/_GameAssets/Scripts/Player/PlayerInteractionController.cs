using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.TryGetComponent(out ICollectible collectible))
        {
            collectible.Collect();
        }    
    }
}
