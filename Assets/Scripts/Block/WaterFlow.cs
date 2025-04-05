using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WaterFlow : MonoBehaviour
{
    public GameObject waterPrefab;
    public float flowSpeed = 0.5f;
    private HashSet<Vector2> waterPositions = new HashSet<Vector2>();
    private List<GameObject> waterBlocks = new List<GameObject>();
    public float destroyDelay = 0.1f;

    void Start()
    {
        StartFlow(transform.position); // ���� �� �� ����
    }

    void StartFlow(Vector2 startPos)
    {
        GameObject firstWater = Instantiate(waterPrefab, startPos, Quaternion.identity);
        waterBlocks.Add(firstWater);
        waterPositions.Add(startPos);
        StartCoroutine(FlowRoutine());
    }

    IEnumerator FlowRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(flowSpeed); // �帧 �ӵ��� �°� ���

            List<GameObject> newWaterBlocks = new List<GameObject>();
            List<Vector2> newWaterPositions = new List<Vector2>();

            // �� ���� ������ �� null ���� ���� �ʵ��� ó��
            waterBlocks.RemoveAll(water => water == null);

            foreach (GameObject water in waterBlocks)
            {
                if (water == null) continue; // �̹� ������ �� �� �ǳʶ�

                Vector2 currentPos = water.transform.position;

                // ��ֹ� ���� �� �帧 ����
                CheckForObstaclesAndStopFlow(currentPos); // ��ֹ� ���� �� �帧 ���߱�

                // �Ʒ������� �帧
                Vector2 downPos = currentPos + Vector2.down;
                if (!IsBlocked(downPos) && !waterPositions.Contains(downPos))  // �ߺ� ��ġ üũ
                {
                    GameObject newWater = Instantiate(waterPrefab, downPos, Quaternion.identity, transform); // �θ� ����
                    newWaterBlocks.Add(newWater);
                    newWaterPositions.Add(downPos); // �� ��ġ �߰�
                }

                // �������� �帧
                Vector2 upPos = currentPos + Vector2.up;
                if (!IsBlocked(upPos) && !waterPositions.Contains(upPos))  // �ߺ� ��ġ üũ
                {
                    GameObject newWater = Instantiate(waterPrefab, upPos, Quaternion.identity, transform); // �θ� ����
                    newWaterBlocks.Add(newWater);
                    newWaterPositions.Add(upPos); // �� ��ġ �߰�
                }
            }

            // ���ο� �� ���� ����Ʈ�� �߰�
            waterBlocks.AddRange(newWaterBlocks);
            waterPositions.UnionWith(newWaterPositions); // �� ���� ���ο� ��ġ���� �߰�
        }
    }

    bool IsBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position);
        if (hit != null) // ������ ��
        {
            if (hit.CompareTag("B_Fire") || hit.CompareTag("B_Blocked_Earth") || hit.CompareTag("OutWall"))
            {
                return true;
            }
        }
        return false;
    }

    //void CheckForObstaclesAndStopFlow(Vector2 position)
    //{
    //    RaycastHit2D[] hitsDown = Physics2D.RaycastAll(position, Vector2.down, 50f); // �Ʒ��� ����ĳ��Ʈ
    //    RaycastHit2D[] hitsUp = Physics2D.RaycastAll(position, Vector2.up, 50f); // ���� ����ĳ��Ʈ

    //    // �Ʒ��� ���⿡�� ��� �浹ü ����
    //    foreach (RaycastHit2D hit in hitsDown)
    //    {
    //        if (hit.collider != null && hit.collider.CompareTag("B_Blocked_Earth"))
    //        {
    //            StopFlowBasedOnObstacle(hit.point, Vector2.down); // ��ֹ����� �帧�� ����
    //            break; // ù ��° ��ֹ����� �帧�� ���� �� �ݺ��� ����
    //        }
    //    }

    //    // ���� ���⿡�� ��� �浹ü ����
    //    foreach (RaycastHit2D hit in hitsUp)
    //    {
    //        if (hit.collider != null && hit.collider.CompareTag("B_Blocked_Earth"))
    //        {
    //            StopFlowBasedOnObstacle(hit.point, Vector2.up); // ��ֹ����� �帧�� ����
    //            break; // ù ��° ��ֹ����� �帧�� ���� �� �ݺ��� ����
    //        }
    //    }
    //}

    //void StopFlowBasedOnObstacle(Vector2 obstaclePosition, Vector2 flowDirection)
    //{
    //    // ��ֹ� �������� �帣�� �� ���� ����
    //    foreach (GameObject water in waterBlocks)
    //    {
    //        if (water == null) continue; // �̹� ������ �� �� �ǳʶ�

    //        Vector2 currentPos = water.transform.position;

    //        // �帧 ���⿡ ���� �� �� ���� ����
    //        if (flowDirection == Vector2.down)
    //        {
    //            // �Ʒ��� �帣�� �� �� �� ��ֹ����� '�� ����' �� ���� ����
    //            if (currentPos.y < obstaclePosition.y && currentPos.y < transform.position.y)
    //            {
    //                Destroy(water); // �Ʒ��� �帣�� �� �� ����
    //            }
    //        }
    //        else if (flowDirection == Vector2.up)
    //        {
    //            // ���� �帣�� �� �� �� ��ֹ����� '�� ����' �� ���� ����
    //            if (currentPos.y > obstaclePosition.y && currentPos.y > transform.position.y)
    //            {
    //                Destroy(water); // ���� �帣�� �� �� ����
    //            }
    //        }
    //    }
    //}

    void CheckForObstaclesAndStopFlow(Vector2 position)
    {
        RaycastHit2D[] hitsDown = Physics2D.RaycastAll(position, Vector2.down, 50f); // �Ʒ��� ����ĳ��Ʈ
        RaycastHit2D[] hitsUp = Physics2D.RaycastAll(position, Vector2.up, 50f); // ���� ����ĳ��Ʈ

        // �Ʒ��� ���⿡�� ��� �浹ü ����
        foreach (RaycastHit2D hit in hitsDown)
        {
            if (hit.collider != null && hit.collider.CompareTag("B_Blocked_Earth"))
            {
                StartCoroutine(FadeOutAndDestroy(hit.point, Vector2.down)); // ��ֹ����� �帧�� ���߰� ������ ����
                break; // ù ��° ��ֹ����� �帧�� ���� �� �ݺ��� ����
            }
        }

        // ���� ���⿡�� ��� �浹ü ����
        foreach (RaycastHit2D hit in hitsUp)
        {
            if (hit.collider != null && hit.collider.CompareTag("B_Blocked_Earth"))
            {
                StartCoroutine(FadeOutAndDestroy(hit.point, Vector2.up)); // ��ֹ����� �帧�� ���߰� ������ ����
                break; // ù ��° ��ֹ����� �帧�� ���� �� �ݺ��� ����
            }
        }
    }
    IEnumerator FadeOutAndDestroy(Vector2 obstaclePosition, Vector2 flowDirection)
    {
        List<GameObject> waterToDelete = new List<GameObject>();

        foreach (GameObject water in waterBlocks)
        {
            if (water == null) continue; // �̹� ������ �� �� �ǳʶ�

            Vector2 currentPos = water.transform.position;

            // �帧 ���⿡ ���� �� �� ���� ����
            if (flowDirection == Vector2.down)
            {
                // �Ʒ��� �帣�� �� �� �� ��ֹ����� '�� ����' �� ���� ����
                if (currentPos.y < obstaclePosition.y && currentPos.y < transform.position.y)
                {
                    waterToDelete.Add(water);
                }
            }
            else if (flowDirection == Vector2.up)
            {
                // ���� �帣�� �� �� �� ��ֹ����� '�� ����' �� ���� ����
                if (currentPos.y > obstaclePosition.y && currentPos.y > transform.position.y)
                {
                    waterToDelete.Add(water);
                }
            }
        }

        // ������ �����ϱ�
        foreach (GameObject water in waterToDelete)
        {
            Destroy(water); // �� �� ����
            yield return new WaitForSeconds(destroyDelay); // ���� �ð� �� ����
        }
    }

}