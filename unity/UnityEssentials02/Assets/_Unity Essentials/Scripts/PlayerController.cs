using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves forward/backward and rotates with WASD/Arrow keys.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Tooltip("전진후진 속도 (units/sec).")]
    public float speed = 2.0f;

    [Tooltip("회전 속도 (degrees/sec).")]
    public float rotationSpeed = 100.0f;

    [Tooltip("점프 강도")]
    public float jumpForce = 5.0f;

    private Rigidbody rb; // 리지드바디

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        if (rb == null) Debug.LogWarning("PlayerController needs a Rigidbody.");
    }

    // 입력처리, 카메라... Frame별 실행
    // LateUpdate(): Update() 후에 실행되는 메서드 - 카메라 추적
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    // 고정시간 간격으로 업데이트, Rigidbody 등의 처리
    private void FixedUpdate()
    {
        Vector2 moveInput = Vector2.zero;

        // 앞뒤 움직임 - w(up key), s(down key)
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y = -1f;

        // 좌우 회전 - a(left key), d(right key)
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x = 1f;

        // Move in facing direction 
        Vector3 movement = transform.forward * moveInput.y * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);    // 위치 이동

        // Y-axis rotation (invert when going backwards)
        float turnDirection = moveInput.x;
        if (moveInput.y < 0)
            turnDirection = -turnDirection;

        float turn = turnDirection * rotationSpeed * Time.fixedDeltaTime;   // 회전값
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
