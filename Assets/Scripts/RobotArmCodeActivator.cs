using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class RobotArmCodeActivator : MonoBehaviour
{
    public RobotArm arm;
    public GameObject interactText;
    [Header("Code")]
    [TextArea(1, 20)]
    public string baseCode;
    [TextArea(1, 20)]
    public string userCode;
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
            panel.currentRobotArm = this;
        }
    }
    private void OnMouseExit()
    {
        interactText.SetActive(false);
    }
}
