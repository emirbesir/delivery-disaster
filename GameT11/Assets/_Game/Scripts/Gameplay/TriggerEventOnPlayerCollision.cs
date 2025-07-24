using UnityEngine;
using UnityEngine.Events;

public class TriggerEventOnPlayerCollision : MonoBehaviour
{   


    [Header("Event Settings")]
    [SerializeField] private UnityEvent onCollisionEvent;
    
    
    private const string PLAYER_TAG = "Player";


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG))
        {
            onCollisionEvent?.Invoke();
        }
    }
}
