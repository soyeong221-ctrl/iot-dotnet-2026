using System.Collections;
using UnityEngine;

public class SensorTrigger : MonoBehaviour
{
    [Header("컨베이어 2")]
    public ConveyorBelt conveyor2;

    [Header("색상 머티리얼")]
    public Material redMaterial;  // 실제 머티리얼 객체와 연결
    public Material greenMaterial;
    public Material blueMaterial;

    //[Header("박스생성기")]
    //public BoxSpawner spawner;

    private bool isProcessing = false;

    private GameObject currProduct;  // 현재 스폰되고 색상판별할 박스 지정

    public void SetColor(string color) {

        if (currProduct == null) return;  // 현재 물체가 없는데 색상 변경불가

        Renderer renderer = currProduct.GetComponent<Renderer>();

        if (color == "R") {
            renderer.material = redMaterial;
        } else if (color == "G") {
            renderer.material = greenMaterial;
        } else if (color == "B") {
            renderer.material = blueMaterial;
        }        
    }

    // 다른 Collider가 들어와서 Trigger 발생하면?
    private void OnTriggerEnter(Collider other)
    {
        if (isProcessing) return;

        if (other.CompareTag("Product"))
        {
            // 시간이 걸리는 작업을 여러 프레임에 나눠서 실행하는 기능
            //StartCoroutine(Process());  // 타이머로 처리 하지 안흠
            isProcessing = true;
            currProduct = other.gameObject;  // 박스가 할당

            // 센서 위치에서 컨베이서 정지
            conveyor2.Stop();

            Debug.Log("제품 도착 - 색상판별 중");
        }
    }

    // MQTT에서 색상판별 완료 후 호출
    public void Resume() {
        if (!isProcessing) return;

        conveyor2.StartBelt();

        isProcessing = false;

        Debug.Log("색상판별 완료");
    }


    private IEnumerator Process()
    {
        isProcessing = true;

        conveyor2.Stop();
        //spawner.Stop();

        yield return new WaitForSeconds(3.0f);  // 3초동안 대기한 뒤 다음로직으로 

        conveyor2.StartBelt();
        //spawner.StartSpawner();

        //Debug.Log("컨베이어/스폰 재시작");

        yield return new WaitForSeconds(1.0f); 

        isProcessing = false;
    }
}
