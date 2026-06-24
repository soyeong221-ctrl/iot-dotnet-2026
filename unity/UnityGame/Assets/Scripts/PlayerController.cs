using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("조작 설정")]
    public float speed = 8f;
    public float jumpForce = 7f;
    public Vector3 respawnPoint = new Vector3(0, 5, 0);

    [Header("아이템 및 설정")]
    public GameObject keyObject;
    public int targetCoins = 13;
    public Transform cameraTransform; 

    private Rigidbody rb;
    private bool isGrounded;
    private bool isClimbing = false;
    private int currentCoins = 0;
    private bool hasKey = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (keyObject != null) keyObject.SetActive(false);
    }

    void Update()
    {
        // 카메라 팔로우 (카메라가 없을 때를 대비해 로직 통합)
        if (cameraTransform != null)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position + new Vector3(0, 3, -5), 0.1f);
            cameraTransform.LookAt(transform);
        }

        // 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // 낙사 리스폰
        if (transform.position.y < -10f) transform.position = respawnPoint;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (isClimbing)
        {
            rb.useGravity = false;
            rb.linearVelocity = new Vector3(h * speed, v * speed, 0);
        }
        else
        {
            rb.useGravity = true;
            // 카메라 기준 이동 계산
            Vector3 camForward = (cameraTransform != null) ? Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized : Vector3.forward;
            Vector3 camRight = (cameraTransform != null) ? cameraTransform.right : Vector3.right;

            Vector3 move = (camForward * -v + camRight * -h).normalized * speed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

            if (move.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(move.x, 0, move.z)), 15f * Time.deltaTime);
            }
        }
    }

    void OnTriggerStay(Collider other) { if (other.name.Contains("Ladder")) isClimbing = true; }
    void OnTriggerExit(Collider other) { if (other.name.Contains("Ladder")) isClimbing = false; }
    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Coin")) { Destroy(other.gameObject); currentCoins++; if (currentCoins >= targetCoins && keyObject != null) keyObject.SetActive(true); }
        if (other.name == "Key") { Destroy(other.gameObject); hasKey = true; }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
        if (collision.gameObject.name.Contains("Spikes")) transform.position = respawnPoint;
        if (collision.gameObject.name == "Door" && hasKey) { Debug.Log("CLEAR!"); Time.timeScale = 0; }
    }
    void OnCollisionExit(Collision collision) { if (collision.gameObject.CompareTag("Ground")) isGrounded = false; }
}