using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    // 카메라가 바라볼 대상
    public Transform target;

    // 카메라 대상사이 거리 5 ~ 60 
    public float distance = 5.0f;
    // 줌속도
    public float zoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    // x,y축 회전 속도
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    private Vector3 originalPos;
    private Quaternion originalRot;

    float x = 0.0f;
    float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        originalPos = transform.position;
        originalRot = transform.rotation;
    }

    void LateUpdate()
    {
        if (target)
        {
            // Check if the mouse is over any UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Skip camera movement if the pointer is over a UI object
                return;
            }

            // 마우스 왼쪽버튼 눌렀을때 
            if (Input.GetMouseButton(0))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            // 스크롤 버튼 눌렀을때
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minZoom, maxZoom);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
        {
            angle += 360F;
        }
        if (angle > 360F)
        {
            angle -= 360F;
        }
        return Mathf.Clamp(angle, min, max);
    }

    public void ResetCameraPosition()
    {
        transform.position = originalPos;
        transform.rotation = originalRot;
        distance = maxZoom; // Reset the zoom
    }

}
