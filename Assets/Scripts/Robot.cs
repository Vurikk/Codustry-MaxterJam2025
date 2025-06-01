using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public float moveDistance = 2f;
    public float moveDuration = 0.3f;
    public float rotationAngle = 90f;
    public float rotationDuration = 0.2f;

    private bool isMoving = false;

    [Header("Code")]
    [TextArea(1, 20)]
    public string baseCode;
    [TextArea(1, 20)]
    public string userCode;

    private void Start()
    {
        InvokeRepeating("Move", 1, 4);
        InvokeRepeating("RotateRight", 2, 10);
    }
    public void Move()
    {
        if (!isMoving)
            StartCoroutine(SmoothMove());
    }

    public void RotateRight()
    {
        if (!isMoving)
            StartCoroutine(SmoothRotate(rotationAngle));
    }

    public void RotateLeft()
    {
        if (!isMoving)
            StartCoroutine(SmoothRotate(-rotationAngle));
    }

    private IEnumerator SmoothMove()
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * moveDistance;

        float elapsed = 0;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        isMoving = false;
    }

    private IEnumerator SmoothRotate(float angle)
    {
        isMoving = true;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, angle, 0);

        float elapsed = 0;
        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRot;
        isMoving = false;
    }
}
