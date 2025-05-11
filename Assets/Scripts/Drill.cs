using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Drill : MonoBehaviour
{
    private OrderManager manager;
    public string itemID;
    public int ItemsToGather = 0;
    public ConveyorItem itemPrefab;
    public ConveyorBelt startingBelt;
    public Transform spawnPoint;

    private float timer = 5f;

    public TextMeshProUGUI amountText;
    public GameObject interactText;

    [Header("Code")]
    [TextArea(1, 20)]
    public string baseCode;
    [TextArea(1, 20)]
    public string userCode;
    public List<int> gatheringList = new List<int>();
    private void Start()
    {
        FindObjectOfType<OrderManager>().OrderReceived += OnOrderReceived;

        manager = FindObjectOfType<OrderManager>();
    }
    private void Update()
    {
        if (ItemsToGather == 0) return;
        if (startingBelt != null && startingBelt.currentItem != null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnItem();
            timer = 5f;
            ItemsToGather--;
            amountText.text = ItemsToGather.ToString();
        }
    }

    public void SpawnItem()
    {

        if (itemPrefab == null || startingBelt == null) return;

        manager.totalCubesProduced++;
        manager.UdpdateStats();
        ConveyorItem newItem = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
        newItem.currentBelt = startingBelt;
        newItem.currentBelt.currentItem = newItem;
        newItem.transform.position = spawnPoint.position;
        newItem.itemID = itemID;
    }

    public void OnOrderReceived()
    {
        foreach (int amount in gatheringList)
        {
            ItemsToGather += amount;
        }
        amountText.text = ItemsToGather.ToString();
    }

    public void AddGatheringCommand(int amount)
    {
        gatheringList.Add(amount);
    }

    public void ResetCommands()
    {
        gatheringList.Clear();
    }

    public void ResetDrill()
    {
        gatheringList.Clear();
        ItemsToGather = 0;
        timer = 5f;
        amountText.text = ItemsToGather.ToString();
    }

    private void OnMouseEnter()
    {
        interactText.SetActive(true);
    }
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CodePanel panel = FindObjectOfType<CodePanel>();
            panel.OpenCode(baseCode, userCode);
            panel.currentDrill = this;
        }
    }
    private void OnMouseExit()
    {
        interactText.SetActive(false);
    }
}
