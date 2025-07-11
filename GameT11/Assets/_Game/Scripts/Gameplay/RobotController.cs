using UnityEngine;
using UnityEngine.InputSystem;

namespace GameT11.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class RobotController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody rb;
        private Vector2 moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        private void Update()
        {
            moveInput = Vector2.zero;

            if (Keyboard.current.wKey.isPressed)
                moveInput.y += 1f;
            if (Keyboard.current.sKey.isPressed)
                moveInput.y -= 1f;
            if (Keyboard.current.aKey.isPressed)
                moveInput.x -= 1f;
            if (Keyboard.current.dKey.isPressed)
                moveInput.x += 1f;
        }

        private void FixedUpdate()
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            rb.linearVelocity = moveDir * moveSpeed + Vector3.up * rb.linearVelocity.y;

            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 20f);
            }
        }
    }
}
