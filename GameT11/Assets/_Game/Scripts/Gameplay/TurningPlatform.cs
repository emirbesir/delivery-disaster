using UnityEngine;

public class TurningPlatform : MonoBehaviour
{


    [Header("Turning Platform Settings")]
    [SerializeField] private Vector3 rotationVector;


    private void Update()
    {
        transform.Rotate(rotationVector);
    }
}
