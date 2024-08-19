using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    public Canvas canvas;
    public GameObject effect;

    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            Instantiate(effect, pos, Quaternion.identity, canvas.transform);
        }
#endif
#if UNITY_ANDROID
        /*
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // ��ġ�� ���۵� �������� Ȯ�� (��ġ ���� ��)
            if (touch.phase == TouchPhase.Began)
            {
                // ��ġ�� ��ġ�� ���� ��ǥ�� ��ȯ
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

                // Z���� 0���� �����Ͽ� 2D ��鿡���� ��ġ�� ����
                touchPosition.z = 0f;

                // ����Ʈ ����
                Instantiate(effect, touchPosition, Quaternion.identity);
            }
        }
        */
#endif
    }
}
