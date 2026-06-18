using System;
using System.Collections;
using UnityEngine;

public class SensorTrigger : MonoBehaviour
{
    [Header("컨베이어 1")]
    public ConveyorBelt conveyor1;

    [Header("컨베이어 2")]
    public ConveyorBelt conveyor2;

    private bool isProcessing = false;

    // 다른 Collider가 들어와서 Trigger 발생하면?
    private void OnTriggerEnter(Collider other)
    {

        if (isProcessing) return;

        if (other.CompareTag("Product"))
        {
            // 시간이 걸리는 작업을 여러 프레임에 나눠서 실행하는 기능
            StartCoroutine(Process());
        }
    }

    private IEnumerator Process()
    {
        isProcessing = true;

        Debug.Log("제품 감지!");

        conveyor1.stop();   // isRunning = false;
        conveyor2.stop();

        yield return new WaitForSeconds(3.0f);  // 3초동안 대기한 뒤 다음로직으로 

        conveyor1.StartBelt();
        conveyor2.StartBelt();

        yield return new WaitForSeconds(1.0f);

        isProcessing = false;
    }
}
