using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RobotArm : MonoBehaviour
{
    public AudioSource AS;
    public Transform rotateObject;
    public int ItemsRemaining = 0;
    public TextMeshProUGUI itemsText;
    public TextMeshProUGUI labelText;
    public string requiredLabel = string.Empty;
    public ConveyorBelt connectedBelt;
    public Transform beltChecker;
    private bool isMovingItem = false;
    public List<int> itemsList = new List<int>(); //from code
    private void Start()
    {
        FindObjectOfType<OrderManager>().OrderReceived += OnOrderReceived;
    }
    private void OnTriggerStay(Collider other)
    {

        if (isMovingItem || ItemsRemaining <= 0)
            return;

        if (other.TryGetComponent(out ConveyorItem item))
        {
            if (item.label == requiredLabel || item.itemID == requiredLabel)
            {
                StartCoroutine(MoveItem(item));
                ItemsRemaining--;
                itemsText.text = "Items: "+ItemsRemaining.ToString();
            }
        }
    }

    IEnumerator MoveItem(ConveyorItem item)
    {
        if(AS != null)
            AS.Play();
        isMovingItem = true;
        item.currentBelt.currentItem = null;
        item.currentBelt = null;
        item.transform.parent = rotateObject.transform;

        // move up
        Vector3 startPos = item.transform.localPosition;
        Vector3 upPos = startPos + new Vector3(0, 2f, 0);
        float moveDuration = 1f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            item.transform.localPosition = Vector3.Lerp(startPos, upPos, t);
            yield return null;
        }

        // rotate 180 degrees
        float rotated = 0f;
        float targetAngle = 180f;
        float rotateSpeed = 90f;

        while (rotated < targetAngle)
        {
            float step = rotateSpeed * Time.deltaTime;
            rotateObject.Rotate(0, step, 0);
            rotated += step;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // try connecting to belt
        if (connectedBelt == null)
            TryAutoConnectConveyor();

        while(connectedBelt == null)
        {
            TryAutoConnectConveyor();
            yield return new WaitForSeconds(1);
        }

        while(connectedBelt.currentItem != null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // move item down
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            item.transform.localPosition = Vector3.Lerp(upPos, startPos, t);
            yield return null;
        }

        item.transform.parent = null;
        item.currentBelt = connectedBelt;
        connectedBelt.currentItem = item;

        //AS.Play();
        // rotate back to 0
        rotated = 0f;
        while (rotated < targetAngle)
        {
            float step = rotateSpeed * Time.deltaTime;
            rotateObject.Rotate(0, -step, 0); // reverse direction
            rotated += step;
            yield return null;
        }
        isMovingItem = false;
    }


    void TryAutoConnectConveyor()
    {
        Transform checkerBack = beltChecker;
        if (checkerBack == null) return;

        Vector3 halfExtents = new Vector3(0.2f, 0.2f, 0.2f);
        Collider[] overlapsBack = Physics.OverlapBox(checkerBack.position, halfExtents);
        if (overlapsBack.Length > 0)
        {
            foreach (Collider collider in overlapsBack)
            {
                if (collider.TryGetComponent(out ConveyorBelt belt))
                {
                    connectedBelt = belt; // assign current belt to belt in behind
                }
            }
        }
    }

    public void OnOrderReceived()
    {
        foreach (int amount in itemsList)
        {
            AddItems(amount);
        }
    }

    public void SetLabel(string _label)
    {
        requiredLabel = _label;
        labelText.text = "Label:" + _label;
    }
    public void AddItems(int amount)
    {
        ItemsRemaining += amount;
        itemsText.text = "Items: " + ItemsRemaining.ToString();
    }

    public void ResetArm()
    {
        ItemsRemaining = 0;
        itemsText.text = "Items: " + ItemsRemaining.ToString();
        labelText.text = "Label: NONE";
    }

    //Ingame code example
    //RobotArm arm;
    //arm.SetLabel("A");
    //arm.AddItems(2);
}
