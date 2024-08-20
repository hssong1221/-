using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform rt;
    public GameObject effect;

    public float limitTime = 0.05f;
    float TouchTime = 0f;

    public List<GameObject> touchObjectPool = new List<GameObject>();
    void Update()
    {
        if(Input.GetMouseButton(0) && TouchTime >= limitTime)
        {
            TouchTime = 0f;
            // Ŭ�� ��ġ
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, Camera.main, out var localPoint);

            for(int i = 0; i < touchObjectPool.Count; i++)
            {
                // ������� �ִ� ���߿� ��� ���ϰ� �����ִ°� �ٽ� ����ϱ�
                if (!touchObjectPool[i].activeSelf)
                {
                    touchObjectPool[i].SetActive(true);
                    touchObjectPool[i].transform.localPosition = localPoint;
                    return;
                }
            }
            // ó���̰ų� ��밡���Ѱ� ������ ���� ���� �־���
            var gameobject = Instantiate(effect, canvas.transform);
            gameobject.transform.localPosition = localPoint;
            touchObjectPool.Add(gameobject);
        }
        TouchTime += Time.deltaTime;

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
