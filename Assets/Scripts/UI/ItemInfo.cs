using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public TextMeshProUGUI itemText;

    public void Init(string id, string label)
    {
        itemText.text = $"ID:{id}\nLabel:{label}";
    }
}
