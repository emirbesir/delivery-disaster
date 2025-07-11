using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountdownSlider : MonoBehaviour
{


    [Header("Duration")]
    [SerializeField] private FloatReference countdownDuration;

    [Header("Event")]
    [SerializeField] private UnityEvent countdownCompleteEvent;


    private float countdownStartTime;
    private Slider slider;


    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        countdownStartTime = Time.time;
    }

    private void Update()
    {
        float elapsedTime = Time.time - countdownStartTime;
        float remainingTime = countdownDuration.Value - elapsedTime;

        if (remainingTime <= 0f)
        {
            slider.value = 0f;
            countdownCompleteEvent.Invoke();
            Destroy(gameObject);
        }
        else
        {
            slider.value = remainingTime / countdownDuration.Value;
        }
    }
}
