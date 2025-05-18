using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : MonoBehaviour
{
    public ConveyorBelt belt;
    public Transform rightChecker;
    public Transform leftChecker;

    public ConveyorBelt rightBelt;
    public ConveyorBelt leftBelt;

    private int toggle = 0; // 0 = left, 1 = right

    public GameObject rotateObject; 

    private void Start()
    {
        rightBelt = GetBeltAt(rightChecker);
        leftBelt = GetBeltAt(leftChecker);
    }

    public void ChangeDirection()
    {
        toggle = 1 - toggle;
        StartCoroutine(RotateMiddlePart());

        if (leftBelt == null)
            leftBelt = GetBeltAt(leftChecker);
        if (rightBelt == null)
            rightBelt = GetBeltAt(rightChecker);


        switch (toggle)
        {
            default:
                break;

            case 0: // left
                if (leftBelt != null)
                    belt.nextBelt = leftBelt;
                else
                    belt.nextBelt = null;
                break;

            case 1: // right
                if (rightBelt != null)
                    belt.nextBelt = rightBelt;
                else 
                    belt.nextBelt = null;
                break;
        }
    }
    private IEnumerator RotateMiddlePart()
    {
        yield return new WaitForSeconds(2);
        Quaternion targetRotation;

        if (toggle == 1) //right direction
            targetRotation = Quaternion.Euler(-90f, -45f, 0f);
        else //left
            targetRotation = Quaternion.Euler(-90f, 45f, 0f); 

        float duration = 0.6f;
        float elapsed = 0f;

        Quaternion initialRotation = rotateObject.transform.localRotation;

        while (elapsed < duration)
        {
            rotateObject.transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rotateObject.transform.localRotation = targetRotation;
    }

    private ConveyorBelt GetBeltAt(Transform checker)
    {
        Vector3 halfExtents = new Vector3(0.2f, 0.2f, 0.2f);
        Collider[] overlaps = Physics.OverlapBox(checker.position, halfExtents);

        foreach (Collider col in overlaps)
        {
            if (col.TryGetComponent(out ConveyorBelt b))
            {
                return b;
            }
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out ConveyorItem item))
        {
            ChangeDirection();
        }
    }
}
