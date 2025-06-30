using UnityEngine;
using UnityEngine.Events;

public class XSimpleHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private float currentHealth;

    [SerializeField] private UnityEvent damageEvent;
    [SerializeField] private UnityEvent deathEvent;

    private void Start()
    {
        currentHealth = startingHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        TakeDamage();
    }

    public void TakeDamage()
    {
        currentHealth -= 10f;
        damageEvent.Invoke();

        if (currentHealth <= 0f)
        {
            deathEvent.Invoke();
        }
    }
}
