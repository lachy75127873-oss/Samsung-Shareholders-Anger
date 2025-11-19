using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;      // 따라갈 대상 (플레이어)
    public Vector3 offset;        // 상대 위치 (플레이어 기준 얼마나 떨어져 있을지)
    public float followSpeed = 10f; // 따라오는 속도 (값이 클수록 빠르게 따라옴)

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 = 타겟 위치 + 오프셋
        Vector3 targetPos = target.position + offset;

        // 부드럽게 따라오기 (Lerp 사용)
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
