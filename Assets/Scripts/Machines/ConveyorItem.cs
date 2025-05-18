using System.Collections;
using UnityEngine;

public class ConveyorItem : MonoBehaviour
{
    public string label; // eg. order 1, craft 1, AB...
    public string itemID; // ID of an item. A B C ...
    public ConveyorBelt currentBelt;
    private bool isMoving = true;
    public Vector3 itemOffset = new Vector3(0, 1, 0);

    public ItemInfo infoPrefab;
    private ItemInfo tempInfoObj; // temporary info obj
    void Update()
    {
        if (!isMoving || currentBelt == null) return;

        Vector3 targetPos = currentBelt.GetExitPosition() + itemOffset;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, currentBelt.speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            TryMoveToNextBelt();
        }

        if (tempInfoObj != null)
            tempInfoObj.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
    }
    void TryMoveToNextBelt()
    {
        if (currentBelt.nextBelt != null && currentBelt.nextBelt.currentItem == null)
        {
            currentBelt.currentItem = null;

            // move to next belt
            currentBelt = currentBelt.nextBelt;
            currentBelt.currentItem = this;
        }
        else
        {
            // wait until next belt is free
            isMoving = false;
            StartCoroutine(CheckNextBeltRoutine());
        }
    }

    IEnumerator CheckNextBeltRoutine() // used to check if the next belt is free
    {
        while (true) // inf loop
        {
            if(currentBelt != null && currentBelt.nextBelt != null && currentBelt.nextBelt.currentItem == null) // next belt not null and is free
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        if (currentBelt.nextBelt != null)
        {
            currentBelt.currentItem = null;
            currentBelt = currentBelt.nextBelt;
            currentBelt.currentItem = this;
            isMoving = true;
        }
    }

    //Inf on mouse hover
    private void OnMouseEnter()
    {
        if(tempInfoObj != null) Destroy(tempInfoObj.gameObject);

        tempInfoObj = Instantiate(infoPrefab, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.Euler(60,0,0));
        tempInfoObj.Init(itemID, label);
    }
    private void OnMouseExit()
    {
        if (tempInfoObj != null) 
            Destroy(tempInfoObj.gameObject);
    }
    private void OnDestroy()
    {
        if (tempInfoObj != null)
            Destroy(tempInfoObj.gameObject);
    }
}
