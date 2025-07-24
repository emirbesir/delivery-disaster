using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TriggerDelayedEventsOnPlayerCollision : MonoBehaviour
{   


    [Header("Event Settings")]
    [SerializeField] private UnityEvent onCollisionEvent;
    [SerializeField] private UnityEvent delayedEvent;
    [SerializeField] private float delayTime = 2f;
    
    private const string PLAYER_TAG = "Player";


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG))
        {
            onCollisionEvent?.Invoke();
            StartCoroutine(TriggerDelayedEvent());
        }
    }

    private IEnumerator TriggerDelayedEvent()
    {
        yield return new WaitForSeconds(delayTime);
        delayedEvent?.Invoke();
    }
}
