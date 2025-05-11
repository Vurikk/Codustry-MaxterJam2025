using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class OrderManager : MonoBehaviour
{
    public GameObject orderPrefab;
    [Header("Canvas")]
    public Transform orderPanel;

    [Space(10)]
    public List<OrderPrefab> activeOrders = new List<OrderPrefab>();

    public List<OrderData> preCodedOrders = new List<OrderData>();
    public List<OrderData> randomOrderPool = new List<OrderData>();

    public Action OrderReceived;

    //stats
    public int ordersCompleted;
    public int ordersFailed;
    public int lastOrderID;
    public int totalCubesProduced = 0;
    public int totalCubesRequired = 0;

    public TextMeshProUGUI statsText; //no time :DDDd

    private void Start()
    {
       Invoke("CreateOrder", 10);
        UdpdateStats();
       //Invoke("CreateOrder", 60);
    }
    public void UdpdateStats()
    {
        int accuracy = 100;
        if (totalCubesProduced == 0 || totalCubesRequired == 0)
            accuracy = 100;
        else
        {
            float temp = (float) totalCubesRequired / totalCubesProduced;
            accuracy = (int)(temp * 100);
        }
        statsText.text = $"Items requested:{totalCubesRequired}\nItems produced:{totalCubesProduced}\nAccuracy:{accuracy}%";
    }
    public void FailOrder()
    {
        ordersFailed++;
        Invoke("CreateOrder", 10);
        UdpdateStats();
    }
    public void CompeteOrder()
    {
        ordersCompleted++;
        Invoke("CreateOrder", 10);
        UdpdateStats();
    }

    public void CreateOrder()
    {
        OrderData order;
        if (ordersCompleted <= preCodedOrders.Count)
        {
            order = preCodedOrders[ordersCompleted];
        }
        else
        {
            //logic behind random order
            // prob something like 
            // max amount = 50 and rnandomly split it between some items
            // max craft amount = 10; and also split between items

            // TODO
            //gg have no time left :DDD
            ordersCompleted = 2; // just reset everything for now
            order = preCodedOrders[ordersCompleted];
        }
        lastOrderID++;

        OrderPrefab newOrder = Instantiate(orderPrefab, orderPanel).GetComponent<OrderPrefab>();
        newOrder.Init(order);
        FindObjectOfType<OrderPlatform>().currentOrder = newOrder; 
        OrderReceived?.Invoke();
    }


}

[System.Serializable]
public class OrderData
{
    public int id;
    public float timeLimit;
    public List<CraftRequirement> requiredItems;

    public string description;
}

[System.Serializable]
public class CraftRequirement
{
    public string itemName;
    public int amount;
}