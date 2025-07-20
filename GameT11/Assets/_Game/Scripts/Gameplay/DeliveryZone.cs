using UnityEngine;
using UnityEngine.Events;

public class DeliveryZone : MonoBehaviour
{


    [Header("Delivery Zone Settings")]
    [SerializeField] private Material deliveryZoneMaterial;

    [Header("Events")]
    [SerializeField] private UnityEvent packageDeliveryStartEvent;
    [SerializeField] private UnityEvent packageDeliveryStopEvent;

    // Material property constants
    private const string EDGE_WIDTH_PROPERTY = "_EdgeWidth";
    private const float EDGE_WIDTH_DEFAULT = 0.2f;
    private const float EDGE_WIDTH_MULTIPLIER = 2f;

    private const string BASE_COLOR_PROPERTY = "_BaseColor";
    private Color defaultBaseColor = new Color(1f, 217f / 255f, 51f / 255f, 1f); // Yellow color
    private Color deliveryBaseColor = new Color(0f, 128f / 255f, 0f, 1f); // Green color

    private const string PULSE_COLOR_PROPERTY = "_PulseColor";
    private Color defaultPulseColor = new Color(1f, 1f, 204f / 255f, 1f); // Faint yellow color
    private Color deliveryPulseColor = new Color(204f / 255f, 1f, 204f / 255f, 1f); // Faint green color


    private void Start()
    {
        ResetMaterialProperties();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            packageDeliveryStartEvent.Invoke();
            SetDeliveryMaterialProperties();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            packageDeliveryStopEvent.Invoke();
            ResetMaterialProperties();
        }
    }

    private void ResetMaterialProperties()
    {
        deliveryZoneMaterial.SetFloat(EDGE_WIDTH_PROPERTY, EDGE_WIDTH_DEFAULT);
        deliveryZoneMaterial.SetColor(BASE_COLOR_PROPERTY, defaultBaseColor);
        deliveryZoneMaterial.SetColor(PULSE_COLOR_PROPERTY, defaultPulseColor);
    }
    
    private void SetDeliveryMaterialProperties()
    {
        deliveryZoneMaterial.SetFloat(EDGE_WIDTH_PROPERTY, EDGE_WIDTH_DEFAULT * EDGE_WIDTH_MULTIPLIER);
        deliveryZoneMaterial.SetColor(BASE_COLOR_PROPERTY, deliveryBaseColor);
        deliveryZoneMaterial.SetColor(PULSE_COLOR_PROPERTY, deliveryPulseColor);
    }
}
