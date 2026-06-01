using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCube : MonoBehaviour
{
    // private Vector3 position;
    private float speed = 20f;
    private float rotatespeed = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // 화면이 시작되고 최초 한번만 실행되는 초기화 메서드
    void Start()
    {
        // position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    // 각 프레임마다 호출. 30fps => 1초에 30번 호출
    void Update()
    {
        // position = Vector3.zero; // 현재 위치 초기화

        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    position.z = 0.1f;
        //    transform.Translate(position);

        //}

        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    position.z = -0.1f;
        //    transform.Translate(position);
        //}

        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    position.x = -1f;
        //    transform.Rotate(position);

        //}

        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    position.x = 1f;
        //    transform.Rotate(position);
        //}

        /// 앞뒤 이동
        float move = Input.GetAxis("Vertical");
        move = move * speed * Time.deltaTime;

        transform.Translate(Vector3.forward * move);

        /// 좌우 회전
        float rotate = Input.GetAxis("Horizontal");
        rotate = rotate * rotatespeed * Time.deltaTime;

        transform.Rotate(Vector3.up * rotate);

    }
}
