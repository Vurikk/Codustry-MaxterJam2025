using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderPlatform : MonoBehaviour
{
    public OrderPrefab currentOrder;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out ConveyorItem item))
        {
            if (currentOrder != null)
                currentOrder.AddItemToOrder(item.itemID); // add to order if order is not null


            Destroy(item.gameObject); // destroy it either way
        }
    }
}
