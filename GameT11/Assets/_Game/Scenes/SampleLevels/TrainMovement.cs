using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [Header("Train Movement Settings")]
    [SerializeField] private FloatReference trainSpeed;

    [Header("Train Path Settings")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private Vector3 moveDirection;

    void Start()
    {   
        transform.position = startPoint.position;
        moveDirection = (endPoint.position - startPoint.position).normalized;
    }

    void Update()
    {
        transform.Translate(moveDirection * trainSpeed.Value * Time.deltaTime);

        if (Vector3.Distance(transform.position, endPoint.position) < 0.1f)
        {
            transform.position = startPoint.position;
        }
    }
}
