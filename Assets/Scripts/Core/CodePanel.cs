using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class CodePanel : MonoBehaviour
{
    public OrderPrefab currentOrder;
    private Building building;
    public CameraMovement cameraMovement;
    public bool isOpen = false;
    public GameObject canvas;
    public GameObject disableWhenCodeObj;

    public Drill currentDrill;
    public RobotArmCodeActivator currentRobotArm;
    public LabelingMachine currentLabelingMachine;

    public TMP_InputField inputField;
    public TMP_Text previewText;
    public TMP_Text baseCodeText;
    public TMP_Text linesText;
    public TMP_Text consoleText;

    private HashSet<string> keywords = new HashSet<string> {
        "int", "float", "string", "if", "else", "for", "while", "return",
        "true", "false", "void", "public", "private", "class", "new", "var",
        "data", "GetItemAmount", "drill", "Gather", "print", "Reset",
        "arm", "SetLabel", "SetAmount", "machine"
    };

    private Dictionary<string, string> keywordColors = new Dictionary<string, string> {
        { "int", "#569CD6" }, { "float", "#B5CEA8" }, { "string", "#D69D85" },
        { "if", "#C586C0" }, { "else", "#C586C0" }, { "for", "#C586C0" }, { "while", "#C586C0" },
        { "return", "#C586C0" }, { "true", "#569CD6" }, { "false", "#569CD6" },
        { "void", "#569CD6" }, { "public", "#4EC9B0" }, { "private", "#4EC9B0" },
        { "class", "#4EC9B0" }, { "new", "#4EC9B0" }, { "var", "#4EC9B0" },
        { "data", "#9CDCFE" },{ "GetItemAmount", "#DBCD6F" },{ "drill", "#FFFFFF" }, {"Gather", "#DBCD6F"},
        { "print", "#DBCD6F" }, { "Reset", "#DBCD6F" },{ "arm", "#FFFFFF" }, {"SetLabel", "#DBCD6F"},{"SetAmount", "#DBCD6F"},
        { "machine", "#FFFFFF" },
    };

    private string baseHeader;

    void Start()
    {
        baseHeader = baseCodeText.text;
        inputField.onValueChanged.AddListener(UpdatePreview);
        UpdatePreview(inputField.text);
        building = FindObjectOfType<Building>();
    }

    public void OpenCode(string baseCode, string userCode)
    {
        if (isOpen) // prevent some annoying bugs
            return;

        cameraMovement.canMove = false;
        baseHeader = baseCode;
        inputField.text = userCode;
        UpdatePreview(inputField.text);
        canvas.SetActive(true);
        disableWhenCodeObj.SetActive(false);
        isOpen = true;
        if (building != null) //disable building + destroying mode
        {
            building.CanBuild = false;
            if (building.isBuilding) building.StopBuilding();
            if (building.destructionMode) building.ClearHoverHighlight();
        }
    }
    public void CloseCode()
    {
        cameraMovement.canMove = true;
        if (currentDrill != null)
            currentDrill.userCode = inputField.text;
        if (currentRobotArm != null)
            currentRobotArm.userCode = inputField.text;
        if (currentLabelingMachine != null)
            currentLabelingMachine.userCode = inputField.text;

        currentDrill = null;
        currentLabelingMachine = null;
        currentRobotArm = null;
        disableWhenCodeObj.SetActive(true);
        canvas.SetActive(false);
        isOpen = false;
        if (building != null) building.CanBuild = true;
    }
    private void Update()
    {
        if (!isOpen) return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCode();
        }
    }
    void UpdatePreview(string raw)
    {

        // Split input lines
        string[] inputLines = raw.Split('\n');
        int lineCount = inputLines.Length;

        // Update line numbers
        string lineNumbers = "";
        for (int i = 1; i <= lineCount + baseHeader.Split('\n').Length; i++)
        {
            lineNumbers += i.ToString() + "\n";
        }
        linesText.text = lineNumbers;

        // Update base code text
        string blankLines = "";
        for (int i = 0; i < lineCount; i++)
        {
            blankLines += "\n";
        }
        baseCodeText.text = baseHeader + blankLines + "}";

        // Apply syntax highlighting
        string formatted = "";
        for (int i = 0; i < inputLines.Length; i++)
        {
            string line = inputLines[i].Replace("\t", "  ");
            int commentIndex = line.IndexOf("//");

            string codePart = commentIndex >= 0 ? line.Substring(0, commentIndex) : line;
            string commentPart = commentIndex >= 0 ? line.Substring(commentIndex) : "";

            var tokens = Regex.Matches(codePart, @"\s+|\w+|[^\s\w]");
            bool expectVariable = false;

            bool insideQuotes = false;
            int parenDepth = 0;

            foreach (Match token in tokens)
            {
                string word = token.Value;
                
                if (word == "\"") // check quotes "
                {
                    insideQuotes = !insideQuotes;
                    formatted += word;
                    continue;
                }
                
                if (word == "(") // check if is (
                {
                    parenDepth++;
                    formatted += word;
                    continue;
                }
                if (word == ")")
                {
                    parenDepth = Math.Max(0, parenDepth - 1);
                    formatted += word;
                    continue;
                }

                if (Regex.IsMatch(word, @"^\s+$")) // = spaces or tabs
                {
                    formatted += word;
                    continue;
                }

                if (insideQuotes)
                {
                    formatted += $"<color=#B5CEA8>{word}</color>"; // string literal
                }
                else if (parenDepth > 0)
                {
                    if (Regex.IsMatch(word, @"^\d+$")) // number inside ()
                        formatted += $"<color=#B5CEA8>{word}</color>";
                    else if (variables.ContainsKey(word)) //check if is a variable inside ()
                        formatted += $"<color=#B5CEA8>{word}</color>";
                    else
                        formatted += $"<color=#D16969><u>{word}</u></color>"; // not number, mark as error
                }
                else if (keywords.Contains(word))
                {
                    string color = keywordColors.ContainsKey(word) ? keywordColors[word] : "#ffffff";
                    formatted += $"<color={color}>{word}</color>";
                    expectVariable = true;
                }
                else if (Regex.IsMatch(word, @"^[a-zA-Z_]\w*$")) // checks if it valid word
                {
                    if (expectVariable)
                    {
                        formatted += $"<color=#9CDCFE>{word}</color>";
                        expectVariable = false;
                    }
                    else
                    {
                        formatted += $"<color=#FF8888><u>{word}</u></color>";
                    }
                }
                else
                {
                    formatted += word;
                    expectVariable = false;
                }
            }

            // Add comment with green color
            if (!string.IsNullOrEmpty(commentPart))
            {
                formatted += $"<color=#6A9955>{commentPart}</color>";
            }


            if (i < inputLines.Length - 1)
                formatted += "\n";
        }

        previewText.text = formatted;

        if (currentDrill != null)
            currentDrill.userCode = inputField.text;

        if (currentLabelingMachine != null)
            currentLabelingMachine.userCode = inputField.text;

        if (currentRobotArm != null)
            currentRobotArm.userCode = inputField.text;

        // linesText.fontSize = previewText.fontSize;
        // baseCodeText.fontSize = previewText.fontSize;
    }


    public void ExecuteCode()
    {
        if (HasErrors(out string errorLine))
        {
            consoleText.text = $"<color=#FF5555>ERROR: {errorLine} (TIP: check ';')</color>";
            Debug.Log("Code has errors");
        }
        else
        {
            consoleText.text = $"";
            Debug.Log("No errors");
            RunCode();
        }
    }
    bool HasErrors(out string errorLine)
    {

        errorLine = "";
        string[] lines = inputField.text.Split('\n');
        foreach (var line in lines)
        {
            string trimmed = line.Trim();


            if (string.IsNullOrEmpty(trimmed) || trimmed == "{" || trimmed == "}")
                continue;

            if (trimmed.Contains("//"))
                continue;

            if (Regex.IsMatch(trimmed, @"^drill"))
            {
                if (currentDrill == null)
                {
                    errorLine = "drill component can not be used on this object!";
                    return true;
                }
            }
            if (Regex.IsMatch(trimmed, @"^arm"))
            {
                if (currentRobotArm == null)
                {
                    errorLine = "arm component can not be used on this object!";
                    return true;
                }
            }
            if (Regex.IsMatch(trimmed, @"^machine\.SetLabel\(")) continue;
            if (Regex.IsMatch(trimmed, @"^arm\.SetLabel\(\""")) continue; // if code has arm.SetLabel("
            if (Regex.IsMatch(trimmed, @"^arm\.SetAmount\(")) continue; // if code has arm.SetAmount(
            if (Regex.IsMatch(trimmed, @"^print\(")) continue; // if code has print(
            if (Regex.IsMatch(trimmed, @"^int\s+\w+\s*=\s*\d+\s*;$")) continue; // int + space + word + space + = + space + int + space + ;
            if (Regex.IsMatch(trimmed, @"^if\s*\(.*\)\s*{?$")) continue;
            if (Regex.IsMatch(trimmed, @"^else\s*{?$")) continue;
            if (Regex.IsMatch(trimmed, @"^\w+\.\w+\(.*\)\s*;$")) continue;
            if (Regex.IsMatch(trimmed, @"^data\.GetItemAmount\(\""(.+)\""\)\s*;$")) continue; // data.order.GetItemAmount("x");
            if (Regex.IsMatch(trimmed, @"^int\s+\w+\s*=\s*(\d+|data\.GetItemAmount\(\"".+\""\))\s*;$")) continue; // int count = data.order.GetItemAmount("x");

            // invalid line
            errorLine = trimmed;
            return true;
        }

        return false;
    }

    private void RunCode()
    {
        if (currentDrill != null)
            currentDrill.ResetCommands();

        variables.Clear();

        string[] lines = inputField.text.Split('\n'); // Split input into lines

        foreach (var line in lines)
        {
            string trimmed = line.Trim(); // Remove whitespace from each line

            if (Regex.IsMatch(trimmed, @"^\s*int\s+\w+\s*(=\s*\d+)?\s*;$"))
            {
                // variable declaration (int x; or int x = 5;)
                var match = Regex.Match(trimmed, @"^\s*int\s+(\w+)(\s*=\s*(\d+))?\s*;$");
                if (match.Success)
                {
                    string varName = match.Groups[1].Value; // Variable name (x or y)
                    string initValueStr = match.Groups[3].Value; // Initial value (if any)

                    int initialValue = string.IsNullOrEmpty(initValueStr) ? 0 : int.Parse(initValueStr);
                    variables[varName] = initialValue; // Store in the dictionary
                    Debug.Log($"[TEST] Declared {varName} with value {initialValue}");
                }
            }
            else if (Regex.IsMatch(trimmed, @"^\s*int\s+\w+\s*(=\s*data\.GetItemAmount\(\""(.+)\""\))?\s*;$")) // get order items amount and assign to a value
            {
                var match = Regex.Match(trimmed, @"GetItemAmount\(\""(.+)\""\)"); // Extract itemId
                if (match.Success)
                {
                    string itemId = match.Groups[1].Value; // Item id inside the quotes
                    int count = GetItemAmountForItem(itemId);

                    var assignmentMatch = Regex.Match(trimmed, @"^\s*int\s+(\w+)\s*=\s*data\.GetItemAmount\(\""(.+)\""\)\s*;$");
                    if (assignmentMatch.Success)
                    {
                        string varName = assignmentMatch.Groups[1].Value; // Variable name (count)
                        variables[varName] = count; 
                        Debug.Log($"[TEST] Assigned {count} to {varName} from GetItemAmount({itemId})");
                    }
                }
            }
            else if (trimmed.StartsWith("data.order.GetItemAmount")) // get order items amount
            {
                var match = Regex.Match(trimmed, @"GetItemAmount\(\""(.+)\""\)"); // get the value inside
                if (match.Success) // if value is the same
                {
                    string itemId = match.Groups[1].Value; //get the item inside brackets (Id)
                    Debug.Log($"[TEST] Requested item amount for: {itemId}");
                }
            }
            else if (trimmed.StartsWith("arm.Reset")) // drill Reset
            {
                currentRobotArm.arm.ResetArm();
            }
            else if (trimmed.StartsWith("arm.SetLabel")) // arm SetLabel
            {
                var match = Regex.Match(trimmed, @"arm.SetLabel\((.+)\)\s*;?"); // get the value inside ()
                if (match.Success)
                {
                    string expression = match.Groups[1].Value.Trim();

                    if (expression.StartsWith("\"") && expression.EndsWith("\"")) //check if the value is a string. Eg. print("abc"); and it will remove the "" and print abc
                    {
                        currentRobotArm.arm.SetLabel(expression.Trim('"'));
                    }
                    else
                        consoleText.text += $"<color=#FF5555>ERROR: unknown variable '{expression}'</color>\n";
                }
            }
            else if (trimmed.StartsWith("arm.SetAmount")) // drill Reset
            {
                var match = Regex.Match(trimmed, @"arm\.SetAmount\((.+)\)\s*;?");
                if (match.Success)
                {
                    string argument = match.Groups[1].Value.Trim();
                    int amount;

                    if (int.TryParse(argument, out amount)) // int passed like gather(5);
                    {
                        Debug.Log($"[Gather] Int amount: {amount}");
                    }
                    else if (variables.TryGetValue(argument, out amount)) //variable like x = 5 passed
                    {
                        Debug.Log($"[Gather] Variable '{argument}' amount: {amount}");
                    }
                    else
                    {
                        Debug.LogWarning($"ERROR: Could not get '{argument}' as number or known variable.");
                        return;
                    }

                    currentRobotArm.arm.AddItems(amount);
                }
            }
            else if (trimmed.StartsWith("machine.SetLabel")) // 
            {
                var match = Regex.Match(trimmed, @"machine.SetLabel\((.+)\)\s*;?"); // get the value inside ()
                if (match.Success)
                {
                    string expression = match.Groups[1].Value.Trim();

                    if (expression.StartsWith("\"") && expression.EndsWith("\"")) //check if the value is a string. Eg. print("abc"); and it will remove the "" and print abc
                    {
                        currentLabelingMachine.SetLabel(expression.Trim('"'));
                    }
                    else
                        consoleText.text += $"<color=#FF5555>ERROR: unknown variable '{expression}'</color>\n";
                }
            }
            else if (trimmed.StartsWith("machine.Reset")) // drill Reset
            {
                currentLabelingMachine.ResetMachine();
            }
            else if (trimmed.StartsWith("drill.Reset")) // drill Reset
            {
                currentDrill.ResetDrill();
            }
            else if (trimmed.StartsWith("drill.Gather")) // drill gather
            {
                var match = Regex.Match(trimmed, @"drill\.Gather\((.+)\)\s*;?");
                if (match.Success)
                {
                    string argument = match.Groups[1].Value.Trim();
                    int amount;

                    if (int.TryParse(argument, out amount)) // int passed like gather(5);
                    {
                        Debug.Log($"[Gather] Int amount: {amount}");
                    }
                    else if (variables.TryGetValue(argument, out amount)) //variable like x = 5 passed
                    {
                        Debug.Log($"[Gather] Variable '{argument}' amount: {amount}");
                    }
                    else
                    {
                        Debug.LogWarning($"ERROR: Could not get '{argument}' as number or known variable.");
                        return;
                    }

                    currentDrill.AddGatheringCommand(amount);
                }
            }
            else if (trimmed.StartsWith("print")) // print + value eg an int. x = 5; print(x)
            {
                var match = Regex.Match(trimmed, @"print\((.+)\)\s*;?"); // get the value inside ()
                if (match.Success)
                {
                    string expression = match.Groups[1].Value.Trim();

                    if (expression.StartsWith("\"") && expression.EndsWith("\"")) //check if the value is a string. Eg. print("abc"); and it will remove the "" and print abc
                    {
                        consoleText.text += expression.Trim('"') + "\n";
                    }
                    else if (variables.TryGetValue(expression, out int val)) // tries to get a value from variables that were assigned before
                    {
                        consoleText.text += val + "\n";
                    }
                    else
                    {
                        consoleText.text += $"<color=#FF5555>ERROR: unknown variable '{expression}'</color>\n";
                    }
                }
            }
            // future functions
        }
        if (string.IsNullOrEmpty(consoleText.text))
            consoleText.text = "Executed successfully!";

        // call the on order as an execute
        if (currentDrill != null)
            currentDrill.OnOrderReceived();
        if (currentRobotArm != null)
            currentRobotArm.arm.OnOrderReceived();
    }


    private int GetItemAmountForItem(string itemId)
    {
        if(currentOrder != null) 
            return currentOrder.GetItemAmount(itemId);

        return 0;
    }
    public Dictionary<string, int> variables = new Dictionary<string, int>(); // these are basically my variables store d from the code above eg. int x = 5; and it will store it as x and 5
}
