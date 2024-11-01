using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    public Canvas canvas;
    public GameObject parent;
    public RectTransform rt;
    public GameObject effect;
    public GameObject effect2;

    public float limitTime = 0.1f;
    float TouchTime = 0f;

    public List<GameObject> touchObjectPool = new List<GameObject>();
    public List<GameObject> touchObjectPool2 = new List<GameObject>();
    void Update()
    {
        if(Input.GetMouseButton(0) && TouchTime >= limitTime)
        {
            TouchTime = 0f;
            // Ŭ�� ��ġ
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, Camera.main, out var localPoint);

            EffectOut1(localPoint);
            EffectOut2(localPoint);
        }
        TouchTime += Time.deltaTime;

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
    }

    void EffectOut1(Vector2 localPoint)
    {
        for (int i = 0; i < touchObjectPool.Count; i++)
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
        var gameobject = Instantiate(effect, parent.transform);
        gameobject.transform.localPosition = localPoint;
        touchObjectPool.Add(gameobject);
    }

    void EffectOut2(Vector2 localPoint)
    {
        for (int i = 0; i < touchObjectPool2.Count; i++)
        {
            // ������� �ִ� ���߿� ��� ���ϰ� �����ִ°� �ٽ� ����ϱ�
            if (!touchObjectPool2[i].activeSelf)
            {
                touchObjectPool2[i].SetActive(true);
                touchObjectPool2[i].transform.localPosition = localPoint;
                return;
            }
        }
        // ó���̰ų� ��밡���Ѱ� ������ ���� ���� �־���
        var gameobject = Instantiate(effect2, parent.transform);
        gameobject.transform.localPosition = localPoint;
        touchObjectPool2.Add(gameobject);
    }
}
