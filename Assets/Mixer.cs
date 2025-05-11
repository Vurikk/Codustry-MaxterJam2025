using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mixer : MonoBehaviour
{
    public string firstID;
    public string secondID;
    public string craftedItemID;

    public Dictionary<string, int> currentItems = new Dictionary<string, int>();
    public TextMeshProUGUI infoText;

    public ConveyorItem craftedPrefab;
    public ConveyorBelt startingBelt;
    public Transform spawnPoint;

    private void Start()
    {
        if (!currentItems.ContainsKey(firstID))
            currentItems[firstID] = 0;

        if (!currentItems.ContainsKey(secondID))
            currentItems[secondID] = 0;

        UpdateText();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out ConveyorItem item))
        {
            if (item.itemID == firstID)
            {
                if (!currentItems.ContainsKey(firstID))
                    currentItems[firstID] = 0;

                currentItems[firstID]++;
            }
            else if (item.itemID == secondID)
            {
                if (!currentItems.ContainsKey(secondID))
                    currentItems[secondID] = 0;

                currentItems[secondID]++;
            }

            CraftItem();
            Destroy(item.gameObject); // destroy the cube
        }
    }

    private void CraftItem()
    {
        if (!currentItems.ContainsKey(firstID)) currentItems[firstID] = 0;
        if (!currentItems.ContainsKey(secondID)) currentItems[secondID] = 0;

        UpdateText();

        if (currentItems[firstID] == 0 || currentItems[secondID] == 0)
            return;

        //craft
        StartCoroutine(SpawnWhenBeltFree());
    }
    private IEnumerator SpawnWhenBeltFree()
    {
        // check if belt is free
        while (startingBelt.currentItem != null)
        {
            yield return new WaitForSeconds(1f);
        }

        // spawn object
        SpawnItem();
    }
    public void SpawnItem()
    {
        // craft item
        currentItems[firstID]--;
        currentItems[secondID]--;
        UpdateText();

        if (craftedPrefab == null || startingBelt == null) return;

        ConveyorItem newItem = Instantiate(craftedPrefab, spawnPoint.position, Quaternion.identity);
        newItem.currentBelt = startingBelt;
        newItem.currentBelt.currentItem = newItem;
        newItem.transform.position = spawnPoint.position;
        newItem.itemID = craftedItemID;
    }
    private void UpdateText()
    {
        string firstColor = currentItems[firstID] > 0 ? "green" : "white"; // if amount > 0 then make it green else white
        string secondColor = currentItems[secondID] > 0 ? "green" : "white";

        infoText.text = $"<color={firstColor}>{firstID}</color>+<color={secondColor}>{secondID}</color>";
    }
}
