using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("회전 설정")]
    [Tooltip("프레임당 회전 속도")]
    [Range(0, 10)]
    public float rotationSpeed = 1.0f;

    [Tooltip("아이템 획득 시 이펙트 지정")]
    public GameObject onCollectEffect;

    [Header("이펙트 사운드")]
    public AudioClip pickupSound;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0);  // 매프레임마다 y축을 1씩 회전
    }

    // 물체끼리 충돌이 발생했을 때 이벤트처리
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            AudioSource.PlayClipAtPoint(pickupSound, transform.position); ;

            // Destroy the collectible - 코인 삭제
            Destroy(gameObject);

            // Instantiate the particle effect
            Instantiate(onCollectEffect, transform.position, transform.rotation);
        }
    }
}
