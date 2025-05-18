using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OrderPrefab : MonoBehaviour
{
    private OrderManager manager;
    public TextMeshProUGUI orderNameText;
    public TextMeshProUGUI orderDetails;
    public Image timeFill;

    private string orderItemsString;
    private float timeRemaining;
    private float timeLimitStart;
    private bool isActive;

    private Dictionary<string, int> requiredAmounts = new Dictionary<string, int>();
    private Dictionary<string, int> deliveredAmounts = new Dictionary<string, int>();

    public void Init(OrderData order)
    {
        manager = FindObjectOfType<OrderManager>();
        //orderItemsString = "";

        if (!string.IsNullOrEmpty(order.description))
            orderNameText.text = $"Order {order.id} / {order.description}";
        else
            orderNameText.text = $"Order {order.id}";

        foreach (CraftRequirement craft in order.requiredItems)
        {
            //orderItemsString += $"{craft.itemName} x{craft.amount}\n";

            requiredAmounts[craft.itemName] = craft.amount;
            deliveredAmounts[craft.itemName] = 0;
            manager.totalCubesRequired += craft.amount;
        }

        manager.UdpdateStats();


        timeLimitStart = order.timeLimit;
        timeRemaining = order.timeLimit;
        isActive = true;

        RefreshOrderDetails(); //this comes before, has += to the string
        UpdateTimeDisplay();
    }
    public int GetItemAmount(string id)
    {
        print(requiredAmounts);
        if(requiredAmounts.ContainsKey(id))
            return requiredAmounts[id];
        else
            return 0;
    }

    private void Update()
    {
        if (!isActive) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isActive = false;
            FindObjectOfType<OrderManager>().FailOrder();
            Destroy(this.gameObject);
        }

        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        string formattedTime = $"{minutes}:{seconds:00}";

        orderDetails.text = orderItemsString + $"\nTime: {formattedTime}";

        if (timeFill != null)
            timeFill.fillAmount = timeRemaining / timeLimitStart;
    }
    public void AddItemToOrder(string itemName)
    {
        if (!requiredAmounts.ContainsKey(itemName)) return;
        //if (deliveredAmounts[itemName] >= requiredAmounts[itemName]) return;

        deliveredAmounts[itemName]++;
        RefreshOrderDetails();

        Debug.Log("item was added to order");

        if (IsOrderComplete())
        {
            isActive = false;
            FindObjectOfType<OrderManager>().CompeteOrder();
            Destroy(this.gameObject);
        }
    }
    private void RefreshOrderDetails()
    {
        orderItemsString = "";
        foreach (var kvp in requiredAmounts)
        {
            int delivered = deliveredAmounts[kvp.Key];
            int required = kvp.Value;
            orderItemsString += $"{kvp.Key} {delivered}/{required}\n";
        }

        UpdateTimeDisplay();
    }
    private bool IsOrderComplete()
    {
        foreach (var kvp in requiredAmounts)
        {
            if (deliveredAmounts[kvp.Key] < kvp.Value)
                return false;
        }
        return true;
    }


}
