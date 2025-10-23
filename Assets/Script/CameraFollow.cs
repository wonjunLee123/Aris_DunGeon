using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;   // 플레이어 Transform
    public float smoothSpeed = 0.125f;  // 따라오는 속도
    public Vector3 offset = new Vector3(0, 0, -10); // 카메라 고정 오프셋

    void LateUpdate()
    {
        if (playerTransform == null)
        {
            // 태그로 플레이어 자동 탐색 (초기 설정)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (playerTransform != null)
        {
            // 목표 위치 계산
            Vector3 desiredPosition = playerTransform.position + offset;
            // 부드러운 이동 처리
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
