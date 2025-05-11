using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public ConveyorItem currentItem;
    public ConveyorBelt nextBelt;
    public Vector3 moveDirection;
    public float speed = 2f;
    public Vector3 itemPosOffset = new Vector3(0, 0.5f, 0);
    public Transform inputCheckerBack;
    public Transform inputCheckerFront;

    void Start()
    {
        moveDirection = transform.forward;
    }

    public Vector3 GetExitPosition()
    {
        return transform.position + moveDirection.normalized * 0.5f; 
    }
}
