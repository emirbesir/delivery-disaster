using UnityEngine;
using System.Collections;

public class RCCarMovement : MonoBehaviour
{   

    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float turnSpeed = 150f;

    [Header("Behavior Settings")]
    [SerializeField] private float minDecisionTime = 1.0f;
    [SerializeField] private float maxDecisionTime = 3.5f;

    [Header("Reset Settings")]
    [SerializeField] private float timeToReset = 2.0f;

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.5f;


    private Rigidbody rb;
    private float turnDirection;
    private float upsideDownTimer;
    private bool isGrounded;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(MovementRoutine());
    }

    void Update()
    {
        CheckIfUpsideDown();
    }

    void FixedUpdate()
    {
        GroundCheck();

        if (isGrounded)
        {
            rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.fixedDeltaTime);

            Quaternion turnRotation = Quaternion.Euler(0f, turnDirection * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
    
    private void GroundCheck()
    {
        Vector3 carPosition = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(carPosition, Vector3.down, groundCheckDistance, groundLayer);
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            turnDirection = Random.Range(-1f, 1f);
            float waitTime = Random.Range(minDecisionTime, maxDecisionTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void CheckIfUpsideDown()
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0)
        {
            upsideDownTimer += Time.deltaTime;
        }
        else
        {
            upsideDownTimer = 0f;
        }

        if (upsideDownTimer > timeToReset)
        {
            ResetCar();
        }
    }

    private void ResetCar()
    {
        transform.position += Vector3.up * 1.5f;

        transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        upsideDownTimer = 0f;
    }
}