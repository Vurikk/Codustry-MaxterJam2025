using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelingMachine : MonoBehaviour
{
    public TextMeshProUGUI labelText;
    public string labelToSet = string.Empty;
    public GameObject interactText;


    [Header("Code")]
    [TextArea(1, 20)]
    public string baseCode;
    [TextArea(1, 20)]
    public string userCode;

    private void OnTriggerEnter(Collider other)
    {
        if (labelToSet == string.Empty)
            return;

        if (other.gameObject.TryGetComponent(out ConveyorItem item))
        {
            item.label = labelToSet;
        }
    }

    public void SetLabel(string _label)
    {
        labelToSet = _label;
        labelText.text = _label;
    }
    public void ResetMachine()
    {
        labelToSet = string.Empty;
        labelText.text = "";
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
            panel.currentLabelingMachine = this;
        }
    }
    private void OnMouseExit()
    {
        interactText.SetActive(false);
    }
}
