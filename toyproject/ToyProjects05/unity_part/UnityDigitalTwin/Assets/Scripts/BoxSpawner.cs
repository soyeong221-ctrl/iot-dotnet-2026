using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [Header("프리팹 지정")]
    public GameObject prdPrefab;

    [Header("생성 간격")]
    public float interval = 3.0f;

    private float timer;
    private bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;  // isRunning이 false면 아래 로직 실행안함

        timer += Time.deltaTime;   // HW 성능별 FPS 고정

        if (timer >= interval)
        {
            timer = 0;

            // instant 예제, 샘플
            // Quaternion.identity 회전값 없는 상태
            Instantiate(prdPrefab,
                        transform.position,
                        Quaternion.identity);
        }
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void StartSpawner()
    {
        isRunning = true;
    } 
}
