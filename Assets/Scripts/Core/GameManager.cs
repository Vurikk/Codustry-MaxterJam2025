using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int money = 100;
    [SerializeField] private TextMeshProUGUI moneyText;

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = money.ToString();
    }
    public void ReduceMoney(int amount)
    {
        money -= amount;
        moneyText.text = money.ToString();
    }
}
