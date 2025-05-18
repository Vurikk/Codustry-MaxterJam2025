using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObject : MonoBehaviour
{
    [HideInInspector] public TutorialManager manager;
    [SerializeField] private string[] textLines;
    private int lineId = 0;

    public void NextLine()
    {
        if (lineId >= textLines.Length)
            manager.SetOrder();
        else
            manager.SetText(textLines[lineId]);
        lineId++;
    }
}
