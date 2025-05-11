using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fastSpeedMultiplier = 2f;
    public float minY = 2f;
    public float maxY = 20f;
    public bool canMove = true;

    void Update()
    {
        if (!canMove) return;

        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveZ = Input.GetAxisRaw("Vertical");   

        float moveY = 0f;
        if (Input.GetKey(KeyCode.E)) moveY = 1f;
        else if (Input.GetKey(KeyCode.Q)) moveY = -1f;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= fastSpeedMultiplier;

        Vector3 move = new Vector3(moveX, moveY, moveZ).normalized;
        Vector3 newPos = transform.position + move * currentSpeed * Time.deltaTime;

        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        transform.position = newPos;
    }
}
