using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;   // �÷��̾� Transform
    public float smoothSpeed = 0.125f;  // ������� �ӵ�
    public Vector3 offset = new Vector3(0, 0, -10); // ī�޶� ���� ������

    void LateUpdate()
    {
        if (playerTransform == null)
        {
            // �±׷� �÷��̾� �ڵ� Ž�� (�ʱ� ����)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (playerTransform != null)
        {
            // ��ǥ ��ġ ���
            Vector3 desiredPosition = playerTransform.position + offset;
            // �ε巯�� �̵� ó��
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
