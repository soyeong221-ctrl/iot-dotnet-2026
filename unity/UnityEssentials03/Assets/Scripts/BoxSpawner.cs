using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [Header("프리팹 지정")]
    public GameObject prdPrefab;

    [Header("생성 간격")]
    public float interval = 3.0f;

    float timer;
    
    void Update()
    {
        timer += Time.deltaTime; // HW 성능별 FPS 고정

        if (timer >= interval)
        {
            timer = 0;

            // Instant = 예제, 샘플
            // Quatenrnion.identity = 회전값 없는 상태
            Instantiate(prdPrefab,
                        transform.position,
                        Quaternion.identity);
        }
    }
}
