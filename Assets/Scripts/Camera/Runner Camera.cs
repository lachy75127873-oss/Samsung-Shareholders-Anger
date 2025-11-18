using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 baseOffset = new Vector3(0f, 6f, -10f);

    [Range(0f, 1f)]
    public float horizontalFactor = 0.3f;
    [Range(0f, 1f)]
    public float verticalFactor = 0.3f;

    public float maxHorizontalOffset = 2f;
    public float maxVerticalOffset = 1f;

    public float smoothTime = 0.12f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 basePos = new Vector3(
            0f,
            baseOffset.y,
            target.position.z + baseOffset.z
        );

        float xOffset = Mathf.Clamp(
            target.position.x * horizontalFactor,
            -maxHorizontalOffset,
            maxHorizontalOffset
        );

        float yOffset = Mathf.Clamp(
            target.position.y * verticalFactor,
            -maxVerticalOffset,
            maxVerticalOffset
        );

        Vector3 targetCamPos = new Vector3(
            basePos.x + xOffset,
            basePos.y + yOffset,
            basePos.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetCamPos,
            ref velocity,
            smoothTime
        );
    }
}
