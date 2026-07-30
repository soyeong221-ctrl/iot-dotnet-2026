using UnityEngine;   // 유니티 엔진 클래스

// MonoBehaviour C# 스크립트가 기본적으로 상속받는 핵심 클래스
// 개발자코드가 유니티 엔진과 인터렉티브하게 소통할 수 있도록 
// 오브젝트에 컴포넌트로 연결, 동작을 제어
public class ConveyorBelt : MonoBehaviour
{
    [Header("물체이동 방향")]
    public Vector3 moveDirection = Vector3.right;

    [Header("물체이동 속도")]
    public float speed = 2.0f;

    [Header("벨트 동작여부")]
    public bool isRunning = true;

    // 매 프레임 두 충돌영역이 접촉하고 있는 동안 발생 이벤트핸들러
    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody; // 충돌감지된 오브젝트 리지드바디 가져오기

        if (rb == null) return; 

        if (!isRunning)
        {
            rb.linearVelocity = Vector3.zero; // 0으로 초기화
            return;
        }

        rb.linearVelocity = moveDirection.normalized * speed;  // 이동방향으로 속도만큼 이동
    }

    public void Stop()
    {
        isRunning = false;  // 중지
    }

    public void StartBelt()
    {
        isRunning = true;  // 재시작
    }
}
