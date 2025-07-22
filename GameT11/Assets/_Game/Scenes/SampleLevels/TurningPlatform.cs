using UnityEngine;

public class TurningPlatform : MonoBehaviour
{

    [SerializeField] protected Vector3 rotationVector;

    protected virtual void Update()
    {
        transform.Rotate(rotationVector);
    }
}
