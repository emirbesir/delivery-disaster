using UnityEngine;
using UnityEngine.Events;

public class DeliveryZone : MonoBehaviour
{

    [Header("Events")]
    [SerializeField] private UnityEvent packageDeliveryStartEvent;
    [SerializeField] private UnityEvent packageDeliveryStopEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            packageDeliveryStartEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            packageDeliveryStopEvent.Invoke();
        }
    }
}
