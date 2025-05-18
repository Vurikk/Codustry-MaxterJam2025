using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private OrderManager orderManager;
    [SerializeField] private GameObject[] tutorialLevels;
    private TutorialObject currentTutorialInstance;
    private int currentTutorial = 0;

    [Header("UI")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;

    private void Start()
    {
        orderManager = FindObjectOfType<OrderManager>();
        orderManager.OrderCompleted += OnOrderCompleted;
        SetNewTutorial();
    }
    public void SetText(string text)
    {
        textPanel.SetActive(true);
        tutorialText.text = text;
        //StartCoroutine(SmoothText(text));
    }
    private IEnumerator SmoothText(string text)
    {
        yield return new WaitForSeconds(1f); 
        textPanel.SetActive(true);
        tutorialText.text = text;
    }

    public void NextLineButton()
    {
        if (currentTutorialInstance != null)
            currentTutorialInstance.NextLine();
    }

    public void SetOrder()
    {
        textPanel.SetActive(false);
        orderManager.CreateOrder();
    }
    public void OnOrderCompleted()
    {
        if (currentTutorial >= tutorialLevels.Length)
            SceneManager.LoadScene(1);
        else
            SetNewTutorial();
    }
    private void SetNewTutorial() // spawns a new prefab of tutorial level
    {
        FindObjectOfType<Building>().DestroyAllPlayerBuilding();
        if (currentTutorialInstance != null) Destroy(currentTutorialInstance.gameObject);

        ConveyorItem[] items = FindObjectsOfType<ConveyorItem>();
        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }

        currentTutorialInstance = Instantiate(tutorialLevels[currentTutorial]).GetComponent<TutorialObject>();
        currentTutorialInstance.manager = this;
        currentTutorialInstance.NextLine();
        currentTutorial++;
    }
}
