using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0.0f;
    [SerializeField] private Transform startPoint = null;
    [SerializeField]private Transform endPoint = null;

    private Rigidbody trainRb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trainRb = GetComponent<Rigidbody>();
        trainRb.position = startPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = trainRb.position + transform.right * speed * Time.deltaTime;
        trainRb.MovePosition(newPosition);

        if (Vector3.Distance(trainRb.position, endPoint.position) < 1.0f)
        {
            trainRb.position = startPoint.position;
        }
    }
}
