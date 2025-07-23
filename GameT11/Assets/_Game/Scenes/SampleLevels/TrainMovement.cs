using UnityEngine;
using System.Collections;

public class TrainMovement : MonoBehaviour
{
    [Header("Train Movement Settings")]
    [SerializeField] private FloatReference trainSpeed;

    [Header("Train Path Settings")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Train Delay Settings")]
    [SerializeField] private float delay = 0f;

    private Vector3 moveDirection;

    void Start()
    {
        StartCoroutine(InitializeTrainMovement());
    }

    private IEnumerator InitializeTrainMovement()
    {
        yield return new WaitForSeconds(delay);

        transform.position = startPoint.position;
        moveDirection = (endPoint.position - startPoint.position).normalized;
    }

    void Update()
    {
        transform.Translate(moveDirection * trainSpeed.Value * Time.deltaTime);

        if (Vector3.Distance(transform.position, endPoint.position) < 1f)
        {
            transform.position = startPoint.position;
        }
    }
}
